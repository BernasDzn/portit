<script setup lang="ts">
import { ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRoute } from 'vue-router'
import TYPES from '@/inversify/types';
import { container } from '@/inversify.config';
import type { IVesselVisitExecutionService } from '@/service/IService/IVesselExecutionService';
import type { IVesselVisitNotificationService } from '@/service/IService/IVesselVisitNotificationService';
import type { VesselVisitExecution } from '@/model/VesselVisitExecution';
import type { VesselVisitNotification } from '@/model/VesselVisitNotification';
import EntityView from '@/components/crud/EntityView.vue';
import VesselVisitNotificationPrinter from '@/components/printers/VesselVisitNotificationPrinter.vue';
import BerthOperationDialog from '@/components/BerthOperationDialog.vue';
import { useAlerts } from '@/composables/alerts';

const vveService = container.get<IVesselVisitExecutionService>(TYPES.vesselVisitExecutionService);
const vvnService = container.get<IVesselVisitNotificationService>(TYPES.vesselVisitNotificationService);
const route = useRoute();
const notif = useAlerts();
const {t} = useI18n();

const relatedVVN = ref<VesselVisitNotification | null>(null);
const berthDialogRef = ref<any>(null);
const refreshKey = ref(0);
const id = route.params.id as string;

const fetchVesselExecution = async (): Promise<VesselVisitExecution> => {
    
    if (!id) {
        throw new Error('No vessel visit execution ID provided');
    }
    const vve = await vveService.getVesselVisitExecutionByVVN(id);
    
    try {
        relatedVVN.value = await vvnService.getVesselVisitNotificationById(vve.relatedVVN);
    } catch (error) {
        console.error('Failed to fetch related VVN:', error);
    }
    
    console.log(vve);
    return vve;
};

const formatDate = (date?: Date) => {
    if (!date) return '';
    const d = new Date(date);
    if (isNaN(d.getTime())) return '';
    return d.toLocaleString();
};

const openBerthDialog = () => {
    berthDialogRef.value?.open();
};

const onBerthUpdated = async () => {
    const vve = await vveService.getVesselVisitExecutionByVVN(id);
    const decisions = await vvnService.getNotificationDecisions(vve.relatedVVN);
    
    if(decisions && vve && decisions[decisions.length - 1].assignedDock.code !== vve.dock)
        notif.enqueueNotification(t('execution.berthOperation.differentThanPlanned'), 'warning');
    
    refreshKey.value++;
};

const getOperationProgress = (vve: any) => {
    if (!vve.operationsExecuted || vve.operationsExecuted.length === 0) {
        return { completed: 0, total: 0, percentage: 0 };
    }
    const total = vve.operationsExecuted.length;
    const completed = vve.operationsExecuted.filter((op: any) => op.status === 'Completed').length;
    const percentage = Math.round((completed / total) * 100);
    return { completed, total, percentage };
};

</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item>
                <RouterLink to="/vessel-visit-executions/dashboard" class="breadcrumb-link">{{ t('execution.tabs.dashboard') }}</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                <RouterLink to="/vessel-visit-executions/search" class="breadcrumb-link">{{ t('execution.tabs.search') }}</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                {{ t('execution.tabs.view') }}
            </sl-breadcrumb-item>
        </sl-breadcrumb>

        <EntityView :key="refreshKey" :fetch-function="fetchVesselExecution" v-slot="entity">
            <div class="opposed">
                <div class="view-header">
                    <span class="material-icons icon" aria-hidden="true">engineering</span>
                    <div>
                        <h2 class="title">{{ entity.element.code }}</h2>
                        <p class="subtitle">{{ entity.element.id }}</p>
                    </div>
                </div>
                <div style="display: flex; gap: 0.5rem; align-items: center;">
                    <!-- Operations Progress Indicator -->
                    <sl-tooltip :content="`${getOperationProgress(entity.element).completed} of ${getOperationProgress(entity.element).total} operations completed`" placement="bottom">
                        <div style="display: flex; align-items: center; gap: 0.5rem; padding: 0.5rem 1rem; background: var(--sl-color-neutral-100); border-radius: var(--sl-border-radius-medium);">
                            <sl-icon name="list-check" style="font-size: 1.25rem;"></sl-icon>
                            <span style="font-weight: 600; font-size: 1rem;">
                                {{ getOperationProgress(entity.element).completed }} / {{ getOperationProgress(entity.element).total }}
                            </span>
                            <sl-progress-bar 
                                :value="getOperationProgress(entity.element).percentage" 
                                style="width: 100px;"
                            ></sl-progress-bar>
                        </div>
                    </sl-tooltip>
                    
                    <RouterLink :to="`/vessel-visit-executions/update/${id}`" v-if="entity.element.status === 'Open'">
                        <sl-button>
                            <sl-icon name="pencil"></sl-icon>
                            {{ t('execution.tabs.update') }}
                        </sl-button>
                    </RouterLink>
                    <sl-button 
                        v-if="entity.element.status === 'Open'" 
                        variant="primary" 
                        @click="openBerthDialog"
                    >
                        <sl-icon name="pencil"></sl-icon>
                        {{ t('execution.berthOperation.button') }}
                    </sl-button>
                    <sl-tag :variant="entity.element.status === 'Open' ? 'success' : 'neutral'" size="large">
                        <sl-icon :name="entity.element.status === 'Open' ? 'unlock' : 'lock'"></sl-icon>
                        {{ entity.element.status }}
                    </sl-tag>
                </div>
            </div>

            <div class="viewing-content">
                <div class="top-row">
                    <sl-card class="info-card">
                        <p>{{ t('execution.infoTitle') }}</p>
                        <div class="info-grid">
                            <div class="info-block">
                                <span class="label">{{ t('execution.fields.code') }}</span>
                                <p>{{ entity.element.code }}</p>
                            </div>
                            <div class="info-block">
                                <span class="label">{{ t('execution.fields.relatedVVN') }}</span>
                                <p>{{ entity.element.relatedVVN }}</p>
                            </div>
                            <div class="info-block">
                                <span class="label">{{ t('execution.fields.status') }}</span>
                                <sl-tag :variant="entity.element.status === 'Open' ? 'success' : 'neutral'" style="width: fit-content;">
                                    {{ entity.element.status }}
                                </sl-tag>
                            </div>
                            <div class="info-block">
                                <span class="label">{{ t('execution.fields.createdBy') }}</span>
                                <sl-tooltip :content="entity.element.createdBy" placement="top">
                                    <p>{{ entity.element.createdBy.length > 20 ? entity.element.createdBy.slice(0, 20) + '...' : entity.element.createdBy }}</p>
                                </sl-tooltip>
                            </div>
                            <div class="info-block">
                                <span class="label">{{ t('execution.fields.dateOpen') }}</span>
                                <p>{{ formatDate(entity.element.dateOpen) }}</p>
                            </div>
                            <div class="info-block" v-if="entity.element.dateClosed">
                                <span class="label">{{ t('execution.fields.dateClosed') }}</span>
                                <p>{{ formatDate(entity.element.dateClosed) }}</p>
                            </div>

                            <div class="info-block" v-if="entity.element.dock">
                                <span class="label">{{ t('execution.fields.dock') }}</span>
                                <p>{{ entity.element.dock }}</p>
                            </div>
                            <div class="info-block" v-if="entity.element.berthTime">
                                <span class="label">{{ t('execution.fields.berthTime') }}</span>
                                <p>{{ formatDate(entity.element.berthTime) }}</p>
                            </div>
                        </div>
                    </sl-card>

                    <sl-card class="info-card" v-if="relatedVVN">
                        <p>{{ t('execution.fields.relatedVesselVisit') }}</p>
                        <VesselVisitNotificationPrinter 
                            class="listing-box"
                            :notification="relatedVVN" 
                            :link="`/vessel-visit-notifications/view/${entity.element.relatedVVN}`" 
                        />
                    </sl-card>
                </div>
            </div>
        </EntityView>

            <BerthOperationDialog 
                ref="berthDialogRef" 
                :related-v-v-n="id" 
                @berth-updated="onBerthUpdated" 
        />
    </div>
</template>

<style scoped>
.icon {
    margin: 0;
    margin-right: 1rem;
    font-size: 48px;
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
    grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
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

.no-data {
    text-align: center;
    color: var(--sl-color-neutral-500);
    padding: 2rem;
    margin-top: 1rem;
}

@media (max-width: 1024px) {
    .top-row {
        grid-template-columns: 1fr;
    }
}
</style>