<template>
  <!--
    GAMEBOARD COMPONENT
    ===================
    Displays the grid of cards currently on the table.
    
    VUE CONCEPTS:
    - TransitionGroup: Animates cards entering/leaving the board
    - v-for: Loop to render multiple Card components
    - Computed functions: Calculate hint indices for each card
  -->
  <div class="game-board">
    <TransitionGroup name="card" tag="div" class="game-board__grid">
      <Card
        v-for="card in cards"
        :key="card.id"
        :card="card"
        :is-selected="selectedCardIds.includes(card.id)"
        :hint-set-indices="getHintIndices(card)"
        @click="$emit('card-click', card)"
      />
    </TransitionGroup>
  </div>
</template>

<script setup>
/**
 * GameBoard.vue - The Playing Field
 * 
 * RESPONSIBILITY:
 * - Display cards in a responsive grid (3 columns mobile, 4 columns desktop)
 * - Pass selection and hint state to individual cards
 * - Handle card click events and forward them to the parent
 * 
 * CSS GRID vs FLEXBOX:
 * - Grid: 2D layouts (rows AND columns) - perfect for card grids
 * - Flexbox: 1D layouts (row OR column) - better for navigation bars
 */

import Card from './Card.vue';

const props = defineProps({
  cards: {
    type: Array,
    required: true,
  },
  selectedCardIds: {
    type: Array,
    default: () => [],
  },
  /**
   * NEW: Available SETs for hint display
   * Each SET is an array of 3 cards: [[card, card, card], [card, card, card], ...]
   */
  availableSets: {
    type: Array,
    default: () => [],
  },
  /**
   * NEW: Whether hints are currently being shown
   */
  showingHints: {
    type: Boolean,
    default: false,
  },
});

defineEmits(['card-click']);

/**
 * Get the SET indices that a specific card belongs to.
 * 
 * ALGORITHM:
 * For each available SET, check if this card is one of the 3 cards.
 * If yes, add that SET's index to the result array.
 * 
 * EXAMPLE:
 * If card #5 is in SET #0 and SET #2, this returns [0, 2]
 * The Card component will then display badges "1" and "3"
 * 
 * @param {Object} card - The card to check
 * @returns {number[]} Array of SET indices this card belongs to
 */
function getHintIndices(card) {
  // If hints are not shown, return empty array (no badges)
  if (!props.showingHints) return [];
  
  const indices = [];
  
  // Check each available SET
  props.availableSets.forEach((set, index) => {
    // A SET is an array of 3 cards
    // Check if any of them has the same ID as our card
    const cardIsInSet = set.some(setCard => setCard.id === card.id);
    
    if (cardIsInSet) {
      indices.push(index);
    }
  });
  
  return indices;
}
</script>

<style scoped>
/*
 * GRID LAYOUT
 * ===========
 * 
 * CSS GRID CONCEPT:
 * display: grid creates a grid container.
 * grid-template-columns: defines how many columns and their sizes.
 * 
 * repeat(3, 1fr) means:
 * - Create 3 columns
 * - Each column gets 1fr (1 fraction of available space)
 * - Result: 3 equal-width columns
 * 
 * gap: space between grid items (like margin but automatic)
 */

.game-board {
  width: 100%;
  max-width: 56rem; /* Limit width on large screens */
  margin: 0 auto;   /* Center horizontally */
  padding: 1rem;
}

.game-board__grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr); /* 3 columns on mobile */
  gap: 0.75rem;
}

/*
 * RESPONSIVE DESIGN with @media
 * -----------------------------
 * @media queries apply styles only when conditions are met.
 * min-width: 640px means "screens 640px wide or larger"
 * 
 * On larger screens, we use 4 columns instead of 3.
 */
@media (min-width: 640px) {
  .game-board__grid {
    grid-template-columns: repeat(4, 1fr); /* 4 columns on desktop */
    gap: 1rem;
  }
}

/*
 * VUE TRANSITION ANIMATIONS
 * -------------------------
 * Vue's <TransitionGroup> uses special CSS classes:
 * - .card-enter-from: Initial state when card appears
 * - .card-enter-active: Applied during enter animation
 * - .card-leave-to: Final state when card disappears
 * - .card-leave-active: Applied during leave animation
 * - .card-move: Applied when cards are reordering
 */

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
