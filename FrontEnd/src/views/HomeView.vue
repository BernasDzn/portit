<script setup lang="ts">
import Topbar from '@/components/Topbar.vue';
import Sidebar from '@/components/Sidebar.vue';
import NotificationQueue from '@/components/NotificationQueue.vue';
import { onMounted, onBeforeUnmount } from 'vue';
import { useRouter } from 'vue-router';
import { useSession } from '@/composables/session';
import { useAlerts } from '@/composables/alerts';
import { useI18n } from 'vue-i18n';
import BroadcastHero from './Dashboards/BroadcastHero.vue';

const session = useSession();
const router = useRouter();
const notifications = useAlerts();
const { t } = useI18n();

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
        notifications.enqueueNotification(t('expirationWarning.messageOneMinute'), notifications.notificationTypes.WARNING, 60000);
      }, warningMs);
    } else if (warningMs <= 0) {
      notifications.enqueueNotification(t('expirationWarning.messageLessThanOneMinute'), notifications.notificationTypes.WARNING, 60000);
    }
  }
}

function handleExpiration() {
  session.clearSession();
  console.warn('Session expired, redirecting to unauthorized page.');
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
    <BroadcastHero />

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

</style>