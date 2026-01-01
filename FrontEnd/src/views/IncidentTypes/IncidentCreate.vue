<script setup lang="ts">
import { ref } from 'vue';
import type { IIncidentService } from '@/service/IService/IIncidentService';
import type { IIncidentTypeService } from '@/service/IService/IIncidentTypeService';
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import { useI18n } from 'vue-i18n';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import ObjectSelector from '@/components/crud/ObjectSelector.vue';
import type { Page } from '@/model/Page';
import { useAlerts } from '@/composables/alerts';
import type { IncidentTypeDto } from '@/model/dto/IncidentTypeDto';
import type { CreateIncidentDto } from '@/model/dto/IncidentDto';

const { t } = useI18n();
const notifications = useAlerts();

const incident = ref({
    type: null,
    startTime: null,
    severity: null,
    description: ""
});

const incidentService = container.get<IIncidentService>(TYPES.incidentService);
const incidentTypeService = container.get<IIncidentTypeService>(TYPES.incidentTypeService);

const submitIncident = async () => {

    if (new Date(incident.value.startTime) > new Date()) {
        throw new Error("Incident cannot start in the future");
    }

    try {
        let dto: CreateIncidentDto = {
            type: incident.value.type.bid,
            startTime: incident.value.startTime,
            severity: incident.value.severity.id,
            description: incident.value.description
        };
        console.log(dto);
        return await incidentService.createIncident(dto);
    } catch (error: any) {
        let message = error?.response?.data?.message || error?.response?.data || error?.message || 'Failed to create incident';
        if (typeof message === 'object' && message !== null) {
            message = message.errors?.[0]?.error || JSON.stringify(message);
        }
        notifications.enqueueNotification(
            String(message),
            notifications.notificationTypes.DANGER
        );
        throw error;
    }
};

const severityOptions = [
    { id: 'Minor', name: t('incident.severity.Minor') },
    { id: 'Major', name: t('incident.severity.Major') },
    { id: 'Critical', name: t('incident.severity.Critical') }
];

const fetchIncidentTypes = async (): Promise<Page<IncidentTypeDto>> => {
    return await incidentTypeService.getAllIncidentTypes();
}

</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item>
                <RouterLink to="/incident-types/dashboard" class="breadcrumb-link">{{ t('incident.tabs.dashboard') }}</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ t('incident.tabs.create') }}</sl-breadcrumb-item>
        </sl-breadcrumb>
        <h1 class="title">{{ t('incident.tabs.create') }}</h1>
        <p class="subtitle">{{ t('incident.subtitle.create') }}</p>

        <EntityForm :object="incident" :submit-function="submitIncident">
            <div class="form">
                <div class="general-info">
                    <p class="section-title">{{ t('incident.generalFields') }}</p>
                    
                    <ObjectSelector class="field" :name="t('incident.fields.type.title') + '*'"
                        v-model="incident.type" :fetch-function="fetchIncidentTypes"
                        :placeholderText="t('incident.fields.type.placeholder')" labelKey="name"
                        valueKey="bid" required />

                    <FormField class="field" inputId="incident-start-time" type="datetime-local"
                        :name="t('incident.fields.startTime.title') + '*'" v-model="incident.startTime"
                        :placeholderText="t('incident.fields.startTime.placeholder')" required />

                    <FormField class="field" inputId="incident-description" :type="'textarea'"
                        :name="t('incident.fields.description.title') + '*'" v-model="incident.description"
                        :placeholderText="t('incident.fields.description.placeholder')" required />
                </div>

                <span class="section-divider"></span>

                <div class="classification">
                    <p class="section-title">{{ t('incident.classification') }}</p>
                    <div class="fields-dropdown">
                        <ObjectSelector class="field-dropdown" :name="t('incident.fields.severity.title') + '*'"
                            v-model="incident.severity"
                            :fetch-function="async () => ({ items: severityOptions, totalItems: 3, currentPage: 1, totalPages: 1 })"
                            :placeholderText="t('incident.fields.severity.placeholder')" labelKey="name"
                            valueKey="id" required />
                    </div>
                </div>
            </div>
        </EntityForm>
    </div>
</template>

<style scoped>
.form {
    display: flex;
    flex-direction: row;
    flex-wrap: wrap;
}

.field {
    margin-bottom: 1rem;
    width: 30rem;
    padding: 0.5rem;
}

.fields-dropdown {
    display: flex;
    flex-direction: row;
}

.field-dropdown {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    margin-bottom: 1rem;
    margin-top: 0.5rem;
    margin-left: 0.5rem;
    max-width: 30rem;
}

.section-title {
    font-size: 0.8rem;
    margin-bottom: 1rem;
    color: var(--sl-color-neutral-400);
}

.section-divider {
    display: block;
    width: 100%;
    height: 1px;
    background-color: var(--sl-color-neutral-200);
    margin: 1.5rem 0;
}
</style>