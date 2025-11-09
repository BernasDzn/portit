<script setup lang="ts">
import type { VesselVisitNotification } from '@/model/VesselVisitNotification';
import AxiosHttpService from '@/service/AxiosHttpService';
import { VesselVisitNotificationService } from '@/service/VesselVisitNotificationService';
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';
import VesselVisitNotificationPrinter from './printers/VesselVisitNotificationPrinter.vue';

const http = new AxiosHttpService();
const vvnService = new VesselVisitNotificationService(http);

const vvnCount = ref(0);
const recentNotifs = ref<VesselVisitNotification[]>([]);

const {t} = useI18n();

const fetchNotifications = async () => {
    try {
        const notifications = await vvnService.getVesselVisitNotifications();
        vvnCount.value = notifications.items.length;
        recentNotifs.value = notifications.items.slice(0, 3);

        console.log('Recent Vessel Visit Notifications:', notifications);
    } catch (error) {
        console.error('Error fetching notifications:', error);
    }
};

onMounted(() => {
    fetchNotifications();
});

</script>

<template>
    <sl-card style="width: 100%;">
        <div class="main-dashboard-statistics">
            <div class="stats-overview">
            <div class="opposed">
                <p>{{vvnCount}}</p>
                <span class="material-icons icon" style="color: var(--accent-1);">directions_boat</span>
            </div>
            <p>{{ t('notification.title') }}</p>
            </div>
            <div class="stats-overview">
            <div class="opposed">
                <p>2</p>
                <span class="material-icons icon" style="color: var(--accent-1);">sailing</span>
            </div>
            <p>{{ t('dashboard.SAOR.pendingNotifications') }}</p>
            </div>
        </div>
    </sl-card>

    <br><br>

    <sl-card class="card-header" >
        <div slot="header" class="header">
          Recent Vessel visit notifications
          <span style="float: right; cursor: pointer;" class="material-icons">notifications</span>
        </div>
      
        <div v-if="recentNotifs.length === 0" style="text-align: center; padding: 1rem;">
            No recent notifications.
        </div>
        <div v-else class="notification-list">
            <VesselVisitNotificationPrinter 
                style="width:fit-content;"
                class="listing-box"
                v-for="notification in recentNotifs" 
                :short="true"
                :notification="notification"
                :link="`/vessel-visit-notifications/view/${notification.notificationId}`"
            />
        </div>
        
      </sl-card>
</template>

<style scoped>

.header {
    padding: 5px;
    font-size: var(--sl-font-size-medium);
}

.header .material-icons {
    font-size: var(--sl-font-size-x-large);
    color: var(--sl-color-primary-600);
}

.notification-list {
    display: flex;
    flex-direction: row;
    gap: 1rem;
    margin-top: 1rem;
}

</style>