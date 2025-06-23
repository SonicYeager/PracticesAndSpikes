<script>
import BaseButton from '@/components/ui/BaseButton.vue';

export default {
  components: { BaseButton },
  data() {
    return {
      firstName: {
        val: '',
        isValid: true,
      },
      lastName: {
        val: '',
        isValid: true,
      },
      description: {
        val: '',
        isValid: true,
      },
      hourlyRate: {
        val: 0,
        isValid: true,
      },
      areas: {
        val: [],
        isValid: true,
      },
      formIsValid: true,
    };
  },
  methods: {
    clearValidity(property) {
      this[property].isValid = true;
    },
    validateForm() {
      this.firstName.isValid = this.firstName.val.trim() !== '';
      this.lastName.isValid = this.lastName.val.trim() !== '';
      this.description.isValid = this.description.val.trim().length >= 10;
      this.hourlyRate.isValid = this.hourlyRate.val > 0;
      this.areas.isValid = this.areas.val.length > 0;

      this.formIsValid =
        this.firstName.isValid &&
        this.lastName.isValid &&
        this.description.isValid &&
        this.hourlyRate.isValid &&
        this.areas.isValid;
    },
    submitForm() {

      this.validateForm();

      if (!this.formIsValid) {
        return;
      }

      const formData = {
        first: this.firstName.val,
        last: this.lastName.val,
        desc: this.description.val,
        rate: this.hourlyRate.val,
        areas: this.areas.val,
      };

      this.$store.dispatch('coaches/addCoach', formData);
      this.$router.replace('/coaches');
    },
  },
};
</script>

<template>
  <form @submit.prevent="submitForm">
    <div class="form-control" :class="{ invalid: !firstName.isValid }">
      <label for="firstname">Firstname</label>
      <input id="firstname" v-model.trim="firstName.val" type="text" @blur="clearValidity('firstName')"/>
      <p v-if="!firstName.isValid">Required</p>
    </div>
    <div class="form-control" :class="{ invalid: !lastName.isValid }">
      <label for="lastname">Lastname</label>
      <input id="lastname" v-model.trim="lastName.val" type="text" @blur="clearValidity('lastName')"/>
      <p v-if="!lastName.isValid">Required</p>
    </div>
    <div class="form-control" :class="{ invalid: !description.isValid }">
      <label for="description">Description</label>
      <textarea id="description" v-model.trim="description.val" rows="5" @blur="clearValidity('description')"/>
      <p v-if="!description.isValid">Required</p>
    </div>
    <div class="form-control" :class="{ invalid: !hourlyRate.isValid }">
      <label for="rate">Hourly Rate</label>
      <input id="rate" v-model.number="hourlyRate.val" type="number" @blur="clearValidity('hourlyRate')"/>
      <p v-if="!hourlyRate.isValid">Required</p>
    </div>
    <div class="form-control" :class="{ invalid: !areas.isValid }">
      <h3>Areas Of Expertise</h3>
      <div>
        <label for="frontend">Frontend Development</label>
        <input id="frontend" v-model="areas.val" type="checkbox" value="frontend" @blur="clearValidity('areas')"/>
      </div>
      <div>
        <label for="backend">Backend Development</label>
        <input id="backend" v-model="areas.val" type="checkbox" value="backend" @blur="clearValidity('areas')"/>
      </div>
      <div>
        <label for="career">Career</label>
        <input id="career" v-model="areas.val" type="checkbox" value="frontend" @blur="clearValidity('areas')"/>
      </div>
      <p v-if="!areas.isValid">Required</p>
    </div>
    <p v-if="!formIsValid">Invalid Input!</p>
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
