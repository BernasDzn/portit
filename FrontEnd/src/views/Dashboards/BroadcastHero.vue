<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { ISystemNotificationService } from '@/service/IService/ISystemNotificationService';
import type { SystemNotification } from '@/model/SystemNotification';

const notificationService = container.get<ISystemNotificationService>(TYPES.systemNotificationService);
const broadcasts = ref<SystemNotification[]>([]);

const fetchBroadcasts = async () => {
    try {
        const all = await notificationService.getSystemNotifications();
        broadcasts.value = all.filter(n => n.urgency === 1 && !n.isRead);
    } catch (err) {
        console.error('Failed to fetch urgent broadcasts', err);
    }
};

const acknowledgeAll = async (id: string) => {
    try {
        for (const b of broadcasts.value) {
            await notificationService.markAsRead(b.id);
        }
        broadcasts.value = [];
    } catch (err) {
        console.error('Failed to acknowledge broadcast', err);
    }
};

onMounted(() => {
    document.addEventListener('sl-request-close', (event) => {
        if (event.detail.source === 'overlay') {
          event.preventDefault();
        }
    });

    fetchBroadcasts();
});
</script>

<template>
  <div>
    <sl-dialog
      v-if="broadcasts.length"
      label="Urgent Notification"
      open
      backdrop
      class="urgent-dialog"
    >

      <div>
        <div v-for="b in broadcasts" :key="b.id">
          <h3 class="text-group">
            <sl-icon name="exclamation-triangle" class="icon-urgent"></sl-icon>
            <span>
                {{ b.title }}
            </span>
        </h3>
          <p>{{ b.message }}</p>
        </div>
    </div>
    <sl-button style="regular" variant="primary" @click="acknowledgeAll">I understand</sl-button>
    </sl-dialog>
  </div>
</template>

<style scoped>

.icon-urgent {
    color: red;
    font-size: 1.5rem;
    margin-right: 0.8rem;
    margin-top: auto;
    margin-bottom: auto;
}

.text-group {
    display: flex;
}

</style>
