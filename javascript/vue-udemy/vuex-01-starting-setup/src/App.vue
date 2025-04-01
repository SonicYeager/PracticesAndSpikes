<template>
  <base-container v-if="isAuthenticated" title="Vuex">
    <TheCounter></TheCounter>
    <favorite-value></favorite-value>
    <ChangeCounter></ChangeCounter>
  </base-container>
  <base-container title="Authentication">
    <user-auth></user-auth>
  </base-container>
</template>

<script>
import BaseContainer from './components/BaseContainer.vue';
import TheCounter from '@/components/TheCounter.vue';
import ChangeCounter from '@/components/ChangeCounter.vue';
import FavoriteValue from '@/components/FavoriteValue.vue';
import UserAuth from '@/components/UserAuth.vue';

export default {
  components: {
    UserAuth,
    FavoriteValue,
    ChangeCounter,
    TheCounter,
    BaseContainer
  },
  computed: {
    isAuthenticated() {
      return this.$store.getters.userIsAuthenticated;
    }
  }
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