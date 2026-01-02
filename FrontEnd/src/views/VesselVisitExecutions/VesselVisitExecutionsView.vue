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

const getOperationsSummary = (vve: any) => {
    if (!vve.operationsExecuted || vve.operationsExecuted.length === 0) {
        return {
            byStatus: { Pending: 0, Started: 0, Delayed: 0, Completed: 0 },
            byType: {} as Record<string, number>,
            complementaryTasks: 0,
            plannedOperations: 0
        };
    }

    const byStatus = {
        Pending: 0,
        Started: 0,
        Delayed: 0,
        Completed: 0
    };

    const byType: Record<string, number> = {};
    let complementaryTasks = 0;
    let plannedOperations = 0;

    vve.operationsExecuted.forEach((op: any) => {
        // Count by status
        byStatus[op.status as keyof typeof byStatus]++;

        // Count by type
        const opType = op.operation?.type?.category || 'Unknown';
        byType[opType] = (byType[opType] || 0) + 1;

        // Count complementary tasks (those that impact other operations)
        if (op.impactedOperations && op.impactedOperations.length > 0) {
            complementaryTasks++;
        } else {
            plannedOperations++;
        }
    });

    return { byStatus, byType, complementaryTasks, plannedOperations };
};

const getSortedOperations = (vve: any) => {
    if (!vve.operationsExecuted || vve.operationsExecuted.length === 0) {
        return [];
    }
    
    // Sort operations by start time
    return [...vve.operationsExecuted].sort((a, b) => {
        const aTime = new Date(a.operation.startTime).getTime();
        const bTime = new Date(b.operation.startTime).getTime();
        return aTime - bTime;
    });
};

const getOperationType = (op: any) => {
    return op.operation?.type?.category || op.operation?.operationType?.category || 'Unknown';
};

const isComplementaryTask = (op: any) => {
    return op.impactedOperations && op.impactedOperations.length > 0;
};

const getResourceCount = (op: any) => {
    return op.operation?.resources?.length || 0;
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

                <!-- Operations Summary Section -->
                <sl-card class="operations-summary-card">
                    <h3 style="margin: 0 0 1rem 0; display: flex; align-items: center; gap: 0.5rem;">
                        <sl-icon name="list-check" style="font-size: 1.5rem;"></sl-icon>
                        Operations Summary
                    </h3>
                    
                    <div class="summary-grid">
                        <!-- Status Breakdown -->
                        <div class="summary-section">
                            <h4>By Status</h4>
                            <div class="status-list">
                                <div class="status-item">
                                    <sl-badge variant="neutral" >Pending</sl-badge>
                                    <span class="status-count">{{ getOperationsSummary(entity.element).byStatus.Pending }}</span>
                                </div>
                                <div class="status-item">
                                    <sl-badge variant="primary" >Started</sl-badge>
                                    <span class="status-count">{{ getOperationsSummary(entity.element).byStatus.Started }}</span>
                                </div>
                                <div class="status-item">
                                    <sl-badge variant="warning" >Delayed</sl-badge>
                                    <span class="status-count">{{ getOperationsSummary(entity.element).byStatus.Delayed }}</span>
                                </div>
                                <div class="status-item">
                                    <sl-badge variant="success">Completed</sl-badge>
                                    <span class="status-count">{{ getOperationsSummary(entity.element).byStatus.Completed }}</span>
                                </div>
                            </div>
                        </div>

                        <!-- Operation Types -->
                        <div class="summary-section">
                            <h4>By Type</h4>
                            <div class="type-list">
                                <div 
                                    v-for="(count, type) in getOperationsSummary(entity.element).byType" 
                                    :key="type" 
                                    class="type-item"
                                >
                                    <span class="type-name">{{ type }}</span>
                                    <sl-badge variant="neutral">{{ count }}</sl-badge>
                                </div>
                                <div v-if="Object.keys(getOperationsSummary(entity.element).byType).length === 0" class="no-data-small">
                                    No operations
                                </div>
                            </div>
                        </div>

                        <!-- Task Categories -->
                        <div class="summary-section">
                            <h4>Task Categories</h4>
                            <div class="category-stats">
                                <div class="stat-item">
                                    <div class="stat-label">
                                        <sl-icon name="calendar-check"></sl-icon>
                                        Planned Operations
                                    </div>
                                    <div class="stat-value">{{ getOperationsSummary(entity.element).plannedOperations }}</div>
                                </div>
                                <div class="stat-item">
                                    <div class="stat-label">
                                        <sl-icon name="exclamation-triangle"></sl-icon>
                                        Complementary Tasks
                                    </div>
                                    <div class="stat-value">{{ getOperationsSummary(entity.element).complementaryTasks }}</div>
                                </div>
                            </div>
                        </div>
                    </div>
                </sl-card>

                <!-- Operations Timeline Table -->
                <sl-card class="operations-table-card">
                    <h3 style="margin: 0 0 1rem 0; display: flex; align-items: center; gap: 0.5rem;">
                        <sl-icon name="clock-history" style="font-size: 1.5rem;"></sl-icon>
                        Operations Timeline
                    </h3>
                    
                    <div class="table-container" v-if="entity.element.operationsExecuted && entity.element.operationsExecuted.length > 0">
                        <table class="operations-table">
                            <thead>
                                <tr>
                                    <th>#</th>
                                    <th>Type</th>
                                    <th>Category</th>
                                    <th>Status</th>
                                    <th>Start Time</th>
                                    <th>End Time</th>
                                    <th>Resources</th>
                                    <th>Impacted Operations</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="(op, idx) in getSortedOperations(entity.element)" :key="op.id || idx">
                                    <td>{{ idx + 1 }}</td>
                                    <td>
                                        <sl-badge :variant="isComplementaryTask(op) ? 'warning' : 'primary'">
                                            {{ isComplementaryTask(op) ? 'Complementary' : 'Planned' }}
                                        </sl-badge>
                                    </td>
                                    <td>
                                        <strong>{{ getOperationType(op) }}</strong>
                                    </td>
                                    <td>
                                        <sl-badge 
                                            :variant="
                                                op.status === 'Completed' ? 'success' :
                                                op.status === 'Started' ? 'primary' :
                                                op.status === 'Delayed' ? 'warning' :
                                                'neutral'
                                            "
                                        >
                                            {{ op.status }}
                                        </sl-badge>
                                    </td>
                                    <td>{{ formatDate(op.operation.startTime) }}</td>
                                    <td>{{ formatDate(op.operation.endTime) }}</td>
                                    <td>
                                        <sl-tooltip :content="getResourceCount(op) + ' resource(s) assigned'" placement="top">
                                            <span style="display: flex; align-items: center; gap: 0.25rem;">
                                                <sl-icon name="people"></sl-icon>
                                                {{ getResourceCount(op) }}
                                            </span>
                                        </sl-tooltip>
                                    </td>
                                    <td>
                                        <span v-if="isComplementaryTask(op)">
                                            <sl-badge variant="danger">{{ op.impactedOperations.length }}</sl-badge>
                                        </span>
                                        <span v-else style="color: var(--sl-color-neutral-400);">—</span>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <div v-else class="no-data">
                        No operations available
                    </div>
                </sl-card>
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

.operations-summary-card {
    margin-top: 1rem;
    width: 100%;
}

.summary-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
    gap: 2rem;
}

.summary-section h4 {
    margin: 0 0 1rem 0;
    font-size: 0.875rem;
    color: var(--sl-color-neutral-600);
    font-weight: 600;
    text-transform: uppercase;
    letter-spacing: 0.05em;
}

.status-list,
.type-list,
.category-stats {
    display: flex;
    flex-direction: column;
    gap: 0.75rem;
}

.status-item,
.type-item {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 0.5rem;
    background: var(--sl-color-neutral-50);
    border-radius: var(--sl-border-radius-medium);
}

.status-count {
    font-weight: 600;
    font-size: 1.125rem;
    color: var(--sl-color-neutral-700);
}

.type-name {
    font-weight: 500;
    color: var(--sl-color-neutral-700);
}

.stat-item {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 0.75rem;
    background: var(--sl-color-neutral-50);
    border-radius: var(--sl-border-radius-medium);
}

.stat-label {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    font-weight: 500;
    color: var(--sl-color-neutral-700);
}

.stat-value {
    font-weight: 700;
    font-size: 1.5rem;
    color: var(--sl-color-primary-600);
}

.no-data-small {
    text-align: center;
    color: var(--sl-color-neutral-500);
    font-style: italic;
    padding: 1rem;
}

.operations-table-card {
    margin-top: 1rem;
    width: 100%;
}

.table-container {
    overflow-x: auto;
}

.operations-table {
    width: 100%;
    border-collapse: collapse;
    font-size: 0.9rem;
}

.operations-table thead {
    background: var(--sl-color-neutral-100);
}

.operations-table th {
    padding: 0.75rem;
    text-align: left;
    font-weight: 600;
    color: var(--sl-color-neutral-700);
    border-bottom: 2px solid var(--sl-color-neutral-200);
}

.operations-table td {
    padding: 0.75rem;
    border-bottom: 1px solid var(--sl-color-neutral-100);
    vertical-align: middle;
}

.operations-table tbody tr:hover {
    background: var(--sl-color-neutral-50);
}

@media (max-width: 1024px) {
    .top-row {
        grid-template-columns: 1fr;
    }
    
    .summary-grid {
        grid-template-columns: 1fr;
    }
    
    .operations-table {
        font-size: 0.8rem;
    }
    
    .operations-table th,
    .operations-table td {
        padding: 0.5rem;
    }
}
</style>