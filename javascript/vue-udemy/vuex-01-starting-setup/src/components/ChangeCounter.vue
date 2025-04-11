<script>
import { mapActions } from 'vuex';

export default {
  data: function() {
    return {
      increment: 2
    };
  },
  methods: {
    ...mapActions(['increase']),
    addCounter() {
      this.increase(this.increment);
    }
  }
};
</script>

<template>
  <button @click="addCounter">Add {{ this.increment }}</button>
</template>

<style scoped>

</style>