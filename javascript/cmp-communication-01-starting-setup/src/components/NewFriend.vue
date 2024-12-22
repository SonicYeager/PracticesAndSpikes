<template>
  <form @submit.prevent="submitForm">
    <div>
      <label for="name">Name</label>
      <input type="text" id="name" name="name" placeholder="Name" v-model="name">
    </div>
    <div>
      <label for="phone">Phone</label>
      <input type="tel" id="phone" name="phone" placeholder="Phone" v-model="phone">
    </div>
    <div>
      <label for="email">Email</label>
      <input type="email" id="email" name="email" placeholder="Email" v-model="email">
    </div>
    <div>
      <button type="submit" @click="submitForm">Add Friend</button>
    </div>
  </form>
</template>

<script>
export default {
  data() {
    return {
      name: '',
      phone: '',
      email: '',
    };
  },
  emits: ['add-friend'],
  methods: {
    submitForm(event) {
      event.preventDefault();
      this.$emit('add-friend', {
        name: this.name,
        phone: this.phone,
        email: this.email,
      });
      this.name = '';
      this.phone = '';
      this.email = '';
    },
  },
};
</script>