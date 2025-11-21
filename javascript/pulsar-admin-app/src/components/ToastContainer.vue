<script setup>
import { useNotificationStore } from '@/stores/notification'

const store = useNotificationStore()
</script>

<template>
  <div class="toast toast-top toast-end z-50">
    <div
      v-for="toast in store.toasts"
      :key="toast.id"
      class="alert shadow-lg transition-all duration-300"
      :class="{
        'alert-info': toast.type === 'info',
        'alert-success': toast.type === 'success',
        'alert-warning': toast.type === 'warning',
        'alert-error': toast.type === 'error'
      }"
    >
      <div class="flex items-center gap-2">
        <span v-if="toast.type === 'success'">✅</span>
        <span v-else-if="toast.type === 'error'">❌</span>
        <span v-else-if="toast.type === 'warning'">⚠️</span>
        <span v-else>ℹ️</span>
        <span>{{ toast.message }}</span>
      </div>
      <button class="btn btn-xs btn-ghost" @click="store.remove(toast.id)">✕</button>
    </div>
  </div>
</template>

<style scoped>
.toast {
  pointer-events: none;
}
.toast > * {
  pointer-events: auto;
}
</style>
