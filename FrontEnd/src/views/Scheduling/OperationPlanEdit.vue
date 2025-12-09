<script setup lang="ts">
import { useRoute } from 'vue-router';
import { useAlerts } from '@/composables/alerts';
import { ref } from 'vue';
import { useI18n } from 'vue-i18n';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { IOperationPlanService } from '@/service/IService/IOperationPlanService';
import type { OperationPlanDto } from '@/model/dto/OperationPlanDto';
import { GGanttChart, GGanttRow } from '@infectoone/vue-ganttastic';

const route = useRoute();
const planId = String(route.params.id || '');

const notifications = useAlerts();

const planService = container.get<IOperationPlanService>(TYPES.operationPlanService);

const plan = ref<OperationPlanDto>({
    id: '',
    relatedVVN: '',
    dock: '',
	operationSchedule: [],
	metadata: {
		createdBy: '',
		createdAt: '',
		algorithmUsed: ''
    }
});

const updatePlan = async () => {
};

const chartStart = ref('2025-12-01 00:00');
const chartEnd = ref('2025-12-01 14:00');

const rows = ref([
    {
        label: 'Loading',
        bars: [
            {   // L1
                myStart: '2025-12-01 00:00',
                myEnd:   '2025-12-01 01:00',
                ganttBarConfig: {
                    id: 'L1',
                    label: 'Container L1',
                    hasHandles: true,
                    style: { background: '#3498db', borderRadius: '8px' }
                }
            },
            {   // L2
                myStart: '2025-12-01 01:00',
                myEnd:   '2025-12-01 02:00',
                ganttBarConfig: {
                    id: 'L2',
                    label: 'Container L2',
                    hasHandles: true,
                    style: { background: '#2980b9', borderRadius: '8px' }
                }
            },
            {   // L3
                myStart: '2025-12-01 02:00',
                myEnd:   '2025-12-01 03:30',
                ganttBarConfig: {
                    id: 'L3',
                    label: 'Container L3',
                    hasHandles: true,
                    style: { background: '#1f618d', borderRadius: '8px' }
                }
            },
            {   // L4
                myStart: '2025-12-01 03:30',
                myEnd:   '2025-12-01 05:00',
                ganttBarConfig: {
                    id: 'L4',
                    label: 'Container L4',
                    hasHandles: true,
                    style: { background: '#154360', borderRadius: '8px' }
                }
            },
            {   // L5
                myStart: '2025-12-01 05:00',
                myEnd:   '2025-12-01 06:00',
                ganttBarConfig: {
                    id: 'L5',
                    label: 'Container L5',
                    hasHandles: true,
                    style: { background: '#21618c', borderRadius: '8px' }
                }
            },
            {   // L6
                myStart: '2025-12-01 06:00',
                myEnd:   '2025-12-01 07:30',
                ganttBarConfig: {
                    id: 'L6',
                    label: 'Container L6',
                    hasHandles: true,
                    style: { background: '#5dade2', borderRadius: '8px' }
                }
            },
            {   // L7
                myStart: '2025-12-01 07:30',
                myEnd:   '2025-12-01 09:00',
                ganttBarConfig: {
                    id: 'L7',
                    label: 'Container L7',
                    hasHandles: true,
                    style: { background: '#85c1e9', borderRadius: '8px' }
                }
            },
            {   // L8
                myStart: '2025-12-01 09:00',
                myEnd:   '2025-12-01 10:00',
                ganttBarConfig: {
                    id: 'L8',
                    label: 'Container L8',
                    hasHandles: true,
                    style: { background: '#aed6f1', borderRadius: '8px' }
                }
            },
            {   // L9
                myStart: '2025-12-01 10:00',
                myEnd:   '2025-12-01 11:30',
                ganttBarConfig: {
                    id: 'L9',
                    label: 'Container L9',
                    hasHandles: true,
                    style: { background: '#d4e6f1', borderRadius: '8px' }
                }
            },
            {   // L10
                myStart: '2025-12-01 11:30',
                myEnd:   '2025-12-01 13:00',
                ganttBarConfig: {
                    id: 'L10',
                    label: 'Container L10',
                    hasHandles: true,
                    style: { background: '#ebf5fb', borderRadius: '8px' }
                }
            }
        ]
    },

    {
        label: 'Unloading',
        bars: [
            {   // U1
                myStart: '2025-12-01 00:00',
                myEnd:   '2025-12-01 01:30',
                ganttBarConfig: {
                    id: 'U1',
                    label: 'Container U1',
                    hasHandles: true,
                    style: { background: '#e74c3c', borderRadius: '8px' }
                }
            },
            {   // U2
                myStart: '2025-12-01 01:30',
                myEnd:   '2025-12-01 03:00',
                ganttBarConfig: {
                    id: 'U2',
                    label: 'Container U2',
                    hasHandles: true,
                    style: { background: '#c0392b', borderRadius: '8px' }
                }
            },
            {   // U3
                myStart: '2025-12-01 03:00',
                myEnd:   '2025-12-01 04:30',
                ganttBarConfig: {
                    id: 'U3',
                    label: 'Container U3',
                    hasHandles: true,
                    style: { background: '#922b21', borderRadius: '8px' }
                }
            },
            {   // U4
                myStart: '2025-12-01 04:30',
                myEnd:   '2025-12-01 06:00',
                ganttBarConfig: {
                    id: 'U4',
                    label: 'Container U4',
                    hasHandles: true,
                    style: { background: '#641e16', borderRadius: '8px' }
                }
            },
            {   // U5
                myStart: '2025-12-01 06:00',
                myEnd:   '2025-12-01 07:00',
                ganttBarConfig: {
                    id: 'U5',
                    label: 'Container U5',
                    hasHandles: true,
                    style: { background: '#7b241c', borderRadius: '8px' }
                }
            },
            {   // U6
                myStart: '2025-12-01 07:00',
                myEnd:   '2025-12-01 08:30',
                ganttBarConfig: {
                    id: 'U6',
                    label: 'Container U6',
                    hasHandles: true,
                    style: { background: '#cd6155', borderRadius: '8px' }
                }
            },
            {   // U7
                myStart: '2025-12-01 08:30',
                myEnd:   '2025-12-01 10:00',
                ganttBarConfig: {
                    id: 'U7',
                    label: 'Container U7',
                    hasHandles: true,
                    style: { background: '#f1948a', borderRadius: '8px' }
                }
            },
            {   // U8
                myStart: '2025-12-01 10:00',
                myEnd:   '2025-12-01 11:00',
                ganttBarConfig: {
                    id: 'U8',
                    label: 'Container U8',
                    hasHandles: true,
                    style: { background: '#f5b7b1', borderRadius: '8px' }
                }
            },
            {   // U9
                myStart: '2025-12-01 11:00',
                myEnd:   '2025-12-01 12:30',
                ganttBarConfig: {
                    id: 'U9',
                    label: 'Container U9',
                    hasHandles: true,
                    style: { background: '#fadbd8', borderRadius: '8px' }
                }
            },
            {   // U10
                myStart: '2025-12-01 12:30',
                myEnd:   '2025-12-01 14:00',
                ganttBarConfig: {
                    id: 'U10',
                    label: 'Container U10',
                    hasHandles: true,
                    style: { background: '#f9ebea', borderRadius: '8px' }
                }
            }
        ]
    }
]);

const { t } = useI18n();

</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item><RouterLink to="/scheduling-dashboard" class="breadcrumb-link">{{ t('scheduling.plans.tabs.dashboard') }}</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item><RouterLink to="/scheduling/plans-search" class="breadcrumb-link">{{ t('scheduling.plans.tabs.search') }}</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ t('scheduling.plans.tabs.edit') }}</sl-breadcrumb-item>
        </sl-breadcrumb>
        
        <h1 class="title">{{ t('scheduling.plans.tabs.edit') }}</h1>
        <p class="subtitle">{{ t('scheduling.plans.subtitle.edit') }}</p>
        
        <GGanttChart
            :chart-start="chartStart"
            :chart-end="chartEnd"
            precision="hour"
            bar-start="myStart"
            bar-end="myEnd"
            :row-height="50"
            :grid="true"
        >
            <GGanttRow
                v-for="row in rows"
                :key="row.label"
                :label="row.label"
                :bars="row.bars"
            />
        </GGanttChart>

    </div>
</template>