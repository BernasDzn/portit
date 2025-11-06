<script setup lang="ts">
import Topbar from '@/components/Topbar.vue';
import Sidebar from '@/components/Sidebar.vue';
import NotificationQueue from '@/components/NotificationQueue.vue';
import { onMounted, onBeforeUnmount } from 'vue';
import { useRouter } from 'vue-router';
import { useSession } from '@/composables/session';

const session = useSession();
const router = useRouter();

let expiryTimer: number | null = null;

function scheduleExpiryCheck() {
  if (expiryTimer) {
    clearTimeout(expiryTimer);
    expiryTimer = null;
  }

  if (session.expirationTime) {
    const msUntilExpiry = session.expirationTime - Date.now();
    if (msUntilExpiry <= 0) {
      handleExpiry();
      return;
    }

    expiryTimer = window.setTimeout(() => {
      handleExpiry();
    }, msUntilExpiry + 1000);
  }
}

function handleExpiry() {
  session.clearSession();
  router.push('/unauthorized');
}

onMounted(() => scheduleExpiryCheck());

onBeforeUnmount(() => {
  if (expiryTimer) clearTimeout(expiryTimer);
});
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