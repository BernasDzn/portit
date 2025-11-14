<script setup lang="ts">
import { onMounted, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import AxiosHttpService from '@/service/AxiosHttpService';
import { StorageAreaService } from '@/service/StorageAreaService';

import EntityDropdown from '@/components/crud/EntityDropdown.vue';
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import { DockService } from '@/service/DockService';
import type { Dock } from '@/model/Dock';
import { useRoute, RouterLink } from 'vue-router';
import type { IDockService } from '@/service/IService/IDockService';
import { container } from '@/inversify.config';
import type { IStorageAreaService } from '@/service/IService/IStorageAreaService';
import TYPES from '@/inversify/types';
import type { DockRelationDto, StorageAreaDto } from '@/model/dto/StorageAreaDto';
import { StorageArea } from '@/model/StorageArea';

const storageAreaService = container.get<IStorageAreaService>(TYPES.storageAreaService);
const dockService = container.get<IDockService>(TYPES.dockService);

const route = useRoute();
const storageAreaNameCode = String(route.params.name || '');

const { t } = useI18n();

const storageArea = ref<StorageAreaDto>({
    nameCode: '',
    location: '',
    type: 0,
    capacity: 0,
    currentOccupancy: 0,
    dockServices: [] as DockRelationDto[],
});

const allDocks = ref<Array<Dock>>([]);

function getDockLabel(rel: any) {
    // If the server already included the nested dock object
    if (rel && rel.dock) {
        if (rel.dock.code) return rel.dock.code;
        if (rel.dock.name) return rel.dock.name;
    }
}

function updateDockRelations(dockCodes: string[]) {
    const selected = new Set(dockCodes || [])

    storageArea.value.dockServices = storageArea.value.dockServices.filter(rel => selected.has(rel.dockCode))

    dockCodes.forEach(dockCode => {
        const existingRelation = storageArea.value.dockServices.find(relation => relation.dockCode === dockCode);
        if (!existingRelation) {
            const dock = allDocks.value.find(d => d.code === dockCode);
            if (dock) {
                storageArea.value.dockServices.push({ dockCode: dock.code, isServingDock: true });
            }
        }
    });
}

function updateType(typeValue: string) {
    storageArea.value.type = parseInt(typeValue);
}

onMounted(async () => {
    if (!storageAreaNameCode) return;
    dockService.getDocks().then(page => {
        allDocks.value = page.items || [];
    });
});

const updateStorageArea = (obj: any) => {

    console.log('Updating storage area:', obj);
    storageAreaService.updateStorageArea(obj);
}

</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item><RouterLink to="/storage-areas/dashboard" class="breadcrumb-link">{{ t('storageArea.tabs.dashboard') }}</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item><RouterLink to="/storage-areas/search" class="breadcrumb-link">{{ t('storageArea.tabs.search') }}</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item>
                <RouterLink 
                    :to="storageAreaNameCode ? `/storage-areas/view/${storageAreaNameCode}` : '/storage-areas/search'" 
                    class="breadcrumb-link"
                >{{ storageAreaNameCode }}</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ t('storageArea.tabs.edit') }}</sl-breadcrumb-item>
        </sl-breadcrumb>

        <h1 class="title">{{ t('storageArea.tabs.edit') }}</h1>
        <p class="subtitle">{{ t('storageArea.subtitle.edit') }}</p>
        <EntityForm :object="storageArea" :editing-id="storageAreaNameCode" :submit-function="updateStorageArea" :fetchingFunction="() => storageAreaService.getStorageAreaById(storageAreaNameCode)">
            <div class="form-fields">
                <FormField class="field" :name="t('storageArea.fields.nameCode.title')" v-model="storageArea.nameCode" :placeholderText="t('storageArea.fields.nameCode.placeholder')" required pattern="^[a-zA-Z0-9]*$"/>
                <FormField class="field" :name="t('storageArea.fields.location.title')" v-model="storageArea.location" :placeholderText="t('storageArea.fields.location.placeholder')" required/>
                
                <div class="field">
                    <label>{{ t('storageArea.fields.type.title') }}</label>
                    <sl-select 
                        :value="storageArea.type.toString()"
                        @sl-change="updateType($event.target.value)" 
                        :placeholder="t('storageArea.fields.type.placeholder')"
                        required
                    >
                        <sl-option v-for="(typeName, typeKey) in StorageArea.sa_type" :key="typeKey" :value="typeKey">{{ t(`storageArea.fields.type.options.${typeName.toLowerCase()}`) }}</sl-option>
                    </sl-select>
                </div>

                <FormField class="field" :name="t('storageArea.fields.capacity.title')" v-model.number="storageArea.capacity" :placeholderText="t('storageArea.capacity.placeholder')" pattern="^[0-9]\d*$" required/>
                <FormField class="field" :name="t('storageArea.fields.occupancy.placeholder')" v-model.number="storageArea.currentOccupancy" :placeholderText="t('storage-areas.currentOccupancy.placeholder')" pattern="^[0-9]\d*$" required/>

                <div style="flex:100%; width: 100%;">
                    <p class="section-title">{{ t('dock.title') }}</p>
                    <EntityDropdown
                        class="field-dropdown"
                        :name="t('physicalResource.fields.servingDocks.title')"
                        :fetch-function="() => dockService.getDocks().then(page => (page.items || []).map(t => t.code))"
                        :fetch-on-mount="true"
                        :placeholderText="t('physicalResource.fields.servingDocks.placeholder')"
                        :default-values="storageArea.dockServices.map(ds => getDockLabel(ds))"
                        :required="false"
                        :multiple="true"
                        valueKey="code"
                        labelKey="name"
                        @sl-change="updateDockRelations($event.target.value)"
                    />
                    <div style="display: flex; flex-wrap: wrap; gap: 1rem;">
                        <sl-card class="card-header" style="width: fit-content;" v-for="dock_p in storageArea.dockServices" :key="dock_p.dockCode">
                            <div slot="header">
                                {{ getDockLabel(dock_p) }} {{ t('storageArea.create.distance_meters') }}
                            </div>
                            <FormField class="field" :name="`null`" v-model="dock_p.distance" :placeholderText="t('storageArea.create.distance_meters')" pattern="^[0-9]+(\.[0-9]{1,2})?$" required/>
                        </sl-card>
                    </div>
                </div>
            </div>
        </EntityForm>
    </div>
</template>

<style scoped>
.form-fields {
    display: flex;
    gap: .5rem;
    flex-wrap: wrap;
}

.field {
    margin-bottom: 1rem;
    max-width: 30rem;
    padding: .5rem
}

.field label {
    display: block;
    margin-bottom: 0.5rem;
    font-weight: 500;
    color: var(--sl-color-neutral-700);
}

.field sl-select {
    width: 100%;
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

.section-title {
    font-size: 0.8rem;
    margin-bottom: 1rem;
    color: var(--sl-color-neutral-400);
}

</style>