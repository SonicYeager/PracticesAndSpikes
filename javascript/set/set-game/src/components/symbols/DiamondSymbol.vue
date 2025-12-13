<template>
  <svg viewBox="0 0 60 30" class="w-full h-full">
    <polygon
      points="30,2 58,15 30,28 2,15"
      :fill="fillColor"
      :stroke="strokeColor"
      stroke-width="2"
    />
    <!-- Stripes pattern for striped fill -->
    <defs v-if="fill === 'striped'">
      <pattern :id="patternId" patternUnits="userSpaceOnUse" width="4" height="4">
        <line x1="0" y1="0" x2="0" y2="4" :stroke="strokeColor" stroke-width="1.5" />
      </pattern>
      <clipPath :id="clipId">
        <polygon points="30,3 57,15 30,27 3,15" />
      </clipPath>
    </defs>
    <rect
      v-if="fill === 'striped'"
      x="2"
      y="2"
      width="56"
      height="26"
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

const patternId = computed(() => `diamond-pattern-${props.uniqueId}`);
const clipId = computed(() => `diamond-clip-${props.uniqueId}`);
</script>
