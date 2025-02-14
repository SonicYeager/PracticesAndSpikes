<template>
  <button @click="saveChanges">Save Changes</button>
  <ul>
    <user-item v-for="user in users" :key="user.id" :name="user.fullName" :role="user.role"></user-item>
  </ul>
</template>

<script>
import UserItem from '../components/users/UserItem.vue';

export default {
  components: {
    UserItem
  },
  inject: ['users'],
  beforeRouteEnter(to, from, next) {
    next();
  },
  data() {
    return {
      changedSaved: false
    };
  },
  methods: {
    saveChanges() {
      this.changedSaved = true;
    }
  },
  beforeRouteLeave(to, from, next) {
    if (this.changedSaved) {
      next();
    } else {
      const confirmed = window.confirm('Do you want to leave without saving changes?');
      if (confirmed) {
        next();
      } else {
        next(false);
      }
    }
  }
};
</script>

<style scoped>
ul {
  list-style: none;
  margin: 2rem auto;
  max-width: 20rem;
  padding: 0;
}
</style>