<template>
  <router-view v-slot="slotProps">
    <transition mode="out-in" name="route">
      <component :is="slotProps.Component" />
    </transition>
  </router-view>
</template>

<script></script>

<style>
.route-enter-from {
  opacity: 0;
}

.route-enter-active {
  transition: opacity 1s;
}

.route-enter-to {
  opacity: 1;
}
</style>
