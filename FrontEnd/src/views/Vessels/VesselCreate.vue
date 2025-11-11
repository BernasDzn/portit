<script setup lang="ts">
import EntityDropdown from '@/components/crud/EntityDropdown.vue';
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import type { Vessel } from '@/model/Vessel';
import { ref } from 'vue';
import { useI18n } from 'vue-i18n';
import { container } from '@/inversify.config';
import type { IVesselService } from '@/service/IService/IVesselService';
import TYPES from '@/inversify/types';
import type { IVesselTypeService } from '@/service/IService/IVesselTypeService';
import type { IShippingAgentOrganizationService } from '@/service/IService/IShippingAgentOrganizationService';

const vesselService = container.get<IVesselService>(TYPES.vesselService);
const vesselTypeService = container.get<IVesselTypeService>(TYPES.vesselTypeService);
const saoService = container.get<IShippingAgentOrganizationService>(TYPES.shippingAgentOrganizationService);

const vessel = ref<Vessel>({
    name: '',
    imoNumber: '',
    type: '',
    owner: '',
    length: null!,
    depth: null!,
    draft: null!
});

const { t } = useI18n();

const submitVessel = (obj: any) => 
    vesselService.createVessel(obj);
</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item><RouterLink to="/vessels/dashboard" class="breadcrumb-link">{{ t('vessel.tabs.dashboard') }}</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ t('vessel.tabs.create') }}</sl-breadcrumb-item>
        </sl-breadcrumb>

        <h1 class="title">{{ t('vessel.tabs.create') }}</h1>
        <p class="subtitle">{{ t('vessel.subtitle.create') }}</p>
        <EntityForm :object="vessel" :submit-function="submitVessel">
            <div class="name-imo">
                <FormField :required="true" class="field" :name="t('vessel.fields.name.title') + '*'" v-model="vessel.name" :placeholderText="t('vessel.fields.name.placeholder')"/>
                <FormField :required="true" class="field" :name="t('vessel.fields.imoNumber.title') + '*'" v-model="vessel.imoNumber" :placeholderText="t('vessel.fields.imoNumber.placeholder')" pattern="IMO [0-9]{7}"/>
                <EntityDropdown
                class="field-dropdown"
                :name="t('vessel.fields.vesselType.title') + '*'"
                v-model="vessel.type"
                :fetch-function="() => vesselTypeService.getVesselTypes().then(page => (page.items || []).map(t => t.name))"
                :fetch-on-mount="true"
                :placeholderText="t('vessel.fields.vesselType.placeholder')"
                :required="true"
                valueKey="name"
                labelKey="name"
                />
                <EntityDropdown
                class="field-dropdown"
                :name="t('vessel.fields.owner.title') + '*'"
                v-model="vessel.owner"
                :fetch-function="() => saoService.getShippingAgentOrganizations().then(page => (page.items || []).map(t => t.name))"
                :fetch-on-mount="true"
                :placeholderText="t('vessel.fields.owner.placeholder')"
                :required="true"
                valueKey="name"
                labelKey="name"
                />
            </div>
            <div class="measurements">
                <FormField :required="true" class="field" :name="t('physicalCharacteristics.length.title') + ' (m)*'" v-model.number="vessel.length"
                :placeholderText="t('physicalCharacteristics.length.placeholder')" pattern="^\d+(\.\d{1,2})?$"/>
                <FormField :required="true" class="field" :name="t('physicalCharacteristics.depth.title') + ' (m)*'" v-model.number="vessel.depth"
                :placeholderText="t('physicalCharacteristics.depth.placeholder')" pattern="^\d+(\.\d{1,2})?$"/>
                <FormField :required="true" class="field" :name="t('physicalCharacteristics.draft.title') + ' (m)*'" v-model.number="vessel.draft"
                :placeholderText="t('physicalCharacteristics.draft.placeholder')" pattern="^\d+(\.\d{1,2})?$"/>
            </div>
        </EntityForm>
    </div>
</template>

<style scoped>
.name-imo {
    display: flex;
    gap: .5rem;
}

.measurements {
    display: flex;
    gap: .5rem;
}

.create-vessel-form {
    width: 100%;
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

.buttons {
    display: flex;
    justify-content: flex-end;
    gap: 1rem;
}

.form-button {
    min-width: 100px;
}

.form-messages {
    margin: 0.5rem 0 1rem 0;
    bottom: 1rem;
}

.form-tip {
    font-size: 0.9rem;
    color: #666666;
    margin-bottom: 1rem;
    display: flex;
    justify-content: flex-end;
}

</style>