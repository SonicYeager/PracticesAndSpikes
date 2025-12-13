<template>
  <button 
    class="hint-button" 
    :class="{ 'hint-button--active': gameStore.showingHints }"
    @click="gameStore.toggleHints"
  >
    <span class="hint-button__icon">{{ gameStore.showingHints ? '🙈' : '💡' }}</span>
    <span class="hint-button__text">
      {{ gameStore.showingHints ? 'Verstecken' : 'Hinweis' }}
    </span>
    <span v-if="gameStore.showingHints" class="hint-button__count">
      ({{ gameStore.availableSets.length }})
    </span>
  </button>
</template>

<script setup>
import { useGameStore } from '@/stores/gameStore';

const gameStore = useGameStore();
</script>

<style scoped>
/*
 * HINT BUTTON STYLING
 * -------------------
 * This button toggles the visibility of SET hints on the board.
 * 
 * CSS CONCEPTS USED:
 * - Flexbox for centering icon and text
 * - CSS transitions for smooth hover/active effects
 * - CSS custom properties (variables) for consistent theming
 */

.hint-button {
  /* Flexbox: Arrange icon and text horizontally, centered */
  display: flex;
  align-items: center;
  gap: 0.5rem;
  
  /* Sizing and spacing */
  padding: 0.75rem 1.25rem;
  border-radius: 9999px; /* Pill shape */
  border: 2px solid rgba(255, 255, 255, 0.2);
  
  /* Colors */
  background: rgba(26, 22, 37, 0.8);
  color: white;
  
  /* Typography */
  font-size: 0.875rem;
  font-weight: 600;
  
  /* Interactive styles */
  cursor: pointer;
  transition: all 0.2s ease;
  
  /* Glassmorphism effect */
  backdrop-filter: blur(8px);
}

.hint-button:hover {
  background: rgba(249, 115, 22, 0.2); /* Orange tint on hover */
  border-color: #f97316;
  transform: scale(1.02);
}

.hint-button--active {
  background: rgba(249, 115, 22, 0.3);
  border-color: #f97316;
  box-shadow: 0 0 15px rgba(249, 115, 22, 0.3);
}

.hint-button__icon {
  font-size: 1.25rem;
}

.hint-button__text {
  /* Text styling */
}

.hint-button__count {
  /* Muted count display */
  opacity: 0.8;
  font-weight: 400;
}
</style>
