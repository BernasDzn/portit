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
    console.log("Queue data loaded:", queue.value);
});

const columns = [
    "day",
    "algorithm",
    "priority",
    "issuer",
    "requestedAt",
    "status",
    "operations"
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
            result: item.result,
            requestedAt: new Date(item.requestedAt).toLocaleString(),
            status: item.status,
            issuer: item.issuer,
        }))
);

const acceptResult = (row: any) => {
    // Implement accept logic here
    console.log("Accepting result for row:", row);
};

const rejectResult = (row: any) => {
    // Implement reject logic here
    console.log("Rejecting result for row:", row);
};

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

            <template #operations="{ row }">

                <div class="operations">
    
                    <RouterLink
                        :to="{
                            name: 'ScheduleResults',
                            query: { request: JSON.stringify({
                                date: row.day,
                                ...row.result
                            }) }
                        }"
                        :class="'is-info ' + (row.result ? '' : 'is-disabled')"
                    >
                        <sl-icon name="eye" label="View Request"></sl-icon>
                    </RouterLink>
    
                    
                    <span 
                        :class="(row.result ? '' : 'is-disabled')"
                        @click="() => {
                            acceptResult(row)
                        }"
                    >
                        <sl-icon name="check" label="Accept Result" class="button is-small is-success" style="margin-left: 5px"></sl-icon>
                    </span>
    
                    <span 
                        :class="(row.result ? '' : 'is-disabled')"
                        @click="() => {
                            rejectResult(row)
                        }"
                    >
                        <sl-icon name="x" label="Reject Result" class="button is-small is-danger" style="margin-left: 5px"></sl-icon>
                    </span>
                </div>
                
            </template>
        </DataTable>
        
    </div>
</template>

<style scoped>

.is-info {
    display: inline-flex;
    text-decoration: none;
    align-items: center;
    color: black;
}

.is-disabled {
    pointer-events: none;
    opacity: 0.5;
}

.operations {

    width: 100%;

    display: inline-flex;
    align-items: center;
    justify-content: space-around;
}

.operations sl-icon {
    cursor: pointer;
}

</style>