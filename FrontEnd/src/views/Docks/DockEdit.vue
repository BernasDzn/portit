<script setup lang="ts">
import EntityDropdown from '@/components/crud/EntityDropdown.vue';
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import { useAlerts } from '@/composables/alerts';
import { Dock } from '@/model/Dock';
import { ref, onMounted } from 'vue';
import { useRoute, RouterLink } from 'vue-router';
import {useI18n} from 'vue-i18n';
import type { IDockService } from '@/service/IService/IDockService';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { IVesselTypeService } from '@/service/IService/IVesselTypeService';
import type { DockDto } from '@/model/dto/DockDto';
import ObjectSelector from '@/components/crud/ObjectSelector.vue';

const {t} = useI18n();

const dockService = container.get<IDockService>(TYPES.dockService);
const vesselTypeService = container.get<IVesselTypeService>(TYPES.vesselTypeService);

const notifications = useAlerts();

const route = useRoute();
const dockCode = String(route.params.code || '');

let dock = ref({
    code: '',
    name: '',
    location: '',
    physicalCharacteristics: {
        length: null!,
        depth: null!,
        draft: null!
    },
    supportedVesselTypes: []
});

onMounted(async () => {

    try {
        const data = await dockService.getDockByCode(dockCode);
        if (!data) return;
        dock.value = data;
        
    } catch (err) {
        console.error('Failed to load dock', err);
    }
});

const updateDock = async (obj: any) => {
    if (!dockCode) {
        notifications.enqueueNotification(
            'Cannot update docks at this time.',
            notifications.notificationTypes.DANGER
        );
        return;
    }

    return dockService.updateDock(new Dock(obj));
};
</script>

<template>
    <div class="dock-edit">
        <sl-breadcrumb>
            <sl-breadcrumb-item>
                <RouterLink to="/docks/dashboard" class="link">{{ t('dock.tabs.dashboard') }}</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                <RouterLink to="/docks/search" class="link">{{ t('dock.tabs.search') }}</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                <RouterLink :to="dock.code ? `/docks/view/${dock.code}` : '/docks/search'" class="link">
                    {{ dock.code || t('dock.fields.code.placeholder') }}
                </RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ t('dock.tabs.edit') }}</sl-breadcrumb-item>
        </sl-breadcrumb>

        <h1 class="title">{{ t('dock.tabs.edit') }}</h1>
        <p class="subtitle">{{ t('dock.subtitle.edit') }}</p>
        <EntityForm :object="dock" editing-id="dockCode" :submit-function="updateDock">
            <div class="form">
                <div class="general-info">
                    <p class="section-title">{{ t('dock.infoTitle') }}</p>
                    <FormField class="field" inputId="dock-code" :name="t('dock.fields.code.title')" v-model="dock.code" :enabled="false"/>
                    <FormField class="field" inputId="dock-name" :name="t('dock.fields.name.title') + '*'" v-model="dock.name" :placeholderText="t('dock.fields.name.placeholder')" required/>
                    <FormField class="field" inputId="dock-location" :name="t('dock.fields.location.title') + '*'" v-model="dock.location" :placeholderText="t('dock.fields.location.placeholder')" required/>
                </div>
                <span class="section-divider"></span>
                
                <div class="measurements">
                    <p class="section-title">{{ t('physicalCharacteristics.title') }}</p>
                    <FormField class="field" inputId="dock-length" :name="t('physicalCharacteristics.length.title') + '*'" v-model.number="dock.physicalCharacteristics.length" :placeholderText="t('physicalCharacteristics.length.placeholder')" pattern="^\d+(\.\d{1,2})?$" required/>
                    <FormField class="field" inputId="dock-depth" :name="t('physicalCharacteristics.depth.title') + '*'" v-model.number="dock.physicalCharacteristics.depth" :placeholderText="t('physicalCharacteristics.depth.placeholder')" pattern="^\d+(\.\d{1,2})?$" required/>
                    <FormField class="field" inputId="dock-draft" :name="t('physicalCharacteristics.draft.title') + '*'" v-model.number="dock.physicalCharacteristics.draft" :placeholderText="t('physicalCharacteristics.draft.placeholder')" pattern="^\d+(\.\d{1,2})?$" required/>
                </div>

                <span class="section-divider"></span>
                <div>
                <p class="section-title">{{ t('dock.fields.supportedVesselTypes.title') }}</p>
                <!-- <EntityDropdown
                    class="field-dropdown"
                    :name="t('dock.fields.supportedVesselTypes.vesselTypes.title') + '*'"
                    v-model=dock.supportedVesselTypes
                    :fetch-function="() => vesselTypeService.getVesselTypes()"
                    :fetch-on-mount="true"
                    :placeholderText="t('dock.fields.supportedVesselTypes.placeholder')"
                    valueKey="name"
                    labelKey="name"
                    inputId="dock-vessel-types"
                    required
                    multiple
                /> -->
                <ObjectSelector
                    class="field-dropdown"
                    :name="t('dock.fields.supportedVesselTypes.vesselTypes.title') + '*'"
                    v-model="dock.supportedVesselTypes"
                    :fetch-function="() => vesselTypeService.getVesselTypes()"
                    :fetch-on-mount="true"
                    :placeholderText="t('dock.fields.supportedVesselTypes.vesselTypes.placeholder')"
                    labelKey="name"
                    required
                    multiple
                />
                </div>
            </div>
        </EntityForm>
    </div>
</template>

<style scoped>

.form{
    display: flex;
    flex-direction: row;
}

.general-info {
    display: flex;
    flex-direction: column;
    width: 20%;
}

.measurements {
    display: flex;
    flex-direction: column;
    width: 15%;
}

.field {
    margin-bottom: 1rem;
    max-width: 30rem;
    padding: .5rem
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

.section-divider {
    width: 1px;
    margin: 0 1rem;
    background-color: var(--sl-color-neutral-200);
}

.section-title {
    font-size: 0.8rem;
    margin-bottom: 1rem;
    color: var(--sl-color-neutral-400);
}

</style>