<script>
export default {
  props: {
    modelValue: String
  },
  emits: ['update:modelValue'],
  computed: {
    activeRating() {
      return this.modelValue;
    }
  },
  methods: {
    setActiveRating(rating) {
      this.activeRating = rating;
      this.$emit('update:modelValue', rating);
    }
  }
};
</script>

<template>
  <ul>
    <li :class="{ active: activeRating === 'poor' }">
      <button type="button" @click="setActiveRating('poor')">Poor</button>
    </li>
    <li :class="{ active: activeRating === 'average' }">
      <button type="button" @click="setActiveRating('average')">Average</button>
    </li>
    <li :class="{ active: activeRating === 'great' }">
      <button type="button" @click="setActiveRating('great')">Great</button>
    </li>
  </ul>
</template>

<style scoped>
.active {
  background-color: purple;
}

.active button {
  color: white;
}

ul {
  list-style-type: none;
  margin: 0.5rem 0;
  padding: 0;
  display: flex;
}

li {
  margin: 0 1rem;
  border: 1px solid #ccc;
  padding: 1rem;
  display: flex;
  justify-content: center;
  align-items: center;
}

button {
  font: inherit;
  background-color: transparent;
  border: none;
  cursor: pointer;
}
</style>