<script setup lang="ts">
import { useRoute, useRouter } from 'vue-router';
import { useAlerts } from '@/composables/alerts';
import { ref, onMounted, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { IOperationPlanService } from '@/service/IService/IOperationPlanService';
import type { IVesselVisitNotificationService } from '@/service/IService/IVesselVisitNotificationService';
import type { OperationPlanDto } from '@/model/dto/OperationPlanDto';
import type { VesselVisitNotification } from '@/model/VesselVisitNotification';
import GanttChart, { type GanttItem, type GanttRowConfig } from '@/components/GanttChart.vue';
import EntityView from '@/components/crud/EntityView.vue';
import VesselVisitNotificationPrinter from '@/components/printers/VesselVisitNotificationPrinter.vue';

const { t } = useI18n();
const route = useRoute();
const router = useRouter();
const notifications = useAlerts();

const planService = container.get<IOperationPlanService>(TYPES.operationPlanService);
const vvnService = container.get<IVesselVisitNotificationService>(TYPES.vesselVisitNotificationService);
const planId = String(route.params.id || '');

const relatedVVN = ref<VesselVisitNotification | null>(null);

const fetchPlan = async (): Promise<OperationPlanDto | undefined> => {
    const plan = await planService.getOperationPlanById(planId);
    if (plan) {
        try {
            relatedVVN.value = await vvnService.getVesselVisitNotificationById(plan.relatedVVN);
        } catch (error) {
            console.error('Failed to fetch related VVN:', error);
        }
    }
    return plan;
};

const getGanttItems = (plan: OperationPlanDto): GanttItem[] => {
    return plan.operationSchedule.map((op, index) => ({
        id: `${op.type}-${index}`,
        startTime: op.startTime,
        endTime: op.endTime,
        name: `${op.type} Operation`,
        group: op.type === 'Load' ? 'Loading Operations' : 'Unloading Operations'
    }));
};

const ganttRowConfigs: GanttRowConfig[] = [
    { name: 'Unloading Operations', color: '#FFA2A2' },
    { name: 'Loading Operations', color: '#7BF1A8' }
];

const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleString();
};
</script>

<template>
  <div>
    <sl-breadcrumb>
        <sl-breadcrumb-item><RouterLink to="/scheduling-dashboard" class="breadcrumb-link">{{ t('scheduling.tabs.dashboard') }}</RouterLink></sl-breadcrumb-item>
        <sl-breadcrumb-item><RouterLink to="/scheduling/plans-search" class="breadcrumb-link">{{ t('scheduling.tabs.search') }}</RouterLink></sl-breadcrumb-item>
        <sl-breadcrumb-item>{{ planId }}</sl-breadcrumb-item>
    </sl-breadcrumb>

    <EntityView :fetch-function="fetchPlan" v-slot="entity">
        <div>
            <div class="opposed">
                <div class="view-header">
                    <span class="material-icons icon" aria-hidden="true">calendar_month</span>
                    <div>
                        <h2 class="title">{{ t('operationPlan.title') }} - {{ entity.element.relatedVVN }}</h2>
                        <p class="subtitle">{{ entity.element.id }}</p>
                    </div>
                </div>
                <div>
                    <RouterLink :to="`/scheduling/plans-edit/${encodeURIComponent(entity.element.id)}`">
                        <sl-button slot="footer" variant="default" size="large">
                            <sl-icon slot="prefix" name="pencil"></sl-icon>
                            {{ t('operationPlan.tabs.edit') }}
                        </sl-button>
                    </RouterLink>
                </div>
            </div>
            <div class="viewing-content">
                <div style="display: grid; grid-template-columns: auto auto; gap: 1rem; width: 100%; margin-bottom: 1rem;">
					<sl-card class="info-card" style="flex: 50%; height: 100%;">
						<p>{{ t('operationPlan.infoTitle') }}</p>
						<div class="info-grid">
							<div class="info-block">
								<span class="label">{{ t('operationPlan.metadata.createdBy') }}</span>
								<p>{{ entity.element.metadata.createdBy }}</p>
							</div>
							<div class="info-block">
								<span class="label">{{ t('operationPlan.metadata.createdAt') }}</span>
								<p>{{ formatDate(entity.element.metadata.createdAt) }}</p>
							</div>
							<div class="info-block">
								<span class="label">{{ t('operationPlan.metadata.algorithmUsed') }}</span>
								<p>{{ entity.element.metadata.algorithmUsed }}</p>
							</div>
						</div>
					</sl-card>
					<sl-card class="info-card" style="flex: 50%; height: 100%;" v-if="relatedVVN">
						<p>{{ t('operationPlan.fields.relatedVVN.title') }}</p>
						<VesselVisitNotificationPrinter 
							class="listing-box"
							:notification="relatedVVN" 
							:link="`/vessel-visit-notifications/view/${entity.element.relatedVVN}`" 
						/>
					</sl-card>
				</div>
                <sl-card class="info-card" style="flex: 100%;">
                    <p>{{ t('operationPlan.schedule.title') }}</p>
                    <div style="margin-top: 1rem;">
                        <GanttChart
                            v-if="entity.element.operationSchedule.length > 0"
                            :items="getGanttItems(entity.element)"
                            :row-configs="ganttRowConfigs"
                            :enable-drag="false"
                        />
                        <p v-else class="no-data">{{ t('operationPlan.schedule.noOperations') }}</p>
                    </div>
                </sl-card>
            </div>
        </div>
    </EntityView>
  </div>
</template>

<style scoped> 
.icon {
    margin: 0;
    margin-right: 1rem;
}

.info-block {
    flex: 1 1 45%;
    min-width: 200px;
}

.viewing-content {
    display: flex;
    flex-wrap: wrap;
}

.no-data {
    text-align: center;
    color: var(--sl-color-neutral-500);
    padding: 2rem;
}
</style>