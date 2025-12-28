<script setup lang="ts">
import { computed } from 'vue';
import { RouterLink } from 'vue-router';
import type { VesselVisitExecution } from '@/model/VesselVisitExecution';
import { useI18n } from 'vue-i18n';
import DataTable from '@/components/crud/DataTable.vue';

const { t } = useI18n();

const props = defineProps<{
    executions: VesselVisitExecution[];
}>();

const calculateMetrics = (execution: VesselVisitExecution) => {
    const operations = execution.operationsExecuted;
    
    let firstStart: Date | undefined = undefined;
    let lastEnd: Date | undefined = undefined;
    
    operations.forEach(op => {
        if (op.operation.startTime) {
            const start = new Date(op.operation.startTime);
            if (!firstStart || start < firstStart) {
                firstStart = start;
            }
        }
        if (op.operation.endTime) {
            const end = new Date(op.operation.endTime);
            if (!lastEnd || end > lastEnd) {
                lastEnd = end;
            }
        }
    });

    // TODO: replace with real value after
    let totalTurnaroundTime = '-';
    if (firstStart && lastEnd) {
        const diffMs = lastEnd.getTime() - firstStart.getTime();
        const hours = Math.floor(diffMs / (1000 * 60 * 60));
        const minutes = Math.floor((diffMs % (1000 * 60 * 60)) / (1000 * 60));
        totalTurnaroundTime = `${hours}h ${minutes}m`;
    }

    // TODO: replace with real value after
    let berthOccupancyTime = '-';
    if (execution.dateOpen) {
        const endDate = execution.dateClosed ? new Date(execution.dateClosed) : new Date();
        const diffMs = endDate.getTime() - new Date(execution.dateOpen).getTime();
        const hours = Math.floor(diffMs / (1000 * 60 * 60));
        const minutes = Math.floor((diffMs % (1000 * 60 * 60)) / (1000 * 60));
        berthOccupancyTime = `${hours}h ${minutes}m`;
    }

    const completedOps = operations.filter(op => op.status === 'Completed').length;
    const startedOps = operations.filter(op => op.status === 'Started').length;
    const delayedOps = operations.filter(op => op.status === 'Delayed').length;
    const pendingOps = operations.filter(op => op.status === 'Pending').length;

    return {
        totalTurnaroundTime,
        berthOccupancyTime,
        completedOps,
        startedOps,
        delayedOps,
        pendingOps,
    };
};

const columns = [
    'code',
    'relatedVVN',
    'status',
    'totalOperations',
    'progress',
    'turnaroundTime',
    'berthOccupancy',
    'dateOpen',
    'dateClosed',
    'actions'
];

const rows = computed(() => {
    return props.executions.map(execution => {
        const metrics = calculateMetrics(execution);
        return {
            id: execution.id,
            code: execution.code,
            relatedVVN: execution.relatedVVN,
            status: execution.status,
            totalOperations: execution.operationsExecuted.length,
            progress: {
                completed: metrics.completedOps,
                total: execution.operationsExecuted.length,
                pending: metrics.pendingOps,
                started: metrics.startedOps,
                delayed: metrics.delayedOps
            },
            turnaroundTime: metrics.totalTurnaroundTime,
            berthOccupancy: metrics.berthOccupancyTime,
            dateOpen: execution.dateOpen ? new Date(execution.dateOpen).toLocaleString() : '-',
            dateClosed: execution.dateClosed ? new Date(execution.dateClosed).toLocaleString() : '-',
            execution: execution
        };
    });
});

const statusVariants: Record<string, string> = {
    'Open': 'primary',
    'Closed': 'success'
};
</script>

<template>
    <div class="container">
        <DataTable
            :columns="columns"
            :rows="rows"
            keyField="id"
            :emptyText="t('common.noData')"
        >
            <template #code="{ value }">
                <span class="code-cell">{{ value }}</span>
            </template>

            <template #status="{ value }">
                <sl-badge :variant="statusVariants[value] || 'default'">
                    {{ value }}
                </sl-badge>
            </template>

            <template #progress="{ value }">
                <div class="progress-cell">
                    <sl-badge v-if="value.pending > 0" variant="neutral" size="small" pill>
                        {{ value.pending }} {{ t('execution.status.pending') }}
                    </sl-badge>
                    <sl-badge v-if="value.started > 0" variant="primary" size="small" pill>
                        {{ value.started }} {{ t('execution.status.started') }}
                    </sl-badge>
                    <sl-badge v-if="value.delayed > 0" variant="warning" size="small" pill>
                        {{ value.delayed }} {{ t('execution.status.delayed') }}
                    </sl-badge>
                    <sl-badge v-if="value.completed > 0" variant="success" size="small" pill>
                        {{ value.completed }} {{ t('execution.status.completed') }}
                    </sl-badge>
                </div>
            </template>

            <template #turnaroundTime="{ value }">
                <span class="time-cell">{{ value }}</span>
            </template>

            <template #berthOccupancy="{ value }">
                <span class="time-cell">{{ value }}</span>
            </template>

            <template #actions="{ row }">
                <div class="operations">
                    <RouterLink
                        :to="`/vessel-visit-executions/${row.relatedVVN}`"
                        class="is-info"
                    >
                        <sl-icon name="eye" label="View Execution"></sl-icon>
                    </RouterLink>
                </div>
            </template>
        </DataTable>
    </div>
</template>

<style scoped>
.container {
    width: 100%;
}

.code-cell {
    color: var(--accent-1);
}

.time-cell {
    color: #374151;
}

.progress-cell {
    display: flex;
    gap: 4px;
    flex-wrap: wrap;
    justify-content: center;
}

.operations {
    width: 100%;
    display: inline-flex;
    align-items: center;
    justify-content: center;
}

.operations sl-icon {
    cursor: pointer;
}

.is-info {
    display: inline-flex;
    text-decoration: none;
    align-items: center;
    color: black;
}
</style>
