<script setup>
import { computed, onMounted, ref } from 'vue';
import { useStore } from 'vuex';
import RequestItem from '@/components/requests/RequestItem.vue';
import BaseDialog from '@/components/ui/BaseDialog.vue';
import BaseSpinner from '@/components/ui/BaseSpinner.vue';

const store = useStore();
const isLoading = ref(false);
const error = ref(null);

const receivedRequests = computed(() => {
  return store.getters['requests/requests'];
});
const hasRequests = computed(() => {
  return store.getters['requests/hasRequests'];
});

const loadRequests = async () => {
  try {
    isLoading.value = true;
    await store.dispatch('requests/fetchRequests');
    isLoading.value = false;
  } catch (err) {
    error.value = err.message || 'Failed to load requests.';
  }
};

onMounted(() => {
  loadRequests();
});
</script>

<template>
  <base-dialog :show="!!error" title="Error" @close="error = null">
    <p>{{ error }}</p>
  </base-dialog>
  <section>
    <base-card>
      <header>
        <h2>Requests Reveived</h2>
      </header>
      <base-spinner v-if="isLoading"></base-spinner>
      <ul v-else-if="hasRequests && !isLoading">
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
