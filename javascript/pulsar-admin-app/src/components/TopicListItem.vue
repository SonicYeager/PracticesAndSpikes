<template>
  <div class="card bg-base-100 shadow">
    <div class="card-body">
      <div class="flex items-start justify-between">
        <div>
          <h3 class="card-title text-lg break-all">{{ topic.fqdn }}</h3>
          <p class="text-xs opacity-70">Tenant: {{ topic.tenant }} | Namespace: {{ topic.namespace }}</p>
        </div>
        <div class="flex gap-2">
          <button class="btn btn-sm" @click="toggleExpand">
            {{ isExpanded ? 'Hide' : 'Details' }}
          </button>
          <button class="btn btn-sm btn-error" @click="$emit('delete', topic)">Delete</button>
        </div>
      </div>

      <div v-if="isExpanded" class="mt-3 space-y-3">
        <div class="divider">Schema</div>
        <div v-if="topic.details?.schema" class="text-sm">
          <div>Type: <span class="badge">{{ topic.details.schema.type }}</span></div>
          <pre class="mt-2 whitespace-pre-wrap break-all bg-base-200 p-2 rounded max-h-40 overflow-auto">{{ topic.details.schema.schema }}</pre>
        </div>
        <div v-else class="text-sm opacity-70">No schema or not loaded.</div>

        <div class="divider">Publishers</div>
        <div v-if="publishers.length">
          <ul class="list-disc list-inside text-sm">
            <li v-for="(p, idx) in publishers" :key="idx">{{ p.producerName || p.producerId || 'Producer' }} (msgRateOut: {{ p.msgRateOut?.toFixed?.(2) ?? p.msgRateOut ?? 0 }})</li>
          </ul>
        </div>
        <div v-else class="text-sm opacity-70">No publishers</div>

        <div class="divider">Consumers</div>
        <div v-if="consumers.length">
          <ul class="list-disc list-inside text-sm">
            <li v-for="(c, idx) in consumers" :key="idx">{{ c.consumerName || 'Consumer' }} (sub: {{ c.subscription }}, msgRateIn: {{ c.msgRateIn?.toFixed?.(2) ?? c.msgRateIn ?? 0 }})</li>
          </ul>
        </div>
        <div v-else class="text-sm opacity-70">No consumers</div>

        <div class="divider">Retention</div>
        <div class="flex flex-wrap gap-2 items-end">
          <div class="form-control">
            <label class="label"><span class="label-text">Time (min)</span></label>
            <input type="number" class="input input-bordered input-sm w-36" v-model="retentionInput.time" placeholder="-1 (infinite)" />
          </div>
          <div class="form-control">
            <label class="label"><span class="label-text">Size (MB)</span></label>
            <input type="number" class="input input-bordered input-sm w-36" v-model="retentionInput.size" placeholder="-1 (infinite)" />
          </div>
          <button class="btn btn-sm btn-primary" @click="saveRetention">Save</button>
        </div>

        <div class="divider">Permissions</div>
        <div class="space-y-2">
          <div v-if="Object.keys(permissions).length">
            <div v-for="(acts, role) in permissions" :key="role" class="flex items-center gap-2">
              <span class="badge badge-outline">{{ role }}</span>
              <span class="text-sm">{{ Array.isArray(acts) ? acts.join(', ') : JSON.stringify(acts) }}</span>
              <button class="btn btn-xs btn-error" @click="revokePerm(role)">Revoke</button>
            </div>
          </div>
          <div v-else class="text-sm opacity-70">No explicit permissions</div>
          <div class="flex flex-wrap gap-2 items-end mt-2">
            <input type="text" class="input input-bordered input-sm w-40" placeholder="Role" v-model="newPerm.role" />
            
            <div class="dropdown dropdown-top">
              <div tabindex="0" role="button" class="btn btn-sm btn-outline m-1">Actions</div>
              <ul tabindex="0" class="dropdown-content z-[1] menu p-2 shadow bg-base-100 rounded-box w-52">
                <li v-for="act in ['produce', 'consume', 'functions']" :key="act">
                  <label class="label cursor-pointer">
                    <span class="label-text">{{ act }}</span> 
                    <input type="checkbox" class="checkbox checkbox-xs" :value="act" v-model="newPerm.actions" />
                  </label>
                </li>
              </ul>
            </div>

            <button class="btn btn-xs btn-primary" @click="grantPerm">Grant</button>
          </div>
        </div>

        <div class="divider">Test Publisher</div>
        <TopicPublisherPanel :tenant="topic.tenant" :namespace="topic.namespace" :topic="topic.topic" :type="topic.type || 'persistent'" />

        <div class="divider">Monitor</div>
        <TopicMonitorPanel :tenant="topic.tenant" :namespace="topic.namespace" :topic="topic.topic" :type="topic.type || 'persistent'" />
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, reactive } from 'vue'
import { usePulsarAdminStore } from '@/stores/pulsar-admin.js'
import { useNotificationStore } from '@/stores/notification.js'
import TopicMonitorPanel from '@/components/TopicMonitorPanel.vue'
import TopicPublisherPanel from '@/components/TopicPublisherPanel.vue'

const props = defineProps({
  topic: {
    type: Object,
    required: true
  }
})

const emit = defineEmits(['delete'])

const store = usePulsarAdminStore()
const notificationStore = useNotificationStore()

const isExpanded = ref(false)
const permissions = ref({})
const retentionInput = reactive({ time: '', size: '' })
const newPerm = reactive({ role: '', actions: [] })

const publishers = computed(() => {
  const stats = props.topic.details?.stats
  return Array.isArray(stats?.publishers) ? stats.publishers : []
})

const consumers = computed(() => {
  const stats = props.topic.details?.stats
  if (!stats || !stats.subscriptions) return []
  const list = []
  for (const [sub, subObj] of Object.entries(stats.subscriptions)) {
    if (Array.isArray(subObj.consumers)) {
      subObj.consumers.forEach(c => list.push({ subscription: sub, ...c }))
    }
  }
  return list
})

const toggleExpand = async () => {
  isExpanded.value = !isExpanded.value
  if (isExpanded.value && !props.topic.details) {
    await store.fetchTopicDetails(props.topic.tenant, props.topic.namespace, props.topic.topic)
    permissions.value = await store.getPermissions({ 
      tenant: props.topic.tenant, 
      namespace: props.topic.namespace, 
      topic: props.topic.topic 
    })
  }
}

const saveRetention = async () => {
  const retention = {
    retentionTimeInMinutes: retentionInput.time === '' ? -1 : Number(retentionInput.time),
    retentionSizeInMB: retentionInput.size === '' ? -1 : Number(retentionInput.size),
  }
  try {
    await store.updateRetention({ 
      tenant: props.topic.tenant, 
      namespace: props.topic.namespace, 
      topic: props.topic.topic, 
      retention 
    })
    notificationStore.success('Retention policy updated')
  } catch (e) {
    // error handled by store
  }
}

const grantPerm = async () => {
  if (!newPerm.role || !newPerm.actions || newPerm.actions.length === 0) {
    notificationStore.warning('Please specify role and at least one action')
    return
  }
  try {
    await store.grantPermissions({ 
      tenant: props.topic.tenant, 
      namespace: props.topic.namespace, 
      topic: props.topic.topic, 
      role: newPerm.role, 
      actions: newPerm.actions 
    })
    permissions.value = await store.getPermissions({ 
      tenant: props.topic.tenant, 
      namespace: props.topic.namespace, 
      topic: props.topic.topic 
    })
    notificationStore.success(`Permissions granted to ${newPerm.role}`)
    newPerm.role = ''
    newPerm.actions = []
  } catch (e) {
    // error handled by store
  }
}

const revokePerm = async (role) => {
  try {
    await store.revokePermissions({ 
      tenant: props.topic.tenant, 
      namespace: props.topic.namespace, 
      topic: props.topic.topic, 
      role 
    })
    permissions.value = await store.getPermissions({ 
      tenant: props.topic.tenant, 
      namespace: props.topic.namespace, 
      topic: props.topic.topic 
    })
    notificationStore.success(`Permissions revoked for ${role}`)
  } catch (e) {
    // error handled by store
  }
}
</script>
