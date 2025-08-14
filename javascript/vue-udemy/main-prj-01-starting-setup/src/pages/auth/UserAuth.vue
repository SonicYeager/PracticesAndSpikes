<script setup>
import BaseButton from '@/components/ui/BaseButton.vue';
import BaseCard from '@/components/ui/BaseCard.vue';
import { computed, ref } from 'vue';
import BaseDialog from '@/components/ui/BaseDialog.vue';
import BaseSpinner from '@/components/ui/BaseSpinner.vue';
import { useStore } from 'vuex';
import { useRoute, useRouter } from 'vue-router';

const email = ref('');
const password = ref('');
const formIsValid = ref(true);
const mode = ref('login');
const isLoading = ref(false);
const error = ref(null);

const store = useStore();
const router = useRouter();
const route = useRoute();

const submitForm = async () => {
  formIsValid.value = true;
  if (
    email.value === '' ||
    !email.value.includes('@') ||
    password.value.length < 6
  ) {
    formIsValid.value = false;
  }

  isLoading.value = true;

  try {
    if (mode.value === 'login') {
      await store.dispatch('login', {
        email: email.value,
        password: password.value,
      });
    } else {
      await store.dispatch('signup', {
        email: email.value,
        password: password.value,
      });
    }

    const redirectUrl = route.query.redirect;
    if (redirectUrl) {
      await router.replace('/' + redirectUrl);
    }

    await router.replace('/coaches');

    isLoading.value = false;
  } catch (err) {
    error.value = err.message || 'Failed to Register.';
    isLoading.value = false;
    console.log(err);
  }
};
const switchAuthMode = () => {
  if (mode.value === 'login') {
    mode.value = 'signup';
  } else {
    mode.value = 'login';
  }
};

const submitButtonCaption = computed(() => {
  return mode.value === 'login' ? 'Login' : 'Sign up';
});
const switchModeButtonCaption = computed(() => {
  return mode.value === 'login' ? 'Sign up' : 'Login';
});
</script>

<template>
  <div>
    <base-dialog :show="!!error" title="Error" @close="error = null" />
    <base-dialog :show="isLoading" fixed title="Loading...">
      <base-spinner></base-spinner>
    </base-dialog>
    <base-card>
      <form @submit.prevent="submitForm">
        <div class="form-control">
          <label for="email">Your E-Mail</label>
          <input id="email" v-model.trim="email" type="email" />
        </div>
        <div class="form-control">
          <label for="password">Your Password</label>
          <input id="password" v-model.trim="password" type="password" />
        </div>
        <p v-if="!formIsValid">Fix your input, kek</p>
        <base-button>{{ submitButtonCaption }}</base-button>
        <base-button mode="flat" type="button" @click="switchAuthMode"
          >{{ switchModeButtonCaption }}
        </base-button>
      </form>
    </base-card>
  </div>
</template>

<style scoped>
form {
  margin: 1rem;
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
</style>
