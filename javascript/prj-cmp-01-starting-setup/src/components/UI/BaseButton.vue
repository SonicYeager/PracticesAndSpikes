<script>
export default {
  props: {
    type: {
      type: String,
      default: 'button',
      validator(value) {
        return ['button', 'submit', 'reset'].includes(value);
      }
    },
    mode: {
      type: String,
      default: 'raised',
      validator(value) {
        return ['raised', 'flat'].includes(value);
      }
    }
  }
};
</script>

<template>
  <button :type="type" :class="mode">
    <slot></slot>
  </button>
</template>

<style scoped>
button {
  padding: 0.75rem 1.5rem;
  font-family: inherit;
  background-color: #3a0061;
  border: 1px solid #3a0061;
  color: white;
  cursor: pointer;
}

button:hover,
button:active {
  background-color: #270041;
  border-color: #270041;
}

.flat {
  background-color: transparent;
  color: #3a0061;
  border: none;
}

.flat:hover,
.flat:active {
  background-color: #edd2ff;
}
</style>