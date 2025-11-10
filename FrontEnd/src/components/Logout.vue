<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { AuthService } from '@/service/AuthService';
import AxiosHttpService from '@/service/AxiosHttpService';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { IAuthService } from '@/service/IService/IAuthService';

const router = useRouter();

const authService = container.get<IAuthService>(TYPES.authService);

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