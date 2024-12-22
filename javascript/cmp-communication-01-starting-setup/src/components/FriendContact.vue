<template>
  <li>
    <h2>{{ friend.name }} {{ this.friend.isFavorite ? '(Favorite)' : '' }}</h2>
    <button @click="toggleDetails">{{ detailsAreVisible ? 'Hide' : 'Show' }} Details</button>
    <button @click="toggleFavorite">{{ this.friend.isFavorite ? 'Unmark' : 'Mark' }} Favorite</button>
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
    <button @click="deleteMe">Delete</button>
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
    };
  },
  emits: {
    'toggle-favorite': function(friendId) {
      return typeof friendId === 'string';
    },
    'delete-friend': function(friendId) {
      return typeof friendId === 'string';
    },
  },
  methods: {
    toggleDetails() {
      this.detailsAreVisible = !this.detailsAreVisible;
    },
    toggleFavorite() {
      this.$emit('toggle-favorite', this.friend.id);
    },
    deleteMe() {
      this.$emit('delete-friend', this.friend.id);
    },
  }
};
</script>