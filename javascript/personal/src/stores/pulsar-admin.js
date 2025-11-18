import {computed, reactive, ref} from 'vue'
import {defineStore} from 'pinia'
import {usePreferencesStore} from "@/stores/preferences.js";

/**
 * Pinia store for interacting with the Pulsar Admin REST API.
 */
export const usePulsarAdminStore = defineStore('pulsar-admin', () => {
        const preferencesStore = usePreferencesStore()
        preferencesStore.loadPreferences()

        /**
         * A map to track the loading state of various asynchronous operations.
         * The key is a string identifying the operation, and the value is a boolean.
         * @type {Map<string, boolean>}
         */
        const loadingStates = reactive(new Map())

        // Health / clusters
        /**
         * Indicates whether the Pulsar cluster is healthy.
         * @type {import('vue').Ref<boolean>}
         */
        const isHealthy = ref(false)
        /**
         * A list of clusters in the Pulsar instance.
         * @type {import('vue').Ref<Array<string>>}
         */
        const clusters = ref([])
        /**
         * The currently selected cluster.
         * @type {import('vue').Ref<string|null>}
         */
        const selectedCluster = ref(null)

        // Tenants / Namespaces / Topics
        /**
         * A list of tenants in the Pulsar instance.
         * @type {import('vue').Ref<Array<string>>}
         */
        const tenants = ref([])
        /**
         * The currently selected tenant.
         * @type {import('vue').Ref<string|null>}
         */
        const selectedTenant = ref(null)
        /**
         * A list of namespaces for the selected tenant.
         * @type {import('vue').Ref<Array<string>>}
         */
        const namespaces = ref([])
        /**
         * The currently selected namespace. 'All' means no namespace is selected.
         * @type {import('vue').Ref<string>}
         */
        const selectedNamespace = ref('All')
        /**
         * A list of topics for the selected tenant and namespace(s).
         * Each topic is an object with properties like `fqdn`, `tenant`, `namespace`, `topic`, and optionally `details`.
         * @type {import('vue').Ref<Array<object>>}
         */
        const topics = ref([]) // [{ fqdn, tenant, namespace, topic, details?: { schema, stats }, loading?: boolean }]

        /**
         * A helper function to make requests to the Pulsar Admin API.
         * It automatically prepends the base path for the admin API.
         * @param {string} endpoint - The API endpoint to call (e.g., 'clusters').
         * @param {object} [options={}] - The options for the `fetch` call.
         * @returns {Promise<Response>} A promise that resolves to the response from the API.
         */
        const fetchAdmin = async (endpoint, options = {}) => {
            // Ensure no leading slash in endpoint
            const ep = endpoint.startsWith('/') ? endpoint.slice(1) : endpoint
            try {
                return await fetch(`/admin/v2/${ep}`, options)
            } catch (e2) {
                // propagate last error-like response
                return new Response(null, {status: 500, statusText: 'Network error'})
            }
        }

        /**
         * Checks the health of the Pulsar cluster.
         * @returns {Promise<void>}
         */
        const checkHealth = async () => {
            loadingStates.set('health', true)
            try {
                const response = await fetchAdmin(`brokers/health`)
                isHealthy.value = !!response && response.ok
            } catch (e) {
                isHealthy.value = false
            } finally {
                loadingStates.set('health', false)
            }
        }

        /**
         * Fetches the list of clusters from the Pulsar instance.
         * @returns {Promise<void>}
         */
        const getClusters = async () => {
            loadingStates.set('clusters', true)
            try {
                const response = await fetchAdmin(`clusters`)
                clusters.value = response.ok ? await response.json() : []
                if (clusters.value.length > 0 && !selectedCluster.value) {
                    selectedCluster.value = clusters.value[0]
                }
            } finally {
                loadingStates.set('clusters', false)
            }
        }

        /**
         * Sets the currently selected cluster.
         * @param {string} cluster - The name of the cluster to select.
         */
        const selectCluster = (cluster) => {
            selectedCluster.value = cluster
        }

        /**
         * Fetches the list of tenants from the Pulsar instance.
         * @returns {Promise<void>}
         */
        const getTenants = async () => {
            loadingStates.set('tenants', true)
            try {
                const res = await fetchAdmin(`tenants`)
                tenants.value = res.ok ? await res.json() : []
                if (!selectedTenant.value && tenants.value.length > 0) {
                    selectedTenant.value = tenants.value[0]
                }
            } finally {
                loadingStates.set('tenants', false)
            }
        }

        /**
         * Sets the currently selected tenant and resets the namespace and topic selections.
         * @param {string} tenant - The name of the tenant to select.
         * @returns {Promise<void>}
         */
        const selectTenant = async (tenant) => {
            selectedTenant.value = tenant
            // Reset dependent state
            namespaces.value = []
            selectedNamespace.value = 'All'
            topics.value = []
        }

        /**
         * Fetches the list of namespaces for a given tenant.
         * @param {string} [tenant] - The name of the tenant. Defaults to the currently selected tenant.
         * @returns {Promise<Array<string>>} A promise that resolves to the list of namespaces.
         */
        const getNamespaces = async (tenant) => {
            const t = tenant || selectedTenant.value
            if (!t) return []
            loadingStates.set('namespaces', true)
            try {
                const res = await fetchAdmin(`namespaces/${encodeURIComponent(t)}`)
                namespaces.value = res.ok ? await res.json() : []
                return namespaces.value
            } finally {
                loadingStates.set('namespaces', false)
            }
        }

        /**
         * Sets the currently selected namespace.
         * @param {string} ns - The name of the namespace to select.
         */
        const selectNamespace = (ns) => {
            selectedNamespace.value = ns
        }

        /**
         * Parses a fully qualified topic name (FQDN) into its components.
         * @param {string} fqdn - The FQDN of the topic (e.g., 'persistent://tenant/namespace/topic').
         * @returns {object|null} An object with the topic components, or null if parsing fails.
         */
        const parseTopicFqdn = (fqdn) => {
            // fqdn like: persistent://tenant/namespace/topic or non-persistent
            const m = fqdn.match(/^(persistent|non-persistent):\/\/([^/]+)\/([^/]+)\/([^\s]+)$/)
            if (!m) return null
            return {type: m[1], tenant: m[2], namespace: m[3], topic: m[4], fqdn}
        }

        /**
         * Fetches the list of topics for a given namespace.
         * @param {string} tenant - The name of the tenant.
         * @param {string} ns - The name of the namespace.
         * @returns {Promise<Array<object>>} A promise that resolves to the list of topics.
         */
        const getTopicsForNamespace = async (tenant, ns) => {
            const results = []
            // persistent topics
            const resP = await fetchAdmin(`persistent/${encodeURIComponent(tenant)}/${encodeURIComponent(ns)}`)
            if (resP.ok) {
                const list = await resP.json()
                results.push(...list.map(parseTopicFqdn).filter(Boolean))
            }
            // non-persistent topics
            const resNP = await fetchAdmin(`non-persistent/${encodeURIComponent(tenant)}/${encodeURIComponent(ns)}`)
            if (resNP.ok) {
                const list = await resNP.json()
                results.push(...list.map(parseTopicFqdn).filter(Boolean))
            }
            return results
        }

        /**
         * Fetches all topics for a given tenant, iterating through all its namespaces.
         * @param {string} [tenant] - The name of the tenant. Defaults to the currently selected tenant.
         * @returns {Promise<Array<object>>} A promise that resolves to the list of all topics for the tenant.
         */
        const getAllTopicsForTenant = async (tenant) => {
            const t = tenant || selectedTenant.value
            if (!t) return []
            loadingStates.set('topics', true)
            try {
                const nss = namespaces.value.length ? namespaces.value : await getNamespaces(t)
                const all = []
                for (const ns of nss) {
                    const nsName = ns.includes('/') ? ns.split('/')[1] : ns // APIs sometimes return tenant/ns
                    const parsed = await getTopicsForNamespace(t, nsName)
                    all.push(...parsed)
                }
                topics.value = all
                return topics.value
            } finally {
                loadingStates.set('topics', false)
            }
        }

        /**
         * A map to store the details (schema and stats) of topics.
         * The key is the topic FQDN.
         * @type {Map<string, object>}
         */
        const topicDetailsMap = reactive(new Map()) // key: fqdn -> { schema, stats }

        /**
         * Fetches the details (schema and stats) for a specific topic.
         * @param {string} tenant - The tenant of the topic.
         * @param {string} ns - The namespace of the topic.
         * @param {string} topic - The name of the topic.
         * @returns {Promise<object>} A promise that resolves to an object containing the schema and stats.
         */
        const fetchTopicDetails = async (tenant, ns, topic) => {
            const key = `topic:${tenant}/${ns}/${topic}:details`
            loadingStates.set(key, true)
            try {
                const [schemaRes, statsRes] = await Promise.all([
                    fetchAdmin(`schemas/${encodeURIComponent(tenant)}/${encodeURIComponent(ns)}/${encodeURIComponent(topic)}/schema`),
                    fetchAdmin(`persistent/${encodeURIComponent(tenant)}/${encodeURIComponent(ns)}/${encodeURIComponent(topic)}/stats`),
                ])
                const schema = schemaRes.ok ? await schemaRes.json() : null
                const stats = statsRes.ok ? await statsRes.json() : null
                topicDetailsMap.set(`persistent://${tenant}/${ns}/${topic}`, {schema, stats})
                // also reflect into topics array for convenience
                const idx = topics.value.findIndex(t => t.tenant === tenant && t.namespace === ns && t.topic === topic)
                if (idx >= 0) {
                    topics.value[idx] = {...topics.value[idx], details: {schema, stats}}
                }
                return {schema, stats}
            } finally {
                loadingStates.set(key, false)
            }
        }

        /**
         * Creates a new non-partitioned topic.
         * @param {object} params - The topic creation parameters.
         * @param {string} params.tenant - The tenant of the topic.
         * @param {string} params.namespace - The namespace of the topic.
         * @param {string} params.topic - The name of the topic.
         * @param {object} [params.retention] - The retention policy for the topic.
         * @param {Array<object>} [params.permissions] - The permissions to grant on the topic.
         * @returns {Promise<boolean>} A promise that resolves to true if the topic was created successfully.
         * @throws {Error} If the topic creation fails.
         */
        const createTopic = async ({tenant, namespace, topic, retention, permissions}) => {
            loadingStates.set('createTopic', true)
            try {
                const createRes = await fetchAdmin(`persistent/${encodeURIComponent(tenant)}/${encodeURIComponent(namespace)}/${encodeURIComponent(topic)}`, {method: 'PUT'})
                if (!createRes.ok) {
                    throw new Error(`Failed to create topic: ${createRes.status}`)
                }
                // Apply optional retention
                if (retention && (retention.retentionSizeInMB != null || retention.retentionTimeInMinutes != null)) {
                    await updateRetention({tenant, namespace, topic, retention})
                }
                // Apply optional permissions
                if (permissions && Array.isArray(permissions)) {
                    for (const p of permissions) {
                        if (p && p.role && Array.isArray(p.actions)) {
                            await grantPermissions({tenant, namespace, topic, role: p.role, actions: p.actions})
                        }
                    }
                }
                // Refresh topics for that namespace
                await getAllTopicsForTenant(tenant)
                return true
            } finally {
                loadingStates.set('createTopic', false)
            }
        }

        /**
         * Deletes a topic.
         * @param {object} params - The topic deletion parameters.
         * @param {string} params.tenant - The tenant of the topic.
         * @param {string} params.namespace - The namespace of the topic.
         * @param {string} params.topic - The name of the topic.
         * @param {boolean} [params.force=true] - Whether to force the deletion.
         * @returns {Promise<boolean>} A promise that resolves to true if the topic was deleted successfully.
         * @throws {Error} If the topic deletion fails.
         */
        const deleteTopic = async ({tenant, namespace, topic, force = true}) => {
            const key = `delete:${tenant}/${namespace}/${topic}`
            loadingStates.set(key, true)
            try {
                const url = `persistent/${encodeURIComponent(tenant)}/${encodeURIComponent(namespace)}/${encodeURIComponent(topic)}${force ? '?force=true' : ''}`
                const res = await fetchAdmin(url, {method: 'DELETE'})
                if (!res.ok) throw new Error('Failed to delete topic')
                // remove from local list
                topics.value = topics.value.filter(t => !(t.tenant === tenant && t.namespace === namespace && t.topic === topic))
                topicDetailsMap.delete(`persistent://${tenant}/${namespace}/${topic}`)
                return true
            } finally {
                loadingStates.set(key, false)
            }
        }

        /**
         * Updates the retention policy for a topic.
         * @param {object} params - The retention policy parameters.
         * @param {string} params.tenant - The tenant of the topic.
         * @param {string} params.namespace - The namespace of the topic.
         * @param {string} params.topic - The name of the topic.
         * @param {object} params.retention - The retention policy to set.
         * @returns {Promise<boolean>} A promise that resolves to true if the retention policy was updated successfully.
         * @throws {Error} If the update fails.
         */
        const updateRetention = async ({tenant, namespace, topic, retention}) => {
            const key = `retention:${tenant}/${namespace}/${topic}`
            loadingStates.set(key, true)
            try {
                const body = JSON.stringify({
                    retentionSizeInMB: retention.retentionSizeInMB ?? -1,
                    retentionTimeInMinutes: retention.retentionTimeInMinutes ?? -1,
                })
                const res = await fetchAdmin(`persistent/${encodeURIComponent(tenant)}/${encodeURIComponent(namespace)}/${encodeURIComponent(topic)}/retention`, {
                    method: 'POST',
                    headers: {'Content-Type': 'application/json'},
                    body,
                })
                if (!res.ok) throw new Error('Failed to update retention')
                return true
            } finally {
                loadingStates.set(key, false)
            }
        }

        /**
         * Fetches the permissions for a topic.
         * @param {object} params - The topic identification parameters.
         * @param {string} params.tenant - The tenant of the topic.
         * @param {string} params.namespace - The namespace of the topic.
         * @param {string} params.topic - The name of the topic.
         * @returns {Promise<object>} A promise that resolves to the permissions object.
         */
        const getPermissions = async ({tenant, namespace, topic}) => {
            const res = await fetchAdmin(`persistent/${encodeURIComponent(tenant)}/${encodeURIComponent(namespace)}/${encodeURIComponent(topic)}/permissions`)
            return res.ok ? await res.json() : {}
        }

        /**
         * Grants permissions to a role on a topic.
         * @param {object} params - The permission parameters.
         * @param {string} params.tenant - The tenant of the topic.
         * @param {string} params.namespace - The namespace of the topic.
         * @param {string} params.topic - The name of the topic.
         * @param {string} params.role - The role to grant permissions to.
         * @param {Array<string>} params.actions - The actions to grant.
         * @returns {Promise<boolean>} A promise that resolves to true if the permissions were granted successfully.
         * @throws {Error} If granting permissions fails.
         */
        const grantPermissions = async ({tenant, namespace, topic, role, actions}) => {
            const key = `perm:${tenant}/${namespace}/${topic}:${role}`
            loadingStates.set(key, true)
            try {
                const res = await fetchAdmin(`persistent/${encodeURIComponent(tenant)}/${encodeURIComponent(namespace)}/${encodeURIComponent(topic)}/permissions/${encodeURIComponent(role)}`, {
                    method: 'POST',
                    headers: {'Content-Type': 'application/json'},
                    body: JSON.stringify(actions || []),
                })
                if (!res.ok) throw new Error('Failed to grant permissions')
                return true
            } finally {
                loadingStates.set(key, false)
            }
        }

        /**
         * Revokes permissions from a role on a topic.
         * @param {object} params - The permission parameters.
         * @param {string} params.tenant - The tenant of the topic.
         * @param {string} params.namespace - The namespace of the topic.
         * @param {string} params.topic - The name of the topic.
         * @param {string} params.role - The role to revoke permissions from.
         * @returns {Promise<boolean>} A promise that resolves to true if the permissions were revoked successfully.
         * @throws {Error} If revoking permissions fails.
         */
        const revokePermissions = async ({tenant, namespace, topic, role}) => {
            const key = `perm-del:${tenant}/${namespace}/${topic}:${role}`
            loadingStates.set(key, true)
            try {
                const res = await fetchAdmin(`persistent/${encodeURIComponent(tenant)}/${encodeURIComponent(namespace)}/${encodeURIComponent(topic)}/permissions/${encodeURIComponent(role)}`, {
                    method: 'DELETE',
                })
                if (!res.ok) throw new Error('Failed to revoke permissions')
                return true
            } finally {
                loadingStates.set(key, false)
            }
        }

        /**
         * A computed property that filters the topics based on the selected namespace.
         * @returns {Array<object>} The filtered list of topics.
         */
        const filteredTopics = computed(() => {
            if (selectedNamespace.value && selectedNamespace.value !== 'All') {
                return topics.value.filter(t => t.namespace === selectedNamespace.value)
            }
            return topics.value
        })

        // init loading states
        loadingStates.set('health', false)
        loadingStates.set('clusters', false)
        loadingStates.set('tenants', false)
        loadingStates.set('namespaces', false)
        loadingStates.set('topics', false)

        return {
            // state
            loadingStates,
            isHealthy,
            clusters,
            selectedCluster,
            tenants,
            selectedTenant,
            namespaces,
            selectedNamespace,
            topics,
            filteredTopics,
            // helpers
            fetchAdmin,
            // actions
            checkHealth,
            getClusters,
            selectCluster,
            getTenants,
            selectTenant,
            getNamespaces,
            selectNamespace,
            getAllTopicsForTenant,
            fetchTopicDetails,
            createTopic,
            deleteTopic,
            updateRetention,
            getPermissions,
            grantPermissions,
            revokePermissions,
        }
    }
)
