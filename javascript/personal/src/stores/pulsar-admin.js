import {ref, reactive} from 'vue'
import {defineStore} from 'pinia'
import {usePreferencesStore} from "@/stores/preferences.js";

export const usePulsarAdminStore = defineStore('pulsar-admin', () => {
        const preferencesStore = usePreferencesStore()
        preferencesStore.loadPreferences()

        //const pulsarUrl = preferencesStore.pulsarUrl

        const loadingStates = reactive(new Map())

        const isHealthy = ref(false)
        const clusters = ref([])
        const selectedCluster = ref(null)

        const checkHealth = async () => {
            loadingStates.set('health', true)

            const response = await fetch(`/admin/v2/brokers/health`)
            console.log(response)

            loadingStates.set('health', false)
            isHealthy.value = response.ok
        }

        const getClusters = async () => {
            loadingStates.set('clusters', true)

            const response = await fetch(`/admin/v2/clusters`)
            console.log(response)
            clusters.value = await response.json()

            if (clusters.value.length > 0 && !selectedCluster.value) {
                selectedCluster.value = clusters.value[0]
            }

            loadingStates.set('clusters', false)
        }

        const selectCluster = (cluster) => {
            selectedCluster.value = cluster
        }


        //init loading states
        loadingStates.set('health', false)
        loadingStates.set('clusters', false)

        return {
            loadingStates,
            isHealthy,
            clusters,
            selectedCluster,
            checkHealth,
            getClusters,
            selectCluster,
        }
    }
)
