import { ref } from 'vue'
import { defineStore } from 'pinia'
import { useFetchAdmin } from './core/api-client.js'
import { useLoadingState } from './core/loading-state.js'

/**
 * Pinia store for managing Pulsar clusters and health checks.
 */
export const useClustersStore = defineStore('clusters', () => {
    const { fetchAdmin } = useFetchAdmin()
    const { loadingStates, withLoading, initLoadingStates } = useLoadingState()

    // State
    const isHealthy = ref(false)
    const clusters = ref([])
    const selectedCluster = ref(null)

    // Initialize loading states
    initLoadingStates(['health', 'clusters'])

    /**
     * Checks the health of the Pulsar cluster.
     * @returns {Promise<void>}
     */
    const checkHealth = async () => {
        await withLoading('health', async () => {
            try {
                const response = await fetchAdmin('broker-stats/health')
                isHealthy.value = !!response && response.ok
            } catch (e) {
                isHealthy.value = false
            }
        })
    }

    /**
     * Fetches the list of clusters from the Pulsar instance.
     * @returns {Promise<void>}
     */
    const getClusters = async () => {
        await withLoading('clusters', async () => {
            const res = await fetchAdmin('clusters')
            if (res.ok) {
                const list = await res.json()
                clusters.value = Array.isArray(list) ? list : []
            } else {
                clusters.value = []
            }
        })
    }

    /**
     * Sets the currently selected cluster.
     * @param {string} cluster - The name of the cluster to select
     */
    const selectCluster = (cluster) => {
        selectedCluster.value = cluster
    }

    return {
        // State
        loadingStates,
        isHealthy,
        clusters,
        selectedCluster,
        // Actions
        checkHealth,
        getClusters,
        selectCluster,
    }
})
