<template>
  <Transition name="hint">
    <div v-if="message" class="hint-banner" :class="`hint-banner--${type}`">
      <span class="hint-banner__icon">
        <template v-if="type === 'success'">✓</template>
        <template v-else-if="type === 'error'">✗</template>
        <template v-else>ℹ</template>
      </span>
      <span class="hint-banner__message">{{ message }}</span>
    </div>
  </Transition>
</template>

<script setup>
defineProps({
  message: {
    type: String,
    default: null,
  },
  type: {
    type: String,
    default: 'info',
    validator: (value) => ['info', 'success', 'error'].includes(value),
  },
});
</script>

<style scoped>
.hint-banner {
  position: fixed;
  bottom: 2rem;
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem 1.5rem;
  border-radius: 9999px;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
  color: white;
  font-weight: 500;
  backdrop-filter: blur(12px);
  z-index: 100;
}

.hint-banner--info {
  background-color: rgba(6, 182, 212, 0.9);
}

.hint-banner--success {
  background-color: rgba(34, 197, 94, 0.9);
}

.hint-banner--error {
  background-color: rgba(239, 68, 68, 0.9);
}

.hint-banner__icon {
  font-size: 1.25rem;
  font-weight: 700;
}

.hint-banner__message {
  font-size: 0.875rem;
}

@media (min-width: 640px) {
  .hint-banner__message {
    font-size: 1rem;
  }
}

/* Transition */
.hint-enter-active,
.hint-leave-active {
  transition: all 0.3s ease;
}

.hint-enter-from,
.hint-leave-to {
  opacity: 0;
  transform: translate(-50%, 20px);
}
</style>
