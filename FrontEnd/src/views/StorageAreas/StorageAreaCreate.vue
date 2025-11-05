<script setup lang="ts">
import { ref } from 'vue';
import { useI18n } from 'vue-i18n';
import AxiosHttpService from '@/service/AxiosHttpService';
import { StorageAreaService } from '@/service/StorageAreaService';
import type { StorageArea } from '@/model/StorageArea';

import EntityDropdown from '@/components/crud/EntityDropdown.vue';
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import { DockService } from '@/service/DockService';

const http = new AxiosHttpService();
const storageAreaService = new StorageAreaService(http);
const dockService = new DockService(http);

const storageArea = ref<StorageArea>({
    nameCode: '',
    location: '',
    type: 0,
    capacity: 0,
    currentOccupancy: 0,
    dockServices: [],
});

const { t } = useI18n();

const submitStorageArea = (obj: any) => 
    storageAreaService.createStorageArea(obj);

</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item><RouterLink to="/storage-area/dashboard" class="breadcrumb-link">{{ t('storageArea.tabs.dashboard') }}</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ t('storageArea.tabs.create') }}</sl-breadcrumb-item>
        </sl-breadcrumb>

        <h1 class="title">{{ t('storageArea.tabs.create') }}</h1>
        <p class="subtitle">{{ t('storageArea.subtitle.create') }}</p>
        <EntityForm :object="storageArea" :submit-function="submitStorageArea">
            <div class="form">
                <div class="general-info">
                    <p class="section-title">{{ t('dock.generalFields') }}</p>
                    <FormField class="field" :name="t('storageArea.fields.nameCode.title')" v-model="storageArea.nameCode" :placeholderText="t('storageArea.fields.nameCode.placeholder')" required/>
                    <FormField class="field" :name="t('storageArea.fields.location.title')" v-model="storageArea.location" :placeholderText="t('storageArea.fields.location.placeholder')" required/>
                </div>

                <span class="section-divider"></span>
                
                <div class="measurements">
                    <p class="section-title">{{ t('storageArea.fields.capacity.title') }}</p>
                        <FormField class="field" :name="t('storageArea.fields.capacity.title')" v-model.number="storageArea.capacity" :placeholderText="t('storageArea.capacity.placeholder')" pattern="^[1-9]\d*$" required/>
                        <FormField class="field" :name="t('storageArea.fields.occupancy.placeholder')" v-model.number="storageArea.currentOccupancy" :placeholderText="t('storage-areas.currentOccupancy.placeholder')" pattern="^[1-9]\d*$" required/>
                </div>

                <span class="section-divider"></span>

                <div class="measurements">
                    <p class="section-title">{{ t('dock.title') }}</p>
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