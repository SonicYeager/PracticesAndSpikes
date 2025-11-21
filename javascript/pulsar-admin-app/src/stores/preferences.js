import {ref, computed} from 'vue'
import {defineStore} from 'pinia'

/**
 * Pinia store for managing user preferences.
 */
export const usePreferencesStore = defineStore('preferences', () => {
    /**
     * The URL of the Pulsar REST API.
     * @type {import('vue').Ref<string>}
     */
    const pulsarUrl = ref('')

    /**
     * Saves the user's preferences to local storage.
     * @param {object} formData - The form data containing the preferences to save.
     * @param {string} formData.pulsarUrl - The Pulsar URL to save.
     */
    const savePreferences = (formData) => {
        console.log('Saving preferences:', formData.value)

        pulsarUrl.value = formData.pulsarUrl

        localStorage.setItem('preferences', JSON.stringify(formData.value))
    }

    /**
     * Loads the user's preferences from local storage.
     */
    const loadPreferences = () => {
        const saved = localStorage.getItem('preferences')
        if (saved) {
            const parsed = JSON.parse(saved)
            pulsarUrl.value = parsed.pulsarUrl
        }
    }

    /**
     * A computed property that returns an object containing the current preferences.
     * @returns {object}
     */
    const preferences = computed(() => ({
        pulsarUrl: pulsarUrl.value,
    }))
    
    return {
        pulsarUrl,
        preferences,
        savePreferences,
        loadPreferences
    }
})
