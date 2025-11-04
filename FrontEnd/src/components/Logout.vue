<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { AuthService } from '@/service/AuthService';
import AxiosHttpService from '@/service/AxiosHttpService';

const router = useRouter();

const http = new AxiosHttpService();
const authService = new AuthService(http);

async function onLogout() {
  try {
    await authService.logout();
  } catch (err) {
  } finally {
    router.push('/login');
  }
}
</script>

<template>
  <sl-button class="logout-button" variant="danger" outline @click="onLogout">Logout</sl-button>
</template>