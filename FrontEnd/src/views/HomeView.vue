<script setup lang="ts">
import Topbar from '@/components/Topbar.vue';
import Sidebar from '@/components/Sidebar.vue';
import NotificationQueue from '@/components/NotificationQueue.vue';

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
    <NotificationQueue />
  </div>
</template>

<style scoped>

.layout-content {
  display: flex;
  flex: 1;
}

.layout-content :deep(.sidebar) {
    width: var(--sidebar-width, 250px);
    flex-shrink: 0;
    background-color: var(--primary, #333);
    color: #fff;
    height: 100%;
    overflow-y: auto;
  }

</style>