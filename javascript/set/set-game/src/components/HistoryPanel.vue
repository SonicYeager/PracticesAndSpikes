<template>
  <!--
    HISTORY PANEL COMPONENT
    =======================
    A collapsible sidebar showing the chronological log of game actions.
    
    VUE CONCEPTS:
    - v-for with computed: Reverse array for newest-first display
    - Date formatting: Using toLocaleTimeString() for time display
    - Conditional rendering: Different content based on entry type
  -->
  <aside class="history-panel" :class="{ 'history-panel--open': isOpen }">
    <!-- Toggle Button -->
    <button class="history-panel__toggle" @click="isOpen = !isOpen">
      <span>📜</span>
      <span v-if="!isOpen">Verlauf</span>
      <span v-else>Schließen</span>
    </button>

    <!-- Panel Content -->
    <div v-if="isOpen" class="history-panel__content">
      <h3 class="history-panel__title">Spielverlauf</h3>
      
      <div v-if="gameStore.history.length === 0" class="history-panel__empty">
        Noch keine Aktionen.
      </div>

      <TransitionGroup name="history-item" tag="div" class="history-panel__list">
        <div 
          v-for="entry in reversedHistory" 
          :key="entry.id"
          class="history-entry"
          :class="[`history-entry--${entry.type}`]"
        >
          <span class="history-entry__time">
            {{ formatTime(entry.timestamp) }}
          </span>
          <span class="history-entry__icon">
            {{ gameStore.getHistoryIcon(entry.type) }}
          </span>
          <div class="history-entry__body">
            <span class="history-entry__message">{{ entry.message }}</span>
            
            <!-- Show cards for SET-related entries -->
            <div v-if="entry.cards && entry.cards.length > 0" class="history-entry__cards">
              <MiniCard 
                v-for="card in entry.cards" 
                :key="card.id" 
                :card="card" 
              />
            </div>
          </div>
        </div>
      </TransitionGroup>
    </div>
  </aside>
</template>

<script setup>
/**
 * HistoryPanel.vue - Game Action History
 * 
 * FEATURES:
 * - Collapsible sidebar
 * - Newest entries at the top
 * - Mini card previews for SET-related actions
 * - Animated entry/exit of history items
 */

import { ref, computed } from 'vue';
import { useGameStore } from '@/stores/gameStore';
import MiniCard from './MiniCard.vue';

const gameStore = useGameStore();

const isOpen = ref(false);

/**
 * Reverse the history array to show newest first
 */
const reversedHistory = computed(() => {
  return [...gameStore.history].reverse();
});

/**
 * Format timestamp as HH:MM:SS
 */
function formatTime(date) {
  return date.toLocaleTimeString('de-DE', {
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit',
  });
}
</script>

<style scoped>
/*
 * HISTORY PANEL STYLING
 * ---------------------
 * 
 * LAYOUT:
 * Fixed positioning on the right side of the screen.
 * Slides in/out using transform for smooth animation.
 */

.history-panel {
  position: fixed;
  top: 50%;
  right: 0;
  transform: translateY(-50%);
  z-index: 50;
  
  display: flex;
  flex-direction: row-reverse;
  align-items: flex-start;
}

.history-panel__toggle {
  /* Pill-shaped toggle button */
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 0.75rem;
  
  background: rgba(26, 22, 37, 0.9);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 0.5rem 0 0 0.5rem;
  
  color: white;
  font-size: 0.75rem;
  cursor: pointer;
  
  transition: all 0.2s ease;
}

.history-panel__toggle:hover {
  background: rgba(26, 22, 37, 1);
}

.history-panel__content {
  width: 280px;
  max-height: 60vh;
  overflow-y: auto;
  
  background: rgba(26, 22, 37, 0.95);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-right: none;
  border-radius: 0.5rem 0 0 0.5rem;
  
  backdrop-filter: blur(8px);
  
  padding: 1rem;
}

.history-panel__title {
  font-size: 0.875rem;
  font-weight: 700;
  color: #06b6d4;
  margin: 0 0 0.75rem 0;
  padding-bottom: 0.5rem;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.history-panel__empty {
  color: #9ca3af;
  font-size: 0.75rem;
  text-align: center;
  padding: 1rem 0;
}

.history-panel__list {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

/*
 * HISTORY ENTRY
 * -------------
 * Each entry shows: time, icon, message, and optionally cards
 */

.history-entry {
  display: grid;
  grid-template-columns: auto auto 1fr;
  gap: 0.5rem;
  align-items: start;
  
  padding: 0.5rem;
  background: rgba(255, 255, 255, 0.03);
  border-radius: 0.375rem;
  
  font-size: 0.75rem;
}

.history-entry--set_found {
  border-left: 2px solid #22c55e;
}

.history-entry--wrong_set,
.history-entry--penalty {
  border-left: 2px solid #ef4444;
}

.history-entry--hint_used {
  border-left: 2px solid #f97316;
}

.history-entry--game_start,
.history-entry--game_over {
  border-left: 2px solid #06b6d4;
}

.history-entry__time {
  color: #6b7280;
  font-size: 0.625rem;
  font-family: monospace;
}

.history-entry__icon {
  font-size: 0.875rem;
}

.history-entry__body {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.history-entry__message {
  color: #d1d5db;
}

.history-entry__cards {
  display: flex;
  gap: 0.25rem;
  flex-wrap: wrap;
}

/*
 * TRANSITION ANIMATIONS
 * ---------------------
 * New history items slide in from the top
 */

.history-item-enter-active {
  transition: all 0.3s ease;
}

.history-item-enter-from {
  opacity: 0;
  transform: translateY(-10px);
}
</style>
