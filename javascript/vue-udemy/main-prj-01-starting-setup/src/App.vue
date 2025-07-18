<script>
import TheHeader from '@/components/layout/TheHeader.vue';

export default {
  components: { TheHeader },
};
</script>

<template>
  <the-header></the-header>
  <router-view v-slot="slotProps">
    <transition mode="out-in" name="route">
      <component :is="slotProps.Component"></component>
    </transition>
  </router-view>
</template>

<style scoped>
.route-enter-from {
  opacity: 0;
  transform: translateY(-30rem);
}

.route-leave-active {
  transition: all 0.3s ease-out;
}

.route-enter-active {
  transition: all 0.3s ease-in;
}

.route-leave-to {
  opacity: 0;
  transform: translateY(30rem);
}

.route-enter-to,
.route-leave-from {
  opacity: 1;
  transform: translateY(0);
}
</style>
