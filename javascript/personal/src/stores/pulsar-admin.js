import {computed, reactive, ref} from 'vue'
import {defineStore} from 'pinia'
import {usePreferencesStore} from "@/stores/preferences.js";

export const usePulsarAdminStore = defineStore('pulsar-admin', () => {
        const preferencesStore = usePreferencesStore()
        preferencesStore.loadPreferences()

        const loadingStates = reactive(new Map())

        // Health / clusters
        const isHealthy = ref(false)
        const clusters = ref([])
        const selectedCluster = ref(null)

        // Tenants / Namespaces / Topics
        const tenants = ref([])
        const selectedTenant = ref(null)
        const namespaces = ref([])
        const selectedNamespace = ref('All')
        const topics = ref([]) // [{ fqdn, tenant, namespace, topic, details?: { schema, stats }, loading?: boolean }]

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

        // Health check
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

        // Clusters
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

        const selectCluster = (cluster) => {
            selectedCluster.value = cluster
        }

        // Tenants
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

        const selectTenant = async (tenant) => {
            selectedTenant.value = tenant
            // Reset dependent state
            namespaces.value = []
            selectedNamespace.value = 'All'
            topics.value = []
        }

        // Namespaces
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

        const selectNamespace = (ns) => {
            selectedNamespace.value = ns
        }

        // Topics
        const parseTopicFqdn = (fqdn) => {
            // fqdn like: persistent://tenant/namespace/topic or non-persistent
            const m = fqdn.match(/^(persistent|non-persistent):\/\/([^/]+)\/([^/]+)\/([^\s]+)$/)
            if (!m) return null
            return {type: m[1], tenant: m[2], namespace: m[3], topic: m[4], fqdn}
        }

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

        const topicDetailsMap = reactive(new Map()) // key: fqdn -> { schema, stats }

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

        // Create topic (non-partitioned)
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

        // Delete topic
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

        // Retention policy (topic-level)
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

        // Permissions
        const getPermissions = async ({tenant, namespace, topic}) => {
            const res = await fetchAdmin(`persistent/${encodeURIComponent(tenant)}/${encodeURIComponent(namespace)}/${encodeURIComponent(topic)}/permissions`)
            return res.ok ? await res.json() : {}
        }

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

        // Derived
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
