import { ref } from 'vue'
import { defineStore } from 'pinia'
import { useFetchAdmin } from './core/api-client.js'
import { useLoadingState } from './core/loading-state.js'

/**
 * Pinia store for managing Pulsar tenants and namespaces.
 */
export const useTenantsStore = defineStore('tenants', () => {
    const { fetchAdmin } = useFetchAdmin()
    const { loadingStates, withLoading, initLoadingStates } = useLoadingState()

    // State
    const tenants = ref([])
    const selectedTenant = ref(null)
    const namespaces = ref([])
    const selectedNamespace = ref('All')

    // Initialize loading states
    initLoadingStates(['tenants', 'namespaces'])

    /**
     * Fetches the list of tenants from the Pulsar instance.
     * @returns {Promise<void>}
     */
    const getTenants = async () => {
        await withLoading('tenants', async () => {
            const res = await fetchAdmin('tenants')
            if (res.ok) {
                const list = await res.json()
                tenants.value = Array.isArray(list) ? list : []
            } else {
                tenants.value = []
            }
        })
    }

    /**
     * Sets the currently selected tenant and resets namespace/topic selections.
     * @param {string} tenant - The name of the tenant to select
     * @returns {Promise<void>}
     */
    const selectTenant = async (tenant) => {
        selectedTenant.value = tenant
        selectedNamespace.value = 'All'
        // Auto-fetch namespaces when tenant is selected
        if (tenant) {
            await getNamespaces(tenant)
        }
    }

    /**
     * Fetches the list of namespaces for a given tenant.
     * @param {string} [tenant] - The name of the tenant (defaults to selected tenant)
     * @returns {Promise<string[]>} The list of namespaces
     */
    const getNamespaces = async (tenant) => {
        const t = tenant || selectedTenant.value
        if (!t) {
            namespaces.value = []
            return []
        }

        return await withLoading('namespaces', async () => {
            const res = await fetchAdmin(`tenants/${encodeURIComponent(t)}/namespaces`)
            if (res.ok) {
                const list = await res.json()
                namespaces.value = Array.isArray(list) ? list : []
                return namespaces.value
            } else {
                namespaces.value = []
                return []
            }
        })
    }

    /**
     * Sets the currently selected namespace.
     * @param {string} ns - The name of the namespace to select
     */
    const selectNamespace = (ns) => {
        selectedNamespace.value = ns
    }

    return {
        // State
        loadingStates,
        tenants,
        selectedTenant,
        namespaces,
        selectedNamespace,
        // Actions
        getTenants,
        selectTenant,
        getNamespaces,
        selectNamespace,
    }
})
