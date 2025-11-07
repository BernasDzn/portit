<script setup lang="ts">
import Topbar from '@/components/Topbar.vue';
import Sidebar from '@/components/Sidebar.vue';
import NotificationQueue from '@/components/NotificationQueue.vue';
import { onMounted, onBeforeUnmount } from 'vue';
import { useRouter } from 'vue-router';
import { useSession } from '@/composables/session';
import { useAlerts } from '@/composables/alerts';

const session = useSession();
const router = useRouter();
const notifications = useAlerts();

let expirationTimer: number | null = null;
let warningTimer: number;

function scheduleExpirationCheck() {
  if (expirationTimer) {
    clearTimeout(expirationTimer);
    expirationTimer = null;
  }

  if (session.expirationTime) {
    const msUntilExpiration = session.expirationTime - Date.now();
    if (msUntilExpiration <= 0) {
      handleExpiration();
      return;
    }

    expirationTimer = window.setTimeout(() => {
      handleExpiration();
    }, msUntilExpiration + 1000);

    const warningMs = msUntilExpiration - 60000;
    if (warningMs > 0) {
      warningTimer = window.setTimeout(() => {
        notifications.enqueueNotification('Your session will expire in 1 minute.', notifications.notificationTypes.WARNING, 60000);
      }, warningMs);
    } else if (warningMs <= 0) {
      notifications.enqueueNotification('Your session will expire soon.', notifications.notificationTypes.WARNING, 60000);
    }
  }
}

function handleExpiration() {
  session.clearSession();
  router.push('/unauthorized');
}

onMounted(() => scheduleExpirationCheck());

onBeforeUnmount(() => {
  if (expirationTimer) clearTimeout(expirationTimer);
  if (warningTimer) clearTimeout(warningTimer);
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