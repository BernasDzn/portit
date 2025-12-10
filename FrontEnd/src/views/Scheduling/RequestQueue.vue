<script setup lang="ts">
import { useRoute, useRouter } from 'vue-router';
import { inject, computed, ref, onMounted } from 'vue';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { ISchedulingService } from '@/service/IService/ISchedulingService';
import { useI18n } from 'vue-i18n';
import DataTable from '@/components/crud/DataTable.vue';
import { useSession } from '@/composables/session';
import { useAlerts } from '@/composables/alerts';
import type LoadingVue from '@/components/Loading.vue';

const scheduleService = container.get<ISchedulingService>(TYPES.schedulingService);
const route = useRouter();
const { t } = useI18n();

const notifications = useAlerts();

const email = useSession().authenticatedUser.email;
const queue = ref<any[]>([]);

const onlyMine = ref(false);

onMounted(async () => {

    loading.value = true;
    queue.value = await scheduleService.getQueueState();
    loading.value = false;
});

const columns = [
    "day",
    "algorithm",
    "priority",
    "issuer",
    "requestedAt",
    "status",
    "operations",
];

const statusVariants: Record<string, string> = {
    "pending": "primary",
    "in_progress": "warning",
    "completed": "primary",
    "failed": "danger",
    "unavailable": "danger",
    "rejected": "danger",
    "accepted": "success",
};

// flatten nested objects for DataTable
const rows = computed(() =>
    queue.value
        .filter(item => (onlyMine.value ? item.issuer === email : true))
        .map(item => ({
            id: item.id,
            day: item.data?.day,
            algorithm: item.data?.alg,
            priority: item.priority,
            result: item.result,
            requestedAt: new Date(item.requestedAt).toLocaleString(),
            status: item.status,
            issuer: item.issuer,
        }))
);

const reload = async () => {
    loading.value = true;
    queue.value = await scheduleService.getQueueState();
    loading.value = false;
};

const acceptResult = async (row: any) => {
    const id = row.id;
    try {

        await scheduleService.acceptSchedulingRequest(id);
        notifications.enqueueNotification(
            "Scheduling result accepted successfully.",
            notifications.notificationTypes.SUCCESS
        );
        
        reload();

    } catch (error) {
        notifications.enqueueNotification(
            "Failed to accept the scheduling result. " + error.response?.data?.message || (error as Error).message,
            notifications.notificationTypes.DANGER
        );
    }
};

const rejectResult = async (row: any) => {
    const id = row.id;

    try {

        await scheduleService.rejectSchedulingRequest(id);
        notifications.enqueueNotification(
            "Scheduling request rejected successfully.",
            notifications.notificationTypes.SUCCESS
        );

        reload();

    } catch (error) {
        console.log(error.response.data.message);
        notifications.enqueueNotification(
            "Failed to reject the scheduling request. " + error.response.data.message,
            notifications.notificationTypes.DANGER
        );
    }
};

const loading = ref(false);

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

        <sl-button style="margin-right: 20px;" variant="default" size="medium" circle @click="reload">
            <sl-icon name="arrow-counterclockwise"></sl-icon>
        </sl-button>

        <label style="margin-right: 10px">{{ t("scheduling.queue.onlyMine") }}</label>
        <sl-switch :checked="onlyMine" @sl-change="() => {
            onlyMine = !onlyMine
        }"></sl-switch>

        <br><br>

        <Loading v-if="loading" />
        <DataTable
            v-else
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
                        style="color: green"
                        @click="() => {
                            acceptResult(row)
                        }"
                    >
                        <sl-icon name="check" label="Accept Result" class="button is-small is-success" style="margin-left: 5px"></sl-icon>
                    </span>
    
                    <span 
                        :class="(row.result ? '' : 'is-disabled')"
                        style="color: red"
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