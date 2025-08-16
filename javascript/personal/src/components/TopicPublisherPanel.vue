<script setup>
import {computed, reactive} from 'vue'
import {useTestPublisherStore} from '@/stores/test-publisher.js'

const props = defineProps({
  tenant: { type: String, required: true },
  namespace: { type: String, required: true },
  topic: { type: String, required: true },
  type: { type: String, default: 'persistent' },
})

const pub = useTestPublisherStore()

const state = reactive({
  producerName: '',
  key: '',
  propertiesText: '', // JSON object as text
  payloadText: '',
  jsonMode: false,
})

const k = computed(() => `${props.type}://${props.tenant}/${props.namespace}/${props.topic}`)
const producer = computed(() => pub.producers.get(k.value))
const status = computed(() => producer.value?.status || 'idle')
const sent = computed(() => producer.value?.sent || 0)
const acks = computed(() => producer.value?.acks || 0)
const error = computed(() => producer.value?.error || '')

const parseProps = () => {
  if (!state.propertiesText) return undefined
  try {
    const obj = JSON.parse(state.propertiesText)
    if (obj && typeof obj === 'object' && !Array.isArray(obj)) return obj
  } catch { /* ignore */ }
  return undefined
}

const connect = async () => {
  await pub.startProducer({ type: props.type, tenant: props.tenant, namespace: props.namespace, topic: props.topic, producerName: state.producerName || undefined })
}

const disconnect = () => {
  pub.closeProducer({ type: props.type, tenant: props.tenant, namespace: props.namespace, topic: props.topic })
}

const send = async () => {
  const common = {
    type: props.type,
    tenant: props.tenant,
    namespace: props.namespace,
    topic: props.topic,
    key: state.key || undefined,
    properties: parseProps(),
  }
  if (state.jsonMode) {
    let json
    try { json = JSON.parse(state.payloadText || '{}') } catch { json = { value: state.payloadText } }
    pub.sendJson({ ...common, json })
  } else {
    pub.sendText({ ...common, payload: state.payloadText })
  }
}
</script>

<template>
  <div class="space-y-2">
    <div class="flex flex-wrap gap-2 items-end">
      <input class="input input-bordered input-sm w-48" placeholder="Producer name (optional)" v-model="state.producerName" />
      <button class="btn btn-sm btn-primary" :disabled="status==='open'" @click="connect">Connect</button>
      <button class="btn btn-sm" :disabled="status!=='open'" @click="disconnect">Disconnect</button>
      <div class="text-xs opacity-70">Status: <span :class="{'text-success': status==='open', 'text-warning': status==='connecting', 'text-error': status==='error'}">{{ status }}</span> | Sent: {{ sent }} | Acks: {{ acks }}</div>
    </div>

    <div class="grid grid-cols-1 md:grid-cols-2 gap-2">
      <div class="form-control">
        <label class="label"><span class="label-text">Key (optional)</span></label>
        <input class="input input-bordered input-sm" v-model="state.key" />
      </div>
      <div class="form-control">
        <label class="label"><span class="label-text">Properties JSON (optional)</span></label>
        <input class="input input-bordered input-sm" v-model="state.propertiesText" placeholder='{"k":"v"}' />
      </div>
      <div class="form-control md:col-span-2">
        <div class="flex items-center justify-between">
          <label class="label"><span class="label-text">Payload</span></label>
          <label class="label cursor-pointer">
            <span class="label-text text-xs mr-2">JSON</span>
            <input type="checkbox" class="toggle toggle-xs" v-model="state.jsonMode" />
          </label>
        </div>
        <textarea class="textarea textarea-bordered h-24" v-model="state.payloadText" placeholder="Enter message payload"></textarea>
      </div>
    </div>

    <div class="flex gap-2">
      <button class="btn btn-sm btn-secondary" :disabled="status!=='open'" @click="send">Send</button>
      <span v-if="error" class="text-error text-xs">{{ error }}</span>
    </div>
  </div>
</template>
