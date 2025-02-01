<script setup>

</script>

<template>
  <h2>Manage Goals</h2>
  <input type="text" placeholder="Course Goal" ref="goal"/>
  <button @click="setGoal">Set Goal</button>
  <teleport to="body">
    <error-alert v-if="inputIsInvalid">
      <h2>Please enter a goal!</h2>
      <p>A goal should be something you want to achieve in this course.</p>
      <button @click="affirm">OK</button>
    </error-alert>
  </teleport>
</template>

<script>
import ErrorAlert from "@/components/ErrorAlert.vue";

export default {
  components: {
    ErrorAlert
  },
  data() {
    return {
      inputIsInvalid: false
    }
  },
  methods: {
    setGoal() {
      const enteredValue = this.$refs.goal.value;
      if (enteredValue === '') {
        this.inputIsInvalid = true;
      }
    },
    affirm() {
      this.inputIsInvalid = false;
    }
  },
}
</script>