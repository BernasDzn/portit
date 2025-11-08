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

const cargoTypes = [
    "Refrigerated goods",
    "General consumer products",
    "Electronics",
    "Hazmat",
    "Oversized industrial equipment",
    "Other"
]

const fetchNotification = async (): Promise<VesselVisitNotification | null> => {
    const n = await notificationService.getVesselVisitNotificationById(notificationId);
    console.log(n);
    return n;
};

const openInfoPopup = () => {
    const dialog = document.querySelector('#infoPopup') as any;
    dialog.show();
}

const openLoadCargoManifest = () => {

    const drawer = document.querySelector('#loadManifestDrawer') as any;
    drawer.show();
}

const openUnloadCargoManifest = () => {

    const drawer = document.querySelector('#unloadManifestDrawer') as any;
    drawer.show();
}

const closeLoadManifest = () => {
    const drawer = document.querySelector('#loadManifestDrawer') as any;
    drawer.hide();
}

const closeUnloadManifest = () => {
    const drawer = document.querySelector('#unloadManifestDrawer') as any;
    drawer.hide();
}

</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item>
                <RouterLink to="/vessel-visit-notifications/dashboard" class="breadcrumb-link">{{
                    t('notification.tabs.dashboard') }}</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                <RouterLink to="/vessel-visit-notifications/search" class="breadcrumb-link">{{
                    t('notification.tabs.search') }}</RouterLink>
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

                    <div class="notification-row">
                        <sl-card class="notification-progress">
                            <div class="timeline">
                                <div class="timeline-point">
                                    <span class="timeline-icon material-icons" aria-hidden="true">check_circle</span>
                                    <p>{{ t("notification.timeline.pending") }}</p>
                                </div>
                                <div class="timeline-point">
                                    <span class="timeline-icon material-icons" aria-hidden="true">check_circle</span>
                                    <p>{{ t("notification.timeline.submitted") }}</p>
                                </div>
                                <div class="timeline-point">
                                    <span class="timeline-icon material-icons" aria-hidden="true">check_circle</span>
                                    <p>{{ t("notification.timeline.inProgress") }}</p>
                                </div>
                                <div class="timeline-point">
                                    <span class="timeline-icon material-icons" aria-hidden="true">check_circle</span>
                                    <p>{{ t("notification.timeline.completed") }}</p>
                                </div>
                            </div>
                            <sl-button @click="openInfoPopup">{{ t("buttons.seeMore") }}</sl-button>
                        </sl-card>

                        <sl-card class="notification-manifest">
                            <p>{{ t("notification.manifestInfo") }}</p>
                            <div>
                                <sl-button @click="openLoadCargoManifest"
                                    :disabled="entity.element.loadCargoManifest.length == 0" size="medium" pill>{{t("notification.fields.openLoadManifest") }}</sl-button>
                                <sl-button @click="openUnloadCargoManifest"
                                    :disabled="entity.element.unloadCargoManifest.length == 0" size="medium" pill>{{t("notification.fields.openUnloadManifest") }}</sl-button>
                            </div>
                        </sl-card>
                    </div>

                    <sl-dialog id="infoPopup" label="Status Overview" class="dialog-overview">
                        <div class="timeline-expanded">
                            <div class="timeline-point" v-for="step in 15" :key="step">
                                <p>{{ t("notification.timeline.inProgress") }}</p>
                                <span class="timeline-icon material-icons" aria-hidden="true">check_circle</span>
                                <p>01/11 12:00</p>
                            </div>
                        </div>
                    </sl-dialog>

                    <div class="notification-main-column">
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
                                                <li v-for="member in entity.element.crewDetails.safetyOfficers"
                                                    :key="member.citizenID">
                                                    {{ member.name }} ({{ member.nationality }}) {{ member.citizenID }}
                                                </li>
                                            </ul>
                                        </div>
                                    </div>
                                    <div class="info-block">
                                        <span class="label"> Submitter </span>
                                        <p>{{ entity.element.submitter.name }} ({{
                                            entity.element.submitter.citizenshipId }})</p>
                                    </div>
                                </div>
                            </div>
                        </sl-card>
                    </div>
                </div>
            </template>
            <template v-else>
                <NoResults :message="t('notification.errors.noNotificationId')" />
            </template>

            <!-- Cargo manifest drawer -->
            <sl-drawer id="loadManifestDrawer" label="Drawer" class="drawer-overview">
                <h2>{{ t("notification.fields.loadCargoManifest") }}</h2>
                <sl-card class="manifest-item" v-for="item in entity.element.loadCargoManifest" :key="item.containerId">
                    <p>{{ item.container.description }} ({{ item.container.container }})</p>
                    <p>To: {{ item.area.nameCode }}</p>
                </sl-card>

                <sl-button @click="closeLoadManifest" slot="footer" variant="primary">Close</sl-button>
            </sl-drawer>

            <sl-drawer id="unloadManifestDrawer" label="Drawer" class="drawer-overview" style="--size: 35vw;">
                <h2>{{ t("notification.fields.unloadCargoManifest") }}</h2>
                <sl-card class="manifest-item" v-for="item in entity.element.unloadCargoManifest"
                    :key="item.containerId">
                    <div class="opposed">
                        <div>
                            <p class="title">{{ item.container.description }}</p>
                            <p class="subtitle">{{ item.container.containerNumber }}</p>
                        </div>
                        <sl-tag variant="neutral">{{ cargoTypes[item.container.cargoType] }}</sl-tag>
                    </div>
                    <div class="manifest-direction">
                        <p>{{ t("notification.from") }}: {{ item.area.nameCode }}</p>
                        <span class="material-icons" aria-hidden="true">arrow_right_alt</span>
                        <p>{{ t("notification.to") }}: ({{ item.position.bay }}, {{ item.position.row }},
                            {{ item.position.tier }})</p>
                    </div>
                </sl-card>

                <sl-button @click="closeUnloadManifest" slot="footer" variant="primary">Close</sl-button>
            </sl-drawer>
        </EntityView>
    </div>
</template>

<style scoped>
.viewing-content {
    display: flex;
    flex-direction: row;
    gap: 1rem;
}

.notification-row {
    display: flex;
    flex-direction: column;
    gap: 1rem;
    width: 30%;
}

.notification-progress {
    height: fit-content;
}

.notification-main-column {
    display: flex;
    flex-direction: column;
    gap: 1rem;
    width: 70%;
}

.notification-main-info {
    width: 100%;
}

.manifest-direction {
    display: flex;
    align-items: center;
    gap: 0.5rem;
}

.notification-manifest>div {
    display: flex;
    gap: 1rem;
    align-items: start;
}

.manifest-item {
    width: 100%;
    margin-bottom: 1rem;
}

.timeline {
    display: flex;
    flex-direction: column;
    padding: 1rem;
}

.dialog-overview {
    --width: 50vw;
}

.timeline-expanded {
    display: flex;
    flex-direction: row;
    padding: 1rem;
    padding-left: 3rem;
    padding-right: 3rem;
    gap: 4rem;
    width: fit-content;
}

.timeline-expanded>.timeline-point {
    flex-direction: column;
    align-items: center;
    text-align: center;
    font-size: small;
    width: 5rem;
}

.timeline-expanded::before {
    content: "";
    position: absolute;
    left: 4.5rem;
    right: 6.5rem;
    top: 5.5rem;
    height: 2px;
    background-color: var(--sl-color-neutral-300);
    z-index: 1;
}

.timeline-icon {
    font-size: 32px;
    color: var(--sl-color-success-600);

    z-index: 1;
    background-color: white;
}

.timeline-point p {
    color: var(--sl-color-neutral-500);
}

/* The vertical line */
.timeline::before {
    content: "";
    position: absolute;
    left: 4.8rem;
    top: 14rem;
    width: 2px;
    height: calc(100% - 30rem);
    background-color: var(--sl-color-neutral-300);
    z-index: 0;
}

.timeline-point {
    display: flex;
    align-items: center;
    gap: 1rem;
    position: relative;
    z-index: 1;
    margin-bottom: 1.5rem;
}

.timeline-point:last-child {
    margin-bottom: 0;
}

.timeline-icon {
    font-size: 32px;
    color: var(--sl-color-success-600);
    background-color: var(--sl-color-neutral-0);
    border-radius: 50%;
    z-index: 1;
}
</style>