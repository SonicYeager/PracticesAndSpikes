<script>
import TheHeader from '@/components/layouts/TheHeader.vue';
import TheResources from '@/components/learning-resource/TheResources.vue';

export default {
  components: {
    TheHeader,
    TheResources
  }
};
</script>

<template>
  <the-header title="RememberMe"></the-header>
  <the-resources></the-resources>
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