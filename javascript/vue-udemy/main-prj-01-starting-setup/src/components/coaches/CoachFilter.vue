<script>
export default {
  emits: ['change-filter'],
  data() {
    return {
      filters: {
        frontend: true,
        backend: true,
        career: true,
      },
    };
  },
  methods: {
    setFilter(event) {
      const inputId = event.target.id;
      const isActive = event.target.checked;
      const updatedFilters = {
        ...this.filters,
        [inputId]: isActive,
      };
      this.filters = updatedFilters;
      this.$emit('change-filter', updatedFilters);
    },
  },
};
</script>

<template>
  <base-card>
    <h2>Find Your Coach</h2>
    <span class="filter-option">
      <input
        id="frontend"
        :checked="filters.frontend"
        type="checkbox"
        @change="setFilter"
      />
      <label for="frontend">Frontend</label>
      <input
        id="backend"
        :checked="filters.backend"
        type="checkbox"
        @change="setFilter"
      />
      <label for="backend">Backend</label>
      <input
        id="career"
        :checked="filters.career"
        type="checkbox"
        @change="setFilter"
      />
      <label for="career">Career</label>
    </span>
  </base-card>
</template>

<style scoped>
h2 {
  margin: 0.5rem 0;
}

.filter-option {
  margin-right: 1rem;
}

.filter-option label,
.filter-option input {
  vertical-align: middle;
}

.filter-option label {
  margin-left: 0.25rem;
}

.filter-option.active label {
  font-weight: bold;
}
</style>
