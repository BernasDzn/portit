<script setup lang="ts">
import { useRoute } from 'vue-router';
import { inject, computed, ref, onMounted } from 'vue';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { ISchedulingService } from '@/service/IService/ISchedulingService';
import { useI18n } from 'vue-i18n';
import DataTable from '@/components/crud/DataTable.vue';
import { useSession } from '@/composables/session';

const scheduleService = container.get<ISchedulingService>(TYPES.schedulingService);
const route = useRoute();
const { t } = useI18n();

const email = useSession().authenticatedUser.email;
const queue = ref<any[]>([]);

const onlyMine = ref(false);

onMounted(async () => {
    queue.value = await scheduleService.getQueueState();
});

const columns = [
    "day",
    "algorithm",
    "priority",
    "issuer",
    "requestedAt",
    "status",
];

const statusVariants: Record<string, string> = {
    "pending": "primary",
    "in_progress": "warning",
    "completed": "success",
    "failed": "danger",
    "unavailable": "danger",
};

// flatten nested objects for DataTable
const rows = computed(() =>
    queue.value
        .filter(item => (onlyMine.value ? item.issuer === email : true))
        .map(item => ({
            day: item.data?.day,
            algorithm: item.data?.alg,
            priority: item.priority,
            requestedAt: new Date(item.requestedAt).toLocaleString(),
            status: item.status,
            issuer: item.issuer,
        }))
);
</script>

<template>
    <div class="container">
        <sl-breadcrumb>
            <sl-breadcrumb-item>
                <RouterLink to="/scheduling-dashboard" class="breadcrumb-link">
                    {{ t('scheduling.tabs.dashboard') }}
                </RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                {{ t('scheduling.tabs.queue') }}
            </sl-breadcrumb-item>
        </sl-breadcrumb>

        <h1 class="title">{{ t("scheduling.queue.title") }}</h1>
        <p class="subtitle">{{ t("scheduling.queue.subtitle") }}</p>

        <label style="margin-right: 10px">{{ t("scheduling.queue.onlyMine") }}</label>
        <sl-switch :checked="onlyMine" @sl-change="() => {
            onlyMine = !onlyMine
        }"></sl-switch>
        <br><br>

        <DataTable
            :columns="columns"
            :rows="rows"
            keyField="_id"
            emptyText="Queue is empty"
        >
            <template #status="{ value }">
                <sl-badge
                    :variant="statusVariants[value] || 'default'"
                >
                    {{ value }}
                </sl-badge>
            </template>
        </DataTable>
        
    </div>
</template>