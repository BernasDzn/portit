<script setup lang="ts">
import { useRoute } from 'vue-router';
import { inject, computed } from 'vue';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { ISchedulingService } from '@/service/IService/ISchedulingService';
import { useI18n } from 'vue-i18n';
import DataTable from '@/components/crud/DataTable.vue';

const scheduleService = container.get<ISchedulingService>(TYPES.schedulingService);
const route = useRoute();
const { t } = useI18n();

// results passed via router
const schedule = JSON.parse(route.query.schedule as string);
const date = new Date(route.query.date as string);

// Process data grouped by dock
const dockSchedules = computed(() => {
    return schedule.data.map((dockData: any, index: number) => {
        const rows = dockData.schedule.map((item: any) => ({
            name: item.name.replace(/_\d+$/, ''),
            start: new Date(date.getTime() + item.loading_enter_time * 3600000).toLocaleString(),
            end: new Date(date.getTime() + item.loading_exit_time * 3600000).toLocaleString(),
            cranes: Array.isArray(item.cranes) ? item.cranes.join(', ') : item.cranes
        }));

        const metrics = schedule.metrics[index] || {};

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

const downloadPDF = async () => {
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
        <sl-button variant="primary" @click="downloadPDF">
            Download as PDF
        </sl-button>
    </div>
</template>