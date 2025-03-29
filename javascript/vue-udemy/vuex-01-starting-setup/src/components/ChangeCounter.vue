<script>
export default {
  methods: {
    addCounter() {
      this.$store.commit('increment');
    },
  }
};
</script>

<template>
  <button @click="addCounter">Add 1</button>
</template>

<style scoped>

</style>