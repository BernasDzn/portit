<script setup lang="ts">
import { computed } from 'vue';
import { RouterLink } from 'vue-router';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();

interface Schedule {
    cranes: string[];
    loadingEnterTime: Date | string;
    loadingLeaveTime: Date | string;
    vvnId: string;
}

interface DockPlan {
    dockId: string;
    schedule: Schedule[];
}

interface Metric {
    algorithm: 'optimal' | 'greedy' | 'genetic' | 'auto';
    computationTime: number;
    strategy: string;
    totalDelay: number;
    vesselCount: number;
    selection?: {
        auto: boolean;
        reason: string;
    };
}

interface OperationPlan {
    date: Date | string;
    dockPlanMap: DockPlan[];
    metrics: Metric[];
}

const props = defineProps<{
    operationPlan: OperationPlan;
    link?: string;
    short?: boolean;
}>();

const formattedDate = computed(() => {
    if (typeof props.operationPlan.date === 'string') {
        return props.operationPlan.date.split('T')[0];
    }
    return new Date(props.operationPlan.date).toISOString().split('T')[0];
});

const totalSchedules = computed(() => {
    return props.operationPlan.dockPlanMap.reduce((total, dock) => total + dock.schedule.length, 0);
});

const primaryMetric = computed(() => {
    return props.operationPlan.metrics && props.operationPlan.metrics.length > 0
        ? props.operationPlan.metrics[0]
        : null;
});
</script>

<template>
    <component :is="props.link ? RouterLink : 'div'" :to="props.link">
        <sl-card :class="'listing-item ' + (props.short ? 'short-card' :'')">
            <div class="operation-plan-display">
                <div class="operation-plan-header">
                    <div class="operation-plan-title">
                        <p>{{ t('operationPlan.title') }}</p>
                        <sl-tag size="small" variant="primary" v-if="primaryMetric">
                            {{ t(`operationPlan.algorithm.${primaryMetric.algorithm}`) }}
                        </sl-tag>
                    </div>
                    <div v-if="!props.short" class="date item-description">
                        {{ formattedDate }}
                    </div>
                </div>
                <div class="opposed">
                    <div>
                        <div v-if="props.short">
                            <p class="subtitle short">{{ formattedDate }}</p>
                        </div>
                        <p v-if="!props.short" class="item-description">
                            {{ t('operationPlan.docks') }}: {{ props.operationPlan.dockPlanMap.length }}<br/>
                            {{ t('operationPlan.totalSchedules') }}: {{ totalSchedules }}<br/>
                            {{ t('operationPlan.vesselCount') }}: {{ primaryMetric?.vesselCount || t('common.unknown') }}
                        </p>
                    </div>
                    <span v-if="!props.short" class="material-icons icon" aria-hidden="true">event_note</span>
                </div>
            </div>
            <div class="details" v-if="!props.short">
                <sl-divider></sl-divider>
                <div class="metrics-section" v-if="primaryMetric">
                    <p class="metrics-title">{{ t('operationPlan.metrics.title') }}</p>
                    <sl-alert v-if="primaryMetric.selection?.auto" variant="primary" open class="auto-selection-info">
                        <sl-icon slot="icon" name="robot"></sl-icon>
                        <strong>{{ t('operationPlan.metrics.autoSelection') }}</strong><br/>
                        {{ primaryMetric.selection.reason }}
                    </sl-alert>
                    <div class="metrics-grid">
                        <div class="metric-item">
                            <span class="metric-label">{{ t('operationPlan.metrics.strategy') }}:</span>
                            <span class="metric-value">{{ primaryMetric.strategy }}</span>
                        </div>
                        <div class="metric-item">
                            <span class="metric-label">{{ t('operationPlan.metrics.totalDelay') }}:</span>
                            <span class="metric-value">{{ primaryMetric.totalDelay }}</span>
                        </div>
                        <div class="metric-item">
                            <span class="metric-label">{{ t('operationPlan.metrics.computationTime') }}:</span>
                            <span class="metric-value">{{ primaryMetric.computationTime }}ms</span>
                        </div>
                    </div>
                </div>
                <slot></slot>
            </div>
            <slot></slot>
        </sl-card>
    </component>
</template>

<style scoped>
.icon {
    font-size: 35px;
    color: var(--accent-1);
}

.date {
    display: flex;
    align-items: center;
    font-size: small;
    width: fit-content;
    gap: 2px;
}

.operation-plan-display {
    display: flex;
    flex-direction: column;
}

.operation-plan-header {
    display: flex;
    flex-direction: column;
}

.operation-plan-title {
    display: flex;
    flex-direction: row;
    justify-content: space-between;
    align-items: center;
}

.metrics-section {
    display: flex;
    flex-direction: column;
    gap: 10px;
}

.metrics-title {
    font-weight: 600;
    margin-bottom: 5px;
}

.metrics-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
    gap: 10px;
}

.metric-item {
    display: flex;
    flex-direction: column;
    gap: 2px;
}

.metric-label {
    font-size: small;
    color: var(--sl-color-neutral-500);
}

.metric-value {
    font-weight: 500;
}

.short {
    margin-top: -10px;
}

.list-badge::part(base) {
    border-radius: var(--sl-border-radius-medium);
    background-color: var(--sl-color-neutral-200);
    color: var(--sl-color-neutral-800);
}

.auto-selection-info {
    margin-bottom: 10px;
}

.auto-selection-info::part(base) {
    background-color: var(--sl-color-primary-50);
}
</style>