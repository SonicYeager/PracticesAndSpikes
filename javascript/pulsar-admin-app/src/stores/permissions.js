import { defineStore } from 'pinia'
import { useFetchAdmin } from './core/api-client.js'
import { useLoadingState } from './core/loading-state.js'

/**
 * Pinia store for managing Pulsar permissions.
 * This is a stateless store that provides permission management operations.
 */
export const usePermissionsStore = defineStore('permissions', () => {
    const { fetchAdmin } = useFetchAdmin()
    const { loadingStates, withLoading } = useLoadingState()

    /**
     * Fetches permissions for a topic.
     * @param {object} params - Topic identification parameters
     * @returns {Promise<object>} The permissions object
     */
    const getPermissions = async ({ tenant, namespace, topic }) => {
        const res = await fetchAdmin(
            `persistent/${encodeURIComponent(tenant)}/${encodeURIComponent(namespace)}/${encodeURIComponent(topic)}/permissions`
        )
        return res.ok ? await res.json() : {}
    }

    /**
     * Grants permissions to a role on a topic.
     * @param {object} params - Permission parameters
     * @returns {Promise<boolean>} True if successful
     */
    const grantPermissions = async ({ tenant, namespace, topic, role, actions }) => {
        const key = `perm:${tenant}/${namespace}/${topic}:${role}`

        return await withLoading(key, async () => {
            const res = await fetchAdmin(
                `persistent/${encodeURIComponent(tenant)}/${encodeURIComponent(namespace)}/${encodeURIComponent(topic)}/permissions/${encodeURIComponent(role)}`,
                {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(actions || []),
                }
            )

            if (!res.ok) throw new Error('Failed to grant permissions')

            return true
        })
    }

    /**
     * Revokes permissions from a role on a topic.
     * @param {object} params - Permission parameters
     * @returns {Promise<boolean>} True if successful
     */
    const revokePermissions = async ({ tenant, namespace, topic, role }) => {
        const key = `perm-del:${tenant}/${namespace}/${topic}:${role}`

        return await withLoading(key, async () => {
            const res = await fetchAdmin(
                `persistent/${encodeURIComponent(tenant)}/${encodeURIComponent(namespace)}/${encodeURIComponent(topic)}/permissions/${encodeURIComponent(role)}`,
                { method: 'DELETE' }
            )

            if (!res.ok) throw new Error('Failed to revoke permissions')

            return true
        })
    }

    /**
     * Grants namespace-level permissions to a role.
     * @param {object} params - Permission parameters
     * @returns {Promise<boolean>} True if successful
     */
    const grantNamespacePermissions = async ({ tenant, namespace, role, actions }) => {
        const key = `ns-perm:${tenant}/${namespace}:${role}`

        return await withLoading(key, async () => {
            const res = await fetchAdmin(
                `namespaces/${encodeURIComponent(tenant)}/${encodeURIComponent(namespace)}/permissions/${encodeURIComponent(role)}`,
                {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(actions || []),
                }
            )

            if (!res.ok) throw new Error('Failed to grant namespace permissions')

            return true
        })
    }

    return {
        // State (for loading indicators)
        loadingStates,
        // Actions
        getPermissions,
        grantPermissions,
        revokePermissions,
        grantNamespacePermissions,
    }
})
