<script setup lang="ts">
import { useRoute } from 'vue-router';
import { AxiosHttpService } from '@/service/AxiosHttpService';
import { DockService } from '@/service/DockService';

import type { Dock } from '@/model/Dock';

import EntityView from '@/components/EntityView.vue';
import ActivityTag from '@/components/ActivityTag.vue';


const route = useRoute();

const http = new AxiosHttpService();
const dockService = new DockService(http);
const dockCode = decodeURIComponent((route.params.code ?? '') as string);

const fetchDock = async (): Promise<Dock | undefined> => {
    return await dockService.getDockByCode(dockCode);
};

</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item>
                <RouterLink to="/docks/dashboard" class="breadcrumb-link">Docks Dashboard</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                <RouterLink to="/docks/search" class="breadcrumb-link">Search Docks</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ dockCode }}</sl-breadcrumb-item>
        </sl-breadcrumb>

        <EntityView :fetch-function="fetchDock" v-slot="entity">
            <div>
                <div class="opposed">
                    <div class="view-header">
                        <span class="material-icons icon" aria-hidden="true">houseboat</span>
                        <div>
                            <h2 class="title">{{ entity.element.name }}</h2>
                            <p class="subtitle">{{ entity.element.code }}</p>
                        </div>
                    </div>
                    <RouterLink :to="`/docks/edit/${encodeURIComponent(entity.element.code)}`">
                        <sl-button variant="default" size="large">
                            <sl-icon slot="prefix" name="pencil"></sl-icon>
                            Edit Dock
                        </sl-button>
                    </RouterLink>

                </div>
                <div class="viewing-content">
                    <div class="top-section">
                        <sl-card class="info-card" style="flex: 65%;">
                            <p>Dock information</p>
                            <div class="info-grid">
                                <div class="info-block">
                                    <span class="label">Code</span>
                                    <p>{{ entity.element.code }}</p>
                                </div>
                                <div class="info-block">
                                    <span class="label">Name</span>
                                    <p>{{ entity.element.name }}</p>
                                </div>
                                <div class="info-block">
                                    <span class="label">Location</span>
                                    <p>{{ entity.element.location }}</p>
                                </div>
                            </div>
                        </sl-card>
                        <sl-card class="info-card">
                            <p>Statistics</p>
                            <div class="statistics">
                                <div class="statistics-overview">
                                    <div>
                                        <p><span class="statistic-data">{{ entity.element.physicalCharacteristics.length }}</span>m</p>
                                        <p>Length</p>
                                    </div>
                                    <div>
                                        <p><span class="statistic-data">{{ entity.element.physicalCharacteristics.depth }}</span>m</p>
                                        <p>Depth</p>
                                    </div>
                                    <div>
                                        <p><span class="statistic-data">{{ entity.element.physicalCharacteristics.draft }}</span>m</p>
                                        <p>Draft</p>
                                    </div>
                                </div>

                                <span class="material-icons icon" aria-hidden="true"
                                    style="color: #485ea9;margin: 0%;">arrow_right</span>

                                <div class="statistics-overview">
                                    <div>
                                        <p class="statistic-data">{{ entity.element.supportedVesselTypes.length }}</p>
                                        <p>Supported Vessel Types</p>
                                    </div>
                                </div>

                            </div>
                        </sl-card>
                    </div>
                    <sl-card class="info-card" style="flex: 100%;">
                        <p>Supported Vessel Types</p>
                        <div class="info-grid">
                            <div class="info-block" v-for="vtype in entity.element.supportedVesselTypes"
                                :key="vtype.name">
                                <p>{{ vtype.name }}</p>
                            </div>
                        </div>
                    </sl-card>
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

.statistics-overview {
    display: flex;
    flex-direction: row;
    text-align: center;
    margin-bottom: 1rem;
    border-radius: 0.5rem;
    padding: 1rem;
    background-color: #f5f6fa;
}


.statistics-overview p {
    margin: 0;
    padding: 0.5rem;
    color: #485ea9;
}

.statistic-data {
    font-size: 2rem;
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

.statistics {
    display: flex;
    align-items: center;
    justify-content: center;
    flex-direction: row;
    gap: 1rem;
}

.info-card {
    border-radius: 0.5rem;
    flex: 1 1 auto;
}

</style>