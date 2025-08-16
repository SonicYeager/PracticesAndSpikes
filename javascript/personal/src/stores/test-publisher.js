import {defineStore} from 'pinia'
import {computed, reactive} from 'vue'
import {usePreferencesStore} from '@/stores/preferences.js'

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

export const useTestPublisherStore = defineStore('test-publisher', () => {
  const prefs = usePreferencesStore()

  // key: `${type}://${tenant}/${namespace}/${topic}`
  const producers = reactive(new Map())
  // producer: { key, type, tenant, namespace, topic, ws, status: 'idle'|'connecting'|'open'|'closed'|'error', error: null, sent: 0, acks: 0 }

  const active = computed(() => Array.from(producers.values()))

  const makeKey = ({ type = 'persistent', tenant, namespace, topic }) => `${type}://${tenant}/${namespace}/${topic}`

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

  const startProducer = async ({ type = 'persistent', tenant, namespace, topic, producerName }) => {
    const key = makeKey({ type, tenant, namespace, topic })
    const existing = producers.get(key)
    if (existing && existing.status === 'open') return existing

    const origin = buildWsOrigin()
    const params = new URLSearchParams()
    if (producerName) params.set('producerName', producerName)
    const wsUrl = `${origin}/ws/v2/producer/${encodeURIComponent(type)}/${encodeURIComponent(tenant)}/${encodeURIComponent(namespace)}/${encodeURIComponent(topic)}${params.toString() ? `?${params.toString()}` : ''}`

    const prod = existing || {
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
    }
    prod.status = 'connecting'
    prod.error = null
    producers.set(key, prod)

    try {
      const ws = new WebSocket(wsUrl)
      prod.ws = ws
      ws.addEventListener('open', () => {
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
        prod.status = 'error'
        prod.error = 'WebSocket error'
      })
      ws.addEventListener('close', () => {
        prod.status = 'closed'
      })
    } catch (e) {
      prod.status = 'error'
      prod.error = String(e)
    }

    return prod
  }

  const ensureOpen = (key) => {
    const p = producers.get(key)
    if (!p || p.status !== 'open' || !p.ws) throw new Error('Producer is not open')
    return p
  }

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

  const sendText = (args) => send(args)
  const sendJson = (args) => {
    const { json, ...rest } = args
    return send({ ...rest, payload: JSON.stringify(json) })
  }

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
