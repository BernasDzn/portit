<script setup lang="ts">
import { useRoute } from 'vue-router';
import { AxiosHttpService } from '@/service/AxiosHttpService';
import { VesselVisitNotificationService } from '@/service/VesselVisitNotificationService';
import type { VesselVisitNotification } from '@/model/VesselVisitNotification';
import NoResults from '@/components/NoResults.vue';
import EntityView from '@/components/crud/EntityView.vue';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();
const route = useRoute();

const http = new AxiosHttpService();
const notificationService = new VesselVisitNotificationService(http);
const notificationId = route.params.notificationId && route.params.notificationId !== 'undefined'
    ? decodeURIComponent(route.params.notificationId as string)
    : '';

const fetchNotification = async (): Promise<VesselVisitNotification | null> => {
    return await notificationService.getVesselVisitNotificationById(notificationId);
};
</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item>
                <RouterLink to="/vessel-visit-notifications/dashboard" class="breadcrumb-link">{{ t('notification.tabs.dashboard') }}</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                <RouterLink to="/vessel-visit-notifications/search" class="breadcrumb-link">{{ t('notification.tabs.search') }}</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ notificationId || t('notification.errors.noNotificationId') }}</sl-breadcrumb-item>
        </sl-breadcrumb>
        <EntityView :fetch-function="fetchNotification" v-slot="entity">
            <template v-if="notificationId">
                <div class="opposed">
                    <div class="view-header">
                        <span class="material-icons icon" aria-hidden="true">notifications</span>
                        <div>
                            <h2 class="title">{{ entity.element.vessel.name }}</h2>
                            <p class="subtitle">{{ entity.element.vessel.imoNumber }}</p>
                        </div>
                    </div>
                    <RouterLink :to="`/notifications/edit/${encodeURIComponent(entity.element.notificationId)}`">
                        <sl-button variant="default" size="large">
                            <sl-icon slot="prefix" name="pencil"></sl-icon>
                            {{ t('notification.tabs.edit') }}
                        </sl-button>
                    </RouterLink>
                </div>
                <div class="viewing-content">
                    <sl-card class="notification-main-info">
                        <p>{{ t('notification.infoTitle') }}</p>
                        <div class="columns-2">
                            <div>
                                <div class="info-block">
                                    <span class="label">{{ t('notification.fields.expectedArrival') }}</span>
                                    <p>{{ entity.element.expectedArrival.split("T")[0].split(".")[0] }}
                                        {{ entity.element.expectedArrival.split("T")[1].split(".")[0] }}
                                    </p>
                                </div>
                                <div class="info-block">
                                    <span class="label">{{ t('notification.fields.expectedDeparture') }}</span>
                                    <p>{{ entity.element.expectedDeparture.split("T")[0].split(".")[0] }}
                                        {{ entity.element.expectedDeparture.split("T")[1].split(".")[0] }}
                                    </p>
                                </div>
                                <div class="info-block">
                                    <span class="label">{{ t('notification.fields.isCargoHazardous') }}</span>
                                    <p>{{ entity.element.isCargoHazardous ? t('common.yes') : t('common.no') }}</p>
                                </div>
                                <div class="info-block" v-if="entity.element.specialRequirements">
                                    <span class="label">{{ t('notification.fields.specialRequirements') }}</span>
                                    <p>{{ entity.element.specialRequirements }}</p>
                                </div>
                            </div>
                            <div>
                                <div class="info-block" v-if="entity.element.crewDetails">
                                    <span class="label">{{ t('notification.fields.captain') }}</span>
                                    <p>{{ entity.element.crewDetails.captain.value }}</p>
                                </div>
                                <div class="info-block" v-if="entity.element.crewDetails">
                                    <span class="label">{{ t('notification.fields.totalCrewMembers') }}</span>
                                    <p>{{ entity.element.crewDetails.totalCrewMembers }}</p>
                                </div>
                                <div class="info-block" v-if="entity.element.crewDetails">
                                    <span class="label">{{ t('notification.fields.crewMembers') }}</span>
                                    <div>
                                        <ul>
                                            <li v-for="member in entity.element.crewDetails.safetyOfficers" :key="member.citizenID">
                                                {{ member.name }} ({{ member.nationality }}) {{ member.citizenID }}
                                            </li>
                                        </ul>
                                    </div>
                                    <div v-if="entity.element.crewDetails.safetyOfficers.length === 0">
                                        {{ t('notification.none') }}
                                    </div>
                                </div>
                            </div>
                        </div>
                    </sl-card>
                    <sl-card class="notification-cargo-info">
                        <p>{{ t('notification.cargoTitle') }}</p>
                        <div>
                            <h4>{{ t('notification.fields.loadCargoManifest') }}</h4>
                            <ul>
                                <li v-for="(item, idx) in entity.element.loadCargoManifest" :key="idx">
                                    <span class="label">{{ t('notification.fields.position') }}:</span>
                                    {{ item.position.bay }}/{{ item.position.row }}/{{ item.position.tier }},
                                    <span class="label">{{ t('notification.fields.area') }}:</span>
                                    {{ item.area.nameCode }} ({{ item.area.location }}),
                                    <span class="label">{{ t('notification.fields.container') }}:</span>
                                    {{ item.container.containerNumber }} - {{ item.container.description }}
                                </li>
                                <div v-if="entity.element.loadCargoManifest.length === 0">
                                    {{ t('notification.none') }}
                                </div>
                            </ul>
                            <h4>{{ t('notification.fields.unloadCargoManifest') }}</h4>
                            <ul>
                                <li v-for="(item, idx) in entity.element.unloadCargoManifest" :key="idx">
                                    <span class="label">{{ t('notification.fields.position') }}:</span>
                                    {{ item.position.bay }}/{{ item.position.row }}/{{ item.position.tier }},
                                    <span class="label">{{ t('notification.fields.area') }}:</span>
                                    {{ item.area.nameCode }} ({{ item.area.location }}),
                                    <span class="label">{{ t('notification.fields.container') }}:</span>
                                    {{ item.container.containerNumber }} - {{ item.container.description }}
                                </li>
                                <div v-if="entity.element.unloadCargoManifest.length === 0">
                                    {{ t('notification.none') }}
                                </div>
                            </ul>
                        </div>
                    </sl-card>
                    <sl-card class="notification-vessel-info">
                        <p>{{ t('notification.vesselTitle') }}</p>
                        <div class="columns-2">
                            <div>
                                <div class="info-block">
                                    <span class="label">{{ t('vessel.fields.name.title') }}</span>
                                    <p>{{ entity.element.vessel.name }}</p>
                                </div>
                                <div class="info-block">
                                    <span class="label">{{ t('vessel.fields.vesselType.title') }}</span>
                                    <p>{{ entity.element.vessel.type.name }}</p>
                                </div>
                                <div class="info-block">
                                    <span class="label">{{ t('vessel.fields.owner.title') }}</span>
                                    <p>{{ entity.element.vessel.owner.name }}</p>
                                </div>
                            </div>
                            <div>
                                <div class="info-block">
                                    <span class="label">{{ t('vessel.fields.imoNumber.title') }}</span>
                                    <p>{{ entity.element.vessel.imoNumber }}</p>
                                </div>
                            </div>
                        </div>
                    </sl-card>
                </div>
            </template>
            <template v-else>
                <NoResults :message="t('notification.errors.noNotificationId')" />
            </template>
        </EntityView>
    </div>
</template>

<style scoped>
.notification-main-info {
    width: 60%;
}
.notification-cargo-info {
    width: 100%;
    margin-top: 1rem;
}
.notification-vessel-info {
    width: 40%;
    margin-top: 1rem;
}
</style>