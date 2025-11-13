<script setup lang="ts">
import { useRoute } from 'vue-router';
import type { NotificationDecision, VesselVisitNotification } from '@/model/VesselVisitNotification';
import NoResults from '@/components/NoResults.vue';
import EntityView from '@/components/crud/EntityView.vue';
import { useI18n } from 'vue-i18n';
import EntityForm from '@/components/crud/EntityForm.vue';
import { computed, ref } from 'vue';
import EntityDropdown from '@/components/crud/EntityDropdown.vue';
import FormField from '@/components/crud/FormField.vue';
import { container } from '@/inversify.config';
import type { IVesselVisitNotificationService } from '@/service/IService/IVesselVisitNotificationService';
import TYPES from '@/inversify/types';
import type { IDockService } from '@/service/IService/IDockService';

const { t } = useI18n();
const route = useRoute();

const decision = ref<NotificationDecision>({
    status: null,
    reason: '',
    decisionDate: new Date(),
    officerID: null,
    assignedDockCode: null,
    isFinal: false 
});

const notificationService = container.get<IVesselVisitNotificationService>(TYPES.vesselVisitNotificationService);
const dockService = container.get<IDockService>(TYPES.dockService);
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

const isRejected = computed(() => {
    const statusNum = typeof decision.value.status === 'number' ? decision.value.status : Number(decision.value.status);
    return statusNum === 2;
});

const submitDecision = (obj: any) => {
    console.log(obj);
    return notificationService.createNotificationDecision(notificationId, obj);
}

const fetchNotification = async (): Promise<VesselVisitNotification | null> => {
    const n = await notificationService.getVesselVisitNotificationById(notificationId);
    return n;
};

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
                <RouterLink to="/vessel-visit-notifications/pending" class="breadcrumb-link">{{
                    t('notification.tabs.review') }}</RouterLink>
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
                </div>
                <div class="review-container">
                    <div class="review-form">
                        <EntityForm :object="decision" :submit-function="submitDecision">
                            <EntityDropdown
                                :name="t('notification.decision.status') + '*'"
                                :items="[
                                    { id: 1, name: t('notification.timeline.accepted') },
                                    { id: 2, name: t('notification.timeline.rejected') }
                                ]"
                                v-model="decision.status"
                                valueKey="id"
                                labelKey="name"
                                required
                            />

                            <FormField type="textarea" :name="t('notification.decision.reason.title') + (isRejected ? '*' : '')" v-model="decision.reason"
                               :placeholder="t('notification.decision.reason.placeholder')" :required="isRejected" />

                            <EntityDropdown
                                v-if="!isRejected"
                                :name="t('notification.decision.assignedDock.title') + '*'"
                                :placeholderText="t('notification.decision.assignedDock.placeholder')"
                                :fetch-function="() => dockService.getDocks()"
                                :fetch-on-mount="true"
                                v-model="decision.assignedDockCode"
                                valueKey="code"
                                labelKey="name"
                                required
                            />

                            <sl-checkbox v-if="isRejected" :label="t('notification.decision.isFinal')" v-model="decision.isFinal" @sl-change="decision.isFinal = $event.target.checked">
                                {{ t('notification.decision.isFinal') }}
                            </sl-checkbox>

                        </EntityForm>
                    </div>
                    <div class="viewing-content">
                        <div class="notification-row">
                            <sl-card class="notification-manifest">
                                <p>{{ t("notification.manifestInfo") }}</p>
                                <div>
                                    <sl-button @click="openLoadCargoManifest"
                                        :disabled="entity.element.loadCargoManifest.length == 0" size="medium" pill>{{
                                            t("notification.fields.openLoadManifest") }}</sl-button>
                                    <sl-button @click="openUnloadCargoManifest"
                                        :disabled="entity.element.unloadCargoManifest.length == 0" size="medium" pill>{{
                                            t("notification.fields.openUnloadManifest") }}</sl-button>
                                </div>
                            </sl-card>
                        </div>

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
                                            <p>{{ entity.element.isCargoHazardous ? t('common.yes') : t('common.no') }}
                                            </p>
                                        </div>
                                        <div class="info-block" v-if="entity.element.specialRequirements">
                                            <span class="label">{{ t('notification.fields.specialRequirements')
                                                }}</span>
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
                                                        {{ member.name }} ({{ member.nationality }}) {{ member.citizenID
                                                        }}
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
                        <p>{{ t("notification.to") }}: ({{ item.position.bay }}, {{ item.position.row }}, {{
                            item.position.tier }})
                        </p>
                    </div>
                </sl-card>

                <sl-button @click="closeUnloadManifest" slot="footer" variant="primary">Close</sl-button>
            </sl-drawer>
        </EntityView>
    </div>
</template>

<style scoped>
.review-container {
    display: flex;
    flex-direction: row;
    width: 100%;
    gap: 2rem;
}

.review-form {
    width: 50%;
}

.viewing-content {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    width: 50%;
}

.notification-row {
    display: flex;
    flex-direction: column;
    gap: 1rem;
}

.notification-progress {
    height: fit-content;
}

.notification-main-column {
    display: flex;
    flex-direction: column;
    gap: 1rem;
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
    flex-wrap: wrap;
}

.notification-manifest>div>sl-button {
    flex-grow: 1;
    min-width: 50px;
    max-width: 200px;
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
    --width: 40vw;
}

.timeline-expanded {
    display: flex;
    flex-direction: row;
    padding: 1rem;
    --point-width: 5rem;
    position: relative;
    padding-left: calc(var(--point-width) / 2);
    padding-right: calc(var(--point-width) / 2);
    gap: 2.5rem;
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
    left: calc(var(--point-width) / 2);
    right: calc(var(--point-width) / 2);
    top: 5.5rem;
    height: 2px;
    background-color: var(--sl-color-neutral-300);
    z-index: 0;
}

.timeline-icon {
    font-size: 32px;
    z-index: 1;
    background-color: white;
}

.accepted {
    color: var(--sl-color-success-600);
}

.rejected {
    color: var(--sl-color-danger-600);
}

.nothing {
    color: var(--sl-color-neutral-400);
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
</style>