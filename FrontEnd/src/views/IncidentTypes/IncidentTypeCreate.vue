<script setup lang="ts">
import { ref } from 'vue';
import type { IIncidentTypeService } from '@/service/IService/IIncidentTypeService';
import { IncidentType, type IncidentTypeDto } from '@/model/IncidentType';
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import { useI18n } from 'vue-i18n';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import ObjectSelector from '@/components/crud/ObjectSelector.vue';
import type { Filter, Page } from '@/model/Page';
import type { IncidentTypeCreateDto } from '@/model/dto/IncidentTypeDto';

const { t } = useI18n();

const incidentType = ref({
    name: '',
    description: '',
    severity: null,
    subtypeOf: null,
    subtypes: [] as IncidentType[]
});

const incidentTypeService = container.get<IIncidentTypeService>(TYPES.incidentTypeService);

const submitIncidentType = () => {
    let dto : IncidentTypeCreateDto = {
        name: incidentType.value.name,
        description: incidentType.value.description,
        severity: incidentType.value.severity.id,
        subtypeOfId: incidentType.value.subtypeOf?.id || null,
        subtypesIds: incidentType.value.subtypes.map(subtype => subtype.id)
    };
    console.log(dto);
    return incidentTypeService.createIncidentType(dto);
};

const severityOptions = [
    { id: 'Minor', name: t('incidentType.severity.Minor') },
    { id: 'Major', name: t('incidentType.severity.Major') },
    { id: 'Critical', name: t('incidentType.severity.Critical') }
];

const fetchIncidentTypes = async (): Promise<Page<IncidentTypeDto>> => {
    return await incidentTypeService.getAllIncidentTypes();
}

</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item>
                <RouterLink to="/incident-types/dashboard" class="breadcrumb-link">{{ t('incidentType.tabs.dashboard') }}</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ t('incidentType.tabs.create') }}</sl-breadcrumb-item>
        </sl-breadcrumb>
        <h1 class="title">{{ t('incidentType.tabs.create') }}</h1>
        <p class="subtitle">{{ t('incidentType.subtitle.create') }}</p>

        <EntityForm :object="incidentType" :submit-function="submitIncidentType">
            <div class="form">
                <div class="general-info">
                    <p class="section-title">{{ t('incidentType.generalFields') }}</p>
                    <FormField class="field" inputId="incident-type-name"
                        :name="t('incidentType.fields.name.title') + '*'" v-model="incidentType.name"
                        :placeholderText="t('incidentType.fields.name.placeholder')" required />
                    <FormField class="field" inputId="incident-type-description" :type="'textarea'"
                        :name="t('incidentType.fields.description.title') + '*'" v-model="incidentType.description"
                        :placeholderText="t('incidentType.fields.description.placeholder')" required />
                </div>

                <span class="section-divider"></span>

                <div class="classification">
                    <p class="section-title">{{ t('incidentType.classification') }}</p>
                    <div class="fields-dropdown">
                        <ObjectSelector class="field-dropdown" :name="t('incidentType.fields.severity.title') + '*'"
                            v-model="incidentType.severity"
                            :fetch-function="async () => ({ items: severityOptions, totalItems: 3, currentPage: 1, totalPages: 1 })"
                            :placeholderText="t('incidentType.fields.severity.placeholder')" labelKey="name"
                            valueKey="id" required />

                        <ObjectSelector class="field-dropdown" :name="t('incidentType.fields.subtypeOf.title')"
                            v-model="incidentType.subtypeOf" :fetch-function="fetchIncidentTypes"
                            :placeholderText="t('incidentType.fields.subtypeOf.placeholder')" labelKey="name"
                            valueKey="id" />

                        <ObjectSelector class="field-dropdown" :name="t('incidentType.fields.subtypes.title')"
                            v-model="incidentType.subtypes" :fetch-function="fetchIncidentTypes"
                            :placeholderText="t('incidentType.fields.subtypes.placeholder')" labelKey="name"
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
