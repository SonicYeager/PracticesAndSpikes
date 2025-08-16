import {defineStore} from 'pinia'
import {computed, reactive} from 'vue'
import {usePreferencesStore} from '@/stores/preferences.js'
import {usePulsarAdminStore} from '@/stores/pulsar-admin.js'

// Helper: decode base64 to UTF-8 string
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

export const useMessageMonitorStore = defineStore('message-monitor', () => {
  const prefs = usePreferencesStore()
  const admin = usePulsarAdminStore()

  // key: `${type}://${tenant}/${namespace}/${topic}@${subscriptionName}`
  const monitors = reactive(new Map())
  // monitor object shape:
  // { key, type:'persistent', tenant, namespace, topic, subscriptionName, ws, status:'idle'|'connecting'|'open'|'closed'|'error', error:null,
  //   messages: [{ messageId, publishTime, key, properties, payload, receivedAt }], maxBuffer: 200 }

  const active = computed(() => Array.from(monitors.values()))

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

  const makeKey = ({ type = 'persistent', tenant, namespace, topic, subscriptionName }) => `${type}://${tenant}/${namespace}/${topic}@${subscriptionName}`

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

    const monitor = existing || {
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
    }
    monitor.status = 'connecting'
    monitor.error = null
    monitors.set(key, monitor)

    try {
      const ws = new WebSocket(wsUrl)
      monitor.ws = ws

      ws.addEventListener('open', () => {
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
        monitor.status = 'error'
        monitor.error = 'WebSocket error'
      })

      ws.addEventListener('close', () => {
        monitor.status = 'closed'
      })
    } catch (e) {
      monitor.status = 'error'
      monitor.error = String(e)
    }

    return monitor
  }

  const stopMonitor = ({ type = 'persistent', tenant, namespace, topic, subscriptionName }) => {
    const key = makeKey({ type, tenant, namespace, topic, subscriptionName })
    const monitor = monitors.get(key)
    if (monitor && monitor.ws) {
      try { monitor.ws.close() } catch (_) { /* no-op */ }
      monitor.ws = null
      monitor.status = 'closed'
    }
  }

  const clearMessages = ({ type = 'persistent', tenant, namespace, topic, subscriptionName }) => {
    const key = makeKey({ type, tenant, namespace, topic, subscriptionName })
    const monitor = monitors.get(key)
    if (monitor) monitor.messages.splice(0)
  }

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
