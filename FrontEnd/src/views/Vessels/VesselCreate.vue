<script setup lang="ts">
import EntityDropdown from '@/components/crud/EntityDropdown.vue';
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import type { Vessel } from '@/model/Vessel';
import AxiosHttpService from '@/service/AxiosHttpService';
import { VesselService } from '@/service/VesselService';
import { VesselTypeService } from '@/service/VesselTypeService';
import { ref } from 'vue';

const http = new AxiosHttpService('https://localhost:5001');
const vesselService = new VesselService(http);
const vesselTypeService = new VesselTypeService(http);

const vessel = ref<Vessel>({
    name: '',
    imoNumber: '',
    type: '',
    owner: 'Global Shipping Co.', // SUBSTITUIR PELO OWNER REPRESENTADO PELO USER DEPOIS
    length: null,
    depth: null,
    draft: null
});

const submitVessel = (obj: any) => 
    vesselService.createVessel(obj);
</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item><RouterLink to="/vessels/dashboard" class="breadcrumb-link">Vessel Dashboard</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item>Create Vessel</sl-breadcrumb-item>
        </sl-breadcrumb>

        <h1 class="title">Create Vessel</h1>
        <p class="subtitle">Register a new vessel into the system</p>
        <EntityForm :object="vessel" :submit-function="submitVessel">
            <div class="name-imo">
                <FormField :required="true" class="field" name="Vessel Name*" v-model="vessel.name" placeholderText="Vessel name"/>
                <FormField :required="true" class="field" name="IMO Number*" v-model="vessel.imoNumber" placeholderText="IMO number" pattern="IMO [0-9]{7}"/>
                <EntityDropdown
                class="field-dropdown"
                name="Vessel Type*"
                v-model="vessel.type"
                :fetch-function="() => vesselTypeService.getVesselTypes().then(page => (page.items || []).map(t => t.name))"
                :fetch-on-mount="true"
                placeholderText="Select vessel type"
                :required="true"
                valueKey="name"
                labelKey="name"
                />
            </div>
            <div class="measurements">
                <FormField :required="true" class="field" name="Length (m)*" v-model.number="vessel.length"
                placeholderText="Length in meters" pattern="^\d+(\.\d{1,2})?$"/>
                <FormField :required="true" class="field" name="Depth (m)*" v-model.number="vessel.depth"
                placeholderText="Depth in meters" pattern="^\d+(\.\d{1,2})?$"/>
                <FormField :required="true" class="field" name="Draft (m)*" v-model.number="vessel.draft"
                placeholderText="Draft in meters" pattern="^\d+(\.\d{1,2})?$"/>
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