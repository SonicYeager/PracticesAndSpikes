<script setup>
import BaseButton from '@/components/ui/BaseButton.vue';
import { ref } from 'vue';
import { useStore } from 'vuex';
import { useRoute, useRouter } from 'vue-router';

const store = useStore();
const router = useRouter();
const route = useRoute();

const email = ref('');
const message = ref('');
const formIsValid = ref(true);

function submitForm() {
  formIsValid.value = true;

  if (
    email.value === '' ||
    !email.value.includes('@') ||
    message.value.length < 5
  ) {
    formIsValid.value = false;
  }

  store.dispatch('requests/contactCoach', {
    coachId: route.params.id,
    email: email.value,
    message: message.value,
  });

  router.replace('/coaches');
}
</script>

<template>
  <form @submit.prevent="submitForm">
    <div class="form-control">
      <label for="email">Your E-Mail</label>
      <input id="email" v-model.trim="email" type="email" />
    </div>
    <div class="form-control">
      <label for="message">Your Message</label>
      <textarea id="message" v-model.trim="message" rows="5"></textarea>
    </div>
    <div class="actions">
      <p v-if="!formIsValid" class="errors">
        Please enter a valid email and a message with at least 5 characters.
      </p>
      <base-button type="submit" @click="submitForm">Send Message</base-button>
    </div>
  </form>
</template>

<style scoped>
form {
  margin: 1rem;
  border: 1px solid #ccc;
  border-radius: 12px;
  padding: 1rem;
}

.form-control {
  margin: 0.5rem 0;
}

label {
  font-weight: bold;
  margin-bottom: 0.5rem;
  display: block;
}

input,
textarea {
  display: block;
  width: 100%;
  font: inherit;
  border: 1px solid #ccc;
  padding: 0.15rem;
}

input:focus,
textarea:focus {
  border-color: #3d008d;
  background-color: #faf6ff;
  outline: none;
}

.errors {
  font-weight: bold;
  color: red;
}

.actions {
  text-align: center;
}
</style>
