import { ref, reactive, computed } from 'vue'
import { defineStore } from 'pinia'
import { useFetchAdmin } from './core/api-client.js'
import { useLoadingState } from './core/loading-state.js'
import { useTenantsStore } from './tenants.js'

/**
 * Pinia store for managing Pulsar topics.
 */
export const useTopicsStore = defineStore('topics', () => {
    const { fetchAdmin } = useFetchAdmin()
    const { loadingStates, withLoading, initLoadingStates } = useLoadingState()
    const tenantsStore = useTenantsStore()

    // State
    const topics = ref([])
    const topicDetailsMap = reactive(new Map())

    // Initialize loading states
    initLoadingStates(['topics'])

    /**
     * Parses a fully qualified topic name (FQDN) into its components.
     * @param {string} fqdn - The FQDN (e.g., 'persistent://tenant/namespace/topic')
     * @returns {object|null} An object with topic components, or null if parsing fails
     */
    const parseTopicFqdn = (fqdn) => {
        const match = fqdn.match(/^(persistent|non-persistent):\/\/([^/]+)\/([^/]+)\/(.+)$/)
        if (!match) return null
        return { type: match[1], tenant: match[2], namespace: match[3], topic: match[4], fqdn }
    }

    /**
     * Fetches topics for a specific namespace.
     * @param {string} tenant - The tenant name
     * @param {string} ns - The namespace name
     * @returns {Promise<object[]>} Array of topic objects
     */
    const getTopicsForNamespace = async (tenant, ns) => {
        const results = []

        for (const type of ['persistent', 'non-persistent']) {
            const res = await fetchAdmin(`${type}/${encodeURIComponent(tenant)}/${encodeURIComponent(ns)}`)
            if (res.ok) {
                const list = await res.json()
                if (Array.isArray(list)) {
                    list.forEach(fqdn => {
                        const parsed = parseTopicFqdn(fqdn)
                        if (parsed) results.push(parsed)
                    })
                }
            }
        }

        return results
    }

    /**
     * Fetches all topics for a tenant.
     * @param {string} [tenant] - The tenant name (defaults to selected tenant)
     * @returns {Promise<object[]>} Array of all topics
     */
    const getAllTopicsForTenant = async (tenant) => {
        const t = tenant || tenantsStore.selectedTenant
        if (!t) {
            topics.value = []
            return []
        }

        return await withLoading('topics', async () => {
            const nsList = tenantsStore.namespaces.length > 0
                ? tenantsStore.namespaces
                : await tenantsStore.getNamespaces(t)

            const allTopics = []
            for (const ns of nsList) {
                const localNs = ns.includes('/') ? ns.split('/')[1] : ns
                const topicsForNs = await getTopicsForNamespace(t, localNs)
                allTopics.push(...topicsForNs)
            }

            topics.value = allTopics
            return allTopics
        })
    }

    /**
     * Fetches details (schema and stats) for a topic.
     * @param {string} tenant - The tenant name
     * @param {string} ns - The namespace name
     * @param {string} topic - The topic name
     * @returns {Promise<object>} Object containing schema and stats
     */
    const fetchTopicDetails = async (tenant, ns, topic) => {
        const fqdn = `persistent://${tenant}/${ns}/${topic}`

        const [schemaRes, statsRes] = await Promise.all([
            fetchAdmin(`schemas/${encodeURIComponent(tenant)}/${encodeURIComponent(ns)}/${encodeURIComponent(topic)}/schema`),
            fetchAdmin(`persistent/${encodeURIComponent(tenant)}/${encodeURIComponent(ns)}/${encodeURIComponent(topic)}/stats`)
        ])

        const details = {
            schema: schemaRes.ok ? await schemaRes.json() : null,
            stats: statsRes.ok ? await statsRes.json() : null,
        }

        topicDetailsMap.set(fqdn, details)

        // Update the topic in the topics array
        const topicObj = topics.value.find(t => t.fqdn === fqdn)
        if (topicObj) {
            topicObj.details = details
        }

        return details
    }

    /**
     * Creates a new non-partitioned topic.
     * @param {object} params - Topic creation parameters
     * @returns {Promise<boolean>} True if successful
     */
    const createTopic = async ({ tenant, namespace, topic, retention, permissions }) => {
        const key = `create:${tenant}/${namespace}/${topic}`

        return await withLoading(key, async () => {
            const res = await fetchAdmin(
                `persistent/${encodeURIComponent(tenant)}/${encodeURIComponent(namespace)}/${encodeURIComponent(topic)}`,
                { method: 'PUT' }
            )

            if (!res.ok) throw new Error('Failed to create topic')

            // Set retention if provided
            if (retention && (retention.retentionTimeInMinutes !== undefined || retention.retentionSizeInMB !== undefined)) {
                await updateRetention({ tenant, namespace, topic, retention })
            }

            // Grant permissions if provided
            if (Array.isArray(permissions) && permissions.length > 0) {
                for (const perm of permissions) {
                    if (perm.role && perm.actions && perm.actions.length > 0) {
                        const { grantPermissions } = await import('./permissions.js').then(m => m.usePermissionsStore())
                        await grantPermissions({ tenant, namespace, topic, role: perm.role, actions: perm.actions })
                    }
                }
            }

            // Refresh topics list
            await getAllTopicsForTenant(tenant)

            return true
        })
    }

    /**
     * Deletes a topic.
     * @param {object} params - Topic deletion parameters
     * @returns {Promise<boolean>} True if successful
     */
    const deleteTopic = async ({ tenant, namespace, topic, force = true }) => {
        const key = `delete:${tenant}/${namespace}/${topic}`

        return await withLoading(key, async () => {
            const queryParam = force ? '?force=true' : ''
            const res = await fetchAdmin(
                `persistent/${encodeURIComponent(tenant)}/${encodeURIComponent(namespace)}/${encodeURIComponent(topic)}${queryParam}`,
                { method: 'DELETE' }
            )

            if (!res.ok) throw new Error('Failed to delete topic')

            // Remove from topics list
            topics.value = topics.value.filter(t =>
                !(t.tenant === tenant && t.namespace === namespace && t.topic === topic)
            )

            // Remove from details map
            const fqdn = `persistent://${tenant}/${namespace}/${topic}`
            topicDetailsMap.delete(fqdn)

            return true
        })
    }

    /**
     * Updates the retention policy for a topic.
     * @param {object} params - Retention update parameters
     * @returns {Promise<boolean>} True if successful
     */
    const updateRetention = async ({ tenant, namespace, topic, retention }) => {
        const key = `retention:${tenant}/${namespace}/${topic}`

        return await withLoading(key, async () => {
            const body = JSON.stringify({
                retentionSizeInMB: retention.retentionSizeInMB ?? -1,
                retentionTimeInMinutes: retention.retentionTimeInMinutes ?? -1,
            })

            const res = await fetchAdmin(
                `persistent/${encodeURIComponent(tenant)}/${encodeURIComponent(namespace)}/${encodeURIComponent(topic)}/retention`,
                {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body,
                }
            )

            if (!res.ok) throw new Error('Failed to update retention')

            return true
        })
    }

    /**
     * Computed property that filters topics by selected namespace.
     */
    const filteredTopics = computed(() => {
        if (tenantsStore.selectedNamespace && tenantsStore.selectedNamespace !== 'All') {
            return topics.value.filter(t => t.namespace === tenantsStore.selectedNamespace)
        }
        return topics.value
    })

    return {
        // State
        loadingStates,
        topics,
        filteredTopics,
        // Actions
        parseTopicFqdn,
        getTopicsForNamespace,
        getAllTopicsForTenant,
        fetchTopicDetails,
        createTopic,
        deleteTopic,
        updateRetention,
    }
})
