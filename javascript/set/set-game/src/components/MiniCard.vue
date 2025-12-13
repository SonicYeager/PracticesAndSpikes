<template>
  <!--
    MINI CARD COMPONENT
    ===================
    A small, simplified version of a card for use in the history panel.
    Shows the card's properties in a compact format.
  -->
  <div class="mini-card">
    <div 
      class="mini-card__symbol"
      :style="{ color: colorHex }"
    >
      {{ symbolChar }}
    </div>
  </div>
</template>

<script setup>
/**
 * MiniCard.vue - Compact Card Display
 * 
 * Used in the history panel to show which cards were in a SET.
 * Much smaller than the full Card component.
 */

import { computed } from 'vue';

const props = defineProps({
  card: {
    type: Object,
    required: true,
  },
});

// Simple character symbols for shapes
const symbolMap = {
  oval: '●',
  diamond: '◆',
  wave: '∿',
};

const symbolChar = computed(() => {
  const shape = symbolMap[props.card.shapeLabel] || '?';
  // Repeat symbol based on count (1, 2, or 3 times)
  return shape.repeat(props.card.count + 1);
});

const colorMap = {
  red: '#ef4444',
  green: '#22c55e',
  purple: '#a855f7',
};

const colorHex = computed(() => colorMap[props.card.colorLabel] || '#fff');
</script>

<style scoped>
.mini-card {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 0.25rem;
  padding: 0.125rem 0.25rem;
  min-width: 2rem;
}

.mini-card__symbol {
  font-size: 0.625rem;
  font-weight: 700;
  letter-spacing: 1px;
}
</style>
