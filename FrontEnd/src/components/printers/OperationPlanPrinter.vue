<script setup lang="ts">
import { computed } from 'vue';
import { RouterLink } from 'vue-router';
import { useI18n } from 'vue-i18n';
import type { OperationPlanDto } from '@/model/dto/OperationPlanDto';

const { t } = useI18n();

const props = defineProps<{
    operationPlan: OperationPlanDto;
    link?: string;
    short?: boolean;
}>();

const formattedCreatedDate = computed(() => {
    const date = new Date(props.operationPlan.metadata.createdAt);
    return date.toISOString().split('T')[0];
});

const operationTimes = computed(() => {
    if (props.operationPlan.operationSchedule.length === 0) return { start: '', end: '' };
    const start = new Date(props.operationPlan.operationSchedule[0].startTime);
    const end = new Date(props.operationPlan.operationSchedule[props.operationPlan.operationSchedule.length - 1].endTime);
    return {
        start: start.toLocaleString(),
        end: end.toLocaleString()
    };
});

const cranes = computed(() => {
    const craneSet = new Set<string>();
    props.operationPlan.operationSchedule.forEach(op => {
        op.resources.forEach(res => {
            if (res.type === 'Crane') craneSet.add(res.name);
        });
    });
    return Array.from(craneSet);
});

const categoryColorMap = {
    'LOAD': 'success',
    'UNLOAD': 'warning',
};

</script>

<template>
    <component :is="props.link ? RouterLink : 'div'" :to="props.link" class="plan-link">
        <sl-card :class="'listing-item ' + (props.short ? 'short-card' :'')">
            <div class="operation-plan-display">
                <div class="operation-plan-header">
                    <div class="operation-plan-title">
                        <p>{{ t('operationPlan.title') }} - {{ props.operationPlan.relatedVVN }}</p>
                        <sl-tag size="small" variant="primary">
                            {{ props.operationPlan.metadata.algorithmUsed }}
                        </sl-tag>
                    </div>
                    <div v-if="!props.short" class="date item-description">
                        {{ formattedCreatedDate }}
                    </div>
                </div>
                <div class="opposed">
                    <div>
                        <div v-if="props.short">
                            <p class="subtitle short">{{ formattedCreatedDate }}</p>
                        </div>
                        <p v-if="!props.short" class="item-description">
                            {{ t('operationPlan.dock') }}: {{ props.operationPlan.dock }}<br/>
                            {{ t('operationPlan.operations') }}: {{ props.operationPlan.operationSchedule.length }}<br/>
                            {{ t('operationPlan.cranes') }}: {{ cranes.join(', ') || t('common.none') }}
                        </p>
                    </div>
                    <span v-if="!props.short" class="material-icons icon" aria-hidden="true">event_note</span>
                </div>
            </div>
            <div class="details" v-if="!props.short">
                <sl-divider></sl-divider>
                <div class="operations-section">
                    <p class="metrics-title">{{ t('operationPlan.schedule') }}</p>
                    <div class="operations-list">
                        <div v-for="(operation, idx) in props.operationPlan.operationSchedule" :key="idx" class="operation-item">
                            <sl-badge :variant="categoryColorMap[operation.type.category.value]">
                                {{ operation.type.description }}
                            </sl-badge>
                            <span class="operation-time">
                                {{ new Date(operation.startTime).toLocaleString() }} -> {{ new Date(operation.endTime).toLocaleString() }}
                            </span>
                        </div>
                    </div>
                </div>
                <div class="metadata-section">
                    <p class="metrics-title">{{ t('operationPlan.metadata') }}</p>
                    <div class="metadata-grid">
                        <div class="metric-item">
                            <span class="metric-label">{{ t('operationPlan.createdBy') }}:</span>
                            <span class="metric-value">{{ props.operationPlan.metadata.createdBy }}</span>
                        </div>
                        <div class="metric-item">
                            <span class="metric-label">{{ t('operationPlan.createdAt') }}:</span>
                            <span class="metric-value">{{ new Date(props.operationPlan.metadata.createdAt).toLocaleString() }}</span>
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

.operations-section, .metadata-section {
    display: flex;
    flex-direction: column;
    gap: 10px;
    margin-bottom: 15px;
}

.metrics-title {
    font-weight: 600;
    margin-bottom: 5px;
}

.operations-list {
    display: flex;
    flex-direction: column;
    gap: 8px;
}

.operation-item {
    display: flex;
    align-items: center;
    gap: 10px;
    padding: 8px;
    background-color: var(--sl-color-neutral-50);
    border-radius: var(--sl-border-radius-small);
}

.operation-time {
    font-size: small;
    color: var(--sl-color-neutral-700);
}

.metadata-grid {
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

.plan-link {
    text-decoration: none;
    color: inherit;
}
</style>