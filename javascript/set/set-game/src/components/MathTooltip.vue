<template>
  <!--
    MATH TOOLTIP COMPONENT
    ======================
    A reusable tooltip that displays mathematical explanations.
    
    VUE CONCEPTS:
    - Slots: The <slot> element allows parent to inject content
    - v-html: Renders HTML string as actual HTML (use carefully!)
    - Mouse events: @mouseenter/@mouseleave for show/hide
  -->
  <div 
    class="tooltip-wrapper"
    @mouseenter="show"
    @mouseleave="hide"
  >
    <!-- 
      DEFAULT SLOT: 
      Whatever the parent puts between <MathTooltip>...</MathTooltip> 
      appears here. This is the "trigger" element.
    -->
    <slot></slot>
    
    <!-- Info icon to indicate tooltip is available -->
    <span class="tooltip-wrapper__icon">ⓘ</span>
    
    <!-- The tooltip itself (only visible when active) -->
    <Transition name="tooltip">
      <div 
        v-if="visible" 
        class="tooltip"
        :class="[`tooltip--${position}`]"
      >
        <div class="tooltip__title">{{ title }}</div>
        <!-- 
          v-html renders HTML from string.
          WARNING: Only use with trusted content! Never with user input.
        -->
        <div class="tooltip__content" v-html="content"></div>
      </div>
    </Transition>
  </div>
</template>

<script setup>
/**
 * MathTooltip.vue - Educational Tooltip Component
 * 
 * USAGE:
 * <MathTooltip :title="'Title'" :content="'<p>HTML content</p>'">
 *   <button>Hover me</button>
 * </MathTooltip>
 * 
 * Or with tooltip content from tooltipContent.js:
 * <MathTooltip v-bind="TOOLTIPS.vectors">
 *   <span>Vectors</span>
 * </MathTooltip>
 */

import { ref } from 'vue';

defineProps({
  /**
   * Title shown at the top of the tooltip
   */
  title: {
    type: String,
    required: true,
  },
  /**
   * HTML content of the tooltip.
   * Can include <p>, <ul>, <li>, <strong>, <code>, <pre>, <table>, etc.
   */
  content: {
    type: String,
    required: true,
  },
  /**
   * Position of the tooltip relative to trigger element.
   * Options: 'top', 'bottom', 'left', 'right'
   */
  position: {
    type: String,
    default: 'bottom',
    validator: (v) => ['top', 'bottom', 'left', 'right'].includes(v),
  },
});

const visible = ref(false);

function show() {
  visible.value = true;
}

function hide() {
  visible.value = false;
}
</script>

<style scoped>
/*
 * TOOLTIP WRAPPER
 * ---------------
 * The wrapper contains both the trigger element and the tooltip.
 * Position: relative creates a positioning context for the absolute tooltip.
 */

.tooltip-wrapper {
  position: relative;
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  cursor: help;
}

.tooltip-wrapper__icon {
  font-size: 0.75rem;
  opacity: 0.6;
  transition: opacity 0.2s;
}

.tooltip-wrapper:hover .tooltip-wrapper__icon {
  opacity: 1;
}

/*
 * TOOLTIP BOX
 * -----------
 * Absolute positioning places the tooltip relative to the wrapper.
 * z-index ensures it appears above other content.
 */

.tooltip {
  position: absolute;
  z-index: 1000;
  width: 320px;
  max-width: 90vw;
  
  /* Appearance */
  background: #1a1625;
  border: 1px solid rgba(255, 255, 255, 0.15);
  border-radius: 0.75rem;
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.3);
  
  /* Text */
  color: white;
  font-size: 0.875rem;
  line-height: 1.5;
  text-align: left;
  
  /* Padding */
  padding: 1rem;
}

/* Position variants */
.tooltip--bottom {
  top: 100%;
  left: 50%;
  transform: translateX(-50%);
  margin-top: 0.5rem;
}

.tooltip--top {
  bottom: 100%;
  left: 50%;
  transform: translateX(-50%);
  margin-bottom: 0.5rem;
}

.tooltip--left {
  right: 100%;
  top: 50%;
  transform: translateY(-50%);
  margin-right: 0.5rem;
}

.tooltip--right {
  left: 100%;
  top: 50%;
  transform: translateY(-50%);
  margin-left: 0.5rem;
}

.tooltip__title {
  font-weight: 700;
  font-size: 1rem;
  margin-bottom: 0.75rem;
  color: #06b6d4; /* Cyan accent */
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  padding-bottom: 0.5rem;
}

/*
 * TOOLTIP CONTENT STYLING
 * -----------------------
 * We use :deep() to style HTML rendered via v-html.
 * Without :deep(), scoped styles wouldn't apply to v-html content.
 */

.tooltip__content :deep(p) {
  margin: 0.5rem 0;
}

.tooltip__content :deep(ul) {
  margin: 0.5rem 0;
  padding-left: 1.25rem;
}

.tooltip__content :deep(li) {
  margin: 0.25rem 0;
}

.tooltip__content :deep(strong) {
  color: #f97316; /* Orange accent */
}

.tooltip__content :deep(em) {
  color: #a855f7; /* Purple accent */
}

.tooltip__content :deep(code) {
  background: rgba(255, 255, 255, 0.1);
  padding: 0.125rem 0.375rem;
  border-radius: 0.25rem;
  font-family: 'Consolas', 'Monaco', monospace;
  font-size: 0.8em;
}

.tooltip__content :deep(pre) {
  background: rgba(0, 0, 0, 0.3);
  padding: 0.5rem;
  border-radius: 0.375rem;
  overflow-x: auto;
  font-family: 'Consolas', 'Monaco', monospace;
  font-size: 0.85em;
  margin: 0.5rem 0;
}

.tooltip__content :deep(table) {
  width: 100%;
  border-collapse: collapse;
  margin: 0.5rem 0;
  font-size: 0.8rem;
}

.tooltip__content :deep(th),
.tooltip__content :deep(td) {
  padding: 0.25rem 0.5rem;
  border: 1px solid rgba(255, 255, 255, 0.1);
  text-align: center;
}

.tooltip__content :deep(th) {
  background: rgba(255, 255, 255, 0.05);
}

/*
 * TRANSITION ANIMATION
 * --------------------
 * Vue transition classes for smooth fade-in/out
 */

.tooltip-enter-active,
.tooltip-leave-active {
  transition: all 0.2s ease;
}

.tooltip-enter-from,
.tooltip-leave-to {
  opacity: 0;
  transform: translateX(-50%) translateY(10px);
}
</style>
