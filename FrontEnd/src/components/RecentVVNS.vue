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
import PlansByDay from './PlansByDay.vue';

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
    <div class="dashboard-container">
        <!-- Statistics Overview (only for role 2) -->
        <sl-card class="stats-card" v-if="role == 2">
            <div class="main-dashboard-statistics">
                <div class="stats-overview">
                    <div class="stats-content">
                        <div class="stats-number">{{ vvnCount }}</div>
                        <div class="stats-label">{{ t('notification.title') }}</div>
                    </div>
                    <span class="material-icons stats-icon">directions_boat</span>
                </div>
                <div class="stats-overview">
                    <div class="stats-content">
                        <div class="stats-number">{{ pendingVVNCount }}</div>
                        <div class="stats-label">{{ t('dashboard.SAOR.pendingNotifications') }}</div>
                    </div>
                    <span class="material-icons stats-icon">sailing</span>
                </div>
            </div>
        </sl-card>

        <!-- Recent Notifications and Chart -->
        <div class="card-row">
            <sl-card class="notifications-card">
                <div slot="header" class="card-header">
                    <span class="header-title">
                        {{ role == 2 ? t('dashboard.SAOR.recent') : t('dashboard.SAOR.recentReview') }}
                    </span>
                    <span class="material-icons header-icon">notifications</span>
                </div>
                
                <div v-if="recentNotifs.length === 0" class="empty-state">
                    <span class="material-icons empty-icon">inbox</span>
                    <p>{{ t('dashboard.SAOR.noRecent') }}</p>
                </div>
                <div v-else class="notification-list">
                    <VesselVisitNotificationPrinter 
                        v-for="notification in recentNotifs" 
                        :key="notification.notificationId"
                        :short="true"
                        :notification="notification"
                        :link="role == 2 ? `/vessel-visit-notifications/view/${notification.notificationId}` : `/vessel-visit-notifications/review/${notification.notificationId}`"
                    />
                </div>
            </sl-card>
            
            <sl-card class="chart-card">
                <div slot="header" class="card-header">
                    <span class="header-title">Statistics</span>
                    <span class="material-icons header-icon">bar_chart</span>
                </div>
                <div class="chart-container">
                    <Bar
                        v-if="role == 2"
                        id="my-chart-id"
                        :options="chartOptions"
                        :data="chartData"
                    />
                    <Pie 
                        v-else
                        id="distribution-pie-chart"
                        :data="pieChartData"
                        :options="pieChartOptions"
                    />
                </div>
            </sl-card>
        </div>
    </div>
</template>

<style scoped>
.dashboard-container {
    display: flex;
    flex-direction: column;
    gap: 1.5rem;
    width: 100%;
}

/* Statistics Card */
.stats-card {
    width: 100%;
}

.main-dashboard-statistics {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
    gap: 1.5rem;
    padding: 0.5rem;
}

.stats-overview {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 1.25rem;
    background: linear-gradient(135deg, var(--sl-color-primary-50) 0%, var(--sl-color-primary-100) 100%);
    border-radius: 12px;
    transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.stats-overview:hover {
    transform: translateY(-2px);
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.stats-content {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
}

.stats-number {
    font-size: 2rem;
    font-weight: 700;
    color: var(--sl-color-primary-700);
    line-height: 1;
}

.stats-label {
    font-size: 0.875rem;
    font-weight: 500;
    color: var(--sl-color-neutral-600);
    text-transform: uppercase;
    letter-spacing: 0.5px;
}

.stats-icon {
    font-size: 3rem;
    color: var(--sl-color-primary-400);
    opacity: 0.6;
}

/* Card Row */
.card-row {
    display: grid;
    padding-top: 1rem;
    grid-template-columns: 2fr 1fr;
    gap: 1.5rem;
    width: 100%;
    align-items: stretch;
}

.card-row > sl-card {
    height: 100%;
}

@media (max-width: 968px) {
    .card-row {
        grid-template-columns: 1fr;
    }
}

/* Card Headers */
.card-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 0.75rem 1rem;
    border-bottom: 1px solid var(--sl-color-neutral-200);
}

.header-title {
    font-size: var(--sl-font-size-large);
    font-weight: 600;
    color: var(--sl-color-neutral-900);
}

.header-icon {
    font-size: 1.5rem;
    color: var(--sl-color-primary-600);
    cursor: pointer;
    transition: color 0.2s ease;
}

.header-icon:hover {
    color: var(--sl-color-primary-700);
}

/* Notifications Card */
.notifications-card {
    min-height: 400px;
    display: flex;
    flex-direction: column;
}

.notification-list {
    display: flex;
    flex-direction: column;
    gap: 0.875rem;
    padding: 1rem;
}

.empty-state {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    padding: 3rem 1rem;
    color: var(--sl-color-neutral-500);
}

.empty-icon {
    font-size: 4rem;
    margin-bottom: 1rem;
    opacity: 0.3;
}

.empty-state p {
    font-size: var(--sl-font-size-medium);
    margin: 0;
}

/* Chart Card */
.chart-card {
    display: flex;
    flex-direction: column;
    min-height: 400px;
}

.chart-container {
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 2rem;
    flex: 1;
}
</style>