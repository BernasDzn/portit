<script setup lang="ts">
import { useRoute } from 'vue-router';
import { AxiosHttpService } from '@/service/AxiosHttpService';
import { VesselTypeService } from '@/service/VesselTypeService';

import type { VesselType } from '@/model/VesselType';

import EntityView from '@/components/crud/EntityView.vue';


const route = useRoute();

const http = new AxiosHttpService();
const vesselTypeService = new VesselTypeService(http);
const vesselTypeName = decodeURIComponent((route.params.name ?? '') as string);

const fetchVesselType = async (): Promise<VesselType | undefined> => {
    return await vesselTypeService.getVesselTypeByName(vesselTypeName);
};

</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item>
                <RouterLink to="/vessel-types/dashboard" class="breadcrumb-link">Vessel Type Dashboard</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                <RouterLink to="/vessel-types/search" class="breadcrumb-link">Search Vessel Types</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ vesselTypeName }}</sl-breadcrumb-item>
        </sl-breadcrumb>

        <EntityView :fetch-function="fetchVesselType" v-slot="entity">
            <div>
                <div class="opposed">
                    <div class="view-header">
                        <span class="material-icons icon" aria-hidden="true">sailing</span>
                        <div>
                            <h2 class="title">{{ entity.element.name }}</h2>
                            <p class="subtitle">{{ entity.element.description }}</p>
                        </div>
                    </div>
                    <RouterLink :to="`/vessel-types/edit/${encodeURIComponent(entity.element.name)}`">
                        <sl-button variant="default" size="large">
                            <sl-icon slot="prefix" name="pencil"></sl-icon>
                            Edit Vessel Type
                        </sl-button>
                    </RouterLink>

                </div>
                <div class="viewing-content">
                    <div class="top-section">
                        <sl-card class="info-card">
                            <p>Vessel Type information</p>
                            <div class="info-grid">
                                <div class="info-block">
                                    <span class="label">Name</span>
                                    <p>{{ entity.element.name }}</p>
                                </div>
                                <div class="info-block">
                                    <span class="label">Description</span>
                                    <p>{{ entity.element.description }}</p>
                                </div>
                            </div>
                        </sl-card>
                        <sl-card class="info-card">
                            <p>Physical Characteristics</p>
                            <p class="info-row"><span class="label">Length:</span> <span>{{
                                entity.element.physicalCharacteristics.length }}m</span></p>
                            <p class="info-row"><span class="label">Depth:</span> <span>{{
                                entity.element.physicalCharacteristics.depth }}m</span></p>
                            <p class="info-row"><span class="label">Draft:</span> <span>{{
                                entity.element.physicalCharacteristics.draft }}m</span></p>
                        </sl-card>
                        <sl-card class="info-card">
                            <p>Statistics</p>
                            <div class="view-statistics">
                                <div class="view-statistics-overview">
                                    <p>Capacity Dimensions (in TEU's)</p>
                                    <div class="capacity-dimensions">
                                        <div>
                                            <p class="view-statistic-data">{{ entity.element.maxNumberOfRows }}</p>
                                            <p>Rows</p>
                                        </div>
                                        <div>
                                            <p class="view-statistic-data">{{ entity.element.maxNumberOfBays }}</p>
                                            <p>Bays</p>
                                        </div>
                                        <div>
                                            <p class="view-statistic-data">{{ entity.element.maxNumberOfTiers }}</p>
                                            <p>Tiers</p>
                                        </div>
                                    </div>
                                </div>

                                <span class="material-icons icon" aria-hidden="true"
                                    style="color: #485ea9;margin: 0%;">arrow_right</span>

                                <div class="view-statistics-overview">
                                    <div>
                                        <p class="view-statistic-data">{{ entity.element.capacity }}</p>
                                        <p>Capacity</p>
                                    </div>
                                </div>

                            </div>
                        </sl-card>
                    </div>
                </div>
            </div>
        </EntityView>
    </div>
</template>

<style scoped>
.info-block {
    flex: 1 1 45%;
    min-width: 200px;
}

.view-statistics-overview {
    display: flex;
    flex-direction: column;
    align-items: center;
}

.capacity-dimensions {
    display: flex;
    flex-direction: row;
    gap: 1rem;
}

.viewing-content {
    display: flex;
    flex-wrap: wrap;
}

.top-section {
    display: flex;
    flex-direction: row;
    gap: 1rem;
    margin-bottom: 1rem;
    width: 100%;
    height: fit-content;
}

.view-statistics {
    width: 100%;
}
</style>