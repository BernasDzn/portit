<script setup lang="ts">
import type { VesselVisitNotification } from '@/model/VesselVisitNotification';
import AxiosHttpService from '@/service/AxiosHttpService';
import { VesselVisitNotificationService } from '@/service/VesselVisitNotificationService';
import { computed, onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';
import VesselVisitNotificationPrinter from './printers/VesselVisitNotificationPrinter.vue';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import { useSession } from '@/composables/session';

import { Bar } from 'vue-chartjs'
import { Chart as ChartJS, Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale } from 'chart.js'
import type { IVesselVisitNotificationService } from '@/service/IService/IVesselVisitNotificationService';
import type { VesselVisitDistributionDto } from '@/model/dto/VesselVisitNotificationDto';
import { Pie } from 'vue-chartjs';
import { ArcElement } from 'chart.js';

ChartJS.register(ArcElement);
ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale);

const vvnService = container.get<IVesselVisitNotificationService>(TYPES.vesselVisitNotificationService);

const vvnCount = ref(0);
const pendingVVNCount = ref(0);
const recentNotifs = ref<VesselVisitNotification[]>([]);

const {t} = useI18n();
const role = ref(-1);

const distribution = ref<VesselVisitDistributionDto>({
    pending: 0,
    accepted: 0,
    rejected: 0
});

const fetchNotifications = async () => {

    role.value = useSession().authenticatedUser.role;

    try {
        const notifications = role.value == 2 ? (await vvnService.getVesselVisitNotificationsByRepresentative()) : (await vvnService.getVesselVisitNotificationsForReview(null));

        vvnCount.value = notifications.items.length;
        recentNotifs.value = notifications.items.slice(0, 3);
        pendingVVNCount.value = notifications.items.filter(item => item.status === 1).length;

        if (role.value != 2) {
            distribution.value = await vvnService.count();
        }

    } catch (error) {
        console.error('Error fetching notifications:', error);
    }
};

onMounted(() => {
    fetchNotifications();
});

const chartData = computed(() => ({
  labels: ['Total', 'Pending'],
  datasets: [
    {
      label: 'Notifications',
      backgroundColor: ['#42A5F5', '#FFA726'],
      data: [vvnCount.value, pendingVVNCount.value]
    }
  ]
}));

const chartOptions = {
  responsive: false,
  plugins: {
    legend: { position: 'top' },
    title: { display: true, text: 'Vessel Visit Notifications' }
  }
};

const pieChartData = computed(() => ({
  labels: ['Pending', 'Accepted', 'Rejected'],
  datasets: [
    {
      label: 'Notifications Distribution',
      backgroundColor: ['#FFA726', '#66BB6A', '#EF5350'],
      data: [
        distribution.value.pending,
        distribution.value.accepted,
        distribution.value.rejected
      ]
    }
  ]
}));

const pieChartOptions = {
  responsive: false,
  plugins: {
    legend: { position: 'top' },
    title: { display: true, text: 'Vessel Visit Notification Distribution' }
  }
};

</script>

<template>

    <sl-card style="width: 100%;" v-if="role == 2">
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
                <p>{{ pendingVVNCount }}</p>
                <span class="material-icons icon" style="color: var(--accent-1);">sailing</span>
            </div>
            <p>{{ t('dashboard.SAOR.pendingNotifications') }}</p>
            </div>
        </div>
    </sl-card>

    <br> <br v-if="role == 2">
    <div class="card-row full-height">
        <sl-card class="notifications-card">
            <div slot="header" class="header">
                {{ role == 2 ? t('dashboard.SAOR.recent') : t('dashboard.SAOR.recentReview') }}
                <span style="float: right; cursor: pointer;" class="material-icons">notifications</span>
            </div>
            
            <div v-if="recentNotifs.length === 0" style="text-align: center; padding: 1rem;">
                {{ t('dashboard.SAOR.noRecent') }}
            </div>
            <div v-else class="notification-list">
                <VesselVisitNotificationPrinter 
                    style="width:fit-content;"
                    class="listing-box"
                    v-for="notification in recentNotifs" 
                    :short="true"
                    :notification="notification"
                    :link="role == 2 ? `/vessel-visit-notifications/view/${notification.notificationId}` : `/vessel-visit-notifications/review/${notification.notificationId}`"
                />
            </div>
        </sl-card>
        <sl-card class="chart-card">
            <Bar
                v-if="role == 2"
                id="my-chart-id"
                :options="chartOptions"
                :data="chartData"
            />
            <Pie v-else
                id="distribution-pie-chart"
                :data="pieChartData"
                :options="pieChartOptions"
            />
            
        </sl-card>
    </div>
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

.card-row.full-height {
    display: flex;
    gap: 20px;
}

</style>