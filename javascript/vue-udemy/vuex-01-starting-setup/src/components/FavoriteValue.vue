<script>
export default {
  computed: {
    count() {
      return this.$store.getters.doubleCount;
    },
  },
};
</script>

<template>
  <h3>{{count}}</h3>
  <p>More is happening here</p>
</template>

<style scoped>

</style>