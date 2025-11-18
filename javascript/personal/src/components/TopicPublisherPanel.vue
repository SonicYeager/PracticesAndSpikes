<script setup>
import {computed, reactive} from 'vue'
import {useTestPublisherStore} from '@/stores/test-publisher.js'

/**
 * Component props.
 * @property {string} tenant - The tenant of the topic.
 * @property {string} namespace - The namespace of the topic.
 * @property {string} topic - The name of the topic.
 * @property {string} [type='persistent'] - The type of the topic (e.g., 'persistent', 'non-persistent').
 */
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

/**
 * A key representing the topic.
 * @returns {string} The topic key.
 */
const k = computed(() => `${props.type}://${props.tenant}/${props.namespace}/${props.topic}`)

/**
 * The producer for the current topic.
 * @returns {object|undefined} The producer object, or undefined if it doesn't exist.
 */
const producer = computed(() => pub.producers.get(k.value))

/**
 * The status of the producer.
 * @returns {string} The producer status (e.g., 'idle', 'connecting', 'open', 'error').
 */
const status = computed(() => producer.value?.status || 'idle')

/**
 * The number of messages sent by the producer.
 * @returns {number} The number of sent messages.
 */
const sent = computed(() => producer.value?.sent || 0)

/**
 * The number of acknowledgments received by the producer.
 * @returns {number} The number of acks.
 */
const acks = computed(() => producer.value?.acks || 0)

/**
 * The last error message from the producer.
 * @returns {string} The error message.
 */
const error = computed(() => producer.value?.error || '')

/**
 * Parses the properties text into a JSON object.
 * @returns {object|undefined} The parsed JSON object, or undefined if parsing fails.
 */
const parseProps = () => {
  if (!state.propertiesText) return undefined
  try {
    const obj = JSON.parse(state.propertiesText)
    if (obj && typeof obj === 'object' && !Array.isArray(obj)) return obj
  } catch { /* ignore */ }
  return undefined
}

/**
 * Connects the producer to the topic.
 * @returns {Promise<void>}
 */
const connect = async () => {
  await pub.startProducer({ type: props.type, tenant: props.tenant, namespace: props.namespace, topic: props.topic, producerName: state.producerName || undefined })
}

/**
 * Disconnects the producer from the topic.
 */
const disconnect = () => {
  pub.closeProducer({ type: props.type, tenant: props.tenant, namespace: props.namespace, topic: props.topic })
}

/**
 * Sends a message to the topic.
 * @returns {Promise<void>}
 */
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
