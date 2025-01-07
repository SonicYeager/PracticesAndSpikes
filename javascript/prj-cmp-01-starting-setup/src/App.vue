<script>
import LearningResource from '@/objects/LearningResource';
import StoredResource from '@/components/learning-resource/StoredResource.vue';
import TheHeader from '@/components/layouts/TheHeader.vue';

export default {
  components: {
    StoredResource,
    TheHeader
  },
  data() {
    return {
      storedResources: [
        new LearningResource(1, 'official guide', 'vvvdfsdf', 'https://vuejs.org/v2/guide/'),
        new LearningResource(2, 'google', 'Google', 'https://google.org')
      ]
    };
  }
};
</script>

<template>
  <the-header title="RememberMe"></the-header>
  <stored-resource :stored-resources="storedResources"></stored-resource>
</template>

<style>
@import url('https://fonts.googleapis.com/css2?family=Roboto:wght@400;700&display=swap');

* {
  box-sizing: border-box;
}

html {
  font-family: 'Roboto', sans-serif;
}

body {
  margin: 0;
}
</style>