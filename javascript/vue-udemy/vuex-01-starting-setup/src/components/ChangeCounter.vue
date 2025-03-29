<script>
export default {
  data: function() {
    return {
      increment: 2
    };
  },
  methods: {
    addCounter() {
      this.$store.dispatch('increase', this.increment);
    }
  }
};
</script>

<template>
  <button @click="addCounter">Add {{ this.increment }}</button>
</template>

<style scoped>

</style>