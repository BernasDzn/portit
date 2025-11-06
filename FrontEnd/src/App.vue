<script setup lang="ts">
import { RouterLink } from 'vue-router'
import Login from './views/Login.vue';
import HomeView from './views/HomeView.vue';
import { AuthService } from './service/AuthService';
import AxiosHttpService from './service/AxiosHttpService';
import router from './router';
import { useSession } from './composables/session';

</script>

<template>
  <header>
    <div>
      <RouterView />
    </div>
  </header>
</template>