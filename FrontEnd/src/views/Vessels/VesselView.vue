<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { AxiosHttpService } from '@/service/AxiosHttpService';
import { VesselService } from '@/service/VesselService';
import type { Vessel } from '@/model/Vessel';
import Loading from '@/components/Loading.vue';
import NoResults from '@/components/NoResults.vue';
import EntityView from '@/components/crud/EntityView.vue';


const route = useRoute();

const http = new AxiosHttpService();
const vesselService = new VesselService(http);
const imoNumber = decodeURIComponent((route.params.imo ?? '') as string);

const fetchVessel = async (): Promise<Vessel | null> => {
    return await vesselService.getVesselByIMO(imoNumber);
};

</script>

<template>
  <div>
    <sl-breadcrumb>
        <sl-breadcrumb-item><RouterLink to="/vessels/dashboard" class="breadcrumb-link">Vessel Dashboard</RouterLink></sl-breadcrumb-item>
        <sl-breadcrumb-item><RouterLink to="/vessels/search" class="breadcrumb-link">Search Vessels</RouterLink></sl-breadcrumb-item>
        <sl-breadcrumb-item>{{ imoNumber }}</sl-breadcrumb-item>
    </sl-breadcrumb>

    <EntityView :fetch-function="fetchVessel" v-slot="entity">
        <div>
            <div class="opposed">
                <div class="view-header">
                    <span class="material-icons icon" aria-hidden="true">directions_boat</span>
                    <div>
                        <h2 class="title">{{ entity.element.name }}</h2>
                        <p class="subtitle">{{ entity.element.imoNumber }}</p>
                    </div>
                </div>
                <RouterLink :to="`/vessels/edit/${encodeURIComponent(entity.element.imoNumber)}`">
                    <sl-button variant="default" size="large">
                        <sl-icon slot="prefix" name="pencil"></sl-icon>
                        Edit Vessel
                    </sl-button>
                </RouterLink>
    
            </div>
            <div class="viewing-content">
                <sl-card class="vessel-main-info">
                    <p>Vessel information</p>
                    <div class="columns-2">
                        <div>
                            <div class="info-block">
                                <span class="label">Vessel name</span>
                                <p>{{ entity.element.name }}</p>
                            </div>

                            <div class="info-block">
                                <span class="label">Type</span>
                                <p>{{ entity.element.type.name }}</p>
                            </div>

                            <div class="info-block">
                                <span class="label">Owning organization</span>
                                <p>{{ entity.element.owner.name }}</p>
                            </div>
                        </div>
                        <div>
                            <div class="info-block">
                                <span class="label">Vessel IMO</span>
                                <p>{{ entity.element.imoNumber }}</p>
                            </div>
                        </div>
                    </div>
                </sl-card>
                <sl-card class="vessel-alt-info">
                    <p>Physical Characteristics</p>
                    <p class="info-row"><span class="label">Length:</span> <span>{{ entity.element.physicalCharacteristics.length }}m</span></p>
                    <p class="info-row"><span class="label">Depth:</span> <span>{{ entity.element.physicalCharacteristics.depth }}m</span></p>
                    <p class="info-row"><span class="label">Draft:</span> <span>{{ entity.element.physicalCharacteristics.draft }}m</span></p>
                </sl-card>
            </div>
        </div>
    </EntityView>
  </div>
</template>

<style scoped> 
.vessel-main-info {
    width: 60%;
}

.vessel-alt-info {
    width: 40%;
}
</style>