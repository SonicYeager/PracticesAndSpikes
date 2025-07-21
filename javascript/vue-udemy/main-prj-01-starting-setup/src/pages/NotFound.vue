<script setup></script>

<template>
  <section>
    <base-card>
      <h2>Page not found</h2>
      <p>Sorry, the page you are looking for does not exist.</p>
      <span> Get <router-link to="/coaches">Home</router-link> </span>
    </base-card>
  </section>
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
