<script setup lang="ts">
import { useRoute } from 'vue-router';
import { AxiosHttpService } from '@/service/AxiosHttpService';
import type { Vessel } from '@/model/Vessel';
import EntityView from '@/components/crud/EntityView.vue';
import { QualificationService } from '@/service/QualificationService';
import type { Qualification } from '@/model/Qualifications';

const route = useRoute();

const http = new AxiosHttpService();
const qualificationService = new QualificationService(http);
const qualId = decodeURIComponent((route.params.id ?? '') as string);

const fetchQualification = async (): Promise<Qualification | null> => {
    return await qualificationService.getQualificationById(qualId);
};

</script>

<template>
  <div>
    <sl-breadcrumb>
        <sl-breadcrumb-item><RouterLink to="/qualifications/dashboard" class="breadcrumb-link">Qualifications Dashboard</RouterLink></sl-breadcrumb-item>
        <sl-breadcrumb-item><RouterLink to="/qualifications/search" class="breadcrumb-link">Search Qualifications</RouterLink></sl-breadcrumb-item>
        <sl-breadcrumb-item>{{ qualId }}</sl-breadcrumb-item>
    </sl-breadcrumb>

    <EntityView :fetch-function="fetchQualification" v-slot="entity">
        <div>
            <div class="opposed">
                <div class="view-header">
                    <span class="material-icons icon" aria-hidden="true">workspace_premium</span>
                    <div>
                        <h2 class="title">{{ entity.element.qualificationName }}</h2>
                        <p class="subtitle">{{ entity.element.idCode }}</p>
                    </div>
                </div>
                <RouterLink :to="`/qualifications/edit/${encodeURIComponent(entity.element.idCode)}`">
                    <sl-button variant="default" size="large">
                        <sl-icon slot="prefix" name="pencil"></sl-icon>
                        Edit Qualification
                    </sl-button>
                </RouterLink>
    
            </div>
            <div class="viewing-content">
                <sl-card class="qual-main-info">
                    <p>Qualification information</p>
                    <div class="columns-2">
                        <div>
                            <div class="info-block">
                                <span class="label">Qualification code</span>
                                <p>{{ entity.element.idCode }}</p>
                            </div>
                        </div>
                        <div>
                            <div class="info-block">
                                <span class="label">Qualification name</span>
                                <p>{{ entity.element.qualificationName }}</p>
                            </div>
                        </div>
                    </div>
                </sl-card>
            </div>
        </div>
    </EntityView>
  </div>
</template>

<style scoped> 
.qual-main-info {
    width: 100%;
}
</style>