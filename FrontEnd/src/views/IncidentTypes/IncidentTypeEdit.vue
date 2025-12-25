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
import { useI18n } from 'vue-i18n';
import type { Filter, Page } from '@/model/Page';
import type { IncidentTypeDto, IncidentTypeFilter, PartialIncidentTypeDto } from '@/model/dto/IncidentTypeDto';

const { t } = useI18n();
const incidentTypeService = container.get<IIncidentTypeService>(TYPES.incidentTypeService);
const notifications = useAlerts();

const route = useRoute();
const incidentTypeId = String(route.params.id || '');

const incidentType = ref({
    name: '',
    description: '',
    severity: null as any,
    subtypeOf: null as string | null,
});

const loading = ref(true);

const fetchIncidentTypes = async (filtering?: Filter<IncidentTypeFilter>): Promise<Page<IncidentTypeDto>> => {
    return await incidentTypeService.getAllIncidentTypes(filtering);
}

onMounted(async () => {
    loading.value = true;

    try {
        const data = await incidentTypeService.getIncidentTypeById(incidentTypeId);
        if (!data) return;
                
        // Map severity string to the corresponding option object so ObjectSelector
        // (which expects an object modelValue) can pre-select it on mount.
        const matchedSeverity = severityOptions.find(s => s.id === data.severity) || null;
        incidentType.value = {
            name: data.name,
            description: data.description,
            severity: matchedSeverity,
            subtypeOf: data.subtypeOf || null,
        };

        console.log('Loaded incident type:', incidentType.value);
        
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

    // ObjectSelector binds the whole object; send its id to the API.
    const severityValue = incidentType.value.severity ? (incidentType.value.severity.id) : undefined;

    const dto: PartialIncidentTypeDto = {
        name: incidentType.value.name,
        description: incidentType.value.description,
        severity: severityValue,
        subtypeOf: incidentType.value.subtypeOf || undefined
    };

    return incidentTypeService.updateIncidentType(incidentTypeId, dto);
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
                <RouterLink :to="incidentTypeId ? `/incident-types/view/${incidentTypeId}` : '/incident-types/search'" class="link">
                    {{ incidentTypeId || t('incidentType.title') }}
                </RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ t('buttons.edit') }}</sl-breadcrumb-item>
        </sl-breadcrumb>

        <h1 class="title">{{ t('incidentType.tabs.edit') }}</h1>
        <p class="subtitle">{{ t('incidentType.subtitle.edit') }}</p>

        <Loading v-if="loading" />
        <EntityForm :object="incidentType" :editing-id="incidentTypeId" :submit-function="updateIncidentType" v-else>
            <div class="form">
                <div class="general-info">
                    <p class="section-title">{{ t('incidentType.generalFields') }}</p>
                    <FormField class="field" inputId="incident-type-name"
                        :name="t('incidentType.fields.name.title') + '*'" 
                        v-model="incidentType.name" required
                        :placeholderText="t('incidentType.fields.name.placeholder')" 
                    />
                    <FormField class="field" inputId="incident-type-description" :type="'textarea'"
                        :name="t('incidentType.fields.description.title') + '*'" v-model="incidentType.description"
                        :placeholderText="t('incidentType.fields.description.placeholder')" required 
                    />
                </div>

                <span class="section-divider"></span>

                <div class="classification">
                    <p class="section-title">{{ t('incidentType.classification') }}</p>
                    <div class="fields-dropdown">
                        <ObjectSelector class="field-dropdown" :name="t('incidentType.fields.severity.title') + '*'"
                            v-model="incidentType.severity"
                            :fetch-function="async () => ({ items: severityOptions, totalItems: 3, currentPage: 1, totalPages: 1 })"
                            :placeholderText="t('incidentType.fields.severity.placeholder')" labelKey="name" valueKey="id" required
                            :fetch-on-mount="true" 
                        />

                        <FormField class="field" inputId="incident-type-subtype-of"
                            :name="t('incidentType.fields.subtypeOf.title')" v-model="incidentType.subtypeOf"
                            :enabled="false" />

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
