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
import QualificationPrinter from '@/components/printers/QualificationPrinter.vue';
import DockPrinter from '@/components/printers/DockPrinter.vue';
import WorkShiftPrinter from '@/components/printers/WorkShiftPrinter.vue';

const route = useRoute();

const http = new AxiosHttpService();
const physicalResourceService = new PhysicalResourceService(http);
const resourceId = decodeURIComponent((route.params.code ?? '') as string);

const fetchResource = async (): Promise<any | null> => {
    return await physicalResourceService.getPhysicalResourceById(resourceId);
};

const icon = ref<string | undefined>('build');
const resourceType = ref<number | null>(null);

onMounted(async () => {
    const res = await fetchResource();
    // console.log('Resource for icon:', res.servingDock);
    resourceType.value = res.servingDock != undefined ? 0 : 
        res.averageSpeed != undefined ? 2 : 1;

    icon.value = (['build', 'precision_manufacturing', 'local_shipping'])[resourceType.value];
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
                            <div v-if="resourceType == 0 || resourceType == 1" class="info-block">
                                <span class="label">Physical Resource Lifting Capacity</span>
                                <p>{{ entity.element.liftingCapacity }}</p>
                            </div>
                            <div v-if="resourceType == 2" class="info-block">
                                <span class="label">Physical Resource Average Speed</span>
                                <p>{{ entity.element.averageSpeed }} km/h</p>
                            </div>
                            <div v-if="resourceType == 2" class="info-block">
                                <span class="label">Physical Resource Containers per Trip</span>
                                <p>{{ entity.element.containersPerTrip }}</p>
                            </div>
                        </div>
                        <div>
                            <div class="info-block">
                                <span class="label">{{ t('physicalResource.fields.description.title') }}</span>
                                <p>{{ entity.element.description }}</p>
                            </div>
                            <div v-if="resourceType == 0 || resourceType == 1" class="info-block">
                                <span class="label">Physical Resource Containers per Hour</span>
                                <p>{{ entity.element.containersPerHour }}</p>
                            </div>
                            <div v-if="resourceType == 2" class="info-block">
                                <span class="label">Physical Resource Maximum Load Capacity</span>
                                <p>{{ entity.element.maxLoadCapacity }} kg</p>
                            </div>
                        </div>
                    </div>
                </sl-card>
                <sl-card class="info-card">
                    <p>Statistics</p>
                    <div class="view-statistics-overview">
                        <div>
                            <p class="view-statistic-data">{{ entity.element.setupTimeInMinutes }}m</p>
                            <p>Setup time</p>
                        </div>
                        <div>
                            <p class="view-statistic-data">{{ entity.element.qualifications.length }}</p>
                            <p>Qualifications</p>
                        </div>
                    </div>
                    <p class="info-row">
                        <span class="label">Status:</span> 
                        <span>
                            <sl-tag 
                                :variant="['success', 'warning', 'danger'][entity.element.status]"
                                >
                                {{ ["Available", "Maintenance", "Out of service"][entity.element.status] }}
                            </sl-tag>
                        </span>
                    </p>
                </sl-card>

                <div></div>

                <sl-card class="info-card" style="flex: 100%;">
                    <p>Qualifications</p>
                    <div class="info-grid">
                        <div v-for="qualification in entity.element.qualifications" :key="qualification.idCode">
                            <QualificationPrinter class="listing-box" :qualification="qualification" :link="`/qualifications/view/${qualification.idCode}`" />
                        </div>
                    </div>
                </sl-card>
                
                <sl-card v-if="resourceType == 0" class="info-card dock-info">
                    <p>Serving dock</p>
                    <div class="info-grid">
                        <DockPrinter class="listing-box" :dock="entity.element.servingDock" :link="`/docks/view/${entity.element.servingDock.code}`" />
                    </div>
                </sl-card>
            </div>

            <br>

            <sl-card class="info-card" style="flex: 100%;">
                <p>Operational Window:</p>
                <WorkShiftPrinter
                    :op_window="entity.element.operationalWindow"
                />
            </sl-card>
        </div>
    </EntityView>
  </div>
</template>

<style scoped> 
.qual-main-info {
    width: 60%;
}

.button-group {
    display: flex;
    gap: 10px;
}

.dock-info div {
    width: 100%;
}

.viewing-content{
    display: flex;
    flex-wrap: wrap;
}

.view-statistics-overview {

    display: flex;
    gap: 40px;
    justify-content: space-around;
}

</style>