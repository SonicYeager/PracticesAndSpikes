import { useNotificationStore } from '@/stores/notification.js'
import { usePreferencesStore } from '@/stores/preferences.js'
import { oauth2Service } from '@/services/auth.js'

/**
 * Composable that provides a centralized HTTP client for the Pulsar Admin REST API.
 * Automatically handles error notifications, dynamic cluster URLs, and OAuth2 authentication.
 * 
 * @returns {object} An object containing the fetchAdmin function
 */
export function useFetchAdmin() {
    const notificationStore = useNotificationStore()
    const preferencesStore = usePreferencesStore()

    /**
     * Makes a request to the Pulsar Admin API.
     * @param {string} endpoint - The API endpoint to call (e.g., 'clusters')
     * @param {object} [options={}] - The options for the fetch call
     * @returns {Promise<Response>} A promise that resolves to the response from the API
     */
    const fetchAdmin = async (endpoint, options = {}) => {
        // Ensure no leading slash in endpoint
        const ep = endpoint.startsWith('/') ? endpoint.slice(1) : endpoint

        // Get cluster configuration
        const { url: clusterUrl, auth } = preferencesStore.clusterConfig

        // Build full URL
        const baseUrl = clusterUrl || 'http://localhost:8080'
        const fullUrl = `${baseUrl}/admin/v2/${ep}`

        // Prepare headers
        const headers = { ...options.headers }

        // Add OAuth2 authentication if enabled
        if (auth?.enabled && auth.type === 'oauth2-client-credentials') {
            try {
                // Initialize OAuth2 service if needed
                if (!oauth2Service.isInitialized()) {
                    oauth2Service.initialize({
                        clientId: auth.clientId,
                        clientSecret: auth.clientSecret,
                        tokenEndpoint: auth.tokenEndpoint,
                        scope: auth.scope
                    })
                }

                // Get valid token (will refresh if needed)
                const token = await oauth2Service.getValidToken()
                headers['Authorization'] = `Bearer ${token}`
            } catch (authError) {
                notificationStore.error(`Authentication failed: ${authError.message}`)
                return new Response(null, { status: 401, statusText: 'Authentication failed' })
            }
        }

        try {
            const res = await fetch(fullUrl, {
                ...options,
                headers,
                mode: 'cors'  // Enable CORS for remote clusters
            })

            if (!res.ok) {
                // Try to parse error message
                let errorMsg = res.statusText
                try {
                    const body = await res.json()
                    if (body && body.reason) errorMsg = body.reason
                } catch (e) {
                    /* ignore parse errors */
                }

                // Only notify for non-404s
                if (res.status !== 404) {
                    notificationStore.error(`API Error (${res.status}): ${errorMsg}`)
                }
                return res
            }

            return res
        } catch (error) {
            notificationStore.error(`Network Error: ${error.message}`)
            // Return error-like response
            return new Response(null, { status: 500, statusText: 'Network error' })
        }
    }

    return { fetchAdmin }
}
