<script setup lang="ts">
import EntityDropdown from '@/components/crud/EntityDropdown.vue';
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import type { Vessel } from '@/model/Vessel';
import AxiosHttpService from '@/service/AxiosHttpService';
import { VesselService } from '@/service/VesselService';
import { VesselTypeService } from '@/service/VesselTypeService';
import { ref, onMounted } from 'vue';
import { useI18n } from 'vue-i18n'
const { t } = useI18n()
import { useRoute, RouterLink } from 'vue-router';

const http = new AxiosHttpService();
const vesselService = new VesselService(http);
const vesselTypeService = new VesselTypeService(http);

const route = useRoute();
const vesselIMO = String(route.params.imo || '');

let vessel = ref<Vessel>({
    name: '',
    imoNumber: '',
    type: {} as any,
    owner: 'Global Shipping Co.', // SUBSTITUIR PELO OWNER REPRESENTADO PELO USER DEPOIS
    length: 0,
    depth: 0,
    draft: 0
});

// Load vessel on mount
onMounted(async () => {
    if (!vesselIMO) return;
    try {
        const data = await vesselService.getVesselByIMO(vesselIMO);
        vessel.value.name = data.name;
        vessel.value.imoNumber = data.imoNumber;
        vessel.value.type = data.type.name;
        vessel.value.length = data.physicalCharacteristics.length;
        vessel.value.depth = data.physicalCharacteristics.depth;
        vessel.value.draft = data.physicalCharacteristics.draft;
    } catch (err) {
        // eslint-disable-next-line no-console
        console.error('Failed to load vessel', err);
    }
});

// Return the promise so the parent EntityForm can attach .catch/.then handlers
const submitVessel = (obj: any) =>
    vesselService.updateVessel(vesselIMO, obj);
    
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
        <EntityForm :editing="true" :object="vessel" :submit-function="submitVessel">
            <div class="name-imo">
                <FormField :required="true" class="field" :name="`${t('vessel.fields.name.title')}*`" v-model="vessel.name" :placeholderText="t('vessel.fields.name.placeholder')"/>
                <FormField :enabled="false" class="field" :name="t('vessel.fields.imoNumber.title')" v-model="vessel.imoNumber" :placeholderText="t('vessel.fields.imoNumber.placeholder')" pattern="IMO [0-9]{7}"/>
                <EntityDropdown
                class="field-dropdown"
                :name="`${t('vessel.fields.vesselType.title')}*`"
                v-model="vessel.type"
                :fetch-function="() => vesselTypeService.getVesselTypes().then(page => (page.items || []).map(t => t.name))"
                :fetch-on-mount="true"
                :placeholderText="t('vessel.fields.vesselType.placeholder')"
                :required="true"
                valueKey="name"
                labelKey="name"
                />
            </div>
            <div class="measurements">
                <FormField :required="true" class="field" :name="`${t('physicalCharacteristics.length.title')} (m)*`" v-model.number="vessel.length"
                :placeholderText="t('physicalCharacteristics.length.placeholder')" pattern="^\d+(\.\d{1,2})?$"/>
                <FormField :required="true" class="field" :name="`${t('physicalCharacteristics.depth.title')} (m)*`" v-model.number="vessel.depth"
                :placeholderText="t('physicalCharacteristics.depth.placeholder')" pattern="^\d+(\.\d{1,2})?$"/>
                <FormField :required="true" class="field" :name="`${t('physicalCharacteristics.draft.title')} (m)*`" v-model.number="vessel.draft"
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