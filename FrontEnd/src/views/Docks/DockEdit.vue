<script setup lang="ts">
import EntityDropdown from '@/components/crud/EntityDropdown.vue';
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import type { Dock } from '@/model/Dock';
import AxiosHttpService from '@/service/AxiosHttpService';
import { DockService } from '@/service/DockService';
import { VesselTypeService } from '@/service/VesselTypeService';
import { ref, onMounted } from 'vue';
import { useRoute, RouterLink } from 'vue-router';

const http = new AxiosHttpService();
const dockService = new DockService(http);
const vesselTypeService = new VesselTypeService(http);

const route = useRoute();
const dockCode = route.params.code as string;

let dock = ref<Dock>({
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
        
        dock.value.code = data.code ?? '';
        dock.value.name = data.name ?? '';
        dock.value.location = data.location ?? '';
        dock.value.physicalCharacteristics.length = data.physicalCharacteristics?.length ?? null!;
        dock.value.physicalCharacteristics.depth = data.physicalCharacteristics?.depth ?? null!;
        dock.value.physicalCharacteristics.draft = data.physicalCharacteristics?.draft ?? null!;
        dock.value.supportedVesselTypes = (data.supportedVesselTypes ?? []).map((vt: any) =>vt.name);
    } catch (err) {
        console.error('Failed to load dock', err);
    }
});

const submitDock = (obj: any) =>
    dockService.updateDock(dock.value.code, obj);

</script>

<template>
    <div class="dock-edit">
        <sl-breadcrumb>
            <sl-breadcrumb-item>
                <RouterLink to="/docks/dashboard" class="link">Dock Dashboard</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                <RouterLink to="/docks/search" class="link">Search Docks</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                <RouterLink :to="dock.code ? `/docks/view/${dock.code}` : '/docks/search'" class="link">
                    {{ dock.code || 'Dock Code' }}
                </RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>Edit Dock</sl-breadcrumb-item>
        </sl-breadcrumb>

        <h1 class="title">Edit Dock</h1>
        <p class="subtitle">Edit an existing dock in the system</p>
        <EntityForm :object="dock" editing :submit-function="submitDock">
            <div class="form">
                <div class="general-info">
                    <p class="section-title">General Information</p>
                    <FormField class="field" name="Code" v-model="dock.code" :enabled="false"/>
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
                    v-model=dock.supportedVesselTypes
                    :fetch-function="() => vesselTypeService.getVesselTypes()"
                    :fetch-on-mount="true"
                    placeholderText="Select vessel types"
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