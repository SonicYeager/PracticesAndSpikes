<script setup>
import {computed, onMounted} from 'vue'
import {usePulsarAdminStore} from "@/stores/pulsar-admin.js";
import {usePreferencesStore} from "@/stores/preferences.js";

const pulsarAdminStore = usePulsarAdminStore()
const preferencesStore = usePreferencesStore()

/**
 * A computed property that returns a CSS class for the health badge based on the cluster's health status.
 * @returns {string} 'badge-success' if healthy, 'badge-error' otherwise.
 */
const healthBadge = computed(() =>
    pulsarAdminStore.isHealthy ? 'badge-success' : 'badge-error'
)
/**
 * A computed property that returns a CSS class to show a skeleton loader while health is being checked.
 * @returns {string} 'skeleton' if loading, '' otherwise.
 */
const isHealthLoadingClass = computed(() =>
    pulsarAdminStore.loadingStates.get('health') ? 'skeleton ' : ''
)

/**
 * A computed property that indicates whether the list of clusters is currently being loaded.
 * @returns {boolean}
 */
const isClustersLoading = computed(() =>
    pulsarAdminStore.loadingStates.get('clusters')
)

onMounted(async () => {
  preferencesStore.loadPreferences()
  await pulsarAdminStore.checkHealth()
  await pulsarAdminStore.getClusters()
})

/**
 * Handles the selection of a cluster from the dropdown menu.
 * @param {string} cluster - The name of the selected cluster.
 */
const handleClusterSelect = (cluster) => {
  pulsarAdminStore.selectCluster(cluster)
}

</script>

<template>
  <main class="flex justify-center items-start min-h-screen p-4 pt-32">

    <div class="flex flex-col gap-2 w-full max-w-2xl">
      <div class="card shadow-lg bg-base-100">
        <div class="card-body">
          <div class="flex items-center justify-between">
            <div class="flex flex-col">
              <h1 class="flex card-title text-3xl">Dashboard</h1>
              <div class="flex gap-1">
                <p class="badge">{{ preferencesStore.pulsarUrl }}</p>
                <div :class="['badge w-16', healthBadge, isHealthLoadingClass]">
                  {{ pulsarAdminStore.loadingStates.get('health') ? '' : pulsarAdminStore.isHealthy ? 'Healthy' : 'Unhealthy' }}
                </div>
              </div>
            </div>
            <div class="flex-shrink-0">
              <details class="dropdown dropdown-end">
                <summary class="btn" :class="{ 'skeleton w-20': isClustersLoading}">
                  {{
                    isClustersLoading
                        ? ''
                        : pulsarAdminStore.selectedCluster || 'Select Cluster'
                  }}
                </summary>

                <ul class="menu dropdown-content bg-base-100 rounded-box z-1 w-52 p-2 shadow-sm">
                  <!-- Loading Skeletons -->
                  <template v-if="isClustersLoading">
                    <li v-for="n in 3" :key="`skeleton-${n}`">
                      <div class="skeleton h-8 w-full"></div>
                    </li>
                  </template>

                  <!-- Actual Clusters -->
                  <template v-else>
                    <li v-if="pulsarAdminStore.clusters.length === 0">
                      <span class="text-gray-500">No clusters found</span>
                    </li>

                    <li v-for="cluster in pulsarAdminStore.clusters" :key="cluster">
                      <a
                          @click="handleClusterSelect(cluster)"
                          :class="{ 'active': pulsarAdminStore.selectedCluster === cluster }"
                      >
                        {{ cluster }}
                      </a>
                    </li>
                  </template>
                </ul>
              </details>
            </div>
          </div>
        </div>
      </div>
    </div>
  </main>
</template>