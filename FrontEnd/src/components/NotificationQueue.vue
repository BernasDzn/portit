<script setup lang="ts">
import { useAlerts } from '@/composables/alerts';
import { onMounted, watch, nextTick, ref } from 'vue';

const notification = useAlerts();

const getNotificationTitle = notification.getNotificationTitle;

// Map of alert element refs keyed by notification id (or index fallback)
const alertEls = ref(new Map())
const shownAlerts = new Set()

function setAlertRef(el: HTMLElement | null, id: string | number) {
  if (el) {
    alertEls.value.set(id, el as any)
  } else {
    alertEls.value.delete(id)
  }
}

function getNotificationKey(n: any, idx: number) {
  return (n && (n as any).id) ?? idx
}

function getNotificationDuration(n: any) {
  return (n && (n as any).duration) ?? 3000
}

function refSetter(id: string | number) {
  return (el: HTMLElement | null) => setAlertRef(el, id)
}

watch(() => notification.notificationList.value.slice(), async (list) => {
  await nextTick()
  list.forEach((n: any, idx: number) => {
    const id = n.id ?? idx
    if (!shownAlerts.has(id)) {
      const el = alertEls.value.get(id)
      if (el && typeof (el as any).show === 'function') {
        try { (el as any).show() } catch (e) { /* ignore */ }
      }
      shownAlerts.add(id)
    }
  })
})

onMounted(() => {
  notification.clearNotifications();
});

</script>

<template>
<div class="notification-pool">
  <sl-alert
    v-for="(notification, idx) in notification.notificationList.value"
    :key="getNotificationKey(notification, idx)"
    :ref="refSetter(getNotificationKey(notification, idx))"
      class="notifications"
      :variant="notification.type"
    :duration="getNotificationDuration(notification)"
      :countdown="true"
      closable
  >
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