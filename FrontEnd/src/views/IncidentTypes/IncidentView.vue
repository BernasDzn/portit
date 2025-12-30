<script setup lang="ts">
import { ref, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRoute, useRouter } from 'vue-router'
import TYPES from '@/inversify/types';
import { container } from '@/inversify.config';
import type { IIncidentService } from '@/service/IService/IIncidentService';
import type { IIncidentTypeService } from '@/service/IService/IIncidentTypeService';
import type { IncidentDto, UpdateIncidentDto } from '@/model/dto/IncidentDto';
import type { IncidentTypeDto } from '@/model/dto/IncidentTypeDto';
import EntityView from '@/components/crud/EntityView.vue';
import IncidentTypePrinter from '@/components/printers/IncidentTypePrinter.vue';
import { useAlerts } from '@/composables/alerts';
import VesselVisitExecutionPrinter from '@/components/printers/VesselVisitExecutionPrinter.vue';

const incidentService = container.get<IIncidentService>(TYPES.incidentService);
const incidentTypeService = container.get<IIncidentTypeService>(TYPES.incidentTypeService);
const route = useRoute();
const router = useRouter();
const notif = useAlerts();
const { t } = useI18n();

const relatedIncidentType = ref<IncidentTypeDto | null>(null);
const showCloseDialog = ref(false);
const endDateTime = ref('');
const refreshKey = ref(0);
const id = route.params.id as string;

const fetchIncident = async (): Promise<IncidentDto> => {
    if (!id) {
        throw new Error('No incident ID provided');
    }
    const incident = await incidentService.getIncidentByBid(id);
    
    try {
        relatedIncidentType.value = await incidentTypeService.getIncidentTypeById(incident.type.bid);
    } catch (error) {
        console.error('Failed to fetch related incident type:', error);
    }
    
    console.log(incident);
    return incident;
};

const formatDate = (date?: string) => {
    if (!date) return '';
    const d = new Date(date);
    if (isNaN(d.getTime())) return '';
    return d.toLocaleString();
};

const formatDateTime = (date?: string) => {
    if (!date) return '';
    const d = new Date(date);
    if (isNaN(d.getTime())) return '';
    return d.toISOString().slice(0, 16);
};

const isOngoing = (incident: IncidentDto) => {
    return !incident.endTime;
};

const getDuration = (incident: IncidentDto) => {
    const start = new Date(incident.startTime);
    const end = incident.endTime ? new Date(incident.endTime) : new Date();
    const diffMs = end.getTime() - start.getTime();
    const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24));
    const diffHours = Math.floor((diffMs % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    const diffMinutes = Math.floor((diffMs % (1000 * 60 * 60)) / (1000 * 60));
    
    if (diffDays > 0) {
        return `${diffDays}d ${diffHours}h ${diffMinutes}m`;
    }
    if (diffHours > 0) {
        return `${diffHours}h ${diffMinutes}m`;
    }
    return `${diffMinutes}m`;
};

const getSeverityVariant = (severity: string): string => {
    switch (severity) {
        case 'Critical': return 'danger';
        case 'Major': return 'warning';
        case 'Minor': return 'primary';
        default: return 'neutral';
    }
};

const openCloseDialog = (incident: IncidentDto) => {
    endDateTime.value = new Date().toISOString().slice(0, 16);
    showCloseDialog.value = true;
};

const closeIncident = async () => {

    const incident = await incidentService.getIncidentByBid(id);

    if (!endDateTime.value) {
        notif.enqueueNotification(
            t('incident.notifications.endTimeRequired'),
            notif.notificationTypes.DANGER
        );
        return;
    }

    const endDate = new Date(endDateTime.value);
    const startDate = new Date(incident.startTime);

    if (endDate < startDate) {
        notif.enqueueNotification(
            t('incident.notifications.endTimeBeforeStart'),
            notif.notificationTypes.DANGER
        );
        return;
    }

    try {
        const updateDto: UpdateIncidentDto = {
            type: incident.type.bid,
            startTime: incident.startTime,
            endTime: endDateTime.value,
            severity: incident.severity,
            description: incident.description,
            affectedVVECodes: incident.affectedVVECodes.map(v => v.code)
        };

        await incidentService.updateIncident(id, updateDto);
        
        notif.enqueueNotification(
            t('incident.notifications.closeSuccess'),
            notif.notificationTypes.SUCCESS
        );
        
        showCloseDialog.value = false;
        refreshKey.value++;
    } catch (error: any) {
        notif.enqueueNotification(
            t('incident.notifications.closeFailed') + ' ' + (error.response?.data?.message || error.message),
            notif.notificationTypes.DANGER
        );
    }
};

</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item>
                <RouterLink to="/incident-types/dashboard" class="breadcrumb-link">
                    {{ t('incident.tabs.dashboard') }}
                </RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                <RouterLink to="/incident/search" class="breadcrumb-link">
                    {{ t('incident.tabs.search') }}
                </RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                {{ t('incident.tabs.view') }}
            </sl-breadcrumb-item>
        </sl-breadcrumb>

        <EntityView :key="refreshKey" :fetch-function="fetchIncident" v-slot="entity">
            <div class="opposed">
                <div class="view-header">
                    <span class="material-icons icon" aria-hidden="true">warning</span>
                    <div>
                        <h2 class="title">{{ entity.element.bid }}</h2>
                        <p class="subtitle">{{ relatedIncidentType?.name || entity.element.type }}</p>
                    </div>
                </div>
                <div style="display: flex; gap: 0.5rem; align-items: center;">
                    <!-- Duration Indicator -->
                    <sl-tooltip 
                        :content="isOngoing(entity.element) ? t('incident.ongoingDuration') : t('incident.totalDuration')" 
                        placement="bottom"
                    >
                        <div style="display: flex; align-items: center; gap: 0.5rem; padding: 0.5rem 1rem; background: var(--sl-color-neutral-100); border-radius: var(--sl-border-radius-medium);">
                            <sl-icon name="clock" style="font-size: 1.25rem;"></sl-icon>
                            <span style="font-weight: 600; font-size: 1rem;">
                                {{ getDuration(entity.element) }}
                            </span>
                        </div>
                    </sl-tooltip>
                    
                    <RouterLink 
                        :to="`/incidents/edit/${id}`" 
                        v-if="isOngoing(entity.element)"
                    >
                        <sl-button>
                            <sl-icon name="pencil"></sl-icon>
                            {{ t('incident.tabs.edit') }}
                        </sl-button>
                    </RouterLink>

                    <sl-button 
                        v-if="isOngoing(entity.element)" 
                        variant="primary" 
                        @click="openCloseDialog(entity.element)"
                    >
                        <sl-icon name="check-circle"></sl-icon>
                        {{ t('incident.actions.resolve') }}
                    </sl-button>

                    <sl-tag 
                        :variant="isOngoing(entity.element) ? 'success' : 'neutral'" 
                        size="large"
                        :pulse="isOngoing(entity.element)"
                    >
                        {{ isOngoing(entity.element) ? t('incident.status.active') : t('incident.status.resolved') }}
                    </sl-tag>
                </div>
            </div>

            <div class="viewing-content">
                <div class="top-row">
                    <sl-card class="info-card">
                        <p>{{ t('incident.infoTitle') }}</p>
                        <div class="info-grid">
                            <div class="info-block">
                                <span class="label">{{ t('incident.fields.bid.title') }}</span>
                                <p>{{ entity.element.bid }}</p>
                            </div>
                            <div class="info-block">
                                <span class="label">{{ t('incident.fields.severity.title') }}</span>
                                <sl-badge :variant="getSeverityVariant(entity.element.severity)">
                                    {{ t(`incident.severity.${entity.element.severity}`) }}
                                </sl-badge>
                            </div>
                            <div class="info-block">
                                <span class="label">{{ t('incident.fields.startTime.title') }}</span>
                                <p>{{ formatDate(entity.element.startTime) }}</p>
                            </div>
                            <div class="info-block">
                                <span class="label">{{ t('incident.fields.endTime.title') }}</span>
                                <p>{{ entity.element.endTime ? formatDate(entity.element.endTime) : t('incident.ongoing') }}</p>
                            </div>
                            <div class="info-block">
                                <span class="label">{{ t('incident.fields.duration.title') }}</span>
                                <p>{{ getDuration(entity.element) }}</p>
                            </div>
                            <div class="info-block">
                                <span class="label">{{ t('incident.fields.createdBy.title') }}</span>
                                <sl-tooltip :content="entity.element.createdBy" placement="top">
                                    <p>{{ entity.element.createdBy.length > 30 ? entity.element.createdBy.slice(0, 30) + '...' : entity.element.createdBy }}</p>
                                </sl-tooltip>
                            </div>
                        </div>
                    </sl-card>

                    <sl-card class="info-card" v-if="relatedIncidentType">
                        <p>{{ t('incident.fields.type.title') }}</p>
                        <IncidentTypePrinter 
                            class="listing-box"
                            :incident-type="relatedIncidentType" 
                            :link="`/incident-types/view/${entity.element.type}`"
                            :show-details="false"
                        />
                    </sl-card>
                </div>

                <sl-card class="description-card">
                    <h3>
                        {{ t('incident.fields.description.title') }}
                    </h3>
                    <p class="description-text">{{ entity.element.description }}</p>
                </sl-card>
            </div>

            <br>

            <sl-card class="vve-card" v-if="entity.element.affectedVVECodes && entity.element.affectedVVECodes.length > 0">
                <h3>
                    {{ t('incident.fields.affectedVVECodes.title') }}
                </h3>
                <div class="vve-codes">
                    <!-- <sl-tag 
                        v-for="code in entity.element.affectedVVECodes" 
                        :key="code" 
                        variant="neutral"
                        size="medium"
                    >
                        {{ code }}
                    </sl-tag> -->
                    <VesselVisitExecutionPrinter v-for="code in entity.element.affectedVVECodes" :execution="code" link=""></VesselVisitExecutionPrinter>
                </div>
            </sl-card>
        </EntityView>

        <!-- Close Incident Dialog -->
        <sl-dialog 
            :label="t('incident.closeDialog.title')" 
            :open="showCloseDialog" 
            @sl-request-close="showCloseDialog = false"
        >
            <div style="padding: 1rem 0;">
                <sl-input 
                    type="datetime-local" 
                    :label="t('incident.fields.endTime.title')"
                    v-model="endDateTime"
                    style="width: 100%;"
                    required
                ></sl-input>
            </div>
            <sl-button slot="footer" variant="default" @click="showCloseDialog = false">
                {{ t('incident.closeDialog.cancel') }}
            </sl-button>
            <sl-button 
                slot="footer" 
                variant="primary" 
                @click="closeIncident()"
            >
                {{ t('incident.closeDialog.confirm') }}
            </sl-button>
        </sl-dialog>
    </div>
</template>
<style scoped>
.icon {
    margin: 0;
    margin-right: 1rem;
    font-size: 48px;
    color: white;
}

.viewing-content {
    display: flex;
    flex-direction: column;
    gap: 1rem;
    margin-top: 1rem;
}

.top-row {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 1rem;
}

.info-card {
    width: 100%;
}

.info-grid {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    gap: 1.5rem;
    margin-top: 1rem;
}

.info-block {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
}

.label {
    font-size: 0.875rem;
    color: var(--sl-color-neutral-600);
    font-weight: 500;
}

.description-card {
    width: 100%;
    height: fit-content;
}

.description-text {
    line-height: 1.6;
    color: var(--sl-color-neutral-700);
    white-space: pre-wrap;
}

.vve-card {
    width: 100%;
    height: fit-content;
}

.vve-codes {
    display: flex;
    flex-wrap: wrap;
    gap: 0.5rem;
}

@media (max-width: 1024px) {
    .top-row {
        grid-template-columns: 1fr;
    }
    
    .info-grid {
        grid-template-columns: repeat(2, 1fr);
    }
}

@media (max-width: 768px) {
    .info-grid {
        grid-template-columns: 1fr;
    }
}
</style>