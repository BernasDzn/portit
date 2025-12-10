<script setup lang="ts">
import { ref, onMounted, watch, onBeforeUnmount } from 'vue';
import { GGanttChart, GGanttRow } from '@infectoone/vue-ganttastic';

export interface GanttItem {
    id: string;
    startTime: string;
    endTime: string;
    name: string;
    group?: string;
    color?: string;
}

export interface GanttRowConfig {
    name: string;
    color?: string;
}

interface Props {
    items: GanttItem[];
    rowConfigs?: GanttRowConfig[];
    groupBy?: (item: GanttItem) => string;
    onUpdate?: (item: GanttItem) => void;
    initialStart?: string;
    initialEnd?: string;
    rowHeight?: number;
    enableGrid?: boolean;
    enableDrag?: boolean;
    enableZoom?: boolean;
    enableScroll?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
    rowHeight: 50,
    enableGrid: true,
    enableDrag: true,
    enableZoom: true,
    enableScroll: true
});

const emit = defineEmits<{
    itemUpdated: [item: GanttItem];
    chartUpdate: [{ start: string; end: string; precision: string }];
}>();

const defaultColor = '#3498db';
const chartStart = ref('');
const chartEnd = ref('');
const chartPrecision = ref<'hour' | 'day' | 'month'>('hour');
const chartRows = ref<Array<{ label: string; bars: any[] }>>([]);

let chartContainer: Element | null = null;

const defaultGroupBy = (item: GanttItem): string => {
    return item.group || 'Default';
};

const getDefaultRowConfigs = (): GanttRowConfig[] => {
    const groups = new Set(props.items.map(item => (props.groupBy || defaultGroupBy)(item)));
    return Array.from(groups).map(name => ({ name }));
};

const getColorForRow = (rowName: string, index: number): string => {
    const config = (props.rowConfigs || getDefaultRowConfigs()).find(r => r.name === rowName);
    if (config?.color) return config.color;
    
    return defaultColor;
};

const generateRows = () => {
    if (!props.items || props.items.length === 0) {
        chartRows.value = [];
        return;
    }

    const groupByFn = props.groupBy || defaultGroupBy;
    const rowConfigs = props.rowConfigs || getDefaultRowConfigs();
    
    const grouped = new Map<string, GanttItem[]>();
    
    props.items.forEach(item => {
        const groupKey = groupByFn(item);
        if (!grouped.has(groupKey)) {
            grouped.set(groupKey, []);
        }
        grouped.get(groupKey)!.push(item);
    });

    const result = rowConfigs.map((config, rowIndex) => {
        const items = grouped.get(config.name) || [];
        const bars = items.map((item, index) => ({
            myStart: new Date(item.startTime).toISOString().slice(0, 16).replace('T', ' '),
            myEnd: new Date(item.endTime).toISOString().slice(0, 16).replace('T', ' '),
            ganttBarConfig: {
                id: item.id,
                label: item.name,
                hasHandles: props.enableDrag,
                immobile: !props.enableDrag,
                style: {
                    background: item.color || getColorForRow(config.name, rowIndex),
                    borderRadius: '8px'
                },
                originalItem: item
            }
        }));

        return {
            label: config.name,
            bars
        };
    });

    chartRows.value = result;
};

const autoFitTimeRange = () => {
    if (!props.items || props.items.length === 0) return;

    const times = props.items.map(item => ({
        start: new Date(item.startTime),
        end: new Date(item.endTime)
    }));

    const minTime = new Date(Math.min(...times.map(t => t.start.getTime())));
    const maxTime = new Date(Math.max(...times.map(t => t.end.getTime())));

    minTime.setHours(minTime.getHours() - 1);
    maxTime.setHours(maxTime.getHours() + 1);

    chartStart.value = minTime.toISOString().slice(0, 16).replace('T', ' ');
    chartEnd.value = maxTime.toISOString().slice(0, 16).replace('T', ' ');
};

watch(() => props.items, (newItems, oldItems) => {
    // Only regenerate if items array structure changed (added/removed items)
    // or if there was no previous data
    if (!oldItems || newItems.length !== oldItems.length || 
        newItems.some((item, i) => item.id !== oldItems[i]?.id)) {
        if (newItems && newItems.length > 0 && !props.initialStart && !props.initialEnd) {
            autoFitTimeRange();
        }
        generateRows();
    }
    // Skip updates when just times changed - that's from our own drag events
}, { immediate: true });

onMounted(() => {

    if (props.initialStart) {
        chartStart.value = props.initialStart;
    }
    
    if (props.initialEnd) {
        chartEnd.value = props.initialEnd;
    }

    if (props.enableZoom || props.enableScroll) {
        setupChartInteractions();
    }
    
});

onBeforeUnmount(() => {
    if (chartContainer) {
        chartContainer.removeEventListener('wheel', handleWheel as EventListener);
    }
});

watch([chartStart, chartEnd, chartPrecision], () => {
    emit('chartUpdate', {
        start: chartStart.value,
        end: chartEnd.value,
        precision: chartPrecision.value
    });
});

const setupChartInteractions = () => {
    setTimeout(() => {
        chartContainer = document.querySelector('.g-gantt-chart');
        if (chartContainer) {
            chartContainer.addEventListener('wheel', handleWheel as EventListener, { passive: false });
        }
    }, 100);
};

const handleWheel = (e: WheelEvent) => {
    e.preventDefault();

    if (e.ctrlKey && props.enableZoom) {
        handleZoom(e.deltaY);
    } else if (props.enableScroll) {
        handleScroll(e.deltaY);
    }
};

const handleZoom = (delta: number) => {
    const currentStart = new Date(chartStart.value.replace(' ', 'T'));
    const currentEnd = new Date(chartEnd.value.replace(' ', 'T'));
    const duration = currentEnd.getTime() - currentStart.getTime();
    
    const zoomFactor = delta > 0 ? 1.1 : 0.9;
    const newDuration = duration * zoomFactor;
    
    const center = (currentStart.getTime() + currentEnd.getTime()) / 2;
    const newStart = new Date(center - newDuration / 2);
    const newEnd = new Date(center + newDuration / 2);
    
    chartStart.value = newStart.toISOString().slice(0, 16).replace('T', ' ');
    chartEnd.value = newEnd.toISOString().slice(0, 16).replace('T', ' ');
    
    updatePrecision(newDuration);
};

const handleScroll = (delta: number) => {
    const currentStart = new Date(chartStart.value.replace(' ', 'T'));
    const currentEnd = new Date(chartEnd.value.replace(' ', 'T'));
    const duration = currentEnd.getTime() - currentStart.getTime();
    
    const scrollAmount = duration * 0.1;
    const offset = delta > 0 ? scrollAmount : -scrollAmount;
    
    const newStart = new Date(currentStart.getTime() + offset);
    const newEnd = new Date(currentEnd.getTime() + offset);
    
    chartStart.value = newStart.toISOString().slice(0, 16).replace('T', ' ');
    chartEnd.value = newEnd.toISOString().slice(0, 16).replace('T', ' ');
};

const updatePrecision = (duration: number) => {
    const hours = duration / (1000 * 60 * 60);
    
    if (hours <= 48) {
        chartPrecision.value = 'hour';
    } else if (hours <= 720) {
        chartPrecision.value = 'day';
    } else {
        chartPrecision.value = 'month';
    }
};

const onBarDragEnd = (event: any) => {
    const originalItem = event.bar.ganttBarConfig.originalItem as GanttItem;
    
    console.log('Bar drag end event:', event);
    console.log('Original item:', originalItem);
    
    if (!originalItem) {
        console.warn('No original item found in bar config');
        return;
    }

    const updatedItem: GanttItem = {
        ...originalItem,
        startTime: new Date(event.bar.myStart.replace(' ', 'T')).toISOString(),
        endTime: new Date(event.bar.myEnd.replace(' ', 'T')).toISOString()
    };

    console.log('Emitting updated item:', updatedItem);

    if (props.onUpdate) {
        props.onUpdate(updatedItem);
    }

    emit('itemUpdated', updatedItem);
};
</script>

<template>
    <div class="interactive-gantt-chart">
        <GGanttChart
            :chart-start="chartStart"
            :chart-end="chartEnd"
            :precision="chartPrecision"
            bar-start="myStart"
            bar-end="myEnd"
            :row-height="rowHeight"
            :grid="enableGrid"
            @bar-dragend="onBarDragEnd"
            @dragend-bar="onBarDragEnd"
            @bar-update="onBarDragEnd"
            @update:bar="onBarDragEnd"
        >
            <GGanttRow
                v-for="row in chartRows"
                :key="row.label"
                :label="row.label"
                :bars="row.bars"
            />
        </GGanttChart>
    </div>
</template>

<style scoped>
.interactive-gantt-chart {
    width: 100%;
    height: 100%;
}
</style>
