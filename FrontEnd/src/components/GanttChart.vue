<script setup lang="ts">
import { ref, onMounted, watch, onBeforeUnmount, computed } from 'vue';
import { GGanttChart, GGanttRow, type GanttBarObject } from '@infectoone/vue-ganttastic';

export interface GanttItem {
    id: string;
    startTime: string;
    endTime: string;
    name: string;
    group?: string;
    color?: string;
    minTime?: string;  // Minimum allowed start time (constraint)
    maxTime?: string;  // Maximum allowed end time (constraint)
}

export interface GanttRowConfig {
    name: string;
    color?: string;
    operationalWindow?: {
        shifts: Array<{
            day: number;
            startTime: string;
            endTime: string;
        }>;
    };
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
    showCurrentTime?: boolean;
    pushOnOverlap?: boolean;
    noOverlap?: boolean;
    enableZoom?: boolean;
    enableScroll?: boolean;
    showOperationalWindows?: boolean;
    onBarClick?: (value: {
        bar: GanttBarObject;
        e: MouseEvent;
        datetime?: string | Date | undefined;
    }) => void;
}

const props = withDefaults(defineProps<Props>(), {
    rowHeight: 45,
    enableGrid: false,
    enableDrag: true,
    showCurrentTime: false,
    pushOnOverlap: false,
    noOverlap: false,
    enableZoom: true,
    enableScroll: true,
    showOperationalWindows: true,
    onBarClick: () => {}
});

const emit = defineEmits<{
    itemUpdated: [item: GanttItem];
    chartUpdate: [{ start: string; end: string; precision: string }];
}>();

const chartStart = ref('');
const chartEnd = ref('');
const chartPrecision = ref<'hour' | 'day' | 'month'>('hour');
const chartRows = ref<Array<{ label: string; bars: any[] }>>([]);

let chartContainer: Element | null = null;
let isDragging = false;

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
    
    return '#3498db';
};

// Generate operational window blocks for visualization
const getOperationalWindowBlocks = (rowConfig: GanttRowConfig) => {
    if (!props.showOperationalWindows || !rowConfig.operationalWindow?.shifts) {
        return [];
    }

    const blocks: Array<{ start: string; end: string }> = [];
    const chartStartTime = new Date(chartStart.value.replace(' ', 'T'));
    const chartEndTime = new Date(chartEnd.value.replace(' ', 'T'));
    
    // For each day in the chart range
    const currentDate = new Date(chartStartTime);
    
    while (currentDate <= chartEndTime) {
        const dayOfWeek = currentDate.getDay();
        
        // Find shifts for this day
        const dayShifts = rowConfig.operationalWindow.shifts.filter(s => s.day === dayOfWeek);
        
        dayShifts.forEach(shift => {
            // Parse shift times (format: "HH:MM:SS")
            const [startHour, startMin, startSec] = shift.startTime.split(':').map(Number);
            const [endHour, endMin, endSec] = shift.endTime.split(':').map(Number);
            
            const shiftStart = new Date(currentDate);
            shiftStart.setHours(startHour, startMin, startSec || 0, 0);
            
            const shiftEnd = new Date(currentDate);
            shiftEnd.setHours(endHour, endMin, endSec || 0, 0);
            
            // Only add if within chart range
            if (shiftEnd >= chartStartTime && shiftStart <= chartEndTime) {
                blocks.push({
                    start: shiftStart.toISOString().slice(0, 16).replace('T', ' '),
                    end: shiftEnd.toISOString().slice(0, 16).replace('T', ' ')
                });
            }
        });
        
        // Move to next day
        currentDate.setDate(currentDate.getDate() + 1);
        currentDate.setHours(0, 0, 0, 0);
    }
    
    return blocks;
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
        
        // Add operational window bars as background elements
        const operationalBlocks = getOperationalWindowBlocks(config);
        const windowBars = operationalBlocks.map((block, idx) => ({
            myStart: block.start,
            myEnd: block.end,
            ganttBarConfig: {
                id: `${config.name}-window-${idx}`,
                label: '',
                hasHandles: false,
                immobile: true,
                style: {
                    background: 'rgba(76, 175, 80, 0.15)',
                    border: '1px dashed rgba(76, 175, 80, 0.4)',
                    borderRadius: '4px',
                    zIndex: 0,
                    pointerEvents: 'none'
                }
            }
        }));
        
        // Add actual operation bars
        const operationBars = items.map((item) => ({
            myStart: new Date(item.startTime).toISOString().slice(0, 16).replace('T', ' '),
            myEnd: new Date(item.endTime).toISOString().slice(0, 16).replace('T', ' '),
            ganttBarConfig: {
                id: item.id,
                label: item.name,
                hasHandles: props.enableDrag,
                immobile: !props.enableDrag,
                style: {
                    background: item.color || getColorForRow(config.name, rowIndex),
                    borderRadius: '8px',
                    zIndex: 10
                },
                originalItem: item
            }
        }));

        return {
            label: config.name,
            bars: [...windowBars, ...operationBars] // Windows first (background), then operations
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

    const totalSpanMs = maxTime.getTime() - minTime.getTime();
    
    const paddingMs = Math.max(
        30 * 60 * 1000,
        Math.min(
            4 * 60 * 60 * 1000,
            totalSpanMs * 0.2
        )
    );

    const paddedStart = new Date(minTime.getTime() - paddingMs);
    const paddedEnd = new Date(maxTime.getTime() + paddingMs);

    chartStart.value = paddedStart.toISOString().slice(0, 16).replace('T', ' ');
    chartEnd.value = paddedEnd.toISOString().slice(0, 16).replace('T', ' ');
};

watch(() => [props.items, props.rowConfigs, chartStart.value, chartEnd.value], () => {
    generateRows();
}, { deep: true });

watch(() => props.items, (newItems, oldItems) => {
    if (!oldItems || newItems.length !== oldItems.length || 
        newItems.some((item, i) => item.id !== oldItems[i]?.id)) {
        if (newItems && newItems.length > 0 && !props.initialStart && !props.initialEnd) {
            autoFitTimeRange();
        }
    }
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
    isDragging = true;
    const originalItem = event.bar.ganttBarConfig.originalItem as GanttItem;
    
    if (!originalItem) {
        isDragging = false;
        return;
    }

    let newStartTime = new Date(event.bar.myStart.replace(' ', 'T'));
    let newEndTime = new Date(event.bar.myEnd.replace(' ', 'T'));
    
    // Apply constraints if defined
    if (originalItem.minTime || originalItem.maxTime) {
        const minTime = originalItem.minTime ? new Date(originalItem.minTime) : null;
        const maxTime = originalItem.maxTime ? new Date(originalItem.maxTime) : null;
        
        // Constrain start time
        if (minTime && newStartTime < minTime) {
            const duration = newEndTime.getTime() - newStartTime.getTime();
            newStartTime = new Date(minTime);
            newEndTime = new Date(newStartTime.getTime() + duration);
        }
        
        // Constrain end time
        if (maxTime && newEndTime > maxTime) {
            const duration = newEndTime.getTime() - newStartTime.getTime();
            newEndTime = new Date(maxTime);
            newStartTime = new Date(newEndTime.getTime() - duration);
        }
        
        // Double-check start time after end adjustment
        if (minTime && newStartTime < minTime) {
            newStartTime = new Date(minTime);
        }
        
        // Ensure end is after start
        if (newEndTime <= newStartTime) {
            newEndTime = new Date(newStartTime.getTime() + 60000); // Minimum 1 minute
        }
        
        // Final validation: ensure we're within bounds
        if (maxTime && newEndTime > maxTime) {
            newEndTime = new Date(maxTime);
        }
    }

    const updatedItem: GanttItem = {
        ...originalItem,
        startTime: newStartTime.toISOString(),
        endTime: newEndTime.toISOString()
    };

    if (props.onUpdate) {
        props.onUpdate(updatedItem);
    }

    emit('itemUpdated', updatedItem);
    
    setTimeout(() => {
        isDragging = false;
    }, 100);
};

const handleBarClick = (value: {
    bar: GanttBarObject;
    e: MouseEvent;
    datetime?: string | Date | undefined;
}) => {
    if (isDragging) {
        return;
    }
    
    // Don't trigger clicks on operational window bars
    if (value.bar.ganttBarConfig.id.includes('-window-')) {
        return;
    }
    
    props.onBarClick(value);
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
            :current-time="showCurrentTime"
            :push-on-overlap="pushOnOverlap"
            :no-overlap="noOverlap"
            @dragend-bar="onBarDragEnd"
            @click-bar="handleBarClick"
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