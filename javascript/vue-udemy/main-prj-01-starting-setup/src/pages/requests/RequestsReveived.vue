<script setup>
import { computed } from 'vue';
import { useStore } from 'vuex';
import RequestItem from '@/components/requests/RequestItem.vue';

const store = useStore();

const receivedRequests = computed(() => {
  return store.getters['requests/requests'];
});
const hasRequests = computed(() => {
  return store.getters['requests/hasRequests'];
});
</script>

<template>
  <section>
    <base-card>
      <header>
        <h2>Requests Reveived</h2>
      </header>
      <ul v-if="hasRequests">
        <request-item
          v-for="item in receivedRequests"
          :key="item.id"
          :email="item.userEmail"
          :message="item.message"
        ></request-item>
      </ul>
      <h3 v-else>No Requests Available</h3>
    </base-card>
  </section>
</template>

<style scoped>
header {
  text-align: center;
}

ul {
  list-style: none;
  margin: 2rem auto;
  padding: 0;
  max-width: 30rem;
}

h3 {
  text-align: center;
}
</style>
