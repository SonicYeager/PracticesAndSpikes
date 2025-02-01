<template>
<ul>
  <li v-for="goal in courseGoals" :key="goal.id">
    <slot :goal="goal"></slot>
  </li>
</ul>
</template>

<script>
export default {
  data() {
    return {
      courseGoals: [
        { id: 'cg1', text: 'Finish the course and learn a lot!' },
        { id: 'cg2', text: 'Help other students in the course Q&A' },
        { id: 'cg3', text: 'Finish the course as one of the top students' },
      ]
    };
  }
}
</script>

<style scoped>

</style>