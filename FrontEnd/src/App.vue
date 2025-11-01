<script setup lang="ts">
import { RouterLink } from 'vue-router'
import Login from './views/Login.vue';
import HomeView from './views/HomeView.vue';
</script>

<template>
  <header>
    <div>
      <RouterView />
    </div>
  </header>
</template>