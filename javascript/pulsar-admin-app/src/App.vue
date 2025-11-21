<script setup>
import {RouterLink, RouterView} from 'vue-router'
import ToastContainer from '@/components/ToastContainer.vue'
</script>

<template>
  <div class="h-dvh w-dvw flex flex-col">
    <header class="fixed top-0 left-0 right-0 z-50 bg-base-100 shadow-md">
      <div class="navbar flex-col py-4">
        <div class="navbar-start w-full justify-center mb-2">
          <h1 class="font-bold md:text-3xl lg:text-4xl">Pulsar Admin App</h1>
        </div>
        <div class="navbar-center">
          <ul class="menu menu-horizontal px-1">
            <li><RouterLink to="/">Dashboard</RouterLink></li>
            <li><RouterLink to="/topics">Topics</RouterLink></li>
            <li><RouterLink to="/preferences">Preferences</RouterLink></li>
          </ul>
        </div>
      </div>
    </header>

    <RouterView class="pt-38"/>
    <ToastContainer />
  </div>

</template>

<style scoped>

</style>
