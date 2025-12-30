<script setup lang="ts">
import { RouterLink } from 'vue-router';
import { useI18n } from 'vue-i18n';
import { computed } from 'vue';
import type { IncidentDto } from '@/model/dto/IncidentDto';

const { t } = useI18n();

const props = withDefaults(defineProps<{
    incident: IncidentDto;
    link?: string;
    showDetails?: boolean;
}>(), {
    showDetails: true
});

const getSeverityVariant = (severity: string): string => {
    switch (severity) {
        case 'Critical': return 'danger';
        case 'Major': return 'warning';
        case 'Minor': return 'primary';
        default: return 'neutral';
    }
};

const isActive = computed(() => !props.incident.endTime);

const formattedStartTime = computed(() => 
    new Date(props.incident.startTime).toLocaleDateString()
);

const formattedEndTime = computed(() => 
    props.incident.endTime 
        ? new Date(props.incident.endTime).toLocaleDateString() 
        : t('incident.ongoing')
);

const duration = computed(() => {
    const start = new Date(props.incident.startTime);
    const end = props.incident.endTime ? new Date(props.incident.endTime) : new Date();
    const diffMs = end.getTime() - start.getTime();
    const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24));
    const diffHours = Math.floor((diffMs % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    
    if (diffDays > 0) {
        return `${diffDays}d ${diffHours}h`;
    }
    return `${diffHours}h`;
});
</script>

<template>
    <component :is="props.link ? RouterLink : 'div'" :to="props.link">
        <sl-card class="listing-item">
            <div class="opposed">
                <div>
                    <div class="header-row">
                        <p class="incident-bid">{{ incident.bid }}</p>
                        <sl-badge v-if="isActive" variant="success" pulse>
                            {{ t('incident.status.active') }}
                        </sl-badge>
                    </div>
                    <p class="item-description">
                        {{ incident.description }}
                    </p>
                    <div class="badges">
                        <sl-badge :variant="getSeverityVariant(incident.severity)">
                            {{ t(`incident.severity.${incident.severity}`) }}
                        </sl-badge>
                        <sl-badge variant="neutral">
                            {{ incident.type.name }}
                        </sl-badge>
                    </div>
                </div>
                <span class="material-icons icon" aria-hidden="true">warning</span>
            </div>
            <div class="details" v-if="props.showDetails">
                <sl-divider></sl-divider>
                <div class="details-grid">
                    <div class="detail-item">
                        <span class="detail-label">{{ t('incident.fields.startTime.title') }}:</span>
                        <span>{{ formattedStartTime }}</span>
                    </div>
                    <div class="detail-item">
                        <span class="detail-label">{{ t('incident.fields.endTime.title') }}:</span>
                        <span>{{ formattedEndTime }}</span>
                    </div>
                    <div class="detail-item">
                        <span class="detail-label">{{ t('incident.fields.duration.title') }}:</span>
                        <span>{{ duration }}</span>
                    </div>
                    <div class="detail-item">
                        <span class="detail-label">{{ t('incident.fields.createdBy.title') }}:</span>
                        <span>{{ incident.createdBy }}</span>
                    </div>
                    <div class="detail-item" v-if="incident.affectedVVECodes && incident.affectedVVECodes.length > 0">
                        <span class="detail-label">{{ t('incident.fields.affectedVVECodes.title') }}:</span>
                        <span>{{ incident.affectedVVECodes.join(', ') }}</span>
                    </div>
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

.icon {
    font-size: 2.5rem;
    color: var(--sl-color-primary-600);
}

.header-row {
    display: flex;
    align-items: center;
    gap: 0.75rem;
}

.incident-bid {
    font-weight: 600;
    font-size: 1.1rem;
    margin: 0;
}

.item-description {
    color: var(--sl-color-neutral-600);
    font-size: 0.9rem;
    margin-top: 0.5rem;
    margin-bottom: 0.5rem;
}

.badges {
    display: flex;
    gap: 0.5rem;
    margin-top: 0.75rem;
    flex-wrap: wrap;
}

.details {
    margin-top: 1rem;
    color: var(--sl-color-neutral-700);
    font-size: 0.9rem;
}

.details-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
    gap: 0.75rem;
    margin-top: 0.75rem;
}

.detail-item {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
}

.detail-label {
    font-weight: 600;
    color: var(--sl-color-neutral-700);
    font-size: 0.85rem;
}

@media (max-width: 768px) {
    .details-grid {
        grid-template-columns: 1fr;
    }
}
</style>