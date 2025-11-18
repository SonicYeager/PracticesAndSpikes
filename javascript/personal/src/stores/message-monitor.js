import {defineStore} from 'pinia'
import {computed, reactive} from 'vue'
import {usePreferencesStore} from '@/stores/preferences.js'
import {usePulsarAdminStore} from '@/stores/pulsar-admin.js'

/**
 * Decodes a base64 encoded string to a UTF-8 string.
 * Falls back to a direct base64 decode if UTF-8 decoding fails.
 * @param {string} b64 - The base64 encoded string.
 * @returns {string} The decoded string.
 */
const decodeBase64Utf8 = (b64) => {
  try {
    const binary = atob(b64)
    // Convert binary string to UTF-8
    const bytes = Uint8Array.from(binary, c => c.charCodeAt(0))
    const decoder = new TextDecoder('utf-8', { fatal: false })
    return decoder.decode(bytes)
  } catch (e) {
    try {
      return atob(b64)
    } catch (e2) {
      return ''
    }
  }
}

/**
 * Pinia store for managing real-time message monitoring from Pulsar topics.
 */
export const useMessageMonitorStore = defineStore('message-monitor', () => {
  const prefs = usePreferencesStore()
  const admin = usePulsarAdminStore()

  /**
   * @type {Map<string, object>}
   * A map of active monitors.
   * The key is a string in the format: `${type}://${tenant}/${namespace}/${topic}@${subscriptionName}`.
   * The value is a reactive monitor object with the following shape:
   * {
   *   key: string,
   *   type: 'persistent' | 'non-persistent',
   *   tenant: string,
   *   namespace: string,
   *   topic: string,
   *   subscriptionName: string,
   *   ws: WebSocket | null,
   *   status: 'idle' | 'connecting' | 'open' | 'closed' | 'error',
   *   error: string | null,
   *   messages: Array<{
   *     messageId: string,
   *     publishTime: string,
   *     key: string,
   *     properties: object,
   *     payload: string,
   *     receivedAt: number
   *   }>,
   *   maxBuffer: number
   * }
   */
  const monitors = reactive(new Map())

  /**
   * A computed property that returns an array of all active monitor objects.
   * @returns {Array<object>}
   */
  const active = computed(() => Array.from(monitors.values()))

  /**
   * Builds the WebSocket origin URL from the configured Pulsar URL or the current window location.
   * @returns {string} The WebSocket origin URL (e.g., 'wss://pulsar.example.com').
   */
  const buildWsOrigin = () => {
    const url = prefs.pulsarUrl
    if (url) {
      try {
        const u = new URL(url)
        const protocol = u.protocol === 'https:' ? 'wss:' : 'ws:'
        return `${protocol}//${u.host}`
      } catch (e) {
        // fall back to location
      }
    }
    const loc = window.location
    const proto = loc.protocol === 'https:' ? 'wss:' : 'ws:'
    return `${proto}//${loc.host}`
  }

  /**
   * Creates a unique key for a monitor based on its properties.
   * @param {object} params - The monitor parameters.
   * @param {string} [params.type='persistent'] - The type of the topic.
   * @param {string} params.tenant - The tenant of the topic.
   * @param {string} params.namespace - The namespace of the topic.
   * @param {string} params.topic - The name of the topic.
   * @param {string} params.subscriptionName - The name of the subscription.
   * @returns {string} The unique monitor key.
   */
  const makeKey = ({ type = 'persistent', tenant, namespace, topic, subscriptionName }) => `${type}://${tenant}/${namespace}/${topic}@${subscriptionName}`

  /**
   * Starts a new message monitor by opening a WebSocket connection to a Pulsar topic.
   * If a monitor for the same subscription already exists and is open, it returns the existing one.
   * @param {object} params - The parameters for the monitor.
   * @param {string} [params.type='persistent'] - The type of the topic.
   * @param {string} params.tenant - The tenant of the topic.
   * @param {string} params.namespace - The namespace of the topic.
   * @param {string} params.topic - The name of the topic.
   * @param {string} [params.subscriptionName] - The name of the subscription. A new one is generated if not provided.
   * @param {string} [params.subscriptionType='Exclusive'] - The subscription type.
   * @param {string} [params.initialPosition='Latest'] - The initial position to start consuming from.
   * @param {number} [params.receiverQueueSize=1000] - The receiver queue size.
   * @param {number} [params.maxBuffer=200] - The maximum number of messages to buffer in the UI.
   * @param {string} [params.consumerName] - The consumer name. Defaults to the subscription name if not provided.
   * @returns {Promise<object>} The reactive monitor object.
   */
  const startMonitor = async ({ type = 'persistent', tenant, namespace, topic, subscriptionName, subscriptionType = 'Exclusive', initialPosition = 'Latest', receiverQueueSize = 1000, maxBuffer = 200, consumerName }) => {
    const sub = subscriptionName || `admin-ui-${Date.now().toString(36)}-${Math.random().toString(36).slice(2, 6)}`
    const key = makeKey({ type, tenant, namespace, topic, subscriptionName: sub })
    // If already open, just return existing
    const existing = monitors.get(key)
    if (existing && existing.status === 'open') return existing

    const origin = buildWsOrigin()
    const params = new URLSearchParams({
      subscriptionType,
      subscriptionInitialPosition: initialPosition,
      receiverQueueSize: String(receiverQueueSize),
      consumerName: consumerName || sub,
    })
    const wsUrl = `${origin}/ws/v2/consumer/${encodeURIComponent(type)}/${encodeURIComponent(tenant)}/${encodeURIComponent(namespace)}/${encodeURIComponent(topic)}/${encodeURIComponent(sub)}?${params.toString()}`

    const monitor = existing || reactive({
      key,
      type,
      tenant,
      namespace,
      topic,
      subscriptionName: sub,
      ws: null,
      status: 'idle',
      error: null,
      messages: [],
      maxBuffer,
    })
    monitor.status = 'connecting'
    monitor.error = null
    monitors.set(key, monitor)

    try {
      const ws = new WebSocket(wsUrl)
      monitor.ws = ws

      // connection timeout to avoid indefinite connecting
      const timeoutMs = 10000
      const onTimeout = () => {
        if (monitor.status === 'connecting') {
          monitor.status = 'error'
          monitor.error = 'Connection timeout'
          try { ws.close() } catch (_) { /* ignore */ }
        }
      }
      const timerId = setTimeout(onTimeout, timeoutMs)
      const clearTimer = () => { try { clearTimeout(timerId) } catch (_) { /* ignore */ } }

      ws.addEventListener('open', () => {
        clearTimer()
        monitor.status = 'open'
      })

      ws.addEventListener('message', (evt) => {
        try {
          const data = typeof evt.data === 'string' ? evt.data : ''
          const msg = data ? JSON.parse(data) : null
          if (msg) {
            const decoded = {
              messageId: msg.messageId,
              publishTime: msg.publishTime,
              key: msg.key,
              properties: msg.properties || {},
              payload: decodeBase64Utf8(msg.payload || ''),
              receivedAt: Date.now(),
            }
            monitor.messages.push(decoded)
            if (monitor.messages.length > monitor.maxBuffer) {
              monitor.messages.splice(0, monitor.messages.length - monitor.maxBuffer)
            }
            // Ack the message to avoid backlog accumulation
            try {
              ws.send(JSON.stringify({ messageId: msg.messageId }))
            } catch (_) { /* ignore */ }
          }
        } catch (e) {
          // ignore malformed
        }
      })

      ws.addEventListener('error', (e) => {
        clearTimer()
        monitor.status = 'error'
        monitor.error = 'WebSocket error'
      })

      ws.addEventListener('close', () => {
        clearTimer()
        monitor.status = 'closed'
      })
    } catch (e) {
      monitor.status = 'error'
      monitor.error = String(e)
    }

    return monitor
  }

  /**
   * Stops an active message monitor by closing its WebSocket connection.
   * @param {object} params - The monitor identification parameters.
   * @param {string} [params.type='persistent'] - The type of the topic.
   * @param {string} params.tenant - The tenant of the topic.
   * @param {string} params.namespace - The namespace of the topic.
   * @param {string} params.topic - The name of the topic.
   * @param {string} params.subscriptionName - The name of the subscription.
   */
  const stopMonitor = ({ type = 'persistent', tenant, namespace, topic, subscriptionName }) => {
    const key = makeKey({ type, tenant, namespace, topic, subscriptionName })
    const monitor = monitors.get(key)
    if (monitor && monitor.ws) {
      try { monitor.ws.close() } catch (_) { /* no-op */ }
      monitor.ws = null
      monitor.status = 'closed'
    }
  }

  /**
   * Clears all buffered messages from a monitor's display.
   * @param {object} params - The monitor identification parameters.
   * @param {string} [params.type='persistent'] - The type of the topic.
   * @param {string} params.tenant - The tenant of the topic.
   * @param {string} params.namespace - The namespace of the topic.
   * @param {string} params.topic - The name of the topic.
   * @param {string} params.subscriptionName - The name of the subscription.
   */
  const clearMessages = ({ type = 'persistent', tenant, namespace, topic, subscriptionName }) => {
    const key = makeKey({ type, tenant, namespace, topic, subscriptionName })
    const monitor = monitors.get(key)
    if (monitor) monitor.messages.splice(0)
  }

  /**
   * Removes a subscription from a topic after stopping its monitor.
   * @param {object} params - The subscription identification parameters.
   * @param {string} [params.type='persistent'] - The type of the topic.
   * @param {string} params.tenant - The tenant of the topic.
   * @param {string} params.namespace - The namespace of the topic.
   * @param {string} params.topic - The name of the topic.
   * @param {string} params.subscriptionName - The name of the subscription to remove.
   * @param {boolean} [params.force=true] - Whether to force the deletion of the subscription.
   * @returns {Promise<boolean>} A promise that resolves to true if the subscription was removed successfully.
   * @throws {Error} If the REST API call to remove the subscription fails.
   */
  const removeSubscription = async ({ type = 'persistent', tenant, namespace, topic, subscriptionName, force = true }) => {
    // Stop monitor if running
    stopMonitor({ type, tenant, namespace, topic, subscriptionName })
    // Delete subscription using admin REST
    const base = `${encodeURIComponent(type)}/${encodeURIComponent(tenant)}/${encodeURIComponent(namespace)}/${encodeURIComponent(topic)}`
    const url = `${base}/subscription/${encodeURIComponent(subscriptionName)}${force ? '?force=true' : ''}`
    const res = await admin.fetchAdmin(url, { method: 'DELETE' })
    if (!res || !res.ok) throw new Error('Failed to remove subscription')
    // cleanup
    const key = makeKey({ type, tenant, namespace, topic, subscriptionName })
    monitors.delete(key)
    return true
  }

  return {
    monitors,
    active,
    startMonitor,
    stopMonitor,
    clearMessages,
    removeSubscription,
  }
})
