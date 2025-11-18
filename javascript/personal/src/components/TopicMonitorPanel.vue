<script setup>
import {computed, reactive} from 'vue'
import {useMessageMonitorStore} from '@/stores/message-monitor.js'

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

const monitorStore = useMessageMonitorStore()
const subInputs = reactive({ sub: '' })

/**
 * Generates a default subscription name.
 * @returns {string} A unique subscription name.
 */
const defaultSubName = () => `admin-ui-${Date.now().toString(36)}-${Math.random().toString(36).slice(2,6)}`
if (!subInputs.sub) subInputs.sub = defaultSubName()

/**
 * A computed property that filters active monitors for the current topic.
 * @returns {Array} An array of active monitors for the current topic.
 */
const monitors = computed(() => monitorStore.active.filter(m => m.tenant === props.tenant && m.namespace === props.namespace && m.topic === props.topic && (props.type ? m.type === props.type : true)))

/**
 * Starts a new message monitor for the topic.
 * @returns {Promise<void>}
 */
const start = async () => {
  await monitorStore.startMonitor({ type: props.type, tenant: props.tenant, namespace: props.namespace, topic: props.topic, subscriptionName: subInputs.sub })
}

/**
 * Stops an active message monitor.
 * @param {string} subscriptionName - The name of the subscription to stop.
 */
const stop = (subscriptionName) => {
  monitorStore.stopMonitor({ type: props.type, tenant: props.tenant, namespace: props.namespace, topic: props.topic, subscriptionName })
}

/**
 * Clears all messages from a monitor's display.
 * @param {string} subscriptionName - The name of the subscription to clear messages from.
 */
const clear = (subscriptionName) => {
  monitorStore.clearMessages({ type: props.type, tenant: props.tenant, namespace: props.namespace, topic: props.topic, subscriptionName })
}

/**
 * Removes a subscription from the topic.
 * @param {string} subscriptionName - The name of the subscription to remove.
 * @returns {Promise<void>}
 */
const remove = async (subscriptionName) => {
  await monitorStore.removeSubscription({ type: props.type, tenant: props.tenant, namespace: props.namespace, topic: props.topic, subscriptionName, force: true })
}
</script>

<template>
  <div class="space-y-2">
    <div class="flex flex-wrap gap-2 items-end">
      <input type="text" class="input input-bordered input-sm w-64" placeholder="Subscription name" v-model="subInputs.sub" />
      <button class="btn btn-sm btn-primary" @click="start">Start</button>
    </div>
    <div v-for="m in monitors" :key="m.key" class="border rounded p-2 bg-base-100">
      <div class="flex items-start justify-between">
        <div class="flex flex-wrap gap-2 items-center">
          <span class="badge badge-outline">Sub: {{ m.subscriptionName }}</span>
          <span class="badge" :class="{ 'badge-success': m.status==='open', 'badge-warning': m.status==='connecting', 'badge-error': m.status==='error' }">{{ m.status }}</span>
        </div>
        <div class="flex gap-2">
          <button class="btn btn-xs" :disabled="m.status!=='open'" @click="stop(m.subscriptionName)">Stop</button>
          <button class="btn btn-xs" :disabled="!m.messages.length" @click="clear(m.subscriptionName)">Clear</button>
          <button class="btn btn-xs btn-error" @click="remove(m.subscriptionName)">Delete Subscription</button>
        </div>
      </div>
      <div class="mt-2 max-h-48 overflow-auto text-xs bg-base-200 p-2 rounded">
        <div v-if="m.messages.length === 0" class="opacity-70">No messages received</div>
        <div v-for="(msg, i) in m.messages.slice().reverse()" :key="i" class="mb-2">
          <div class="opacity-70">{{ new Date(msg.publishTime || msg.receivedAt).toLocaleString() }}</div>
          <pre class="whitespace-pre-wrap break-all">{{ msg.payload }}</pre>
          <div v-if="msg.key">Key: {{ msg.key }}</div>
          <div v-if="Object.keys(msg.properties||{}).length">Props: {{ JSON.stringify(msg.properties) }}</div>
        </div>
      </div>
    </div>
  </div>
</template>
