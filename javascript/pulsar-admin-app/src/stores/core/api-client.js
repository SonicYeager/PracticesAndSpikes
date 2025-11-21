import { useNotificationStore } from '@/stores/notification.js'

/**
 * Composable that provides a centralized HTTP client for the Pulsar Admin REST API.
 * Automatically handles error notifications and response parsing.
 * 
 * @returns {object} An object containing the fetchAdmin function
 */
export function useFetchAdmin() {
    const notificationStore = useNotificationStore()

    /**
     * Makes a request to the Pulsar Admin API.
     * @param {string} endpoint - The API endpoint to call (e.g., 'clusters')
     * @param {object} [options={}] - The options for the fetch call
     * @returns {Promise<Response>} A promise that resolves to the response from the API
     */
    const fetchAdmin = async (endpoint, options = {}) => {
        // Ensure no leading slash in endpoint
        const ep = endpoint.startsWith('/') ? endpoint.slice(1) : endpoint

        try {
            const res = await fetch(`/admin/v2/${ep}`, options)

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
