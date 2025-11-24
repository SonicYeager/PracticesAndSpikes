import { ref, computed } from 'vue'
import { defineStore } from 'pinia'

/**
 * Pinia store for managing user preferences including cluster configuration.
 */
export const usePreferencesStore = defineStore('preferences', () => {
    /**
     * Cluster configuration including URL and authentication settings.
     */
    const clusterConfig = ref({
        url: 'http://localhost:8080',
        auth: {
            enabled: false,
            type: 'oauth2-client-credentials',
            clientId: '',
            clientSecret: '',
            tokenEndpoint: '',
            scope: ''
        }
    })

    /**
     * Saves the user's preferences to local storage.
     * @param {object} config - The configuration to save
     */
    const savePreferences = (config) => {
        console.log('Saving preferences:', config)

        if (config.clusterConfig) {
            clusterConfig.value = { ...clusterConfig.value, ...config.clusterConfig }
        }

        localStorage.setItem('preferences', JSON.stringify({
            clusterConfig: clusterConfig.value
        }))
    }

    /**
     * Loads the user's preferences from local storage.
     */
    const loadPreferences = () => {
        const saved = localStorage.getItem('preferences')
        if (saved) {
            try {
                const parsed = JSON.parse(saved)
                if (parsed.clusterConfig) {
                    clusterConfig.value = { ...clusterConfig.value, ...parsed.clusterConfig }
                }
            } catch (e) {
                console.error('Failed to load preferences:', e)
            }
        }
    }

    /**
     * Resets preferences to defaults.
     */
    const resetPreferences = () => {
        clusterConfig.value = {
            url: 'http://localhost:8080',
            auth: {
                enabled: false,
                type: 'oauth2-client-credentials',
                clientId: '',
                clientSecret: '',
                tokenEndpoint: '',
                scope: ''
            }
        }
        localStorage.removeItem('preferences')
    }

    /**
     * Computed property for easy access to preferences.
     */
    const preferences = computed(() => ({
        clusterConfig: clusterConfig.value
    }))

    return {
        clusterConfig,
        preferences,
        savePreferences,
        loadPreferences,
        resetPreferences
    }
})
