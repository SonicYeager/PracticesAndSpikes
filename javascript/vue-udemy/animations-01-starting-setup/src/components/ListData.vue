<script>
export default {
  data() {
    return {
      users: ['John', 'Doe', 'Jane'],
    };
  },
  methods: {
    /**
     * Adds a new user to the list.
     */
    addUser() {
      const input = this.$refs.userInput.value;
      this.users.unshift(input);
    },
    /**
     * Removes a user from the list.
     * @param {string} user - The user to remove.
     */
    removeUser(user) {
      this.users = this.users.filter((u) => u !== user);
    },
  },
};
</script>

<template>
  <transition-group name="data-list" tag="ul">
    <li v-for="user in users" :key="user" @click="removeUser(user)">
      {{ user }}
    </li>
  </transition-group>
  <div>
    <input ref="userInput" type="text" />
    <button @click="addUser">Add User</button>
  </div>
</template>

<style scoped>
ul {
  list-style: none;
  margin: 1rem 0;
  padding: 0;
}

li {
  border: 1px solid #ccc;
  padding: 1rem;
  text-align: center;
}

.data-list-enter-from,
.data-list-leave-to {
  opacity: 0;
  transform: translateX(-30px);
}

.data-list-enter-active {
  transition: all 0.5s;
}

.data-list-leave-active {
  transition: all 0.5s;
  position: absolute;
}

.data-list-enter-to,
.data-list-leave-from {
  opacity: 1;
  transform: translateX(0);
}

.data-list-move {
  transition: all 0.5s;
}
</style>
