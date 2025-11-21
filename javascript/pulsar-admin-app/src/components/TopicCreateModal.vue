<template>
  <dialog class="modal" :class="{ 'modal-open': modelValue }">
    <div class="modal-box">
      <h3 class="font-bold text-lg">Create New Topic</h3>
      <div class="py-4 form-control w-full">
        <label class="label"><span class="label-text">Tenant</span></label>
        <input type="text" class="input input-bordered w-full" :value="tenant" disabled />
        
        <label class="label"><span class="label-text">Namespace</span></label>
        <select class="select select-bordered w-full" v-model="form.namespace">
          <option disabled value="">Select Namespace</option>
          <option v-for="ns in namespaces" :key="ns" :value="ns">{{ ns }}</option>
        </select>

        <label class="label"><span class="label-text">Topic Name</span></label>
        <input type="text" class="input input-bordered w-full" v-model="form.topic" placeholder="my-topic" />

        <div class="divider">Retention (Optional)</div>
        <div class="flex gap-2">
          <div class="w-1/2">
            <label class="label"><span class="label-text">Time (min)</span></label>
            <input type="number" class="input input-bordered w-full" v-model="form.retentionTimeInMinutes" placeholder="-1" />
          </div>
          <div class="w-1/2">
            <label class="label"><span class="label-text">Size (MB)</span></label>
            <input type="number" class="input input-bordered w-full" v-model="form.retentionSizeInMB" placeholder="-1" />
          </div>
        </div>

        <div class="divider">Permissions (Optional)</div>
        <label class="label"><span class="label-text">Role</span></label>
        <input type="text" class="input input-bordered w-full" v-model="form.role" placeholder="role-name" />
        
        <label class="label"><span class="label-text">Actions</span></label>
        <div class="flex gap-2 flex-wrap">
          <label class="label cursor-pointer gap-2">
            <span class="label-text">produce</span>
            <input type="checkbox" class="checkbox" value="produce" v-model="form.actions" />
          </label>
          <label class="label cursor-pointer gap-2">
            <span class="label-text">consume</span>
            <input type="checkbox" class="checkbox" value="consume" v-model="form.actions" />
          </label>
        </div>
      </div>
      <div class="modal-action">
        <button class="btn" @click="$emit('update:modelValue', false)">Cancel</button>
        <button class="btn btn-primary" @click="create" :disabled="!isValid">Create</button>
      </div>
    </div>
  </dialog>
</template>

<script setup>
import { ref, computed, watch } from 'vue'

const props = defineProps({
  modelValue: Boolean,
  tenant: String,
  namespaces: Array,
  selectedNamespace: String
})

const emit = defineEmits(['update:modelValue', 'create'])

const form = ref({
  namespace: '',
  topic: '',
  retentionTimeInMinutes: '',
  retentionSizeInMB: '',
  role: '',
  actions: []
})

// Initialize namespace when modal opens or props change
watch(() => props.modelValue, (newVal) => {
  if (newVal) {
    form.value.namespace = props.selectedNamespace !== 'All' ? props.selectedNamespace : ''
  }
})

const isValid = computed(() => {
  return form.value.namespace && form.value.topic
})

const create = () => {
  emit('create', { ...form.value })
}
</script>
