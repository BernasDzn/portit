<script setup lang="ts">
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import { useAlerts } from '@/composables/alerts';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { VesselTypeDto } from '@/model/dto/VesselTypeDto';
import { VesselType } from '@/model/VesselType';
import type { IVesselTypeService } from '@/service/IService/IVesselTypeService';
import { ref, onMounted, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRoute, RouterLink } from 'vue-router';

const { t } = useI18n();

const vesselTypeService = container.get<IVesselTypeService>(TYPES.vesselTypeService);

const route = useRoute();
const vesselTypeName = String(route.params.name || '');
const notifications = useAlerts();

let vesselType = ref<VesselTypeDto>({
    name: '',
    description: '',
    physicalCharacteristics: {
        length: null!,
        depth: null!,
        draft: null!
    },
    maxNumberOfRows: null!,
    maxNumberOfBays: null!,
    maxNumberOfTiers: null!
});

const capacity = computed(() => {
    const rows = Number(vesselType.value.maxNumberOfRows) || 0;
    const bays = Number(vesselType.value.maxNumberOfBays) || 0;
    const tiers = Number(vesselType.value.maxNumberOfTiers) || 0;
    return rows * bays * tiers;
});

onMounted(async () => {

    try {
        const data = await vesselTypeService.getVesselTypeByName(vesselTypeName);
        if (!data) return;
        vesselType.value = data;

    } catch (err) {
        console.error('Failed to load vessel type', err);
    }
});


const updateVesselType = async (obj: VesselType) => {
    if (!vesselTypeName) {
        notifications.enqueueNotification(
            'Cannot update vessel types at this time.',
            notifications.notificationTypes.DANGER
        );
        return;
    }

    vesselTypeService.updateVesselType(new VesselType(obj));
};

</script>

<template>
    <div class="vessel-type-edit">
        <sl-breadcrumb>
            <sl-breadcrumb-item>
                <RouterLink to="/vessels/dashboard" class="link">{{ t('vessel.tabs.dashboard') }}</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                <RouterLink to="/vessel-types/search" class="link">{{ t('vesselType.tabs.search') }}</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                <RouterLink :to="vesselType.name ? `/vessel-types/view/${vesselType.name}` : '/vessel-types/search'" class="link">
                    {{ vesselTypeName || 'Vessel Type Name' }}
                </RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ t('vesselType.tabs.edit') }}</sl-breadcrumb-item>
        </sl-breadcrumb>

        <h1 class="title">{{ t('vesselType.tabs.edit') }}</h1>
        <p class="subtitle">{{ t('vesselType.subtitle.edit') }}</p>
        <EntityForm :object="vesselType" :editing-id="vesselTypeName" :submit-function="updateVesselType">
            <div class="form">
                <div class="general-info">
                    <p class="section-title">{{ t('vesselType.infoTitle') }}</p>
                    <FormField class="field" :enabled="false" :name="t('vesselType.fields.name.title') + '*'" v-model="vesselType.name" :placeholderText="t('vesselType.fields.name.placeholder')" required/>
                    <FormField class="field" :name="t('vesselType.fields.description.title') + '*'" v-model="vesselType.description" :placeholderText="t('vesselType.fields.description.placeholder')" required/>
                </div>

                <span class="section-divider"></span>
                
                <div class="measurements">
                    <p class="section-title">{{ t('physicalCharacteristics.title') }}</p>
                        <FormField class="field" :name="t('physicalCharacteristics.length.title') + '*'" v-model.number="vesselType.physicalCharacteristics.length" :placeholderText="t('physicalCharacteristics.length.placeholder')" pattern="^\d+(\.\d{1,2})?$" required/>
                        <FormField class="field" :name="t('physicalCharacteristics.depth.title') + '*'" v-model.number="vesselType.physicalCharacteristics.depth" :placeholderText="t('physicalCharacteristics.depth.placeholder')" pattern="^\d+(\.\d{1,2})?$" required/>
                        <FormField class="field" :name="t('physicalCharacteristics.draft.title') + '*'" v-model.number="vesselType.physicalCharacteristics.draft" :placeholderText="t('physicalCharacteristics.draft.placeholder')" pattern="^\d+(\.\d{1,2})?$" required/>
                </div>

                <span class="section-divider"></span>

                <div class="measurements">
                    <p class="section-title">{{ t('vesselType.fields.containerCapacity.title') }}</p>
                    <FormField class="field" :name="t('vesselType.fields.containerCapacity.maxNumberOfRows.title') + '*'" v-model.number="vesselType.maxNumberOfRows" :placeholderText="t('vesselType.fields.containerCapacity.maxNumberOfRows.placeholder')" pattern="^\d+$" required/>
                    <FormField class="field" :name="t('vesselType.fields.containerCapacity.maxNumberOfBays.title') + '*'" v-model.number="vesselType.maxNumberOfBays" :placeholderText="t('vesselType.fields.containerCapacity.maxNumberOfBays.placeholder')" pattern="^\d+$" required/>
                    <FormField class="field" :name="t('vesselType.fields.containerCapacity.maxNumberOfTiers.title') + '*'" v-model.number="vesselType.maxNumberOfTiers" :placeholderText="t('vesselType.fields.containerCapacity.maxNumberOfTiers.placeholder')" pattern="^\d+$" required/>
                    <p class="section-title">{{ t('vesselType.fields.capacity.title') }}: {{ capacity }} TEU</p>
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
}

.measurements {
    display: flex;
    flex-direction: column;
}

.measurements-grid {
    display: flex;
    flex-direction: row;
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