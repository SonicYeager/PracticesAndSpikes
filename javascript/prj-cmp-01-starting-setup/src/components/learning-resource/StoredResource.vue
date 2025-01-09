<script>
import LearningResource from '@/objects/LearningResource';
import LearningResourceComponent from '@/components/learning-resource/LearningResource.vue';

export default {
  components: { LearningResource: LearningResourceComponent },
  inject: {
    resources: {
      type: Array,
      required: true,
      validator(value) {
        return value.every(item => item instanceof LearningResource);
      }
    }
  },
}
</script>

<template>
  <ul>
    <learning-resource v-for="item in resources" :key="item.id" :learning-resource="item"></learning-resource>
  </ul>
</template>

<style scoped>
ul {
  list-style: none;
  margin: 0;
  padding: 0;
  margin: auto;
  max-width: 40rem;
}
</style>