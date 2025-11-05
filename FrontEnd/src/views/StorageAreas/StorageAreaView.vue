<script setup lang="ts">
// @ts-ignore: missing type declarations for 'vue-i18n' in this project
import {useI18n} from 'vue-i18n';
import { useRoute } from 'vue-router';
import { AxiosHttpService } from '@/service/AxiosHttpService';
import { StorageAreaService } from '@/service/StorageAreaService';

import type { StorageArea } from '@/model/StorageArea';

import EntityView from '@/components/crud/EntityView.vue';
import StorageCapacityPrinter from '@/components/StorageCapacityPrinter.vue';
import DockPrinter from '@/components/printers/DockPrinter.vue';

const { t } = useI18n();

const route = useRoute();

const http = new AxiosHttpService();
const storageAreaService = new StorageAreaService(http);
const nameCode = decodeURIComponent((route.params.nameCode ?? '') as string);

const fetchStorageArea = async (): Promise<StorageArea | undefined> => {
    return await storageAreaService.getStorageAreaById(nameCode);
};

const statuses = [
	"Yard", "Warehouse"
];

</script>

<template>
  <div>
    <sl-breadcrumb>
        <sl-breadcrumb-item><RouterLink to="/storage-areas/dashboard" class="breadcrumb-link">{{ t('storageArea.tabs.dashboard') }}</RouterLink></sl-breadcrumb-item>
        <sl-breadcrumb-item><RouterLink to="/storage-areas/search" class="breadcrumb-link">{{ t('storageArea.tabs.search') }}</RouterLink></sl-breadcrumb-item>
        <sl-breadcrumb-item>{{ nameCode }}</sl-breadcrumb-item>
    </sl-breadcrumb>

    <EntityView :fetch-function="fetchStorageArea" v-slot="entity">
        <div>
            <div class="opposed">
                <div class="view-header">
                    <span class="material-icons icon" aria-hidden="true">warehouse</span>
                    <div>
                        <h2 class="title">{{ entity.element.nameCode }}</h2>
						<div class="email-group">
							<span class="material-icons sec_icon" aria-hidden="true">location_on</span>
							<span class="item-description">{{ entity.element.location }}</span>
						</div>
                    </div>
                </div>
                <RouterLink :to="`/storage-areas/edit/${encodeURIComponent(entity.element.id)}`">
                    <sl-button variant="default" size="large">
                        <sl-icon slot="prefix" name="pencil"></sl-icon>
                        {{ t('storageArea.tabs.edit') }}
                    </sl-button>
                </RouterLink>
            </div>
            <div class="viewing-content">
                <sl-card class="info-card" style="flex: 65%;">
                    <p>{{ t('storageArea.infoTitle') }}</p>
                    <div class="info-grid">
                        <div class="info-block">
                            <span class="label">{{ t('storageArea.fields.nameCode.title') }}</span>
                            <p>{{ entity.element.nameCode }}</p>
                        </div>
                        <div class="info-block">
                            <span class="label">{{ t('storageArea.fields.location.title') }}</span>
                            <p>{{ entity.element.location }}</p>
                        </div>
                        <div class="info-block">
                            <span class="label">{{ t('storageArea.fields.type.title') }}</span>
                            <p>{{ statuses[entity.element.type] }}</p>
                        </div>
                        <div class="info-block">
                            <span class="label">{{ t('storageArea.fields.occupancy.title') }}</span>
                            <p>{{ entity.element.currentOccupancy }} / {{ entity.element.capacity }}</p>
                        </div>
                    </div>
                </sl-card>
                <sl-card class="info-card">
                    <p>{{ t('common.statistics') }}</p>
                    <div class="view-statistics-overview">
                        <p class="view-statistic-data">{{ entity.element.dockServices.length }}</p>
                        <p>{{ t('storageArea.printer.docks_serviced') }}</p>
                    </div>
                </sl-card>
                <sl-card class="info-card" style="flex: 100%;">
                    <p>Capacity:</p>
                    <StorageCapacityPrinter
                        :storage-area="entity.element"
                    />
                </sl-card>
                <sl-card class="info-card" style="flex: 100%;">
                    <p>{{ t('storageArea.printer.docks_serviced').slice(0, 1).toUpperCase() + t('storageArea.printer.docks_serviced').slice(1) }}:</p>
                    <div class="info-grid">
                        <div
                            style="flex: 3;" 
                            v-if="entity.element.dockServices.length > 0" 
                            v-for="dock_relation of entity.element.dockServices" :key="dock_relation.dock.code"
                        >
                            <DockPrinter
                                class="listing-box"
                                :dock="dock_relation.dock"
                                :link="`/docks/view/${dock_relation.dock.code}`"
                                :show-details="false"
                            />
                        </div>
                        <p v-else style="color: var(--sl-color-neutral-400)">{{ t('storageArea.printer.no_docks_serviced') }}</p>
                    </div>
                </sl-card>
            </div>
        </div>
    </EntityView>
  </div>
</template>

<style scoped> 

.viewing-content{
    display: flex;
    flex-wrap: wrap;
}

.info-block {
    flex: 1 1 45%;
    min-width: 200px;
}

.sec_icon {
    font-size: 35px;
    color: var(--accent-1);
}

.email-group {
    display: flex;
    align-items: center;
    gap: 8px;
    margin-top: 8px;
}

.email-group .sec_icon {
    font-size: 20px;
}

.vessel-main-info {
    width: 60%;
}

.vessel-alt-info {
    width: 40%;
}

</style>