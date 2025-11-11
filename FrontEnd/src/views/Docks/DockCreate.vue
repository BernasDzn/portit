<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import type { IDockService } from '@/service/IService/IDockService';
import type { Dock } from '@/model/Dock';
import type { IVesselTypeService } from '@/service/IService/IVesselTypeService';
import EntityForm from '@/components/crud/EntityForm.vue';
import EntityDropdown from '@/components/crud/EntityDropdown.vue';
import FormField from '@/components/crud/FormField.vue';
import {useI18n} from 'vue-i18n';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';

const { t } = useI18n();

const dock = ref<Dock>({
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

const dockService = container.get<IDockService>(TYPES.dockService);
const vesselTypeService = container.get<IVesselTypeService>(TYPES.vesselTypeService);

const submitDock = (obj: any) => 
    dockService.createDock(obj);


</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item>
                <RouterLink to="/docks/dashboard" class="breadcrumb-link">{{ t('dock.tabs.dashboard') }}</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ t('dock.tabs.create') }}</sl-breadcrumb-item>
        </sl-breadcrumb>
        <h1 class="title">{{ t('dock.tabs.create') }}</h1>
        <p class="subtitle">{{ t('dock.subtitle.create') }}</p>
            <EntityForm :object="dock" :submit-function="submitDock">
                <div class="form">
                    <div class="general-info">
                        <p class="section-title">{{ t('dock.generalFields') }}</p>
                        <FormField class="field" :name="t('dock.fields.code.title') + '*'" v-model="dock.code" :placeholderText="t('dock.fields.code.placeholder')" pattern="^[a-zA-Z0-9]+$" required/>
                        <FormField class="field" :name="t('dock.fields.name.title') + '*'" v-model="dock.name" :placeholderText="t('dock.fields.name.placeholder')" required/>
                        <FormField class="field" :name="t('dock.fields.location.title') + '*'" v-model="dock.location" :placeholderText="t('dock.fields.location.placeholder')" required/>
                    </div>
                    <span class="section-divider"></span>
                    
                    <div class="measurements">
                        <p class="section-title">{{ t('physicalCharacteristics.title') }}</p>
                        <FormField class="field" :name="t('physicalCharacteristics.length.title') + '*'" v-model.number="dock.physicalCharacteristics.length" :placeholderText="t('physicalCharacteristics.length.placeholder')" pattern="^\d+(\.\d{1,2})?$" required/>
                        <FormField class="field" :name="t('physicalCharacteristics.depth.title') + '*'" v-model.number="dock.physicalCharacteristics.depth" :placeholderText="t('physicalCharacteristics.depth.placeholder')" pattern="^\d+(\.\d{1,2})?$" required/>
                        <FormField class="field" :name="t('physicalCharacteristics.draft.title') + '*'" v-model.number="dock.physicalCharacteristics.draft" :placeholderText="t('physicalCharacteristics.draft.placeholder')" pattern="^\d+(\.\d{1,2})?$" required/>
                    </div>

                    <span class="section-divider"></span>
                    <div>
                        <p class="section-title">{{ t('dock.fields.supportedVesselTypes.title') }}</p>
                        <EntityDropdown
                            class="field-dropdown"
                            :name="t('dock.fields.supportedVesselTypes.vesselTypes.title') + '*'"
                            v-model="dock.supportedVesselTypes"
                            :fetch-function="() => vesselTypeService.getVesselTypes()"
                            :fetch-on-mount="true"
                            :placeholderText="t('dock.fields.supportedVesselTypes.vesselTypes.placeholder')"
                            valueKey="name"
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
    flex-wrap: wrap;
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
    margin: 0 2rem;
    background-color: var(--sl-color-neutral-200);
}

.section-title {
    font-size: 0.8rem;
    margin-bottom: 1rem;
    color: var(--sl-color-neutral-400);
}

</style>