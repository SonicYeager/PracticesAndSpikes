<template>
  <section>
    <base-card>
      <h2>Submitted Experiences</h2>
      <div>
        <base-button @click="loadExperiences">Load Submitted Experiences</base-button>
      </div>
      <p v-if="isLoading">Fetching...</p>
      <p v-else-if="!isLoading && error">{{error}}</p>
      <p v-else-if="!isLoading && results.length === 0">It's Empty</p>
      <ul v-else-if="!isLoading && results.length > 0">
        <survey-result
          v-for="result in results"
          :key="result.id"
          :name="result.name"
          :rating="result.rating"
        ></survey-result>
      </ul>
    </base-card>
  </section>
</template>

<script>
import SurveyResult from './SurveyResult.vue';

export default {
  components: {
    SurveyResult
  },
  data() {
    return {
      results: [],
      isLoading: false,
      error: null
    };
  },
  methods: {
    loadExperiences() {
      this.isLoading = true;
      this.error = null;
      fetch('https://vue-http-demo-sy-default-rtdb.europe-west1.firebasedatabase.app/surveys.json')
        .then((response) => {
          if (response.ok) {
            return response.json();
          }
        })
        .then((json) => {
          const results = [];
          for (let key in json) {
            results.push({
              id: key,
              name: json[key].userName,
              rating: json[key].rating
            });
          }
          this.results = results;
          this.isLoading = false;
        })
        .catch((error) => {
          this.isLoading = false;
          this.error = error;
        });
    }
  },
  mounted() {
    this.loadExperiences();
  }
};
</script>

<style scoped>
ul {
  list-style: none;
  margin: 0;
  padding: 0;
}
</style>