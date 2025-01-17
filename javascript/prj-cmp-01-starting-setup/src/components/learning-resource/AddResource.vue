<script>
import BaseDialog from '@/components/UI/BaseDialog.vue';

export default {
  components: { BaseDialog },
  inject: ['addResource'],
  data() {
    return {
      inputIsInvalid: false
    };
  },
  methods: {
    submitData() {
      const enteredTitle = this.$refs.titleInput.value;
      const enteredDesc = this.$refs.descInput.value;
      const enteredLink = this.$refs.linkInput.value;

      if (enteredTitle.trim() === '' || enteredDesc.trim() === '' || enteredLink.trim() === '') {
        this.inputIsInvalid = true;
        return;
      }

      this.addResource(enteredTitle, enteredDesc, enteredLink);
    },
    confirmAck() {
      this.inputIsInvalid = false;
    }
  }
};
</script>

<template>
  <base-dialog v-if="inputIsInvalid" title="MissInput" @close="confirmAck">
    <template #default>
      <p>At least one input is empty!</p>
      <p>Make sure all fields are properly filled.</p>
    </template>
    <template #actions>
      <base-button @click="confirmAck">Acknowledge</base-button>
    </template>
  </base-dialog>
  <base-card>
    <form @submit.prevent="submitData">
      <div class="form-control">
        <label for="title">Title</label>
        <input type="text" id="title" name="title" ref="titleInput" required>
      </div>
      <div class="form-control">
        <label for="description">Description</label>
        <textarea id="description" name="description" rows="3" ref="descInput" required></textarea>
      </div>
      <div class="form-control">
        <label for="link">URL</label>
        <input type="url" id="link" name="link" ref="linkInput" required>
      </div>
      <div class="form-control">
        <base-button type="submit">Add Resource</base-button>
      </div>
    </form>
  </base-card>
</template>

<style scoped>
label {
  font-weight: bold;
  display: block;
  margin-bottom: 0.5rem;
}

input,
textarea {
  display: block;
  width: 100%;
  font: inherit;
  padding: 0.15rem;
  border: 1px solid #ccc;
}

input:focus,
textarea:focus {
  outline: none;
  border-color: #3a0061;
  background-color: #f7ebff;
}

.form-control {
  margin: 1rem 0;
}
</style>