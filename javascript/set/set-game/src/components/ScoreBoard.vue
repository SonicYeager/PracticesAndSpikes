<template>
  <div class="score-board">
    <div class="score-board__item">
      <span class="score-board__label">Punkte</span>
      <span class="score-board__value">{{ score }}</span>
    </div>
    <div class="score-board__item">
      <span class="score-board__label">Sets gefunden</span>
      <span class="score-board__value">{{ setsFound }}</span>
    </div>
    <div class="score-board__item">
      <span class="score-board__label">Karten im Deck</span>
      <span class="score-board__value">{{ cardsInDeck }}</span>
    </div>
    <div class="score-board__item">
      <span class="score-board__label">Karten auf dem Tisch</span>
      <span class="score-board__value">{{ cardsOnBoard }}</span>
    </div>
  </div>
</template>

<script setup>
defineProps({
  score: {
    type: Number,
    default: 0,
  },
  setsFound: {
    type: Number,
    default: 0,
  },
  cardsInDeck: {
    type: Number,
    default: 0,
  },
  cardsOnBoard: {
    type: Number,
    default: 0,
  },
});
</script>

<style scoped>
.score-board {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 1rem;
  padding: 1rem;
  background-color: rgba(26, 22, 37, 0.8);
  backdrop-filter: blur(4px);
  border-radius: 0.75rem;
  border: 1px solid rgba(255, 255, 255, 0.1);
}

@media (min-width: 640px) {
  .score-board {
    gap: 2rem;
  }
}

.score-board__item {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.score-board__label {
  font-size: 0.75rem;
  color: #9ca3af;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.score-board__value {
  font-size: 1.5rem;
  font-weight: 700;
  color: white;
}

@media (min-width: 640px) {
  .score-board__value {
    font-size: 1.875rem;
  }
}
</style>
