<script setup lang="ts">
import { ref } from 'vue';
import type { IIncidentTypeService } from '@/service/IService/IIncidentTypeService';
import { IncidentType } from '@/model/IncidentType';
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import { useI18n } from 'vue-i18n';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import ObjectSelector from '@/components/crud/ObjectSelector.vue';

const { t } = useI18n();

const incidentType = ref({
    name: '',
    description: '',
    severity: null,
    subtypeOf: null,
    subtypes: [] as string[]
});

const incidentTypeService = container.get<IIncidentTypeService>(TYPES.incidentTypeService);

const submitIncidentType = (obj: any) => {
    const it = new IncidentType({
        name: obj.name,
        description: obj.description,
        severity: obj.severity.id,
        subtypeOfId: obj.subtypeOf,
        subtypesIds: obj.subtypes
    }
    );
    return incidentTypeService.createIncidentType(it);
};

const severityOptions = [
    { id: 'Minor', name: 'Minor' },
    { id: 'Major', name: 'Major' },
    { id: 'Critical', name: 'Critical' }
];

const fetchIncidentTypes = async () => {
    const types = await incidentTypeService.getAllIncidentTypes();
    return {
        items: types,
        totalItems: types.length,
        currentPage: 1,
        totalPages: 1
    };
};

</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item>
                <RouterLink to="/incident-types/dashboard" class="breadcrumb-link">Incident Types Dashboard</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>Create Incident Type</sl-breadcrumb-item>
        </sl-breadcrumb>
        <h1 class="title">Create Incident Type</h1>
        <p class="subtitle">Register a new incident type into the system</p>

        <EntityForm :object="incidentType" :submit-function="submitIncidentType">
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

                        <ObjectSelector class="field-dropdown" name="Subtype Of"
                            v-model="incidentType.subtypeOf" :fetch-function="fetchIncidentTypes"
                            placeholderText="Select parent type (optional)" labelKey="name"
                            valueKey="id" />

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
