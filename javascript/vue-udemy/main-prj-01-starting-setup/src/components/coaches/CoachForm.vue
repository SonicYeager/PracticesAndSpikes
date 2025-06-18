<script>
import BaseButton from '@/components/ui/BaseButton.vue';

export default {
  components: { BaseButton },
  data() {
    return {
      firstName: '',
      lastName: '',
      description: '',
      hourlyRate: '',
      areas: [],
    };
  },
  methods: {
    submitForm() {
      const formData = {
        first: this.firstName,
        last: this.lastName,
        desc: this.description,
        rate: this.hourlyRate,
        areas: this.areas,
      };

      this.$store.dispatch('coaches/addCoach', formData);
      this.$router.replace('/coaches');
    },
  },
};
</script>

<template>
  <form @submit.prevent="submitForm">
    <div class="form-control">
      <label for="firstname">Firstname</label>
      <input id="firstname" v-model.trim="firstName" type="text" />
    </div>
    <div class="form-control">
      <label for="lastname">Lastname</label>
      <input id="lastname" v-model.trim="lastName" type="text" />
    </div>
    <div class="form-control">
      <label for="description">Description</label>
      <textarea id="description" v-model.trim="description" rows="5" />
    </div>
    <div class="form-control">
      <label for="rate">Hourly Rate</label>
      <input id="rate" v-model.number="hourlyRate" type="number" />
    </div>
    <div class="form-control">
      <h3>Areas Of Expertise</h3>
      <div>
        <label for="frontend">Frontend Development</label>
        <input id="frontend" v-model="areas" type="checkbox" value="frontend" />
      </div>
      <div>
        <label for="backend">Backend Development</label>
        <input id="backend" v-model="areas" type="checkbox" value="backend" />
      </div>
      <div>
        <label for="career">Career</label>
        <input id="career" v-model="areas" type="checkbox" value="frontend" />
      </div>
    </div>
    <base-button>Register</base-button>
  </form>
</template>

<style scoped>
.form-control {
  margin: 0.5rem 0;
}

label {
  font-weight: bold;
  display: block;
  margin-bottom: 0.5rem;
}

input[type='checkbox'] + label {
  font-weight: normal;
  display: inline;
  margin: 0 0 0 0.5rem;
}

input,
textarea {
  display: block;
  width: 100%;
  border: 1px solid #ccc;
  font: inherit;
}

input:focus,
textarea:focus {
  background-color: #f0e6fd;
  outline: none;
  border-color: #3d008d;
}

input[type='checkbox'] {
  display: inline;
  width: auto;
  border: none;
}

input[type='checkbox']:focus {
  outline: #3d008d solid 1px;
}

h3 {
  margin: 0.5rem 0;
  font-size: 1rem;
}

.invalid label {
  color: red;
}

.invalid input,
.invalid textarea {
  border: 1px solid red;
}
</style>
