<template>
  <div class="activate-page">
    <h2>Account activation</h2>
    <div v-if="loading">Activating your account, please wait...</div>
    <div v-else-if="success">Your account has been activated. You can <a href="/login">log in</a> now.</div>
    <div v-else-if="error">Activation failed: {{ errorMessage }}</div>
    <div v-else>Invalid activation link.</div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { api } from '@/service/api'

const loading = ref(true)
const success = ref(false)
const error = ref(false)
const errorMessage = ref('')

onMounted(async () => {
  const params = new URLSearchParams(window.location.search)
  const token = params.get('token')
  const sub = params.get('sub')

  if (!sub) {
    loading.value = false
    error.value = true
    errorMessage.value = 'Missing user identifier.'
    return
  }

  try {
    // The API expects a PUT to /SystemUser/{sub}/activate
    // The token is not used by the current API, but we include it in the request body for future-proofing
    await api.put(`/SystemUser/${encodeURIComponent(sub)}/activate`, { token })
    success.value = true
  } catch (e: any) {
    error.value = true
    // try to read error message
    errorMessage.value = e?.response?.data || e?.message || 'Unknown error'
  } finally {
    loading.value = false
  }
})
</script>

<style scoped>
.activate-page {
  max-width: 600px;
  margin: 40px auto;
  padding: 24px;
  border-radius: 6px;
  background: var(--card-bg, #fff);
}
</style>
