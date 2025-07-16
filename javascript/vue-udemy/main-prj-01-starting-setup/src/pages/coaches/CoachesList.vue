<script>
import CoachItem from '@/components/coaches/CoachItem.vue';
import BaseCard from '@/components/ui/BaseCard.vue';
import BaseButton from '@/components/ui/BaseButton.vue';
import CoachFilter from '@/components/coaches/CoachFilter.vue';
import BaseSpinner from '@/components/ui/BaseSpinner.vue';
import BaseDialog from '@/components/ui/BaseDialog.vue';

export default {
  components: {
    BaseDialog,
    BaseSpinner,
    CoachFilter,
    BaseButton,
    BaseCard,
    CoachItem,
  },
  data() {
    return {
      filters: {
        frontend: true,
        backend: true,
        career: true,
      },
      isLoading: false,
      error: null,
    };
  },
  computed: {
    filteredCoaches() {
      const coaches = this.$store.getters['coaches/coaches'];
      return coaches.filter((c) => {
        return (
          (this.filters.frontend && c.areas.includes('frontend')) ||
          (this.filters.backend && c.areas.includes('backend')) ||
          (this.filters.career && c.areas.includes('career'))
        );
      });
    },
    hasCoaches() {
      return !this.isLoading && this.$store.getters['coaches/hasCoaches'];
    },
    isCoach() {
      return this.$store.getters['coaches/isCoach'];
    },
  },
  async created() {
    await this.fetchCoaches();
  },
  methods: {
    setFilters(filters) {
      this.filters = filters;
    },
    async fetchCoaches() {
      this.isLoading = true;
      try {
        await this.$store.dispatch('coaches/fetchCoaches');
      } catch (error) {
        this.error = error.message || 'Failed to fetch coaches.';
      }
      this.isLoading = false;
    },
  },
};
</script>

<template>
  <base-dialog :show="!!error" title="Error" @close="error = null">
    <p>{{ error }}</p>
  </base-dialog>
  <section>
    <coach-filter @change-filter="setFilters"></coach-filter>
  </section>
  <section>
    <base-card>
      <div class="controls">
        <base-button mode="outline" @click="fetchCoaches">Refresh</base-button>
        <base-button v-if="!isCoach" link to="/register"
          >Register As Coach
        </base-button>
      </div>
      <div v-if="isLoading">
        <base-spinner></base-spinner>
      </div>
      <ul v-else-if="hasCoaches">
        <coach-item
          v-for="coach in filteredCoaches"
          :key="coach.id"
          :coach="coach"
        ></coach-item>
      </ul>
      <h3 v-else>No Coaches Available</h3>
    </base-card>
  </section>
</template>

<style scoped>
ul {
  list-style: none;
  margin: 0;
  padding: 0;
}

.controls {
  display: flex;
  justify-content: space-between;
}
</style>
