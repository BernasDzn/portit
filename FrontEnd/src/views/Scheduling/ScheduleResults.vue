<script setup lang="ts">
import { useRoute, useRouter } from 'vue-router';
import { inject, computed, ref } from 'vue';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { ISchedulingService } from '@/service/IService/ISchedulingService';
import { useI18n } from 'vue-i18n';
import DataTable from '@/components/crud/DataTable.vue';
import { useAlerts } from '@/composables/alerts';

const scheduleService = container.get<ISchedulingService>(TYPES.schedulingService);
const route = useRoute();
const router = useRouter();
const { t } = useI18n();
const notifications = useAlerts();

// results passed via router
const schedule = JSON.parse(route.query.request as string);
const requestId = route.query.requestId as string;
const loading = ref(false);

// Process data grouped by dock
const dockSchedules = computed(() => {

    const date = schedule.date;

    return schedule.data.map((dockData: any) => {
        const rows = dockData.schedule.map((item: any) => {
            // Convert hours to a Date
            const startDate = new Date(date);
            const endDate = new Date(date);

            const startHours = Math.floor(item.loading_enter_time);
            const startMinutes = Math.floor((item.loading_enter_time - startHours) * 60);

            const endHours = Math.floor(item.loading_exit_time);
            const endMinutes = Math.floor((item.loading_exit_time - endHours) * 60);

            startDate.setHours(startHours, startMinutes, 0, 0);
            endDate.setHours(endHours, endMinutes, 0, 0);

            console.log("Processed item:", item.name, startDate, endDate);

            return {
                name: item.name,
                start: startDate.toLocaleTimeString(),
                end: endDate.toLocaleTimeString(),
                cranes: Array.isArray(item.cranes) ? item.cranes.join(', ') : item.cranes
            };
        });

        const metrics = schedule.metrics.find((m: any) => m.selection) || {};

        return {
            dock: dockData.dock,
            rows,
            metrics
        };
    });
});

const columns = ["name", "start", "end", "cranes"];

const overallMetrics = computed(() => {
    const totalVessels = schedule.metrics.reduce((sum: number, m: any) => sum + (m.vesselCount || 0), 0);
    const totalDelay = schedule.metrics.reduce((sum: number, m: any) => sum + (m.totalDelay || 0), 0);
    const avgComputationTime = schedule.metrics.reduce((sum: number, m: any) => sum + (m.computationTime || 0), 0) / schedule.metrics.length;
    const algorithm = schedule.metrics[0]?.algorithm || 'unknown';

    return {
        algorithm,
        totalVessels,
        totalDelay,
        avgComputationTime
    };
});

const acceptResult = async () => {
    if (!requestId) {
        notifications.enqueueNotification(
            "No request ID found. Cannot accept this result.",
            notifications.notificationTypes.DANGER
        );
        return;
    }

    try {
        loading.value = true;
        await scheduleService.acceptSchedulingRequest(requestId);
        notifications.enqueueNotification(
            "Scheduling result accepted successfully.",
            notifications.notificationTypes.SUCCESS
        );
        
        // Redirect to queue or dashboard
        router.push({ name: 'Scheduling Queue' });
    } catch (error: any) {
        notifications.enqueueNotification(
            "Failed to accept the scheduling result. " + (error.response?.data?.message || error.message),
            notifications.notificationTypes.DANGER
        );
        loading.value = false;
    }
};

const rejectResult = async () => {
    if (!requestId) {
        notifications.enqueueNotification(
            "No request ID found. Cannot reject this result.",
            notifications.notificationTypes.DANGER
        );
        return;
    }

    try {
        loading.value = true;
        await scheduleService.rejectSchedulingRequest(requestId);
        notifications.enqueueNotification(
            "Scheduling request rejected successfully.",
            notifications.notificationTypes.SUCCESS
        );
        
        // Redirect to queue or dashboard
        router.push({ name: 'Scheduling Queue' });
    } catch (error: any) {
        notifications.enqueueNotification(
            "Failed to reject the scheduling request. " + (error.response?.data?.message || error.message),
            notifications.notificationTypes.DANGER
        );
        loading.value = false;
    }
};

const downloadPDF = async () => {
    const date = new Date(schedule.date);
    const pdf = await scheduleService.generateSchedulePDF(schedule, date);
    const blob = new Blob([pdf], { type: "application/pdf" });
    const url = URL.createObjectURL(blob);
    window.open(url, "_blank");
};
</script>

<template>
    <div class="container">
        <sl-breadcrumb>
            <sl-breadcrumb-item><RouterLink to="/scheduling-dashboard" class="breadcrumb-link">{{ t('scheduling.tabs.dashboard') }}</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ t('scheduling.tabs.results') }}</sl-breadcrumb-item>
        </sl-breadcrumb>

        <h1 class="title">{{ t("scheduling.results.title") }}</h1>
        <p class="subtitle">{{ t("scheduling.results.subtitle") }}</p>

        <sl-card>
            <p>
                <strong>Algorithm:</strong> {{ overallMetrics.algorithm }} <br>
                <strong>Total Vessels:</strong> {{ overallMetrics.totalVessels }} <br>
                <strong>Total Delay:</strong> {{ overallMetrics.totalDelay }}h <br>
                <strong>Avg Computation Time:</strong> {{ (overallMetrics.avgComputationTime * 1000).toFixed(2) }}ms
            </p>
        </sl-card>

        <div v-for="dockSchedule in dockSchedules" :key="dockSchedule.dock" >

            <sl-card style="margin-top: 20px; width: 100%">
                <h2>{{ dockSchedule.dock }}</h2>
            
                <!-- Dock-specific metrics -->
                <div>
                    <p>
                        <strong>Vessels:</strong> {{ dockSchedule.metrics.vesselCount }} |
                        <strong>Delay:</strong> {{ dockSchedule.metrics.totalDelay }}h |
                        <strong>Strategy:</strong> {{ dockSchedule.metrics.strategy }} |
                        <strong>Time:</strong> {{ (dockSchedule.metrics.computationTime * 1000).toFixed(2) }}ms
                    </p>
                </div>
    
                <DataTable
                    :columns="columns"
                    :rows="dockSchedule.rows"
                    keyField="name"
                    emptyText="No vessels"
                />
            </sl-card>
        </div>

        <br>
        <div class="action-buttons">
            <sl-button variant="primary" @click="downloadPDF">
                <sl-icon slot="prefix" name="download"></sl-icon>
                Download as PDF
            </sl-button>
            
            <div class="right-buttons">
                <sl-button 
                    variant="success" 
                    @click="acceptResult"
                    :loading="loading"
                    :disabled="!requestId"
                >
                    <sl-icon slot="prefix" name="check-circle"></sl-icon>
                    Accept Result
                </sl-button>
                
                <sl-button 
                    variant="danger" 
                    @click="rejectResult"
                    :loading="loading"
                    :disabled="!requestId"
                >
                    <sl-icon slot="prefix" name="x-circle"></sl-icon>
                    Reject Result
                </sl-button>
            </div>
        </div>
    </div>
</template>

<style scoped>
.action-buttons {
    display: flex;
    justify-content: space-between;
    align-items: center;
    gap: 1rem;
    flex-wrap: wrap;
}

.right-buttons {
    display: flex;
    gap: 1rem;
}
</style>