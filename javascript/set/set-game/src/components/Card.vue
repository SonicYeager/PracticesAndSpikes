<template>
  <div
    class="card"
    :class="{
      'card--selected': isSelected,
      'card--correct': state === 'correct',
      'card--wrong': state === 'wrong',
    }"
    @click="$emit('click')"
  >
    <div class="card__symbols">
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
import { computed } from 'vue';
import OvalSymbol from './symbols/OvalSymbol.vue';
import DiamondSymbol from './symbols/DiamondSymbol.vue';
import WaveSymbol from './symbols/WaveSymbol.vue';

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
});

defineEmits(['click']);

// Number of symbols to display (1, 2, or 3)
const symbolCount = computed(() => props.card.count + 1);

// Which symbol component to use
const symbolComponent = computed(() => {
  const components = {
    oval: OvalSymbol,
    diamond: DiamondSymbol,
    wave: WaveSymbol,
  };
  return components[props.card.shapeLabel];
});
</script>

<style scoped>
.card {
  background-color: #f5f5f0;
  border-radius: 0.75rem;
  padding: 1rem;
  cursor: pointer;
  transition: all 0.2s ease;
  border: 4px solid transparent;
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -4px rgba(0, 0, 0, 0.1);
  display: flex;
  align-items: center;
  justify-content: center;
  aspect-ratio: 3 / 4;
  min-height: 140px;
}

.card:hover {
  transform: scale(1.02);
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 8px 10px -6px rgba(0, 0, 0, 0.1);
}

.card--selected {
  border-color: #06b6d4;
  box-shadow: 0 0 20px rgba(6, 182, 212, 0.5);
  transform: scale(1.05);
}

.card--correct {
  border-color: #22c55e;
  box-shadow: 0 0 20px rgba(34, 197, 94, 0.5);
  animation: pulse-success 0.5s ease-out;
}

.card--wrong {
  border-color: #ef4444;
  box-shadow: 0 0 20px rgba(239, 68, 68, 0.5);
  animation: shake 0.5s ease-out;
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
