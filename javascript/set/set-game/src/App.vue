<template>
  <!--
    APP.VUE - Main Application Component
    =====================================
    This is the root component that orchestrates all other components.
    
    VUE CONCEPTS:
    - onMounted: Lifecycle hook that runs when component is first rendered
    - Pinia Store: Centralized state management accessed via useGameStore()
    - Conditional Rendering: v-if/v-else to show different UI states
  -->
  <div class="app">
    <!-- Action History Panel (Sidebar) -->
    <HistoryPanel v-if="gameStore.board.length > 0" />

    <!-- Header -->
    <header class="app__header">
      <MathTooltip v-bind="TOOLTIPS.gameRules">
        <h1 class="app__title">SET</h1>
      </MathTooltip>
      <p class="app__subtitle">Das Kartenspiel</p>
    </header>

    <!-- Score Board -->
    <div class="app__scoreboard-wrapper">
      <ScoreBoard
        :score="gameStore.score"
        :sets-found="gameStore.setsFound"
        :cards-in-deck="gameStore.cardsInDeck"
        :cards-on-board="gameStore.board.length"
      />
      <MathTooltip v-bind="TOOLTIPS.score" position="left">
        <div class="app__info-icon">ℹ️</div>
      </MathTooltip>
    </div>

    <!-- Controls Bar: Hint Button & Explanations -->
    <div v-if="gameStore.board.length > 0" class="app__controls">
      <MathTooltip v-bind="TOOLTIPS.hintButton" position="top">
        <HintButton />
      </MathTooltip>
      
      <!-- Educational Buttons -->
      <MathTooltip v-bind="TOOLTIPS.vectors" position="top">
        <button class="app__edu-button">Vektoren?</button>
      </MathTooltip>
      <MathTooltip v-bind="TOOLTIPS.combinatorics" position="top">
        <button class="app__edu-button">Kombinatorik?</button>
      </MathTooltip>
    </div>

    <!-- Game Board -->
    <main class="app__main">
      <GameBoard
        v-if="gameStore.board.length > 0"
        :cards="gameStore.board"
        :selected-card-ids="gameStore.selectedCardIds"
        :available-sets="gameStore.availableSets"
        :showing-hints="gameStore.showingHints"
        @card-click="gameStore.selectCard"
      />

      <!-- Start Screen -->
      <div v-else class="app__start">
        <h2 class="app__start-title">Willkommen zu SET!</h2>
        <p class="app__start-description">
          Finde Dreiergruppen von Karten, bei denen jedes Merkmal
          entweder bei allen drei Karten gleich oder bei allen verschieden ist.
        </p>
        <button class="app__start-button" @click="gameStore.startGame">
          Neues Spiel starten
        </button>
      </div>
    </main>

    <!-- Game Over Overlay -->
    <Transition name="fade">
      <div v-if="gameStore.gameOver" class="app__overlay">
        <div class="app__game-over">
          <h2 class="app__game-over-title">🎉 Spiel beendet!</h2>
          <p class="app__game-over-score">
            Endpunktzahl: <strong>{{ gameStore.score }}</strong>
          </p>
          <p class="app__game-over-sets">
            Sets gefunden: <strong>{{ gameStore.setsFound }}</strong>
          </p>
          <button class="app__start-button" @click="gameStore.startGame">
            Neues Spiel
          </button>
        </div>
      </div>
    </Transition>

    <!-- Selection Info -->
    <div v-if="gameStore.board.length > 0" class="app__selection-info">
      <MathTooltip v-bind="TOOLTIPS.thirdCard" position="left">
        <span>{{ gameStore.selectedCards.length }} von 3 Karten ausgewählt</span>
      </MathTooltip>
    </div>

    <!-- Hint Banner (Toast Messages) -->
    <HintBanner
      :message="gameStore.hintMessage"
      :type="gameStore.hintType"
    />
  </div>
</template>

<script setup>
/**
 * App.vue - Root Component
 * 
 * ARCHITECTURE:
 * This component connects the UI to the Pinia store.
 * It doesn't contain game logic - that's all in gameStore.js.
 * This separation makes the code easier to test and maintain.
 * 
 * COMPONENT HIERARCHY:
 * App.vue
 * ├── HistoryPanel.vue (sidebar log)
 * ├── ScoreBoard.vue (displays stats)
 * ├── HintButton.vue (toggles hints)
 * ├── MathTooltip.vue (educational popups)
 * ├── GameBoard.vue
 * │   └── Card.vue (x12-18 cards)
 * │       └── OvalSymbol.vue / DiamondSymbol.vue / WaveSymbol.vue
 * └── HintBanner.vue (toast notifications)
 */

import { onMounted } from 'vue';
import { useGameStore } from './stores/gameStore';
import { TOOLTIPS } from './stores/tooltipContent';
import GameBoard from './components/GameBoard.vue';
import ScoreBoard from './components/ScoreBoard.vue';
import HintBanner from './components/HintBanner.vue';
import HintButton from './components/HintButton.vue';
import HistoryPanel from './components/HistoryPanel.vue';
import MathTooltip from './components/MathTooltip.vue';

const gameStore = useGameStore();

/**
 * LIFECYCLE HOOK: onMounted
 * -------------------------
 * This function runs once when the component is first rendered to the DOM.
 * We use it to automatically start a new game when the page loads.
 * 
 * Other lifecycle hooks:
 * - onBeforeMount: Before component is rendered
 * - onUpdated: After component re-renders
 * - onUnmounted: When component is removed from DOM
 */
onMounted(() => {
  gameStore.startGame();
});
</script>

<style scoped>
/*
 * ROOT APP STYLING
 * ================
 * 
 * LAYOUT STRATEGY:
 * We use Flexbox with flex-direction: column to stack elements vertically.
 * The main content area gets flex: 1 to fill remaining space.
 * 
 * COLOR SCHEME:
 * Dark mode with purple/navy gradient background.
 * Accent colors: Cyan (#06b6d4), Purple (#a855f7)
 */

.app {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 1.5rem 1rem;
  
  /* 
   * CSS GRADIENT:
   * linear-gradient(direction, color1 stop1%, color2 stop2%)
   * 180deg = top to bottom
   */
  background: linear-gradient(180deg, #0f0d1a 0%, #1a1625 100%);
}

.app__header {
  text-align: center;
  margin-bottom: 1.5rem;
}

.app__title {
  font-size: 3rem;
  font-weight: 900;
  letter-spacing: -0.025em;
  cursor: help; /* Indicate tooltip available */
  
  /*
   * GRADIENT TEXT:
   * This technique fills text with a gradient instead of solid color.
   * 1. Set background to gradient
   * 2. Clip background to text shape
   * 3. Make text fill transparent so gradient shows through
   */
  background: linear-gradient(135deg, #06b6d4, #a855f7);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  margin: 0;
}

@media (min-width: 640px) {
  .app__title {
    font-size: 3.75rem;
  }
}

.app__subtitle {
  color: #9ca3af;
  font-size: 0.875rem;
  margin-top: 0.25rem;
}

/* Scoreboard Wrapper needed for Tooltip positioning */
.app__scoreboard-wrapper {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.app__info-icon {
  width: 1.5rem;
  height: 1.5rem;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.1);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.875rem;
  cursor: help;
  color: #9ca3af;
  transition: all 0.2s;
}

.app__info-icon:hover {
  background: rgba(255, 255, 255, 0.2);
  color: white;
}

/* Controls bar for hint button and future controls */
.app__controls {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 1rem;
  margin: 1rem 0;
  flex-wrap: wrap;
}

.app__edu-button {
  background: transparent;
  border: 1px solid rgba(255, 255, 255, 0.2);
  color: #d1d5db;
  padding: 0.5rem 1rem;
  border-radius: 9999px;
  font-size: 0.875rem;
  cursor: help;
  transition: all 0.2s;
}

.app__edu-button:hover {
  background: rgba(255, 255, 255, 0.1);
  border-color: rgba(255, 255, 255, 0.4);
  color: white;
}

.app__main {
  flex: 1; /* Grow to fill available space */
  width: 100%;
  max-width: 56rem;
}

.app__start {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  text-align: center;
  padding: 4rem 0;
}

.app__start-title {
  font-size: 1.875rem;
  font-weight: 700;
  color: white;
  margin-bottom: 1rem;
}

.app__start-description {
  color: #9ca3af;
  max-width: 24rem;
  margin-bottom: 2rem;
}

.app__start-button {
  padding: 1rem 2rem;
  border-radius: 9999px;
  font-weight: 700;
  font-size: 1.125rem;
  background: linear-gradient(to right, #06b6d4, #a855f7);
  color: white;
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
  cursor: pointer;
  border: none;
  transition: all 0.2s ease;
}

.app__start-button:hover {
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1);
  transform: scale(1.05);
}

.app__selection-info {
  position: fixed;
  top: 1rem;
  right: 1rem;
  padding: 0.5rem 1rem;
  border-radius: 9999px;
  background-color: rgba(26, 22, 37, 0.8);
  backdrop-filter: blur(4px);
  font-size: 0.875rem;
  color: #d1d5db;
  border: 1px solid rgba(255, 255, 255, 0.1);
  cursor: help;
}

/*
 * OVERLAY & MODAL
 * ---------------
 * Fixed positioning covers the entire viewport.
 * inset: 0 is shorthand for top/right/bottom/left: 0
 * z-index controls stacking order (higher = on top)
 */

.app__overlay {
  position: fixed;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: rgba(0, 0, 0, 0.8);
  backdrop-filter: blur(4px);
  z-index: 200;
}

.app__game-over {
  background-color: #1a1625;
  border-radius: 1rem;
  padding: 2rem;
  text-align: center;
  border: 1px solid rgba(255, 255, 255, 0.1);
  max-width: 24rem;
  margin: 0 1rem;
}

.app__game-over-title {
  font-size: 1.875rem;
  font-weight: 700;
  color: white;
  margin-bottom: 1rem;
}

.app__game-over-score,
.app__game-over-sets {
  color: #d1d5db;
  margin-bottom: 0.5rem;
}

.app__game-over strong {
  color: #06b6d4;
  font-size: 1.25rem;
}

/* Vue transition classes for fade effect */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
