import { defineStore } from 'pinia'
import { useClustersStore } from './clusters.js'
import { useTenantsStore } from './tenants.js'
import { useTopicsStore } from './topics.js'
import { usePermissionsStore } from './permissions.js'
import { computed } from 'vue'

/**
 * Main Pulsar Admin store - acts as a facade that re-exports all domain stores.
 * This maintains backward compatibility while allowing the use of refactored stores.
 * 
 * Components can now use either:
 * - `usePulsarAdminStore()` (backward compatible)
 * - Individual domain stores for more explicit dependencies
 */
export const usePulsarAdminStore = defineStore('pulsar-admin', () => {
    // Import all domain stores
    const clustersStore = useClustersStore()
    const tenantsStore = useTenantsStore()
    const topicsStore = useTopicsStore()
    const permissionsStore = usePermissionsStore()

    // Merge all loading states into a single map for backward compatibility
    const loadingStates = computed(() => {
        const merged = new Map()

        // Copy all loading states from domain stores
        clustersStore.loadingStates.forEach((value, key) => merged.set(key, value))
        tenantsStore.loadingStates.forEach((value, key) => merged.set(key, value))
        topicsStore.loadingStates.forEach((value, key) => merged.set(key, value))
        permissionsStore.loadingStates.forEach((value, key) => merged.set(key, value))

        return merged
    })

    // Re-export everything for backward compatibility
    return {
        // Merged loading states
        loadingStates,

        // Clusters store - direct refs for writable properties
        isHealthy: clustersStore.isHealthy,
        clusters: clustersStore.clusters,
        selectedCluster: clustersStore.selectedCluster,
        checkHealth: clustersStore.checkHealth,
        getClusters: clustersStore.getClusters,
        selectCluster: clustersStore.selectCluster,

        // Tenants store - direct refs for writable properties
        tenants: tenantsStore.tenants,
        selectedTenant: tenantsStore.selectedTenant,
        namespaces: tenantsStore.namespaces,
        selectedNamespace: tenantsStore.selectedNamespace,
        getTenants: tenantsStore.getTenants,
        selectTenant: tenantsStore.selectTenant,
        getNamespaces: tenantsStore.getNamespaces,
        selectNamespace: tenantsStore.selectNamespace,

        // Topics store - direct refs for writable properties
        topics: topicsStore.topics,
        filteredTopics: topicsStore.filteredTopics,
        parseTopicFqdn: topicsStore.parseTopicFqdn,
        getTopicsForNamespace: topicsStore.getTopicsForNamespace,
        getAllTopicsForTenant: topicsStore.getAllTopicsForTenant,
        fetchTopicDetails: topicsStore.fetchTopicDetails,
        createTopic: topicsStore.createTopic,
        deleteTopic: topicsStore.deleteTopic,
        updateRetention: topicsStore.updateRetention,

        // Permissions store
        getPermissions: permissionsStore.getPermissions,
        grantPermissions: permissionsStore.grantPermissions,
        revokePermissions: permissionsStore.revokePermissions,
        grantNamespacePermissions: permissionsStore.grantNamespacePermissions,
    }
})
