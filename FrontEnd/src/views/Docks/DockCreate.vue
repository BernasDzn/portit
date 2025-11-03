<script setup lang="ts">
import { ref } from 'vue';
import { AxiosHttpService } from '@/service/AxiosHttpService';
import { DockService } from '@/service/DockService';
import type { Dock } from '@/model/Dock';
import { VesselTypeService } from '@/service/VesselTypeService';
import EntityForm from '@/components/crud/EntityForm.vue';
import EntityDropdown from '@/components/crud/EntityDropdown.vue';
import FormField from '@/components/crud/FormField.vue';


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

const http = new AxiosHttpService('https://localhost:5001');
const dockService = new DockService(http);
const vesselTypeService = new VesselTypeService(http);

const submitDock = (obj: any) => 
    dockService.createDock(obj);

</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item>
                <RouterLink to="/docks/dashboard" class="breadcrumb-link">Dock Dashboard</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>Create Dock</sl-breadcrumb-item>
        </sl-breadcrumb>
        <h1 class="title">Create Dock</h1>
        <p class="subtitle">Register a new dock into the system</p>
        <EntityForm :object="dock" :submit-function="submitDock">
            <div class="form">
                <div class="general-info">
                    <p class="section-title">General Information</p>
                    <FormField class="field" name="Code*" v-model="dock.code" placeholderText="Dock code" pattern="^[a-zA-Z0-9]+$" required/>
                    <FormField class="field" name="Name*" v-model="dock.name" placeholderText="Dock name" required/>
                    <FormField class="field" name="Location*" v-model="dock.location" placeholderText="Dock location" required/>
                </div>
                <span class="section-divider"></span>
                
                <div class="measurements">
                    <p class="section-title">Physical Characteristics</p>
                    <FormField class="field" name="Length (m)*" v-model.number="dock.physicalCharacteristics.length" placeholderText="Length in meters" pattern="^\d+(\.\d{1,2})?$" required/>
                    <FormField class="field" name="Depth (m)*" v-model.number="dock.physicalCharacteristics.depth" placeholderText="Depth in meters" pattern="^\d+(\.\d{1,2})?$" required/>
                    <FormField class="field" name="Draft (m)*" v-model.number="dock.physicalCharacteristics.draft" placeholderText="Draft in meters" pattern="^\d+(\.\d{1,2})?$" required/>
                </div>

                <span class="section-divider"></span>
                <div>
                <p class="section-title">Supported Vessel Types</p>
                <EntityDropdown
                    class="field-dropdown"
                    name="Vessel Types*"
                    v-model="dock.supportedVesselTypes"
                    :fetch-function="() => vesselTypeService.getVesselTypes()"
                    :fetch-on-mount="true"
                    placeholderText="Select vessel type"
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