<script setup lang="ts">
import { useRoute } from 'vue-router';
import { AxiosHttpService } from '@/service/AxiosHttpService';
import type { Vessel } from '@/model/Vessel';
import EntityView from '@/components/crud/EntityView.vue';
import { QualificationService } from '@/service/QualificationService';
import type { Qualification } from '@/model/Qualifications';
import { PhysicalResourceService } from '@/service/PhysicalResourceService';
import { computed, onMounted, ref } from 'vue';

const route = useRoute();

const http = new AxiosHttpService();
const physicalResourceService = new PhysicalResourceService(http);
const resourceId = decodeURIComponent((route.params.code ?? '') as string);

const fetchResource = async (): Promise<any | null> => {
    return await physicalResourceService.getPhysicalResourceById(resourceId);
};

const icon = ref('build');

onMounted(async () => {
    const res = await fetchResource();
    console.log('Resource for icon:', res.servingDock);
    icon.value = res.servingDock != undefined ? 'build' : 
        res.averageSpeed != undefined ? 'local_shipping' : 'precision_manufacturing'
});

</script>

<template>
  <div>
    <sl-breadcrumb>
        <sl-breadcrumb-item><RouterLink to="/resources/dashboard" class="breadcrumb-link">Physical Resource Dashboard</RouterLink></sl-breadcrumb-item>
        <sl-breadcrumb-item><RouterLink to="/resources/search" class="breadcrumb-link">Search Physical Resources</RouterLink></sl-breadcrumb-item>
        <sl-breadcrumb-item>{{ resourceId }}</sl-breadcrumb-item>
    </sl-breadcrumb>

    <EntityView :fetch-function="fetchResource" v-slot="entity">
        <div>
            <div class="opposed">
                <div class="view-header">
                    <span class="material-icons icon" aria-hidden="true">{{icon}}</span>
                    <div>
                        <h2 class="title">{{ entity.element.description }}</h2>
                        <p class="subtitle">{{ entity.element.code }}</p>
                    </div>
                </div>
                <div class="button-group">
                    <RouterLink :to="`/resources/edit/${encodeURIComponent(entity.element.code)}`">
                        <sl-button variant="default" size="large">
                            <sl-icon slot="prefix" name="pencil"></sl-icon>
                            Edit Physical Resource
                        </sl-button>
                    </RouterLink>

                    <RouterLink :to="`/resources/deactivate/${encodeURIComponent(entity.element.code)}`">
                        <sl-button variant="danger" size="large">
                            <sl-icon slot="prefix" name="trash"></sl-icon>
                            Deactivate Physical Resource
                        </sl-button>
                    </RouterLink>
                </div>
    
            </div>
            <div class="viewing-content">
                <sl-card class="qual-main-info">
                    <p>Physical Resource information</p>
                    <div class="columns-2">
                        <div>
                            <div class="info-block">
                                <span class="label">Physical Resource code</span>
                                <p>{{ entity.element.code }}</p>
                            </div>
                        </div>
                        <div>
                            <div class="info-block">
                                <span class="label">Physical Resource name</span>
                                <p>{{ entity.element.description }}</p>
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

.button-group {
    display: flex;
    gap: 10px;
}

</style>