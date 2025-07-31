<script setup>
import {ref} from 'vue'
import {usePreferencesStore} from "@/stores/preferences.js";

const formData = ref({
  pulsarUrl: '',
})

const preferencesStore = usePreferencesStore()

const savePreferences = () => {
  preferencesStore.savePreferences(formData)
  formData.value.pulsarUrl = preferencesStore.preferences.pulsarUrl
}

preferencesStore.loadPreferences()
formData.value.pulsarUrl = preferencesStore.preferences.pulsarUrl

</script>

<template>
  <main class="flex justify-center items-start min-h-screen p-4 pt-32">
    <div class="card shadow-lg bg-base-100 w-full max-w-md">
      <div class="card-body">
        <h1 class="card-title text-center mb-6">Preferences</h1>

        <form @submit.prevent="savePreferences" class="space-y-4">
          <!-- Pulsar URL Field -->
          <div class="form-control">
            <label class="label">
              <span class="label-text font-medium">Pulsar URL</span>
            </label>
            <input
                type="url"
                v-model="formData.pulsarUrl"
                placeholder="https://pulsar.example.com"
                class="input input-bordered w-full"
                required
            />
          </div>

          <!-- Submit Button -->
          <div class="card-actions justify-end mt-6">
            <button type="submit" class="btn btn-primary">
              Einstellungen speichern
            </button>
          </div>
        </form>
      </div>
    </div>
  </main>

</template>