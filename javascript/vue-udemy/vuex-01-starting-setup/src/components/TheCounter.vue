<script>
export default {
  computed: {
    count() {
      return this.$store.state.count;
    },
  },
};
</script>

<template>
  <h3>{{count}}</h3>
</template>

<style scoped>

</style>