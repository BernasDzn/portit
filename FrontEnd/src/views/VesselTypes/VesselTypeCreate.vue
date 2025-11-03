<script setup lang="ts">
import { ref, computed } from 'vue';
import { AxiosHttpService } from '@/service/AxiosHttpService';
import { VesselTypeService } from '@/service/VesselTypeService';
import type { VesselType } from '@/model/VesselType';
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';


const vesselType = ref<VesselType>({
    name: '',
    description: '',
    maxNumberOfRows: null!,
    maxNumberOfBays: null!,
    maxNumberOfTiers: null!,
    capacity: 0,
    physicalCharacteristics: {
        length: null!,
        depth: null!,
        draft: null!
    }
});

const capacity = computed(() => {
    const rows = Number(vesselType.value.maxNumberOfRows) || 0;
    const bays = Number(vesselType.value.maxNumberOfBays) || 0;
    const tiers = Number(vesselType.value.maxNumberOfTiers) || 0;
    return rows * bays * tiers;
});

const http = new AxiosHttpService();
const vesselTypeService = new VesselTypeService(http);

const submitVesselType = (obj: any) =>
    vesselTypeService.createVesselType(obj);

</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item>
                <RouterLink to="/vessel-types/dashboard" class="breadcrumb-link">Vessel Type Dashboard</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>Create Vessel Type</sl-breadcrumb-item>
        </sl-breadcrumb>
        <h1 class="title">Create Vessel Type</h1>
        <p class="subtitle">Register a new vessel type into the system</p>
        <EntityForm :object="vesselType" :submit-function="submitVesselType">
            <div class="form">
                <div class="general-info">
                    <p class="section-title">General Information</p>
                    <FormField class="field" name="Name*" v-model="vesselType.name" placeholderText="Vessel type name" required/>
                    <FormField class="field" name="Description*" v-model="vesselType.description" placeholderText="Vessel type description" required/>
                </div>

                <span class="section-divider"></span>
                
                <div class="measurements">
                    <p class="section-title">Physical Characteristics (m)</p>
                        <FormField class="field" name="Length*" v-model.number="vesselType.physicalCharacteristics.length" placeholderText="Length in meters" pattern="^\d+(\.\d{1,2})?$" required/>
                        <FormField class="field" name="Depth*" v-model.number="vesselType.physicalCharacteristics.depth" placeholderText="Depth in meters" pattern="^\d+(\.\d{1,2})?$" required/>
                        <FormField class="field" name="Draft*" v-model.number="vesselType.physicalCharacteristics.draft" placeholderText="Draft in meters" pattern="^\d+(\.\d{1,2})?$" required/>
                </div>

                <span class="section-divider"></span>

                <div class="measurements">
                    <p class="section-title">Container Capacity (TEU)</p>
                    <FormField class="field" name="Max Number of Rows*" v-model.number="vesselType.maxNumberOfRows" placeholderText="Max Rows in TEU's" pattern="^\d+$" required/>
                    <FormField class="field" name="Max Number of Bays*" v-model.number="vesselType.maxNumberOfBays" placeholderText="Max Bays in TEU's" pattern="^\d+$" required/>
                    <FormField class="field" name="Max Number of Tiers*" v-model.number="vesselType.maxNumberOfTiers" placeholderText="Max Tiers in TEU's" pattern="^\d+$" required/>
                    <p class="section-title">Capacity: {{ capacity }} TEU</p>
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