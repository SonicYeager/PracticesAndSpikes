<script setup>
import {RouterLink, RouterView} from 'vue-router'
</script>

<template>
  <div class="h-dvh w-dvw flex flex-col">
    <header class="fixed top-0 left-0 right-0 z-50 bg-base-100 shadow-md">
      <div class="navbar flex-col py-4">
        <div class="navbar-start w-full justify-center mb-2">
          <h1 class="font-bold md:text-3xl lg:text-4xl">Pulsar Admin App</h1>
        </div>
        <div class="navbar-center">
          <div class="menu menu-horizontal px-1">
            <li><RouterLink to="/">Dashboard</RouterLink></li>
            <li><RouterLink to="/preferences">Preferences</RouterLink></li>
          </div>
        </div>
      </div>
    </header>

    <RouterView class="pt-38"/>
  </div>

</template>

<style scoped>

</style>
