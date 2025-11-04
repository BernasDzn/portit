<script setup lang="ts">
import { useRoute } from 'vue-router';
import { AxiosHttpService } from '@/service/AxiosHttpService';
import type { Vessel } from '@/model/Vessel';
import EntityView from '@/components/crud/EntityView.vue';
import { QualificationService } from '@/service/QualificationService';
import type { Qualification } from '@/model/Qualifications';
import { PhysicalResourceService } from '@/service/PhysicalResourceService';
import { computed, onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';

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

const { t } = useI18n();

</script>

<template>
  <div>
    <sl-breadcrumb>
        <sl-breadcrumb-item><RouterLink to="/resources/dashboard" class="breadcrumb-link">{{ t('physicalResource.tabs.dashboard') }}</RouterLink></sl-breadcrumb-item>
        <sl-breadcrumb-item><RouterLink to="/resources/search" class="breadcrumb-link">{{ t('physicalResource.tabs.search') }}</RouterLink></sl-breadcrumb-item>
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
                            {{ t('physicalResource.tabs.edit') }}
                        </sl-button>
                    </RouterLink>

                    <RouterLink :to="`/resources/deactivate/${encodeURIComponent(entity.element.code)}`">
                        <sl-button variant="danger" size="large">
                            <sl-icon slot="prefix" name="trash"></sl-icon>
                            {{ t('physicalResource.actions.deactivate') }}
                        </sl-button>
                    </RouterLink>
                </div>
    
            </div>
            <div class="viewing-content">
                <sl-card class="qual-main-info">
                    <p>{{ t('physicalResource.infoTitle') }}</p>
                    <div class="columns-2">
                        <div>
                            <div class="info-block">
                                <span class="label">{{ t('physicalResource.fields.code.title') }}</span>
                                <p>{{ entity.element.code }}</p>
                            </div>
                        </div>
                        <div>
                            <div class="info-block">
                                <span class="label">{{ t('physicalResource.fields.description.title') }}</span>
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