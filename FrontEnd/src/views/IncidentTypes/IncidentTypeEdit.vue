<script setup lang="ts">
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import { useAlerts } from '@/composables/alerts';
import { IncidentType } from '@/model/IncidentType';
import { ref, onMounted } from 'vue';
import { useRoute, RouterLink } from 'vue-router';
import type { IIncidentTypeService } from '@/service/IService/IIncidentTypeService';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import ObjectSelector from '@/components/crud/ObjectSelector.vue';
import Loading from '@/components/Loading.vue';

const incidentTypeService = container.get<IIncidentTypeService>(TYPES.incidentTypeService);
const notifications = useAlerts();

const route = useRoute();
const incidentTypeId = String(route.params.id || '');

let incidentType = ref({
    id: '',
    name: '',
    description: '',
    severity: null as any,
    subtypeOf: null as string | null,
    subtypes: [] as string[]
});

const loading = ref(true);
onMounted(async () => {
    loading.value = true;

    try {
        const data = await incidentTypeService.getIncidentTypeById(incidentTypeId);
        if (!data) return;
        
        incidentType.value = {
            id: data.id,
            name: data.name,
            description: data.description,
            severity: { id: data.severity, name: data.severity },
            subtypeOf: data.subtypeOfId || null,
            subtypes: data.subtypesIds || []
        };
        
    } catch (err) {
        console.error('Failed to load incident type', err);
    } finally {
        loading.value = false;
    }
});

const updateIncidentType = async (obj: any) => {
    if (!incidentTypeId) {
        notifications.enqueueNotification(
            'Cannot update incident type at this time.',
            notifications.notificationTypes.DANGER
        );
        return;
    }

    const severityValue = obj.severity?.id || obj.severity;
    
    const it = new IncidentType({
        id: obj.id,
        name: obj.name,
        description: obj.description,
        severity: severityValue,
        subtypeOfId: obj.subtypeOf,
        subtypesIds: obj.subtypes
    });

    return incidentTypeService.updateIncidentType(incidentTypeId, it);
};

const severityOptions = [
    { id: 'Minor', name: 'Minor' },
    { id: 'Major', name: 'Major' },
    { id: 'Critical', name: 'Critical' }
];

const fetchIncidentTypes = async () => {
    const types = await incidentTypeService.getAllIncidentTypes();
    const filteredTypes = types.filter(t => 
        t.id !== incidentTypeId && 
        t.id !== incidentType.value.subtypeOf
    );
    return {
        items: filteredTypes,
        totalItems: filteredTypes.length,
        currentPage: 1,
        totalPages: 1
    };
};
</script>

<template>
    <div class="incident-type-edit">
        <sl-breadcrumb>
            <sl-breadcrumb-item>
                <RouterLink to="/incident-types/dashboard" class="link">Incident Types Dashboard</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                <RouterLink to="/incident-types/search" class="link">Search Incident Types</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                <RouterLink :to="incidentType.id ? `/incident-types/view/${incidentType.id}` : '/incident-types/search'" class="link">
                    {{ incidentType.name || 'Incident Type' }}
                </RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>Edit</sl-breadcrumb-item>
        </sl-breadcrumb>

        <h1 class="title">Edit Incident Type</h1>
        <p class="subtitle">Modify an existing incident type in the system</p>

        <Loading v-if="loading" />
        <EntityForm :object="incidentType" editing-id="incidentTypeId" :submit-function="updateIncidentType" v-else>
            <div class="form">
                <div class="general-info">
                    <p class="section-title">General fields</p>
                    <FormField class="field" inputId="incident-type-name"
                        name="Name*" v-model="incidentType.name"
                        placeholderText="Incident type name" required />
                    <FormField class="field" inputId="incident-type-description" :type="'textarea'"
                        name="Description*" v-model="incidentType.description"
                        placeholderText="Detailed description of the incident type" required />
                </div>

                <span class="section-divider"></span>

                <div class="classification">
                    <p class="section-title">Classification</p>
                    <div class="fields-dropdown">
                        <ObjectSelector class="field-dropdown" name="Severity*"
                            v-model="incidentType.severity"
                            :fetch-function="async () => ({ items: severityOptions, totalItems: 3, currentPage: 1, totalPages: 1 })"
                            placeholderText="Select severity level" labelKey="name"
                            valueKey="id" required />

                        <FormField class="field" inputId="incident-type-subtype-of"
                            name="Subtype Of" v-model="incidentType.subtypeOf"
                            :enabled="false" />

                        <ObjectSelector class="field-dropdown" name="Subtypes"
                            v-model="incidentType.subtypes" :fetch-function="fetchIncidentTypes"
                            placeholderText="Select child types (optional)" labelKey="name"
                            valueKey="id" multiple />
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
