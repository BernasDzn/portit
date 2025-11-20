<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';
import EntityDropdown from '@/components/crud/EntityDropdown.vue';
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import type { Dock } from '@/model/Dock';
import { container } from '@/inversify.config';
import type { IStorageAreaService } from '@/service/IService/IStorageAreaService';
import TYPES from '@/inversify/types';
import type { IDockService } from '@/service/IService/IDockService';
import type { StorageAreaDto } from '@/model/dto/StorageAreaDto';
import { StorageArea } from '@/model/StorageArea';

const storageAreaService = container.get<IStorageAreaService>(TYPES.storageAreaService);
const dockService = container.get<IDockService>(TYPES.dockService);

const { t } = useI18n();

const storageArea = ref<StorageAreaDto>({
    nameCode: '',
    location: '',
    type: 0,
    capacity: 0,
    currentOccupancy: 0,
    dockServices: [],
});

const allDocks = ref<Array<Dock>>([]);

function updateDockRelations(dockCodes: string[]) {
    const selected = new Set(dockCodes || [])

    storageArea.value.dockServices = storageArea.value.dockServices.filter(rel => selected.has(rel.dockCode))

    // Add new relations for any selected codes not already present
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

onMounted(() => {
    dockService.getDocks().then(page => {
        allDocks.value = page.items || [];
    });
});

const submitStorageArea = (obj: any) => 
    storageAreaService.createStorageArea(obj);

</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item><RouterLink to="/storage-areas/dashboard" class="breadcrumb-link">{{ t('storageArea.tabs.dashboard') }}</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ t('storageArea.tabs.create') }}</sl-breadcrumb-item>
        </sl-breadcrumb>

        <h1 class="title">{{ t('storageArea.tabs.create') }}</h1>
        <p class="subtitle">{{ t('storageArea.subtitle.create') }}</p>
        <EntityForm :object="storageArea" :submit-function="submitStorageArea">
            <div class="form-fields">
                <FormField input-id="storagearea-namecode" class="field" :name="t('storageArea.fields.nameCode.title')" v-model="storageArea.nameCode" :placeholderText="t('storageArea.fields.nameCode.placeholder')" required pattern="^[a-zA-Z0-9]*$"/>
                <FormField input-id="storagearea-location" class="field" :name="t('storageArea.fields.location.title')" v-model="storageArea.location" :placeholderText="t('storageArea.fields.location.placeholder')" required/>
                
                <div class="field">
                    <label for="storagearea-type">{{ t('storageArea.fields.type.title') }}</label>
                    <sl-select 
                        id="storagearea-type"
                        @sl-change="updateType($event.target.value)" 
                        :placeholder="t('storageArea.fields.type.placeholder')"
                        required
                    >
                        <sl-option v-for="(typeName, typeKey) in StorageArea.sa_type" :key="typeKey" :id="`${typeKey}`" :value="typeKey">{{ t(`storageArea.fields.type.options.${typeName.toLowerCase()}`) }}</sl-option>
                    </sl-select>
                </div>

                <FormField input-id="storagearea-capacity" class="field" :name="t('storageArea.fields.capacity.title')" v-model.number="storageArea.capacity" :placeholderText="t('storageArea.capacity.placeholder')" pattern="^[0-9]\d*$" required/>
                <FormField input-id="storagearea-occupancy" class="field" :name="t('storageArea.fields.occupancy.placeholder')" v-model.number="storageArea.currentOccupancy" :placeholderText="t('storage-areas.currentOccupancy.placeholder')" pattern="^[0-9]\d*$" required/>

                <div style="flex:100%; width: 100%;">
                    <p class="section-title">{{ t('dock.title') }}</p>
                    <EntityDropdown
                        input-id="storagearea-docks"
                        class="field-dropdown"
                        :name="t('physicalResource.fields.servingDocks.title')"
                        :fetch-function="() => dockService.getDocks().then(page => (page.items || []).map(t => t.code))"
                        :fetch-on-mount="true"
                        :placeholderText="t('physicalResource.fields.servingDocks.placeholder')"
                        :required="false"
                        :multiple="true"
                        valueKey="code"
                        labelKey="name"
                        @sl-change="updateDockRelations($event.target.value)"
                    />
                    <div style="display: flex; flex-wrap: wrap; gap: 1rem;">
                        <sl-card class="card-header" style="width: fit-content;" v-for="dock in storageArea.dockServices" :key="dock.dockCode" >
                            <div slot="header">
                                {{ dock.dockCode }} {{ t('storageArea.create.distance_meters') }}
                            </div>
                            <FormField class="field" :name="`null`" v-model="dock.distance" :placeholderText="t('storageArea.create.distance_meters')" pattern="^[0-9]+(\.[0-9]{1,2})?$" required/>
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