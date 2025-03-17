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
    <h1>All Users</h1>
    <list-data />
    <router-link to="/goals">Course Goals</router-link>
  </div>
</template>
