<template>
  <div class="app">
    <!-- Header -->
    <header class="app__header">
      <h1 class="app__title">SET</h1>
      <p class="app__subtitle">Das Kartenspiel</p>
    </header>

    <!-- Score Board -->
    <ScoreBoard
      :score="gameStore.score"
      :sets-found="gameStore.setsFound"
      :cards-in-deck="gameStore.cardsInDeck"
      :cards-on-board="gameStore.board.length"
    />

    <!-- Game Board -->
    <main class="app__main">
      <GameBoard
        v-if="gameStore.board.length > 0"
        :cards="gameStore.board"
        :selected-card-ids="gameStore.selectedCardIds"
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
      {{ gameStore.selectedCards.length }} von 3 Karten ausgewählt
    </div>

    <!-- Hint Banner -->
    <HintBanner
      :message="gameStore.hintMessage"
      :type="gameStore.hintType"
    />
  </div>
</template>

<script setup>
import { onMounted } from 'vue';
import { useGameStore } from './stores/gameStore';
import GameBoard from './components/GameBoard.vue';
import ScoreBoard from './components/ScoreBoard.vue';
import HintBanner from './components/HintBanner.vue';

const gameStore = useGameStore();

// Auto-start game on mount
onMounted(() => {
  gameStore.startGame();
});
</script>

<style scoped>
.app {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 1.5rem 1rem;
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

.app__main {
  flex: 1;
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
}

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

/* Fade transition */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
