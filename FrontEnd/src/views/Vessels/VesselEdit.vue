<script setup lang="ts">
import EntityDropdown from '@/components/crud/EntityDropdown.vue';
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import ObjectSelector from '@/components/crud/ObjectSelector.vue';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { VesselDto } from '@/model/dto/VesselDto';
import { Vessel } from '@/model/Vessel';
import type { IShippingAgentOrganizationService } from '@/service/IService/IShippingAgentOrganizationService';
import type { IVesselService } from '@/service/IService/IVesselService';
import type { IVesselTypeService } from '@/service/IService/IVesselTypeService';
import { ref, onMounted } from 'vue';
import { useI18n } from 'vue-i18n'
const { t } = useI18n()
import { useRoute, RouterLink } from 'vue-router';

const vesselService = container.get<IVesselService>(TYPES.vesselService);
const vesselTypeService = container.get<IVesselTypeService>(TYPES.vesselTypeService);
const saoService = container.get<IShippingAgentOrganizationService>(TYPES.shippingAgentOrganizationService);

const route = useRoute();
const vesselIMO = String(route.params.imo || '');


const vessel = ref({
    name: '',
    imoNumber: '',
    type: null,
    owner: null,
    physicalCharacteristics: {
        length: null,
        depth: null,
        draft: null
    }
});

// Load vessel on mount
onMounted(async () => {
    try {
        const data: Vessel = await vesselService.getVesselByIMO(vesselIMO);
        if (!data) return;
        vessel.value = data;

    } catch (err) {
        console.error('Failed to load vessel', err);
    }
});

// Return the promise so the parent EntityForm can attach .catch/.then handlers
const submitVessel = (obj: any) =>
    vesselService.updateVessel(new Vessel(obj));
    
</script>

<template>
    <div class="vessel-edit">
        <sl-breadcrumb>
            <sl-breadcrumb-item>
                <RouterLink to="/vessels/dashboard" class="link">{{ t('vessel.tabs.dashboard') }}</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                <RouterLink to="/vessels/search" class="link">{{ t('vessel.tabs.search') }}</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                <RouterLink :to="vessel.imoNumber ? `/vessels/view/${vessel.imoNumber}` : '/vessels/search'" class="link">
                    {{ vessel.imoNumber || 'IMO' }}
                </RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ t('vessel.tabs.edit') }}</sl-breadcrumb-item>
        </sl-breadcrumb>

        <h1 class="title">{{ t('vessel.tabs.edit') }}</h1>
        <p class="subtitle">{{ t('vessel.subtitle.edit') }}</p>
        <EntityForm editing-id="imoNumber" :object="vessel" :submit-function="submitVessel">
            <div class="name-imo">
                <FormField inputId="vessel-name" :required="true" class="field" :name="`${t('vessel.fields.name.title')}*`" v-model="vessel.name" :placeholderText="t('vessel.fields.name.placeholder')"/>
                <FormField inputId="vessel-imo" :enabled="false" class="field" :name="t('vessel.fields.imoNumber.title')" v-model="vessel.imoNumber" :placeholderText="t('vessel.fields.imoNumber.placeholder')" pattern="IMO [0-9]{7}"/>

                <ObjectSelector
                    class="field-dropdown"
                    :name="t('vessel.fields.vesselType.title') + '*'"
                    v-model="vessel.type"
                    :fetch-function="() => vesselTypeService.getVesselTypes()"
                    :fetch-on-mount="true"
                    :placeholderText="t('vessel.fields.vesselType.placeholder')"
                    labelKey="name"
                    required
                />

                <ObjectSelector
                    class="field-dropdown"
                    :name="t('vessel.fields.owner.title') + '*'"
                    v-model="vessel.owner"
                    :fetch-function="() => saoService.getShippingAgentOrganizations()"
                    :fetch-on-mount="true"
                    :placeholderText="t('vessel.fields.owner.placeholder')"
                    labelKey="name"
                    required
                />

            </div>
            <div class="measurements">
                <FormField inputId="vessel-length" :required="true" class="field" :name="`${t('physicalCharacteristics.length.title')} (m)*`" v-model.number="vessel.physicalCharacteristics.length"
                :placeholderText="t('physicalCharacteristics.length.placeholder')" pattern="^\d+(\.\d{1,2})?$"/>
                <FormField inputId="vessel-depth" :required="true" class="field" :name="`${t('physicalCharacteristics.depth.title')} (m)*`" v-model.number="vessel.physicalCharacteristics.depth"
                :placeholderText="t('physicalCharacteristics.depth.placeholder')" pattern="^\d+(\.\d{1,2})?$"/>
                <FormField inputId="vessel-draft" :required="true" class="field" :name="`${t('physicalCharacteristics.draft.title')} (m)*`" v-model.number="vessel.physicalCharacteristics.draft"
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

.link {
  text-decoration: none;
  color: inherit;
}

</style>