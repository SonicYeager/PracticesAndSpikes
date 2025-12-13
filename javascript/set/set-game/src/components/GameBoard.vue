<template>
  <div class="game-board">
    <TransitionGroup name="card" tag="div" class="game-board__grid">
      <Card
        v-for="card in cards"
        :key="card.id"
        :card="card"
        :is-selected="selectedCardIds.includes(card.id)"
        @click="$emit('card-click', card)"
      />
    </TransitionGroup>
  </div>
</template>

<script setup>
import Card from './Card.vue';

defineProps({
  cards: {
    type: Array,
    required: true,
  },
  selectedCardIds: {
    type: Array,
    default: () => [],
  },
});

defineEmits(['card-click']);
</script>

<style scoped>
.game-board {
  width: 100%;
  max-width: 56rem;
  margin: 0 auto;
  padding: 1rem;
}

.game-board__grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 0.75rem;
}

@media (min-width: 640px) {
  .game-board__grid {
    grid-template-columns: repeat(4, 1fr);
    gap: 1rem;
  }
}

/* Transition animations */
.card-enter-active,
.card-leave-active {
  transition: all 0.3s ease;
}

.card-enter-from {
  opacity: 0;
  transform: scale(0.8) translateY(-20px);
}

.card-leave-to {
  opacity: 0;
  transform: scale(0.8) translateY(20px);
}

.card-move {
  transition: transform 0.3s ease;
}
</style>
