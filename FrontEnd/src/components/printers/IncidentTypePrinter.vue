<script setup lang="ts">
import { RouterLink } from 'vue-router';
import { useI18n } from 'vue-i18n';
import type { IncidentTypeDto } from '@/model/dto/IncidentTypeDto';

const { t } = useI18n();

const props = withDefaults(defineProps<{
    incidentType: IncidentTypeDto;
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
</script>

<template>
    <component :is="props.link ? RouterLink : 'div'" :to="props.link">
        <sl-card class="listing-item">
            <div class="opposed">
                <div>
                    <p>{{ incidentType.name }}</p>
                    <p class="item-description">
                        {{ incidentType.description }}
                    </p>
                    <sl-badge :variant="getSeverityVariant(incidentType.severity)" style="margin-top: 8px;">
                        {{ t(`incidentType.severity.${incidentType.severity}`) }}
                    </sl-badge>
                </div>
                <span class="material-icons icon" aria-hidden="true">nearby_error</span>
            </div>
            <div class="details" v-if="props.showDetails && incidentType.subtypes && incidentType.subtypes.length > 0">
                <sl-divider></sl-divider>
                <p>{{ t('incidentType.fields.subtypes.title') }}: {{ incidentType.subtypes.length }}</p>
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
</style>
