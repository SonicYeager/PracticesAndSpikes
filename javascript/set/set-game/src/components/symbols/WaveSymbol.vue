<template>
  <svg viewBox="0 0 60 30" class="w-full h-full">
    <path
      :d="wavePath"
      :fill="fillColor"
      :stroke="strokeColor"
      stroke-width="2"
      stroke-linejoin="round"
    />
    <!-- Stripes pattern for striped fill -->
    <defs v-if="fill === 'striped'">
      <pattern :id="patternId" patternUnits="userSpaceOnUse" width="4" height="4">
        <line x1="0" y1="0" x2="0" y2="4" :stroke="strokeColor" stroke-width="1.5" />
      </pattern>
      <clipPath :id="clipId">
        <path :d="wavePath" />
      </clipPath>
    </defs>
    <rect
      v-if="fill === 'striped'"
      x="0"
      y="0"
      width="60"
      height="30"
      :fill="`url(#${patternId})`"
      :clip-path="`url(#${clipId})`"
    />
  </svg>
</template>

<script setup>
import { computed } from 'vue';

const props = defineProps({
  color: {
    type: String,
    required: true,
    validator: (value) => ['red', 'green', 'purple'].includes(value),
  },
  fill: {
    type: String,
    required: true,
    validator: (value) => ['empty', 'striped', 'solid'].includes(value),
  },
  uniqueId: {
    type: String,
    required: true,
  },
});

// S-curve wave path - a proper closed shape
// This creates a wave/squiggle shape that looks like an S rotated 90 degrees
// The path draws the top edge, then the bottom edge, creating a closed shape
const wavePath = `
  M 4 15
  C 4 6, 15 4, 22 10
  C 29 16, 31 16, 38 10
  C 45 4, 56 6, 56 15
  C 56 24, 45 26, 38 20
  C 31 14, 29 14, 22 20
  C 15 26, 4 24, 4 15
  Z
`;

const colorMap = {
  red: '#ef4444',
  green: '#22c55e',
  purple: '#a855f7',
};

const strokeColor = computed(() => colorMap[props.color]);

const fillColor = computed(() => {
  if (props.fill === 'solid') return colorMap[props.color];
  return 'transparent';
});

const patternId = computed(() => `wave-pattern-${props.uniqueId}`);
const clipId = computed(() => `wave-clip-${props.uniqueId}`);
</script>
