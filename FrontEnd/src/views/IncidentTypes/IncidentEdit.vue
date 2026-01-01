<script setup lang="ts">
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import { useAlerts } from '@/composables/alerts';
import { ref, onMounted } from 'vue';
import { useRoute, RouterLink } from 'vue-router';
import type { IIncidentService } from '@/service/IService/IIncidentService';
import type { IIncidentTypeService } from '@/service/IService/IIncidentTypeService';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import ObjectSelector from '@/components/crud/ObjectSelector.vue';
import Loading from '@/components/Loading.vue';
import { useI18n } from 'vue-i18n';
import type { Page } from '@/model/Page';
import type { IncidentTypeDto } from '@/model/dto/IncidentTypeDto';
import type { UpdateIncidentDto } from '@/model/dto/IncidentDto';
import { error } from 'three';
import type { IVesselVisitExecutionService } from '@/service/IService/IVesselExecutionService';
import type { VesselVisitExecutionDto } from '@/model/dto/VesselVisitExecutionDto';

const { t } = useI18n();
const incidentService = container.get<IIncidentService>(TYPES.incidentService);
const incidentTypeService = container.get<IIncidentTypeService>(TYPES.incidentTypeService);
const vveService = container.get<IVesselVisitExecutionService>(TYPES.vesselVisitExecutionService);

const route = useRoute();
const incidentId = String(route.params.id || '');

const incident = ref({
    type: null as any,
    startTime: '',
    endTime: '',
    severity: null as any,
    description: '',
    affectedVVECodes: [] as VesselVisitExecutionDto[],
});

const loading = ref(true);

const fetchIncidentTypes = async (): Promise<Page<IncidentTypeDto>> => {
    return await incidentTypeService.getAllIncidentTypes();
}

onMounted(async () => {
    loading.value = true;

    try {
        const data = await incidentService.getIncidentByBid(incidentId);
        if (!data) return;

        // Fetch incident type details
        const incidentType = await incidentTypeService.getIncidentTypeById(data.type.bid);
        
        // Map severity string to the corresponding option object
        const matchedSeverity = severityOptions.find(s => s.id === data.severity) || null;
        
        incident.value = {
            type: incidentType,
            startTime: new Date(data.startTime).toISOString().slice(0, 16),
            endTime: data.endTime ? new Date(data.endTime).toISOString().slice(0, 16) : '',
            severity: matchedSeverity,
            description: data.description,
            affectedVVECodes: data.affectedVVECodes || [],
        };

        console.log('Loaded incident:', incident.value.affectedVVECodes);
        
    } catch (err) {
        console.error('Failed to load incident', err);
    } finally {
        loading.value = false;
    }
});

const updateIncident = async () => {
    if (!incidentId) throw new Error('Cannot update incident at this time.');

    const severityValue = incident.value.severity ? incident.value.severity.id : undefined;
    const typeValue = incident.value.type ? incident.value.type.bid : undefined;

    console.log("type: " + typeValue);
    console.log("sev: " + severityValue);

    if (!typeValue || !severityValue) {
        throw new Error('Type and severity are required.');
    }

    const dto: UpdateIncidentDto = {
        type: typeValue,
        startTime: incident.value.startTime,
        endTime: undefined,
        severity: severityValue,
        description: incident.value.description,
        affectedVVECodes: incident.value.affectedVVECodes.length > 0 ? incident.value.affectedVVECodes.map(v => v.code) : [],
    };

    return incidentService.updateIncident(incidentId, dto);
};

const severityOptions = [
    { id: 'Minor', name: t('incident.severity.Minor') },
    { id: 'Major', name: t('incident.severity.Major') },
    { id: 'Critical', name: t('incident.severity.Critical') }
];

</script>

<template>
    <div class="incident-edit">
        <sl-breadcrumb>
            <sl-breadcrumb-item>
                <RouterLink to="/incident-types/dashboard" class="link">{{ t('incident.tabs.dashboard') }}</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                <RouterLink to="/incident/search" class="link">{{ t('incident.tabs.search') }}</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                <RouterLink :to="incidentId ? `/incidents/view/${incidentId}` : '/incidents/search'" class="link">
                    {{ incidentId || t('incident.title') }}
                </RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ t('buttons.edit') }}</sl-breadcrumb-item>
        </sl-breadcrumb>

        <h1 class="title">{{ t('incident.tabs.edit') }}</h1>
        <p class="subtitle">{{ t('incident.subtitle.edit') }}</p>

        <Loading v-if="loading" />
        <EntityForm :object="incident" :editing-id="incidentId" :submit-function="updateIncident" v-else>
            <div class="form">
                <div class="general-info">
                    <p class="section-title">{{ t('incident.generalFields') }}</p>
                    
                    <ObjectSelector class="field" :name="t('incident.fields.type.title') + '*'"
                        v-model="incident.type" :fetch-function="fetchIncidentTypes"
                        :placeholderText="t('incident.fields.type.placeholder')" labelKey="name"
                        valueKey="id" required :fetch-on-mount="true" />

                    <FormField class="field" inputId="incident-start-time" type="datetime-local"
                        :name="t('incident.fields.startTime.title') + '*'" 
                        v-model="incident.startTime" required
                        :placeholderText="t('incident.fields.startTime.placeholder')" 
                    />

                    <FormField class="field" inputId="incident-description" :type="'textarea'"
                        :name="t('incident.fields.description.title') + '*'" v-model="incident.description"
                        :placeholderText="t('incident.fields.description.placeholder')" required 
                    />
                </div>

                <span class="section-divider"></span>

                <div class="classification">
                    <div class="fields-dropdown">
                        <ObjectSelector class="field-dropdown" :name="t('incident.fields.severity.title') + '*'"
                            v-model="incident.severity"
                            :fetch-function="async () => ({ items: severityOptions, totalItems: 3, currentPage: 1, totalPages: 1 })"
                            :placeholderText="t('incident.fields.severity.placeholder')" labelKey="name" valueKey="id" required
                            :fetch-on-mount="true" 
                        />
                    </div>
                </div>

                <ObjectSelector
                    class="field-dropdown"
                    name="Affected VVEs*"
                    v-model="incident.affectedVVECodes"
                    :fetch-function="() => vveService.getAllVesselVisitExecutions()"
                    :fetch-on-mount="true"
                    placeholderText="VVEs"
                    labelKey="code"
                    multiple
                    :super-secret-option="true"
                />

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
    flex-wrap: wrap;
    gap: 1rem;
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