<script>
import CoachForm from '@/components/coaches/CoachForm.vue';
import BaseCard from '@/components/ui/BaseCard.vue';

export default {
  components: { BaseCard, CoachForm },
};
</script>

<template>
  <section>
    <base-card>
      <h2>Register As Coach</h2>
      <coach-form></coach-form>
    </base-card>
  </section>
</template>

<style scoped></style>
