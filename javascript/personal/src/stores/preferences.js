import {ref, computed} from 'vue'
import {defineStore} from 'pinia'

export const usePreferencesStore = defineStore('preferences', () => {
    const pulsarUrl = ref('')

    const savePreferences = (formData) => {
        console.log('Saving preferences:', formData.value)

        pulsarUrl.value = formData.pulsarUrl

        localStorage.setItem('preferences', JSON.stringify(formData.value))
    }

    const loadPreferences = () => {
        const saved = localStorage.getItem('preferences')
        if (saved) {
            const parsed = JSON.parse(saved)
            pulsarUrl.value = parsed.pulsarUrl
        }
    }

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
