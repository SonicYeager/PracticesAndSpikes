<template>
  <li>
    <h2>{{ friend.name }} {{ friendIsFavorite ? '(Favorite)' : '' }}</h2>
    <button @click="toggleDetails">{{ detailsAreVisible ? 'Hide' : 'Show' }} Details</button>
    <button @click="toggleFavorite">{{ friendIsFavorite ? 'Unmark' : 'Mark' }} Favorite</button>
    <ul v-if="detailsAreVisible">
      <li>
        <strong>Phone:</strong>
        {{ friend.phone }}
      </li>
      <li>
        <strong>Email:</strong>
        {{ friend.email }}
      </li>
    </ul>
  </li>
</template>

<script>
export default {
  //props: ['friend'],
  props: {
    friend: {
      type: Object,
      required: true,
      validator(value) {
        return (
            typeof value.id === 'string' &&
            typeof value.name === 'string' &&
            typeof value.phone === 'string' &&
            typeof value.email === 'string' &&
            typeof value.isFavorite === 'boolean'
        );
      },
    },
  },
  data() {
    return {
      detailsAreVisible: false,
      friendIsFavorite: this.friend.isFavorite,
    };
  },
  methods: {
    toggleDetails() {
      this.detailsAreVisible = !this.detailsAreVisible;
    },
    toggleFavorite() {
      this.friendIsFavorite = !this.friendIsFavorite;
    },
  }
};
</script>