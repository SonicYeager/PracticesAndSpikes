import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useNotificationStore = defineStore('notification', () => {
    const toasts = ref([])
    let nextId = 0

    const add = (message, type = 'info', duration = 5000) => {
        const id = nextId++
        toasts.value.push({ id, message, type })
        if (duration > 0) {
            setTimeout(() => {
                remove(id)
            }, duration)
        }
    }

    const remove = (id) => {
        const idx = toasts.value.findIndex(t => t.id === id)
        if (idx !== -1) {
            toasts.value.splice(idx, 1)
        }
    }

    const success = (msg, duration) => add(msg, 'success', duration)
    const error = (msg, duration) => add(msg, 'error', duration)
    const info = (msg, duration) => add(msg, 'info', duration)
    const warning = (msg, duration) => add(msg, 'warning', duration)

    return {
        toasts,
        add,
        remove,
        success,
        error,
        info,
        warning
    }
})
