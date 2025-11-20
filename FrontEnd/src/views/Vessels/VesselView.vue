<script setup lang="ts">
import { useRoute } from 'vue-router';
import type { Vessel } from '@/model/Vessel';
import EntityView from '@/components/crud/EntityView.vue';
import {useI18n} from 'vue-i18n';
import type { IVesselService } from '@/service/IService/IVesselService';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';

const { t } = useI18n();

const route = useRoute();

const vesselService = container.get<IVesselService>(TYPES.vesselService);
const imoNumber = decodeURIComponent((route.params.imo ?? '') as string);

const fetchVessel = async (): Promise<Vessel | null> => {
    return await vesselService.getVesselByIMO(imoNumber);
};

</script>

<template>
  <div>
    <sl-breadcrumb>
        <sl-breadcrumb-item><RouterLink to="/vessels/dashboard" class="breadcrumb-link">{{ t('vessel.tabs.dashboard') }}</RouterLink></sl-breadcrumb-item>
        <sl-breadcrumb-item><RouterLink to="/vessels/search" class="breadcrumb-link">{{ t('vessel.tabs.search') }}</RouterLink></sl-breadcrumb-item>
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
                    <sl-button id="vessel-edit" variant="default" size="large">
                        <sl-icon slot="prefix" name="pencil"></sl-icon>
                        {{ t('vessel.tabs.edit') }}
                    </sl-button>
                </RouterLink>
    
            </div>
            <div class="viewing-content">
                <sl-card class="vessel-main-info">
                    <p>{{ t('vessel.infoTitle') }}</p>
                    <div class="columns-2">
                        <div>
                            <div class="info-block">
                                <span class="label">{{ t('vessel.fields.name.title') }}</span>
                                <p>{{ entity.element.name }}</p>
                            </div>

                            <div class="info-block">
                                <span class="label">{{ t('vessel.fields.vesselType.title') }}</span>
                                <p>{{ entity.element.type.name }}</p>
                            </div>

                            <div class="info-block">
                                <span class="label">{{ t('vessel.fields.owner.title') }}</span>
                                <p>{{ entity.element.owner.name }}</p>
                            </div>
                        </div>
                        <div>
                            <div class="info-block">
                                <span class="label">{{ t('vessel.fields.imoNumber.title') }}</span>
                                <p>{{ entity.element.imoNumber }}</p>
                            </div>
                        </div>
                    </div>
                </sl-card>
                <sl-card class="vessel-alt-info">
                    <p>{{ t('physicalCharacteristics.title') }}</p>
                    <p class="info-row"><span class="label">{{ t('physicalCharacteristics.length.title') }}:</span> <span>{{ entity.element.physicalCharacteristics.length }}m</span></p>
                    <p class="info-row"><span class="label">{{ t('physicalCharacteristics.depth.title') }}:</span> <span>{{ entity.element.physicalCharacteristics.depth }}m</span></p>
                    <p class="info-row"><span class="label">{{ t('physicalCharacteristics.draft.title') }}:</span> <span>{{ entity.element.physicalCharacteristics.draft }}m</span></p>
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