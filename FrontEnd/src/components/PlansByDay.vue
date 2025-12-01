<script setup lang="ts">
import { useI18n } from 'vue-i18n';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { IOperationPlanService } from '@/service/IService/IOperationPlanService';
import { computed, onMounted, ref } from 'vue';
import { Chart, Legend, LinearScale, PointElement, ScatterController, Title, Tooltip } from 'chart.js';
import { Scatter } from 'vue-chartjs';

const props = defineProps<{
    width?: number;
    height?: number;
}>();

Chart.register(ScatterController, LinearScale, PointElement, Title, Tooltip, Legend);
const oemService = container.get<IOperationPlanService>(TYPES.operationPlanService);

const dayMap = ref<{
    date: string;
    count: number;
}[]>([]);

onMounted(async () => {
    const c = await oemService.groupOperationPlansByDate();
    dayMap.value = c;
});

const chartdata = computed(() => {
    return {
        datasets: [
            {
                label: 'Operation Plans by Day',
                data: dayMap.value.map(item => ({
                    x: new Date(item.date).getTime(),
                    y: item.count
                })),
                backgroundColor: 'rgba(75, 192, 192, 0.6)',
                pointRadius: 6,
            },
        ],
    };
});

const chartoptions = {
    responsive: false,
    maintainAspectRatio: false,
    scales: {
        x: {
            type: 'linear',
            title: {
                display: true,
                text: 'Date'
            },
            ticks: {
                callback: function(value: number) {
                    const date = new Date(value);
                    return date.toLocaleDateString('en-US', { 
                        month: 'short', 
                        day: 'numeric',
                        year: 'numeric'
                    });
                }
            }
        },
        y: {
            beginAtZero: true,
            title: {
                display: true,
                text: 'Number of Plans'
            },
            ticks: {
                precision: 0
            }
        }
    },
    plugins: {
        legend: { 
            position: 'top' 
        },
        title: { 
            display: true, 
            text: 'Operation Plan Distribution',
            font: {
                size: 16
            }
        },
        tooltip: {
            callbacks: {
                title: function(context: any) {
                    const date = new Date(context[0].parsed.x);
                    return date.toLocaleDateString('en-US', { 
                        month: 'short', 
                        day: 'numeric',
                        year: 'numeric'
                    });
                }
            }
        }
    }
};
</script>

<template>
  <div>
    <Scatter :data="chartdata" :options="chartoptions" :width="width || 600" :height="height || 400" />
  </div>
</template>