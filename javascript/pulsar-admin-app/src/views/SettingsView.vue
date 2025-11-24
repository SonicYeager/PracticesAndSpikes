<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { usePreferencesStore } from '@/stores/preferences.js'
import { useClustersStore } from '@/stores/clusters.js'
import { useNotificationStore } from '@/stores/notification.js'

const preferencesStore = usePreferencesStore()
const clustersStore = useClustersStore()
const notificationStore = useNotificationStore()

// Load preferences on mount
onMounted(() => {
  preferencesStore.loadPreferences()
})

// Form state
const form = reactive({
  url: '',
  authEnabled: false,
  clientId: '',
  clientSecret: '',
  tokenEndpoint: '',
  scope: ''
})

// UI state
const showClientSecret = ref(false)
const isTesting = ref(false)
const testResult = ref(null)

// Initialize form from preferences
const loadFormFromPreferences = () => {
  const config = preferencesStore.clusterConfig
  form.url = config.url
  form.authEnabled = config.auth.enabled
  form.clientId = config.auth.clientId
  form.clientSecret = config.auth.clientSecret
  form.tokenEndpoint = config.auth.tokenEndpoint
  form.scope = config.auth.scope
}

// Load form on mount
onMounted(() => {
  loadFormFromPreferences()
})

// Validation
const isValidUrl = computed(() => {
  try {
    new URL(form.url)
    return true
  } catch {
    return false
  }
})

const canSave = computed(() => {
  if (!isValidUrl.value) return false
  if (form.authEnabled) {
    return form.clientId && form.clientSecret && form.tokenEndpoint
  }
  return true
})

// Actions
const saveConfiguration = () => {
  preferencesStore.savePreferences({
    clusterConfig: {
      url: form.url,
      auth: {
        enabled: form.authEnabled,
        type: 'oauth2-client-credentials',
        clientId: form.clientId,
        clientSecret: form.clientSecret,
        tokenEndpoint: form.tokenEndpoint,
        scope: form.scope
      }
    }
  })
  notificationStore.success('Configuration saved successfully')
}

const testConnection = async () => {
  isTesting.value = true
  testResult.value = null
  
  try {
    // Test basic connectivity by checking health
    await clustersStore.checkHealth()
    
    if (clustersStore.isHealthy) {
      testResult.value = {
        success: true,
        message: 'Connection successful! Cluster is healthy.'
      }
    } else {
      testResult.value = {
        success: false,
        message: 'Connection failed. Cluster is not healthy.'
      }
    }
  } catch (error) {
    testResult.value = {
      success: false,
      message: `Connection failed: ${error.message}`
    }
  } finally {
    isTesting.value = false
  }
}

const resetToDefaults = () => {
  if (!confirm('Are you sure you want to reset to default settings?')) return
  
  preferencesStore.resetPreferences()
  loadFormFromPreferences()
  testResult.value = null
  notificationStore.success('Settings reset to defaults')
}

const clearCredentials = () => {
  form.clientId = ''
  form.clientSecret = ''
  form.tokenEndpoint = ''
  form.scope = ''
}
</script>

<template>
  <main class="flex justify-center items-start min-h-screen p-4 pt-32">
    <div class="w-full max-w-4xl space-y-6">
      <!-- Header -->
      <div class="card bg-base-100 shadow-md">
        <div class="card-body">
          <h1 class="card-title text-3xl">Settings</h1>
          <p class="text-sm opacity-70">Configure cluster connection and authentication</p>
        </div>
      </div>

      <!-- Cluster Configuration -->
      <div class="card bg-base-100 shadow-md">
        <div class="card-body">
          <h2 class="card-title text-xl mb-4">Cluster Configuration</h2>
          
          <div class="form-control">
            <label class="label">
              <span class="label-text font-semibold">Cluster URL</span>
            </label>
            <input 
              type="text" 
              v-model="form.url"
              placeholder="http://localhost:8080"
              class="input input-bordered"
              :class="{ 'input-error': form.url && !isValidUrl }"
            />
            <label class="label">
              <span class="label-text-alt" :class="{ 'text-error': form.url && !isValidUrl }">
                {{ form.url && !isValidUrl ? 'Invalid URL format' : 'The base URL for the Pulsar Admin API' }}
              </span>
            </label>
          </div>

          <button 
            class="btn btn-outline btn-sm"
            :class="{ 'loading': isTesting }"
            :disabled="!isValidUrl || isTesting"
            @click="testConnection"
          >
            Test Connection
          </button>

          <div v-if="testResult" 
               class="alert mt-2"
               :class="testResult.success ? 'alert-success' : 'alert-error'">
            <span>{{ testResult.message }}</span>
          </div>
        </div>
      </div>

      <!-- Authentication -->
      <div class="card bg-base-100 shadow-md">
        <div class="card-body">
          <h2 class="card-title text-xl mb-4">Authentication</h2>
          
          <div class="form-control">
            <label class="label cursor-pointer">
              <span class="label-text font-semibold">Enable OAuth2 Authentication</span>
              <input 
                type="checkbox" 
                v-model="form.authEnabled"
                class="checkbox" 
              />
            </label>
          </div>

          <div v-if="form.authEnabled" class="space-y-4 mt-4">
            <div class="alert alert-warning">
              <svg xmlns="http://www.w3.org/2000/svg" class="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
              </svg>
              <span class="text-sm">
                <strong>Security Warning:</strong> Client credentials will be stored in browser localStorage. 
                Only use this for trusted environments or development. For production, use a backend proxy.
              </span>
            </div>

            <div class="form-control">
              <label class="label">
                <span class="label-text">Authentication Type</span>
              </label>
              <input 
                type="text" 
                value="OAuth2 Client Credentials"
                class="input input-bordered"
                disabled
              />
            </div>

            <div class="form-control">
              <label class="label">
                <span class="label-text">Client ID</span>
              </label>
              <input 
                type="text" 
                v-model="form.clientId"
                placeholder="your-client-id"
                class="input input-bordered"
              />
            </div>

            <div class="form-control">
              <label class="label">
                <span class="label-text">Client Secret</span>
              </label>
              <div class="input-group">
                <input 
                  :type="showClientSecret ? 'text' : 'password'"
                  v-model="form.clientSecret"
                  placeholder="your-client-secret"
                  class="input input-bordered flex-1"
                />
                <button 
                  class="btn btn-square"
                  @click="showClientSecret = !showClientSecret"
                >
                  <svg v-if="!showClientSecret" xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                  </svg>
                  <svg v-else xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.88 9.88l-3.29-3.29m7.532 7.532l3.29 3.29M3 3l3.59 3.59m0 0A9.953 9.953 0 0112 5c4.478 0 8.268 2.943 9.543 7a10.025 10.025 0 01-4.132 5.411m0 0L21 21" />
                  </svg>
                </button>
              </div>
            </div>

            <div class="form-control">
              <label class="label">
                <span class="label-text">Token Endpoint</span>
              </label>
              <input 
                type="text" 
                v-model="form.tokenEndpoint"
                placeholder="https://auth.example.com/oauth/token"
                class="input input-bordered"
              />
              <label class="label">
                <span class="label-text-alt">The OAuth2 token endpoint URL</span>
              </label>
            </div>

            <div class="form-control">
              <label class="label">
                <span class="label-text">Scope (Optional)</span>
              </label>
              <input 
                type="text" 
                v-model="form.scope"
                placeholder="pulsar:admin"
                class="input input-bordered"
              />
            </div>

            <button 
              class="btn btn-outline btn-error btn-sm"
              @click="clearCredentials"
            >
              Clear Credentials
            </button>
          </div>
        </div>
      </div>

      <!-- Actions -->
      <div class="card bg-base-100 shadow-md">
        <div class="card-body">
          <div class="flex gap-4 justify-end">
            <button 
              class="btn btn-outline"
              @click="resetToDefaults"
            >
              Reset to Defaults
            </button>
            <button 
              class="btn btn-primary"
              :disabled="!canSave"
              @click="saveConfiguration"
            >
              Save Configuration
            </button>
          </div>
        </div>
      </div>
    </div>
  </main>
</template>
