<template>
  <!--
    CARD COMPONENT
    ==============
    This component displays a single SET game card.
    
    VUE CONCEPTS USED:
    - Props: Data passed from parent component (card data, selection state)
    - Computed Properties: Reactive calculations (symbol count, component type)
    - Dynamic Components: <component :is="..."> to render different symbols
    - Class Bindings: :class="{}" for conditional CSS classes
    - Events: @click to emit click events to parent
  -->
  <div
    class="card"
    :class="{
      'card--selected': isSelected,
      'card--correct': state === 'correct',
      'card--wrong': state === 'wrong',
    }"
    @click="$emit('click')"
  >
    <!-- Hint Badges: Show colored+numbered badges when hints are active -->
    <div v-if="hintSetIndices.length > 0" class="card__badges">
      <span 
        v-for="setIndex in hintSetIndices" 
        :key="setIndex"
        class="card__badge"
        :style="{ backgroundColor: getSetColor(setIndex) }"
      >
        {{ setIndex + 1 }}
      </span>
    </div>

    <!-- Card Symbols: 1-3 symbols displayed vertically -->
    <div class="card__symbols">
      <!--
        DYNAMIC COMPONENT RENDERING:
        The :is attribute allows us to dynamically choose which component to render.
        Based on the card's shape, we render OvalSymbol, DiamondSymbol, or WaveSymbol.
      -->
      <component
        v-for="i in symbolCount"
        :key="i"
        :is="symbolComponent"
        :color="card.colorLabel"
        :fill="card.fillLabel"
        :unique-id="`${card.id}-${i}`"
        class="card__symbol"
      />
    </div>
  </div>
</template>

<script setup>
/**
 * Card.vue - Single SET Game Card Component
 * 
 * VECTOR REPRESENTATION:
 * Each card represents a point in the vector space F₃⁴ (4-dimensional space over field with 3 elements).
 * The 4 dimensions are: count (0-2), color (0-2), fill (0-2), shape (0-2).
 * 
 * VISUAL MAPPING:
 * - count: 0→1 symbol, 1→2 symbols, 2→3 symbols
 * - color: 0→red, 1→green, 2→purple
 * - fill: 0→empty, 1→striped, 2→solid
 * - shape: 0→oval, 1→diamond, 2→wave
 */

import { computed } from 'vue';
import OvalSymbol from './symbols/OvalSymbol.vue';
import DiamondSymbol from './symbols/DiamondSymbol.vue';
import WaveSymbol from './symbols/WaveSymbol.vue';

/**
 * PROPS EXPLANATION:
 * Props are the "inputs" to a Vue component. The parent component passes data down.
 * 
 * - card: The card data object with count, color, fill, shape properties
 * - isSelected: Whether this card is currently selected by the player
 * - state: Visual state for animations ('normal', 'correct', 'wrong')
 * - hintSetIndices: Array of SET indices this card belongs to (for hint display)
 */
const props = defineProps({
  card: {
    type: Object,
    required: true,
  },
  isSelected: {
    type: Boolean,
    default: false,
  },
  state: {
    type: String,
    default: 'normal',
    validator: (value) => ['normal', 'selected', 'correct', 'wrong'].includes(value),
  },
  hintSetIndices: {
    type: Array,
    default: () => [],
  },
});

defineEmits(['click']);

/**
 * COMPUTED PROPERTIES:
 * Computed properties are reactive: they automatically update when their dependencies change.
 * Vue tracks which reactive data they depend on and recalculates when needed.
 */

// Number of symbols to display (1, 2, or 3)
// The card stores count as 0, 1, or 2, so we add 1 to get the actual count
const symbolCount = computed(() => props.card.count + 1);

// Which symbol component to use based on the card's shape
const symbolComponent = computed(() => {
  const components = {
    oval: OvalSymbol,
    diamond: DiamondSymbol,
    wave: WaveSymbol,
  };
  return components[props.card.shapeLabel];
});

/**
 * Color palette for SET hint badges.
 * Each SET on the board gets a unique color + number combination.
 * 
 * MODULO OPERATION:
 * If there are more than 5 SETs, we cycle through colors using modulo.
 * setIndex % 5 gives us: 0, 1, 2, 3, 4, 0, 1, 2, 3, 4, ...
 */
const SET_COLORS = [
  '#f97316', // Orange
  '#3b82f6', // Blue
  '#ec4899', // Pink
  '#eab308', // Yellow
  '#14b8a6', // Teal
];

function getSetColor(setIndex) {
  return SET_COLORS[setIndex % SET_COLORS.length];
}
</script>

<style scoped>
/*
 * CARD STYLING
 * ============
 * 
 * CSS LAYOUT CONCEPT: Flexbox
 * ---------------------------
 * We use display: flex to center content both horizontally and vertically.
 * - justify-content: center → horizontal centering
 * - align-items: center → vertical centering
 * 
 * CSS CONCEPT: Box Model
 * ----------------------
 * Every element has: content → padding → border → margin
 * - padding: space inside the border
 * - border: the visible edge
 * - margin: space outside the border (not used here)
 */

.card {
  /* Position relative allows absolute positioning of badge children */
  position: relative;
  
  /* Card appearance */
  background-color: #f5f5f0;
  border-radius: 0.75rem;
  padding: 1rem;
  
  /* Border: starts transparent, changes color when selected */
  border: 4px solid transparent;
  
  /* Shadow for depth effect */
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -4px rgba(0, 0, 0, 0.1);
  
  /* Flexbox centering */
  display: flex;
  align-items: center;
  justify-content: center;
  
  /* Aspect ratio keeps cards proportional (3 wide : 4 tall) */
  aspect-ratio: 3 / 4;
  min-height: 140px;
  
  /* Interactive */
  cursor: pointer;
  
  /*
   * CSS TRANSITIONS:
   * Smooth animation between states. Format: property duration timing-function
   * "all" means animate all changing properties
   * "0.2s" is 200 milliseconds
   * "ease" starts slow, speeds up, then slows down
   */
  transition: all 0.2s ease;
}

.card:hover {
  /* Scale up slightly on hover for interactive feedback */
  transform: scale(1.02);
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 8px 10px -6px rgba(0, 0, 0, 0.1);
}

.card--selected {
  /* Cyan border and glow effect for selected cards */
  border-color: #06b6d4;
  box-shadow: 0 0 20px rgba(6, 182, 212, 0.5);
  transform: scale(1.05);
}

.card--correct {
  /* Green for correct SET */
  border-color: #22c55e;
  box-shadow: 0 0 20px rgba(34, 197, 94, 0.5);
  animation: pulse-success 0.5s ease-out;
}

.card--wrong {
  /* Red for wrong SET */
  border-color: #ef4444;
  box-shadow: 0 0 20px rgba(239, 68, 68, 0.5);
  animation: shake 0.5s ease-out;
}

/*
 * HINT BADGES
 * -----------
 * Small colored circles with numbers in the top-left corner.
 * Each badge represents a SET this card belongs to.
 */
.card__badges {
  /* Absolute positioning: relative to the .card parent */
  position: absolute;
  top: 0.25rem;
  left: 0.25rem;
  
  /* Arrange badges horizontally with small gap */
  display: flex;
  gap: 0.25rem;
  flex-wrap: wrap;
  max-width: 60%;
}

.card__badge {
  /* Circle shape */
  width: 1.25rem;
  height: 1.25rem;
  border-radius: 50%;
  
  /* Center the number inside */
  display: flex;
  align-items: center;
  justify-content: center;
  
  /* Text styling */
  color: white;
  font-size: 0.625rem;
  font-weight: 700;
  
  /* Subtle shadow for depth */
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
}

.card__symbols {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  width: 100%;
}

.card__symbol {
  width: 100%;
  max-width: 80px;
  height: auto;
}

/*
 * CSS KEYFRAME ANIMATIONS
 * -----------------------
 * @keyframes defines animation steps.
 * The animation property on an element triggers the animation.
 * 
 * pulse-success: Scale up and down to celebrate correct SET
 * shake: Wiggle left/right to indicate wrong SET
 */

@keyframes pulse-success {
  0%, 100% {
    transform: scale(1.05);
  }
  50% {
    transform: scale(1.1);
  }
}

@keyframes shake {
  0%, 100% {
    transform: translateX(0);
  }
  20%, 60% {
    transform: translateX(-5px);
  }
  40%, 80% {
    transform: translateX(5px);
  }
}
</style>
