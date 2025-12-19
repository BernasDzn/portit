<script setup lang="ts">
import { RouterLink } from 'vue-router';
import type { VesselVisitExecution } from '@/model/VesselVisitExecution';
import ActivityTag from '@/components/ActivityTag.vue';
import { useI18n } from 'vue-i18n';
import { computed } from 'vue';

const { t } = useI18n();

const props = defineProps<{
    execution: VesselVisitExecution;
    link?: string;
}>();

const operationCounts = computed(() => {
    const counts = {
        pending: 0,
        inProgress: 0,
        completed: 0
    };
    
    props.execution.operationsExecuted.forEach(op => {
        if (op.status === 'Pending') counts.pending++;
        else if (op.status === 'InProgress') counts.inProgress++;
        else if (op.status === 'Completed') counts.completed++;
    });
    
    return counts;
});

const totalOperations = computed(() => props.execution.operationsExecuted.length);

const formatDate = (date?: Date) => {
    if (!date) return '-';
    return new Date(date).toLocaleString();
};
</script>

<template>
    <component :is="props.link ? RouterLink : 'div'" :to="props.link">
        <sl-card class="listing-item">
            <div class="opposed">
                <div>
                    <p class="code">{{ execution.code }}</p>
                    <p class="item-description">{{ t('execution.fields.relatedVVN') }}: {{ execution.relatedVVN }}</p>
                </div>
            </div>
            
            <div class="info-group">
                <span class="material-icons icon" aria-hidden="true">event</span>
                <div>
                    <span class="item-description">{{ t('execution.fields.dateOpen') }}: {{ formatDate(execution.dateOpen) }}</span>
                    <span v-if="execution.dateClosed" class="item-description">{{ t('execution.fields.dateClosed') }}: {{ formatDate(execution.dateClosed) }}</span>
                </div>
            </div>

            <div class="info-group">
                <span class="material-icons icon" aria-hidden="true">person</span>
                <span class="item-description">{{ t('execution.fields.createdBy') }}: {{ execution.createdBy }}</span>
            </div>

            <sl-divider></sl-divider>

            <div class="operations-section">
                <p>{{ t('execution.fields.operations') }}: {{ totalOperations }}</p>
                <div class="operations-stats">
                    <sl-badge variant="neutral" pill>
                        <span class="material-icons badge-icon">schedule</span>
                        {{ operationCounts.pending }} {{ t('execution.status.pending') }}
                    </sl-badge>
                    <sl-badge variant="primary" pill>
                        <span class="material-icons badge-icon">play_arrow</span>
                        {{ operationCounts.inProgress }} {{ t('execution.status.inProgress') }}
                    </sl-badge>
                    <sl-badge variant="success" pill>
                        <span class="material-icons badge-icon">check_circle</span>
                        {{ operationCounts.completed }} {{ t('execution.status.completed') }}
                    </sl-badge>
                </div>
            </div>

            <!-- For view details in the future -->
            <slot></slot>
        </sl-card>
    </component>
</template>

<style scoped>
.code {
    font-weight: 600;
    font-size: 1.1rem;
}

.icon {
    font-size: 20px;
    color: var(--accent-1);
}

.info-group {
    display: flex;
    align-items: center;
    gap: 8px;
    margin-top: 8px;
}

.info-group > div {
    display: flex;
    flex-direction: column;
    gap: 4px;
}

.operations-section {
    margin-top: 12px;
}

.operations-section p {
    margin-bottom: 8px;
    font-weight: 500;
}

.operations-stats {
    display: flex;
    gap: 8px;
    flex-wrap: wrap;
}

.operations-stats sl-badge {
    display: flex;
    align-items: center;
    gap: 4px;
}

.badge-icon {
    font-size: 16px;
}

</style>