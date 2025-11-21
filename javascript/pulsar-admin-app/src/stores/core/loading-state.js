import { reactive } from 'vue'

/**
 * Composable that provides centralized loading state management.
 * Uses a reactive Map to track loading states across the application.
 * 
 * @returns {object} An object containing the loadingStates map and withLoading function
 */
export function useLoadingState() {
    /**
     * A reactive map to track loading states for various operations.
     * Key: string identifier for the operation
     * Value: boolean indicating if the operation is in progress
     */
    const loadingStates = reactive(new Map())

    /**
     * Higher-order function that wraps an async operation with loading state management.
     * Automatically sets loading state to true before executing, and false after completion.
     * 
     * @param {string} key - The identifier for this operation's loading state
     * @param {Function} fn - The async function to execute
     * @returns {Promise<any>} A promise that resolves to the result of the async function
     */
    const withLoading = async (key, fn) => {
        loadingStates.set(key, true)
        try {
            return await fn()
        } finally {
            loadingStates.set(key, false)
        }
    }

    /**
     * Initialize common loading states
     */
    const initLoadingStates = (states) => {
        states.forEach(state => loadingStates.set(state, false))
    }

    return {
        loadingStates,
        withLoading,
        initLoadingStates
    }
}
