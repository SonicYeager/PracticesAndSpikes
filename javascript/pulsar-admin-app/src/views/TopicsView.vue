<script setup>
import {computed, onMounted, ref} from 'vue'
import {usePulsarAdminStore} from '@/stores/pulsar-admin.js'
import {useNotificationStore} from '@/stores/notification.js'
import TopicCreateModal from '@/components/TopicCreateModal.vue'
import TopicListItem from '@/components/TopicListItem.vue'

const store = usePulsarAdminStore()
const notificationStore = useNotificationStore()

const isTenantsLoading = computed(() => store.loadingStates.get('tenants'))
const isNamespacesLoading = computed(() => store.loadingStates.get('namespaces'))
const isTopicsLoading = computed(() => store.loadingStates.get('topics'))

const addTopicOpen = ref(false)

onMounted(async () => {
  try {
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
  } catch (e) {
    console.error('Error loading initial data:', e)
    notificationStore.error('Failed to load initial data')
  }
})

const handleTenantSelect = async (tenant) => {
  await store.selectTenant(tenant)
  await store.getNamespaces(tenant)
  await store.getAllTopicsForTenant(tenant)
}

const handleNamespaceSelect = async (ns) => {
  store.selectNamespace(ns)
}

const doDelete = async (t) => {
  if (!confirm(`Are you sure you want to delete topic ${t.fqdn}?`)) return
  try {
    await store.deleteTopic({ tenant: t.tenant, namespace: t.namespace, topic: t.topic, force: true })
    notificationStore.success(`Topic ${t.topic} deleted`)
  } catch (e) {
    // handled by store or here if needed
  }
}

const openAdd = () => {
  addTopicOpen.value = true
}

const handleCreate = async (formData) => {
  const ns = formData.namespace || store.selectedNamespace
  const localNs = ns.includes('/') ? ns.split('/')[1] : ns
  
  try {
    await store.createTopic({
      tenant: formData.tenant || store.selectedTenant,
      namespace: localNs,
      topic: formData.topic,
      retention: {
        retentionTimeInMinutes: formData.retentionTimeInMinutes === '' ? undefined : Number(formData.retentionTimeInMinutes),
        retentionSizeInMB: formData.retentionSizeInMB === '' ? undefined : Number(formData.retentionSizeInMB),
      },
      permissions: formData.role && formData.actions.length ? [
        { role: formData.role, actions: formData.actions }
      ] : [],
    })
    notificationStore.success(`Topic ${formData.topic} created successfully`)
    addTopicOpen.value = false
  } catch (e) {
    // error handled by store
  }
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
          <TopicListItem 
            v-for="t in store.filteredTopics" 
            :key="t.fqdn" 
            :topic="t" 
            @delete="doDelete" 
          />
        </template>
      </div>

      <TopicCreateModal 
        v-model="addTopicOpen"
        :tenant="store.selectedTenant"
        :namespaces="store.namespaces.map(ns => ns.includes('/') ? ns.split('/')[1] : ns)"
        :selectedNamespace="store.selectedNamespace"
        @create="handleCreate"
      />
    </div>
  </main>
</template>
