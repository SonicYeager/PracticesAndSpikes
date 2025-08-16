<script setup>
import {computed, onMounted, reactive, ref} from 'vue'
import {usePulsarAdminStore} from '@/stores/pulsar-admin.js'
import TopicMonitorPanel from '@/components/TopicMonitorPanel.vue'
import TopicPublisherPanel from '@/components/TopicPublisherPanel.vue'

const store = usePulsarAdminStore()

const isTenantsLoading = computed(() => store.loadingStates.get('tenants'))
const isNamespacesLoading = computed(() => store.loadingStates.get('namespaces'))
const isTopicsLoading = computed(() => store.loadingStates.get('topics'))

// Local UI state
const expanded = reactive(new Map()) // key: fqdn -> boolean
const permCache = reactive(new Map()) // key: fqdn -> permissions object

const addTopicOpen = ref(false)
const addForm = ref({
  tenant: '',
  namespace: '',
  topic: '',
  retentionTimeInMinutes: '',
  retentionSizeInMB: '',
  role: '',
  actions: '', // csv
})

onMounted(async () => {
  if (!store.tenants.length) {
    await store.getTenants()
  }
  const tenant = store.selectedTenant || (store.tenants[0] || null)
  if (tenant) {
    if (!store.selectedTenant) {
      await store.selectTenant(tenant)
    }
    await store.getNamespaces(tenant)
    await store.getAllTopicsForTenant(tenant)
  }
})

const handleTenantSelect = async (tenant) => {
  await store.selectTenant(tenant)
  await store.getNamespaces(tenant)
  await store.getAllTopicsForTenant(tenant)
  addForm.value.tenant = tenant
}

const handleNamespaceSelect = async (ns) => {
  store.selectNamespace(ns)
}

const toggleExpand = async (t) => {
  const key = t.fqdn
  const now = !expanded.get(key)
  expanded.set(key, now)
  if (now && !t.details) {
    await store.fetchTopicDetails(t.tenant, t.namespace, t.topic)
    // load permissions too
    const perms = await store.getPermissions({ tenant: t.tenant, namespace: t.namespace, topic: t.topic })
    permCache.set(key, perms)
  }
}

const publishersFor = (t) => {
  const stats = t.details?.stats
  return Array.isArray(stats?.publishers) ? stats.publishers : []
}

const consumersFor = (t) => {
  const stats = t.details?.stats
  if (!stats || !stats.subscriptions) return []
  const list = []
  for (const [sub, subObj] of Object.entries(stats.subscriptions)) {
    if (Array.isArray(subObj.consumers)) {
      subObj.consumers.forEach(c => list.push({ subscription: sub, ...c }))
    }
  }
  return list
}

const doDelete = async (t) => {
  await store.deleteTopic({ tenant: t.tenant, namespace: t.namespace, topic: t.topic, force: true })
}


const retentionInputs = reactive(new Map()) // fqdn -> {time, size}

const ensureRetentionInputs = (t) => {
  const r = retentionInputs.get(t.fqdn)
  if (!r) {
    retentionInputs.set(t.fqdn, { time: '', size: '' })
  }
}

const saveRetention = async (t) => {
  ensureRetentionInputs(t)
  const r = retentionInputs.get(t.fqdn)
  const retention = {
    retentionTimeInMinutes: r.time === '' ? -1 : Number(r.time),
    retentionSizeInMB: r.size === '' ? -1 : Number(r.size),
  }
  await store.updateRetention({ tenant: t.tenant, namespace: t.namespace, topic: t.topic, retention })
}

const grantPerm = async (t, role, actionsCsv) => {
  const actions = actionsCsv.split(',').map(a => a.trim()).filter(Boolean)
  await store.grantPermissions({ tenant: t.tenant, namespace: t.namespace, topic: t.topic, role, actions })
  const perms = await store.getPermissions({ tenant: t.tenant, namespace: t.namespace, topic: t.topic })
  permCache.set(t.fqdn, perms)
}

const revokePerm = async (t, role) => {
  await store.revokePermissions({ tenant: t.tenant, namespace: t.namespace, topic: t.topic, role })
  const perms = await store.getPermissions({ tenant: t.tenant, namespace: t.namespace, topic: t.topic })
  permCache.set(t.fqdn, perms)
}

const openAdd = () => {
  addForm.value = {
    tenant: store.selectedTenant || '',
    namespace: store.selectedNamespace && store.selectedNamespace !== 'All' ? store.selectedNamespace : (store.namespaces[0] || ''),
    topic: '',
    retentionTimeInMinutes: '',
    retentionSizeInMB: '',
    role: '',
    actions: '',
  }
  addTopicOpen.value = true
}

const createTopic = async () => {
  const payload = {
    tenant: addForm.value.tenant,
    namespace: addForm.value.namespace,
    topic: addForm.value.topic,
    retention: {
      retentionTimeInMinutes: addForm.value.retentionTimeInMinutes === '' ? undefined : Number(addForm.value.retentionTimeInMinutes),
      retentionSizeInMB: addForm.value.retentionSizeInMB === '' ? undefined : Number(addForm.value.retentionSizeInMB),
    },
    permissions: addForm.value.role && addForm.value.actions ? [
      { role: addForm.value.role, actions: addForm.value.actions.split(',').map(a => a.trim()).filter(Boolean) }
    ] : [],
  }
  await store.createTopic(payload)
  addTopicOpen.value = false
}

</script>

<template>
  <main class="flex justify-center items-start min-h-screen p-4 pt-32">
    <div class="flex flex-col gap-4 w-full max-w-6xl">
      <div class="card bg-base-100 shadow-md">
        <div class="card-body">
          <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
            <div>
              <h2 class="card-title text-2xl">Topics</h2>
              <p class="text-sm opacity-70">Manage topics across namespaces for your selected tenant.</p>
            </div>
            <div class="flex gap-2">
              <!-- Tenant selector -->
              <details class="dropdown">
                <summary class="btn" :class="{ 'skeleton w-40': isTenantsLoading }">
                  {{ isTenantsLoading ? '' : (store.selectedTenant || 'Select Tenant') }}
                </summary>
                <ul class="menu dropdown-content bg-base-100 rounded-box z-1 w-56 p-2 shadow">
                  <li v-if="!isTenantsLoading && store.tenants.length === 0"><span>No tenants</span></li>
                  <li v-for="t in store.tenants" :key="t">
                    <a @click="handleTenantSelect(t)" :class="{ 'active': store.selectedTenant === t }">{{ t }}</a>
                  </li>
                </ul>
              </details>
              <!-- Namespace selector -->
              <details class="dropdown">
                <summary class="btn" :class="{ 'skeleton w-40': isNamespacesLoading }">
                  {{ isNamespacesLoading ? '' : (store.selectedNamespace || 'All') }}
                </summary>
                <ul class="menu dropdown-content bg-base-100 rounded-box z-1 w-56 p-2 shadow">
                  <li><a @click="handleNamespaceSelect('All')">All</a></li>
                  <li v-for="ns in store.namespaces" :key="ns">
                    <a @click="handleNamespaceSelect(ns.includes('/') ? ns.split('/')[1] : ns)" :class="{ 'active': (store.selectedNamespace === (ns.includes('/') ? ns.split('/')[1] : ns)) }">
                      {{ ns.includes('/') ? ns.split('/')[1] : ns }}
                    </a>
                  </li>
                </ul>
              </details>
              <button class="btn btn-primary" @click="openAdd">Add Topic</button>
            </div>
          </div>
        </div>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        <template v-if="isTopicsLoading">
          <div v-for="n in 6" :key="n" class="card bg-base-100 shadow">
            <div class="card-body">
              <div class="skeleton h-4 w-1/2 mb-2"></div>
              <div class="skeleton h-4 w-1/3 mb-4"></div>
              <div class="skeleton h-24 w-full"></div>
            </div>
          </div>
        </template>
        <template v-else>
          <div v-if="store.filteredTopics.length === 0" class="col-span-full">
            <div class="alert">
              <span>No topics found for the selected tenant/namespace.</span>
            </div>
          </div>
          <div v-for="t in store.filteredTopics" :key="t.fqdn" class="card bg-base-100 shadow">
            <div class="card-body">
              <div class="flex items-start justify-between">
                <div>
                  <h3 class="card-title text-lg break-all">{{ t.fqdn }}</h3>
                  <p class="text-xs opacity-70">Tenant: {{ t.tenant }} | Namespace: {{ t.namespace }}</p>
                </div>
                <div class="flex gap-2">
                  <button class="btn btn-sm" @click="toggleExpand(t)">
                    {{ expanded.get(t.fqdn) ? 'Hide' : 'Details' }}
                  </button>
                  <button class="btn btn-sm btn-error" @click="doDelete(t)">Delete</button>
                </div>
              </div>

              <div v-if="expanded.get(t.fqdn)" class="mt-3 space-y-3">
                <div class="divider">Schema</div>
                <div v-if="t.details?.schema" class="text-sm">
                  <div>Type: <span class="badge">{{ t.details.schema.type }}</span></div>
                  <pre class="mt-2 whitespace-pre-wrap break-all bg-base-200 p-2 rounded max-h-40 overflow-auto">{{ t.details.schema.schema }}</pre>
                </div>
                <div v-else class="text-sm opacity-70">No schema or not loaded.</div>

                <div class="divider">Publishers</div>
                <div v-if="publishersFor(t).length">
                  <ul class="list-disc list-inside text-sm">
                    <li v-for="(p, idx) in publishersFor(t)" :key="idx">{{ p.producerName || p.producerId || 'Producer' }} (msgRateOut: {{ p.msgRateOut?.toFixed?.(2) ?? p.msgRateOut ?? 0 }})</li>
                  </ul>
                </div>
                <div v-else class="text-sm opacity-70">No publishers</div>

                <div class="divider">Consumers</div>
                <div v-if="consumersFor(t).length">
                  <ul class="list-disc list-inside text-sm">
                    <li v-for="(c, idx) in consumersFor(t)" :key="idx">{{ c.consumerName || 'Consumer' }} (sub: {{ c.subscription }}, msgRateIn: {{ c.msgRateIn?.toFixed?.(2) ?? c.msgRateIn ?? 0 }})</li>
                  </ul>
                </div>
                <div v-else class="text-sm opacity-70">No consumers</div>

                <div class="divider">Retention</div>
                <div class="flex flex-wrap gap-2 items-end">
                  <div class="form-control">
                    <label class="label"><span class="label-text">Time (min)</span></label>
                    <input type="number" class="input input-bordered input-sm w-36" v-model="(retentionInputs.get(t.fqdn) ?? (ensureRetentionInputs(t), retentionInputs.get(t.fqdn))).time" placeholder="-1 (infinite)" />
                  </div>
                  <div class="form-control">
                    <label class="label"><span class="label-text">Size (MB)</span></label>
                    <input type="number" class="input input-bordered input-sm w-36" v-model="(retentionInputs.get(t.fqdn) ?? (ensureRetentionInputs(t), retentionInputs.get(t.fqdn))).size" placeholder="-1 (infinite)" />
                  </div>
                  <button class="btn btn-sm btn-primary" @click="saveRetention(t)">Save</button>
                </div>

                <div class="divider">Permissions</div>
                <div class="space-y-2">
                  <div v-if="Object.keys(permCache.get(t.fqdn) || {}).length">
                    <div v-for="(acts, role) in permCache.get(t.fqdn)" :key="role" class="flex items-center gap-2">
                      <span class="badge badge-outline">{{ role }}</span>
                      <span class="text-sm">{{ Array.isArray(acts) ? acts.join(', ') : JSON.stringify(acts) }}</span>
                      <button class="btn btn-xs btn-error" @click="revokePerm(t, role)">Revoke</button>
                    </div>
                  </div>
                  <div v-else class="text-sm opacity-70">No explicit permissions</div>
                  <div class="flex flex-wrap gap-2 items-end mt-2">
                    <input type="text" class="input input-bordered input-sm w-40" placeholder="Role" v-model="(permCache.get(t.fqdn + ':new') || (permCache.set(t.fqdn + ':new', {role:'',actions:''}), permCache.get(t.fqdn + ':new'))).role" />
                    <input type="text" class="input input-bordered input-sm w-64" placeholder="Actions (e.g. produce,consume)" v-model="(permCache.get(t.fqdn + ':new') || (permCache.set(t.fqdn + ':new', {role:'',actions:''}), permCache.get(t.fqdn + ':new'))).actions" />
                    <button class="btn btn-xs btn-primary" @click="grantPerm(t, permCache.get(t.fqdn + ':new').role, permCache.get(t.fqdn + ':new').actions)">Grant</button>
                  </div>
                </div>

                <div class="divider">Test Publisher</div>
                <TopicPublisherPanel :tenant="t.tenant" :namespace="t.namespace" :topic="t.topic" :type="t.type || 'persistent'" />

                <div class="divider">Monitor</div>
                <TopicMonitorPanel :tenant="t.tenant" :namespace="t.namespace" :topic="t.topic" :type="t.type || 'persistent'" />
              </div>
            </div>
          </div>
        </template>
      </div>

      <!-- Add Topic Modal -->
      <dialog class="modal" :open="addTopicOpen">
        <div class="modal-box">
          <h3 class="font-bold text-lg mb-2">Create Topic</h3>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-3">
            <div class="form-control">
              <label class="label"><span class="label-text">Tenant</span></label>
              <select class="select select-bordered" v-model="addForm.tenant">
                <option value="" disabled>Select tenant</option>
                <option v-for="t in store.tenants" :key="t" :value="t">{{ t }}</option>
              </select>
            </div>
            <div class="form-control">
              <label class="label"><span class="label-text">Namespace</span></label>
              <select class="select select-bordered" v-model="addForm.namespace">
                <option value="" disabled>Select namespace</option>
                <option v-for="ns in store.namespaces" :key="ns" :value="(ns.includes('/') ? ns.split('/')[1] : ns)">{{ ns.includes('/') ? ns.split('/')[1] : ns }}</option>
              </select>
            </div>
            <div class="form-control md:col-span-2">
              <label class="label"><span class="label-text">Topic</span></label>
              <input class="input input-bordered" v-model="addForm.topic" placeholder="topic-name" />
            </div>
            <div class="form-control">
              <label class="label"><span class="label-text">Retention Time (min)</span></label>
              <input type="number" class="input input-bordered" v-model="addForm.retentionTimeInMinutes" placeholder="optional" />
            </div>
            <div class="form-control">
              <label class="label"><span class="label-text">Retention Size (MB)</span></label>
              <input type="number" class="input input-bordered" v-model="addForm.retentionSizeInMB" placeholder="optional" />
            </div>
            <div class="form-control">
              <label class="label"><span class="label-text">Initial Role</span></label>
              <input class="input input-bordered" v-model="addForm.role" placeholder="optional" />
            </div>
            <div class="form-control">
              <label class="label"><span class="label-text">Initial Actions</span></label>
              <input class="input input-bordered" v-model="addForm.actions" placeholder="e.g. produce,consume" />
            </div>
          </div>
          <div class="modal-action">
            <button class="btn" @click="addTopicOpen = false">Cancel</button>
            <button class="btn btn-primary" :disabled="!addForm.tenant || !addForm.namespace || !addForm.topic" @click="createTopic">Create</button>
          </div>
        </div>
      </dialog>
    </div>
  </main>
</template>
