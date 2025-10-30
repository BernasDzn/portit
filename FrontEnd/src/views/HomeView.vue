<script setup lang="ts">
import Topbar from '@/components/Topbar.vue';
import Sidebar from '@/components/Sidebar.vue';
</script>

<template>
  <div class="app-layout">
    <Topbar />
    <div class="layout-content">
      <Sidebar />
      <main class="main-content">
        <!-- This is where our app views will go based on routing -->
        <RouterView />
      </main>
    </div>
  </div>
</template>

<style scoped>

.layout-content {
  display: flex;
  flex: 1;
  overflow: hidden;
}
</style>