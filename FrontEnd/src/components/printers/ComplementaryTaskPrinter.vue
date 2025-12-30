<script setup lang="ts">
import { RouterLink } from 'vue-router';
import type { ComplementaryTaskDto } from '@/model/dto/VesselVisitExecutionDto';

const props = withDefaults(defineProps<{
    task: ComplementaryTaskDto;
    link?: string;
    showDetails?: boolean;
}>(), {
    showDetails: true
});

const formatDate = (date?: Date | string) => {
    if (!date) return '';
    const d = new Date(date);
    if (isNaN(d.getTime())) return '';
    return d.toLocaleString();
};

const getStatusVariant = (status: string) => {
    switch (status) {
        case 'Completed': return 'success';
        case 'Started': return 'primary';
        case 'Delayed': return 'warning';
        case 'Pending': return 'neutral';
        default: return 'neutral';
    }
};

</script>

<template>
    <component :is="props.link ? RouterLink : 'div'" :to="props.link">
        <sl-card class="listing-item">
            <div class="opposed">
                <div>
                    <div class="task-header">
                        <sl-badge :variant="getStatusVariant(task.status)">
                            {{ task.status }}
                        </sl-badge>
                        <span class="vve-code">{{ task.vveCode }}</span>
                    </div>
                    
                    <p class="task-type">
                        <strong>{{ task.operation?.type?.name || 'Unknown Task' }}</strong>
                        <sl-badge variant="neutral" size="small">{{ task.operation?.type?.category || 'N/A' }}</sl-badge>
                    </p>
                    
                    <p class="item-description">
                        {{ formatDate(task.operation?.startTime) }} → {{ formatDate(task.operation?.endTime) }}
                    </p>
                </div>
                <span class="material-icons icon" style="color: #485ea9;" aria-hidden="true">library_add</span>
            </div>
            <div class="details" v-if="props.showDetails">
                <sl-divider></sl-divider>
                <div class="detail-row" v-if="task.operation?.resources && task.operation.resources.length > 0">
                    <sl-icon name="people"></sl-icon>
                    <span>{{ task.operation.resources.map((r: any) => r.name).join(', ') }}</span>
                </div>
                <div class="detail-row" v-if="task.impactedOperations.length > 0">
                    <sl-icon name="exclamation-triangle"></sl-icon>
                    <span>Impacts {{ task.impactedOperations.length }} operation(s)</span>
                </div>
            </div>
        </sl-card>
    </component>
</template>

<style scoped>
.listing-item {
    cursor: pointer;
    transition: transform 0.2s;
}

.listing-item:hover {
    transform: translateY(-2px);
}

.opposed {
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
    gap: 1rem;
}

.opposed > div {
    flex: 1;
}

.task-header {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    margin-bottom: 0.5rem;
}

.vve-code {
    font-size: 0.875rem;
    color: var(--sl-color-neutral-600);
    font-weight: 500;
}

.task-type {
    margin: 0.5rem 0;
    display: flex;
    align-items: center;
    gap: 0.5rem;
}

.icon {
    font-size: 2.5rem;
    color: var(--sl-color-warning-600);
}

.item-description {
    color: var(--sl-color-neutral-600);
    font-size: 0.9rem;
    margin-top: 0.5rem;
}

.details {
    margin-top: 1rem;
    color: var(--sl-color-neutral-700);
    font-size: 0.9rem;
}

.detail-row {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    margin-top: 0.5rem;
}

.detail-row sl-icon {
    font-size: 1rem;
    color: var(--sl-color-neutral-500);
}
</style>
