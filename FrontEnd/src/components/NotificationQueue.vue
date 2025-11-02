<script setup lang="ts">
import { useAlerts } from '@/composables/alerts';
import { onMounted } from 'vue';

const notification = useAlerts();

const getNotificationTitle = notification.getNotificationTitle;

onMounted(() => {
    notification.clearNotifications();
});

</script>

<template>
<div class="notification-pool">
    <sl-alert class="notifications" v-for="notification in notification.notificationList.value" :variant="notification.type" duration="3000" closable open>
        <sl-icon slot="icon" name="exclamation-octagon"></sl-icon>
        <strong>{{ getNotificationTitle(notification) }}</strong><br />
        {{ notification.message }}
    </sl-alert>
</div>
</template>

<style scoped>

.notification-pool {
    position: fixed;
    top: 1rem;
    right: 1rem;
    display: flex;
    flex-direction: column;
    align-items: flex-end;
    gap: 0.5rem;
    z-index: 1000;
    max-width: 300px;
    margin: 20px;
  }
  
  .notification-pool::part(base),
  sl-alert::part(base) {
    width: 400px;
    max-width: 90vw;
  }

</style>