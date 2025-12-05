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
    <sl-dialog v-if="broadcasts.length" label="Urgent Notification" open backdrop class="urgent-dialog">
      <div class="broadcast-container">
        <div v-for="b in broadcasts" :key="b.id" class="broadcast-card">
          <div class="broadcast-header">
            <sl-icon name="exclamation-triangle-fill" class="icon-urgent"></sl-icon>
            <h3 class="broadcast-title">{{ b.title }}</h3>
          </div>
          <p class="broadcast-message">{{ b.message }}</p>
        </div>
      </div>
      <sl-button slot="footer" variant="primary" @click="acknowledgeAll">
        <sl-icon slot="prefix" name="check-circle"></sl-icon>
        I understand
      </sl-button>
    </sl-dialog>
  </div>
</template>

<style scoped>
.urgent-dialog::part(panel) {
  max-width: 600px;
}

.broadcast-container {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
  max-height: 60vh;
  overflow-y: auto;
}

.broadcast-card {
  padding: 1.25rem;
  background: linear-gradient(135deg, #fff5f5 0%, #ffe5e5 100%);
  border-left: 4px solid #dc2626;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(220, 38, 38, 0.1);
}

.broadcast-header {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 0.75rem;
}

.icon-urgent {
  color: #dc2626;
  font-size: 1.75rem;
  flex-shrink: 0;
}

.broadcast-title {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 600;
  color: #991b1b;
}

.broadcast-message {
  margin: 0;
  line-height: 1.6;
  color: #7f1d1d;
  font-size: 0.95rem;
}
</style>
