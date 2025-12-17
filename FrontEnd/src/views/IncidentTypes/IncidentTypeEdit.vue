<script setup lang="ts">
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import { useAlerts } from '@/composables/alerts';
import { IncidentType, type IncidentTypeDto } from '@/model/IncidentType';
import { ref, onMounted } from 'vue';
import { useRoute, RouterLink } from 'vue-router';
import type { IIncidentTypeService } from '@/service/IService/IIncidentTypeService';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import ObjectSelector from '@/components/crud/ObjectSelector.vue';
import Loading from '@/components/Loading.vue';
import { useI18n } from 'vue-i18n';
import type { Filter, Page } from '@/model/Page';

const { t } = useI18n();
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
    subtypes: [] as IncidentTypeDto[]
});

const loading = ref(true);

const fetchIncidentTypes = async (filtering?: Filter<IncidentTypeDto>): Promise<Page<IncidentTypeDto>> => {
    return await incidentTypeService.getAllIncidentTypes(filtering);
}

onMounted(async () => {
    loading.value = true;

    try {
        const data = await incidentTypeService.getIncidentTypeById(incidentTypeId);
        if (!data) return;
        
        // Fetch all incident types to get the options that will be in the selector
        const allTypes = await incidentTypeService.getAllIncidentTypes();
        
        // Find the subtypes from the fetched options by matching IDs
        const subtypes: IncidentTypeDto[] = [];
        if (data.subtypesIds && data.subtypesIds.length > 0) {
            for (const subtypeId of data.subtypesIds) {
                const matchedSubtype = allTypes.items.find(it => it.id === subtypeId);
                if (matchedSubtype) {
                    subtypes.push(matchedSubtype);
                }
            }
        }
        
        incidentType.value = {
            id: data.id,
            name: data.name,
            description: data.description,
            severity: { id: data.severity, name: data.severity },
            subtypeOf: data.subtypeOfId || null,
            subtypes: subtypes
        };
        
    } catch (err) {
        console.error('Failed to load incident type', err);
    } finally {
        loading.value = false;
    }
});

const notifyError = (msg: string) => {
  notifications.enqueueNotification(msg, notifications.notificationTypes.DANGER);
};

const updateIncidentType = async () => {
    if (!incidentTypeId) return notifyError('Cannot update incident type at this time.');

    const severityValue = incidentType.value.severity?.id || incidentType.value.severity;
    
    const it = new IncidentType({
        name: incidentType.value.name,
        description: incidentType.value.description,
        severity: severityValue,
        subtypeOfId: incidentType.value.subtypeOf,
        subtypesIds: incidentType.value.subtypes.map(subtype => subtype.id)
    });

    return incidentTypeService.updateIncidentType(incidentTypeId, it);
};

const severityOptions = [
    { id: 'Minor', name: t('incidentType.severity.Minor') },
    { id: 'Major', name: t('incidentType.severity.Major') },
    { id: 'Critical', name: t('incidentType.severity.Critical') }
];

</script>

<template>
    <div class="incident-type-edit">
        <sl-breadcrumb>
            <sl-breadcrumb-item>
                <RouterLink to="/incident-types/dashboard" class="link">{{ t('incidentType.tabs.dashboard') }}</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                <RouterLink to="/incident-types/search" class="link">{{ t('incidentType.tabs.search') }}</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                <RouterLink :to="incidentType.id ? `/incident-types/view/${incidentType.id}` : '/incident-types/search'" class="link">
                    {{ incidentType.id || t('incidentType.title') }}
                </RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ t('buttons.edit') }}</sl-breadcrumb-item>
        </sl-breadcrumb>

        <h1 class="title">{{ t('incidentType.tabs.edit') }}</h1>
        <p class="subtitle">{{ t('incidentType.subtitle.edit') }}</p>

        <Loading v-if="loading" />
        <EntityForm :object="incidentType" editing-id="incidentTypeId" :submit-function="updateIncidentType" v-else>
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

                        <FormField class="field" inputId="incident-type-subtype-of"
                            :name="t('incidentType.fields.subtypeOf.title')" v-model="incidentType.subtypeOf"
                            :enabled="false" />

                        <ObjectSelector class="field-dropdown" :name="t('incidentType.fields.subtypes.title')"
                            v-model="incidentType.subtypes" :fetch-function="fetchIncidentTypes"
                            :placeholderText="t('incidentType.fields.subtypes.placeholder')" labelKey="name"
                            valueKey="id" multiple :fetch-on-mount="true" />
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
