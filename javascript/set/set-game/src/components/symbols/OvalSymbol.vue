<template>
  <svg viewBox="0 0 60 30" class="w-full h-full">
    <ellipse
      cx="30"
      cy="15"
      rx="25"
      ry="12"
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
        <ellipse cx="30" cy="15" rx="24" ry="11" />
      </clipPath>
    </defs>
    <rect
      v-if="fill === 'striped'"
      x="5"
      y="3"
      width="50"
      height="24"
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

const patternId = computed(() => `oval-pattern-${props.uniqueId}`);
const clipId = computed(() => `oval-clip-${props.uniqueId}`);
</script>
