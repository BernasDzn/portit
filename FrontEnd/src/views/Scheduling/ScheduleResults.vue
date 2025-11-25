<script setup lang="ts">
import { useRoute } from 'vue-router';
import { inject } from 'vue';
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

const rows = schedule.data.map((item: any) => ({
    name: item.name.replace(/_\d+$/, ''),
    start: new Date(date.getTime() + item.loading_enter_time * 3600000).toLocaleString(),
    end: new Date(date.getTime() + item.loading_exit_time * 3600000).toLocaleString(),
    cranes: Array.isArray(item.cranes) ? item.cranes.join(', ') : item.cranes
}));

const columns = ["name", "start", "end", "cranes"];

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
            <sl-breadcrumb-item>
                <RouterLink to="/schedule" class="breadcrumb-link">
                    {{ t('scheduling.title') }}
                </RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                {{ t('scheduling.results.title') }}
            </sl-breadcrumb-item>
        </sl-breadcrumb>

        <h1 class="title">{{ t("scheduling.results.title") }}</h1>
        <p class="subtitle">{{ t("scheduling.results.subtitle") }}</p>

        <p>
            Algorithm: {{ schedule.metrics.algorithm }} <br>
            Total Delay: {{ schedule.metrics.totalDelay }}h <br>
            Computation Time: {{ (schedule.metrics.computationTime * 1000).toFixed(2) }}ms <br>
            Vessels: {{ schedule.metrics.vesselCount }}
        </p>

        <DataTable
            :columns="columns"
            :rows="rows"
            keyField="name"
            emptyText="No vessels"
        />

        <br>
        <sl-button variant="primary" @click="downloadPDF">
            Download as PDF
        </sl-button>
    </div>
</template>