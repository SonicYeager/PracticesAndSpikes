<script>
import ListData from '@/components/ListData.vue';

export default {
  components: {
    ListData,
  },
};
</script>

<template>
  <div class="container">
    <h1>Course Goals</h1>
    <list-data />
    <router-link to="/">All Users</router-link>
  </div>
</template>
