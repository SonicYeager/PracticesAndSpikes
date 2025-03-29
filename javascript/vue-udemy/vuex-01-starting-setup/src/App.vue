<template>
  <base-container title="Vuex">
    <TheCounter></TheCounter>
    <ChangeCounter></ChangeCounter>
  </base-container>
</template>

<script>
import BaseContainer from './components/BaseContainer.vue';
import TheCounter from '@/components/TheCounter.vue';
import ChangeCounter from '@/components/ChangeCounter.vue';

export default {
  components: {
    ChangeCounter,
    TheCounter,
    BaseContainer,
  },
};
</script>

<style>
* {
  box-sizing: border-box;
}

html {
  font-family: sans-serif;
}

body {
  margin: 0;
}
</style>