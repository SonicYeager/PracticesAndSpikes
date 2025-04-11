<script>
import { mapGetters } from 'vuex';

export default {
  computed: {
    ...mapGetters(['count'])
  },
};
</script>

<template>
  <h3>{{count}}</h3>
</template>

<style scoped>

</style>