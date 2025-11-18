import {defineStore} from 'pinia'
import {computed, reactive} from 'vue'
import {usePreferencesStore} from '@/stores/preferences.js'

/**
 * Encodes a string or Uint8Array to a base64 string.
 * @param {string|Uint8Array|null} strOrBytes - The string or byte array to encode.
 * @returns {string} The base64 encoded string.
 */
const toBase64 = (strOrBytes) => {
  if (strOrBytes == null) return ''
  if (strOrBytes instanceof Uint8Array) {
    let binary = ''
    for (let i = 0; i < strOrBytes.length; i++) binary += String.fromCharCode(strOrBytes[i])
    return btoa(binary)
  }
  try {
    return btoa(unescape(encodeURIComponent(String(strOrBytes))))
  } catch (e) {
    return btoa(String(strOrBytes))
  }
}

/**
 * Pinia store for managing test publishers to Pulsar topics.
 */
export const useTestPublisherStore = defineStore('test-publisher', () => {
  const prefs = usePreferencesStore()

  /**
   * @type {Map<string, object>}
   * A map of active producers.
   * The key is a string in the format: `${type}://${tenant}/${namespace}/${topic}`.
   * The value is a reactive producer object with the following shape:
   * {
   *   key: string,
   *   type: 'persistent' | 'non-persistent',
   *   tenant: string,
   *   namespace: string,
   *   topic: string,
   *   ws: WebSocket | null,
   *   status: 'idle' | 'connecting' | 'open' | 'closed' | 'error',
   *   error: string | null,
   *   sent: number,
   *   acks: number
   * }
   */
  const producers = reactive(new Map())

  /**
   * A computed property that returns an array of all active producer objects.
   * @returns {Array<object>}
   */
  const active = computed(() => Array.from(producers.values()))

  /**
   * Creates a unique key for a producer based on its properties.
   * @param {object} params - The producer parameters.
   * @param {string} [params.type='persistent'] - The type of the topic.
   * @param {string} params.tenant - The tenant of the topic.
   * @param {string} params.namespace - The namespace of the topic.
   * @param {string} params.topic - The name of the topic.
   * @returns {string} The unique producer key.
   */
  const makeKey = ({ type = 'persistent', tenant, namespace, topic }) => `${type}://${tenant}/${namespace}/${topic}`

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
      } catch (_) { /* ignore */ }
    }
    const loc = window.location
    const proto = loc.protocol === 'https:' ? 'wss:' : 'ws:'
    return `${proto}//${loc.host}`
  }

  /**
   * Starts a new producer by opening a WebSocket connection to a Pulsar topic.
   * If a producer for the same topic already exists and is open, it returns the existing one.
   * @param {object} params - The parameters for the producer.
   * @param {string} [params.type='persistent'] - The type of the topic.
   * @param {string} params.tenant - The tenant of the topic.
   * @param {string} params.namespace - The namespace of the topic.
   * @param {string} params.topic - The name of the topic.
   * @param {string} [params.producerName] - The name of the producer.
   * @returns {Promise<object>} The reactive producer object.
   */
  const startProducer = async ({ type = 'persistent', tenant, namespace, topic, producerName }) => {
    const key = makeKey({ type, tenant, namespace, topic })
    const existing = producers.get(key)
    if (existing && existing.status === 'open') return existing

    const origin = buildWsOrigin()
    const params = new URLSearchParams()
    if (producerName) params.set('producerName', producerName)
    const wsUrl = `${origin}/ws/v2/producer/${encodeURIComponent(type)}/${encodeURIComponent(tenant)}/${encodeURIComponent(namespace)}/${encodeURIComponent(topic)}${params.toString() ? `?${params.toString()}` : ''}`

    const prod = existing || reactive({
      key,
      type,
      tenant,
      namespace,
      topic,
      ws: null,
      status: 'idle',
      error: null,
      sent: 0,
      acks: 0,
    })
    prod.status = 'connecting'
    prod.error = null
    producers.set(key, prod)

    try {
      const ws = new WebSocket(wsUrl)
      prod.ws = ws

      // connection timeout to avoid indefinite connecting
      const timeoutMs = 10000
      const onTimeout = () => {
        if (prod.status === 'connecting') {
          prod.status = 'error'
          prod.error = 'Connection timeout'
          try { ws.close() } catch (_) { /* ignore */ }
        }
      }
      const timerId = setTimeout(onTimeout, timeoutMs)
      const clearTimer = () => { try { clearTimeout(timerId) } catch (_) { /* ignore */ } }

      ws.addEventListener('open', () => {
        clearTimer()
        prod.status = 'open'
      })
      ws.addEventListener('message', (evt) => {
        try {
          // Ack messages from broker are JSON
          const data = typeof evt.data === 'string' ? JSON.parse(evt.data) : null
          if (data && (data.result === 'ok' || data.context != null)) {
            prod.acks++
          }
        } catch (_) { /* ignore */ }
      })
      ws.addEventListener('error', () => {
        clearTimer()
        prod.status = 'error'
        prod.error = 'WebSocket error'
      })
      ws.addEventListener('close', () => {
        clearTimer()
        prod.status = 'closed'
      })
    } catch (e) {
      prod.status = 'error'
      prod.error = String(e)
    }

    return prod
  }

  /**
   * Ensures that a producer is open and returns it.
   * @param {string} key - The key of the producer.
   * @returns {object} The producer object.
   * @throws {Error} If the producer is not open.
   */
  const ensureOpen = (key) => {
    const p = producers.get(key)
    if (!p || p.status !== 'open' || !p.ws) throw new Error('Producer is not open')
    return p
  }

  /**
   * Sends a message to a topic.
   * @param {object} params - The message parameters.
   * @param {string} [params.type='persistent'] - The type of the topic.
   * @param {string} params.tenant - The tenant of the topic.
   * @param {string} params.namespace - The namespace of the topic.
   * @param {string} params.topic - The name of the topic.
   * @param {string|Uint8Array} params.payload - The message payload.
   * @param {string} [params.key] - The message key.
   * @param {object} [params.properties] - The message properties.
   * @param {string} [params.context] - The producer context.
   * @throws {Error} If the message fails to send.
   */
  const send = ({ type = 'persistent', tenant, namespace, topic, payload, key: messageKey, properties, context }) => {
    const k = makeKey({ type, tenant, namespace, topic })
    const p = ensureOpen(k)
    const frame = {
      payload: toBase64(payload ?? ''),
      key: messageKey || undefined,
      properties: properties || undefined,
      context: context || undefined,
    }
    try {
      p.ws.send(JSON.stringify(frame))
      p.sent++
    } catch (e) {
      p.error = String(e)
      p.status = 'error'
      throw e
    }
  }

  /**
   * Sends a text message to a topic. This is an alias for `send`.
   * @param {object} args - The arguments for the `send` function.
   */
  const sendText = (args) => send(args)

  /**
   * Sends a JSON message to a topic.
   * @param {object} args - The message parameters, including a `json` property to be stringified.
   */
  const sendJson = (args) => {
    const { json, ...rest } = args
    return send({ ...rest, payload: JSON.stringify(json) })
  }

  /**
   * Closes a producer's WebSocket connection.
   * @param {object} params - The producer identification parameters.
   * @param {string} [params.type='persistent'] - The type of the topic.
   * @param {string} params.tenant - The tenant of the topic.
   * @param {string} params.namespace - The namespace of the topic.
   * @param {string} params.topic - The name of the topic.
   */
  const closeProducer = ({ type = 'persistent', tenant, namespace, topic }) => {
    const k = makeKey({ type, tenant, namespace, topic })
    const p = producers.get(k)
    if (p && p.ws) {
      try { p.ws.close() } catch (_) { /* no-op */ }
      p.ws = null
      p.status = 'closed'
    }
  }

  return {
    producers,
    active,
    startProducer,
    send,
    sendText,
    sendJson,
    closeProducer,
  }
})
