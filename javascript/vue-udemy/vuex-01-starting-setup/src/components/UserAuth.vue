<script>
export default {
  methods: {
    login() {
      this.$store.dispatch('login');
    },
    logout() {
      this.$store.dispatch('logout');
    }
  },
  computed: {
    isAuthenticated() {
      return this.$store.getters.userIsAuthenticated;
    }
  }
};
</script>

<template>
  <button v-if="!isAuthenticated" @click="login()">Login</button>
  <button v-else @click="logout()">Logout</button>
</template>

<style scoped>

</style>