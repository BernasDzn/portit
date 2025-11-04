<template>
  <div class="activate-page">
    <h2>Account activation</h2>
    <div id="google-signin-btn"></div>
    <div v-if="loading">Activating your account, please wait...</div>
    <div v-else-if="success">Your account has been activated. You can <a href="/login">log in</a> now.</div>
    <div v-else-if="error">Activation failed: {{ errorMessage }}</div>
    <div v-else>Invalid activation link.</div>
  </div>
</template>

<script setup lang="ts">
import { useSession } from '@/composables/session'
import { AuthService } from '@/service/AuthService'
import AxiosHttpService from '@/service/AxiosHttpService'
import type { AppJWTResponse } from '@/service/IService/IAuthService';
import { ref, onMounted } from 'vue'


const loading = ref(true)
const success = ref(false)
const error = ref(false)
const errorMessage = ref('')
const httpService = new AxiosHttpService();
const authService = new AuthService(httpService);

const session = useSession();

const loginFinished = async (googleResponse: any) => {
  const params = new URLSearchParams(window.location.search)
  const token = params.get('token')

  if (!token) {
    loading.value = false
    error.value = true
    errorMessage.value = 'Missing token.'
    return
  }

  try {
    // googleResponse contains the Google credential (id_token) in .credential
    const idToken = googleResponse?.credential;
    if (!idToken) throw new Error('Missing Google ID token');

    // Helper: parse a JWT (id_token) payload to extract claims such as email.
    // We avoid adding a heavy dependency; this is a small, defensive decoder.
    function parseJwt(token: string) {
      try {
        const parts = token.split('.')
        if (parts.length < 2) return null
  const payload = parts[1] ?? ''
        // base64url -> base64
        const base64 = payload.replace(/-/g, '+').replace(/_/g, '/')
        // atob on base64 string -> decode percent-encoded UTF-8
        const json = decodeURIComponent(
          atob(base64)
            .split('')
            .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
            .join('')
        )
        return JSON.parse(json)
      } catch (err) {
        console.error('Failed to parse JWT', err)
        return null
      }
    }

    const payload = parseJwt(idToken)
    const email = payload?.email as string | undefined
    if (!email) throw new Error('Unable to determine email from Google ID token')

    // Call backend to validate id_token and activate the account (bind sub)
    await authService.activateUser(email, token, idToken);

    // Activation succeeded: exchange the Google id_token for our app JWT so we can
    // populate the client session (login flow). After activation the /Login/google
    // endpoint should accept the id_token and return our app JWT.
    const appRes = await authService.getAppJWTToken(idToken);

    // Set the session like the login flow so we have authenticatedUser available
    const sessionUser: import('@/model/User').User = {
      id: appRes.user.id,
      name: appRes.user.name,
      email: appRes.user.email,
      avatar: appRes.user.picture
    }

    // store token and expiry in session (mirrors LoginBox behaviour)
    session.setSession(sessionUser, appRes.token, appRes.expiresIn);

    success.value = true
  } catch (e: any) {
    error.value = true
    // try to read error message
    errorMessage.value = e?.response?.data || e?.message || 'Unknown error'
  } finally {
    loading.value = false
  }
}

const errorCallback = (err: any) => {
    console.error('Google Sign-In error:', err);
    loading.value = false
    error.value = true
    errorMessage.value = 'Error during Google Sign-In. Please try again.'
}

onMounted(async () => {
  const existingScript = document.getElementById('google-client-script')
  if (!existingScript) {
      const script = document.createElement('script')
      script.src = 'https://accounts.google.com/gsi/client'
      script.async = true
      script.defer = true
      script.id = 'google-client-script'
      script.onload = () => authService.initGoogleLoginAccountActivation(loginFinished, errorCallback)
      document.head.appendChild(script)
  } else {
      authService.initGoogleLoginAccountActivation(loginFinished, errorCallback)
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
