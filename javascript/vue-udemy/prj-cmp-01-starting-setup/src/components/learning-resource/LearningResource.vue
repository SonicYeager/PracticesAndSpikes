﻿<script>
import LearningResource from '../../objects/LearningResource';
import BaseCard from '@/components/UI/BaseCard.vue';

export default {
  components: { BaseCard },
  props: {
    learningResource: {
      type: LearningResource,
      required: true
    }
  },
  inject: ['deleteResource'],
};
</script>

<template>
  <li>
    <base-card>
      <header>
        <h3>{{ learningResource.title }}</h3>
        <base-button mode="flat" @click="deleteResource(learningResource.id)">Delete</base-button>
      </header>
    <p>{{ learningResource.description }}</p>
    <nav>
      <a :href="learningResource.link" target="_blank">Visit Resource</a>
    </nav>
    </base-card>
  </li>
</template>

<style scoped>
li {
  margin: auto;
  max-width: 40rem;
}

header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

h3 {
  font-size: 1.25rem;
  margin: 0.5rem 0;
}

p {
  margin: 0.5rem 0;
}

a {
  text-decoration: none;
  color: #ce5c00;
}

a:hover,
a:active {
  color: #c89300;
}
</style>