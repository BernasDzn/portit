<script setup lang="ts">
import { useI18n } from 'vue-i18n';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { IOperationPlanService } from '@/service/IService/IOperationPlanService';
import type { ISchedulingService } from '@/service/IService/ISchedulingService';
import type { IVesselVisitNotificationService } from '@/service/IService/IVesselVisitNotificationService';
import OperationPlanPrinter from '@/components/printers/OperationPlanPrinter.vue';
import VesselVisitNotificationPrinter from '@/components/printers/VesselVisitNotificationPrinter.vue';
import CalendarEvents from '@/components/crud/CalendarEvents.vue';
import { computed, onMounted, ref } from 'vue';
import type { Filter, Page } from '@/model/Page';
import ListingBox from '@/components/crud/ListingBox.vue';
import type { VesselVisitNotification } from '@/model/VesselVisitNotification';
import type { OperationPlanDto, OperationPlanFilter } from '@/model/dto/OperationPlanDto';
import { useAlerts } from '@/composables/alerts';
import { useRouter } from 'vue-router';
import EntityDropdown from '@/components/crud/EntityDropdown.vue';
import Loading from '@/components/Loading.vue';

const operationPlanService = container.get<IOperationPlanService>(TYPES.operationPlanService);
const schedulingService = container.get<ISchedulingService>(TYPES.schedulingService);
const vvnService = container.get<IVesselVisitNotificationService>(TYPES.vesselVisitNotificationService);
const notifications = useAlerts();

const { t, locale } = useI18n();
const router = useRouter();

const operationPlans = ref<Page<OperationPlanDto>>({ items: [], 
    pageCount: 0, pageNumber: 0, pageSize: 0
});
const plansByDate = ref<{ date: string; plans: OperationPlanDto[] }[]>([]);
const selectedDate = ref(new Date());
const events = ref<Array<{ title: string, start: string }>>([]);
const unplannedVVNIds = ref<string[]>([]);
const unplannedVVNs = ref<VesselVisitNotification[]>([]);
const isLoadingUnplanned = ref(false);

// Regeneration state
const showRegenerationModal = ref(false);
const selectedDayForRegeneration = ref<string | null>(null);
const selectedAlgorithm = ref<string>('auto');
const isRegenerating = ref(false);

// Regenerate all state
const showRegenerateAllModal = ref(false);
const selectedAlgorithmAll = ref<string>('auto');
const isRegeneratingAll = ref(false);

const algorithmList = [
    { label: "Auto (Recommended) - Selects best algorithm based on problem size", value: "auto" },
    { label: "Optimal Scheduling (Exhaustive)", value: "optimal" },
    { label: "Greedy Scheduling (Fast - EDD)", value: "greedy" },
    { label: "Genetic Scheduling (Generational)", value: "genetic" }
];

const fetchOperationPlans = async (filtering?: Filter<OperationPlanFilter>): Promise<Page<OperationPlanDto>> => {
    const plans = await operationPlanService.getAllOperationPlans(filtering);
    operationPlans.value = plans;
    return plans;
};

const fetchPlansByDate = async () => {
    plansByDate.value = await operationPlanService.groupOperationPlansByDate();
    
    // Create calendar events
    events.value = plansByDate.value.map(group => ({
        title: `${group.plans.length} ${t('operationPlan.title')}${group.plans.length > 1 ? 's' : ''}`,
        start: group.date
    }));
};

const plansOnDate = computed(() => {
    if (!selectedDate.value) return [];
    const selectedDateStr = selectedDate.value.toISOString().split('T')[0];
    const group = plansByDate.value.find(g => g.date === selectedDateStr);
    return group ? group.plans : [];
});

const fetchUnplannedVVNs = async () => {
    isLoadingUnplanned.value = true;
    try {
        // Get the list of unplanned VVN IDs
        unplannedVVNIds.value = await schedulingService.getUnplannedVVNs();
        
        // Fetch the full VVN details for each ID
        const vvnPromises = unplannedVVNIds.value.map(id => vvnService.getVesselVisitNotificationById(id));
        unplannedVVNs.value = await Promise.all(vvnPromises);
    } catch (error) {
        console.error('Error fetching unplanned VVNs:', error);
        unplannedVVNs.value = [];
    } finally {
        isLoadingUnplanned.value = false;
    }
};

onMounted(async () => {
    await fetchOperationPlans();
    await fetchPlansByDate();
    await fetchUnplannedVVNs();
});

const filterDefinition = ref({
    startDate: {
        type: 'date',
        label: t('operationPlan.filters.startDate')
    },
    endDate: {
        type: 'date',
        label: t('operationPlan.filters.endDate')
    }
});

// Group unplanned VVNs by date
const unplannedByDate = computed(() => {
    const grouped: Record<string, any[]> = {};
    
    unplannedVVNs.value.forEach(vvn => {
        const arrivalDate = new Date(vvn.expectedArrival).toISOString().split('T')[0];
        if (!grouped[arrivalDate]) {
            grouped[arrivalDate] = [];
        }
        grouped[arrivalDate].push(vvn);
    });
    
    return Object.entries(grouped)
        .map(([date, vvns]) => ({ date, vvns }))
        .sort((a, b) => a.date.localeCompare(b.date));
});

const openRegenerationModal = (date: string) => {
    selectedDayForRegeneration.value = date;
    selectedAlgorithm.value = 'auto';
    const dialog = document.getElementById('regeneration-modal') as any;
    dialog.show();
};

const closeRegenerationModal = () => {
    const dialog = document.getElementById('regeneration-modal') as any;
    dialog.hide();
    selectedDayForRegeneration.value = null;
    selectedAlgorithm.value = 'auto';
};

const confirmRegeneration = async () => {
    if (!selectedDayForRegeneration.value || !selectedAlgorithm.value) return;
    
    isRegenerating.value = true;
    
    try {
        const dayDate = new Date(selectedDayForRegeneration.value);
        const result = await schedulingService.scheduleForDay(
            dayDate,
            selectedAlgorithm.value,
            1
        );
        
        notifications.enqueueNotification(
            result.message || `Regeneration request queued for ${selectedDayForRegeneration.value}`,
            notifications.notificationTypes.SUCCESS
        );
        
        closeRegenerationModal();
        
        // Refresh unplanned VVNs to remove any that might have been queued
        router.push({ name: 'Scheduling Queue' });
    } catch (error: any) {
        notifications.enqueueNotification(
            `Error queueing regeneration: ${error.message || error}`,
            notifications.notificationTypes.DANGER
        );
    } finally {
        isRegenerating.value = false;
    }
};

const openRegenerateAllModal = () => {
    selectedAlgorithmAll.value = 'auto';
    const dialog = document.getElementById('regenerate-all-modal') as any;
    dialog.show();
};

const closeRegenerateAllModal = () => {
    const dialog = document.getElementById('regenerate-all-modal') as any;
    dialog.hide();
    selectedAlgorithmAll.value = 'auto';
};

const confirmRegenerateAll = () => {
    if (!selectedAlgorithmAll.value || unplannedByDate.value.length === 0) return;
    
    // Hide first modal and show warning modal
    closeRegenerateAllModal();
    const warningDialog = document.getElementById('regenerate-danger-modal') as any;
    warningDialog.show();
};

const executeRegenerateAll = async () => {
    isRegeneratingAll.value = true;
    
    try {
        // Queue a request for each unique date
        const requests = unplannedByDate.value.map(async (group) => {
            const dayDate = new Date(group.date);
            return schedulingService.scheduleForDay(
                dayDate,
                selectedAlgorithmAll.value,
                1
            );
        });
        
        await Promise.all(requests);
        
        notifications.enqueueNotification(
            `Successfully queued regeneration for ${unplannedByDate.value.length} day(s)`,
            notifications.notificationTypes.SUCCESS
        );
        
        const warningDialog = document.getElementById('regenerate-danger-modal') as any;
        warningDialog.hide();
        selectedAlgorithmAll.value = 'auto';
        
        // Navigate to queue to see all requests
        router.push({ name: 'Scheduling Queue' });
    } catch (error: any) {
        notifications.enqueueNotification(
            `Error queueing regeneration: ${error.message || error}`,
            notifications.notificationTypes.DANGER
        );
    } finally {
        isRegeneratingAll.value = false;
    }
};

const cancelRegenerateWarning = () => {
    const warningDialog = document.getElementById('regenerate-danger-modal') as any;
    warningDialog.hide();
    selectedAlgorithmAll.value = 'auto';
};

</script>

<template>
  <div>

    <sl-breadcrumb>
      <sl-breadcrumb-item><RouterLink to="/scheduling-dashboard" class="breadcrumb-link">{{ t('scheduling.tabs.dashboard') }}</RouterLink></sl-breadcrumb-item>
      <sl-breadcrumb-item>{{ t('scheduling.tabs.search') }}</sl-breadcrumb-item>
    </sl-breadcrumb>

    <header>
        <h1 class="title">{{ t('scheduling.tabs.search') }}</h1>
        <p class="subtitle">{{ t('scheduling.subtitles.search') }}</p>

        <sl-tab-group>
            <sl-tab slot="nav" panel="general">{{ t('notification.tabs.general') }}</sl-tab>
            <sl-tab slot="nav" panel="byDate">{{ t('notification.tabs.byDate') }}</sl-tab>
            <sl-tab slot="nav" panel="unplanned">
                {{ t('operationPlan.tabs.unplannedVVNs') }}
                <sl-badge 
                    v-if="unplannedVVNs.length > 0" 
                    variant="danger" pill
                    style="margin-left: 0.5rem;"
                    >
                    {{ unplannedVVNs.length }}
                </sl-badge>
            </sl-tab>

            <sl-tab-panel name="general">
                <ListingBox listing-style="listing-triples" :fetch-function="fetchOperationPlans" v-slot="{elements}" :filter-definition="filterDefinition">
                    <li v-for="(plan, index) in operationPlans.items" :key="index" class="link">
                        <OperationPlanPrinter :operation-plan="plan" :link="`/scheduling/plans-view/${plan.id}`" />
                    </li>
                </ListingBox>
            </sl-tab-panel>

            <sl-tab-panel name="byDate">
                <div class="calendar-events">
                    <CalendarEvents class="calendar" :events="events" v-model="selectedDate" />
                    <div class="mt-4">
                        <h2 class="subtitle">{{ t('operationPlan.plansOnDate', { date: selectedDate.toDateString() }) }}</h2>
                        <ul class="plans-list" v-if="plansOnDate.length > 0">
                            <li v-for="(plan, index) in plansOnDate" :key="index">
                                <OperationPlanPrinter class="listing-box" 
                                    :operation-plan="plan"
                                    :link="`/scheduling/plans-view/${plan.id}`"
                                />
                            </li>
                        </ul>
                        <p v-else>{{ t('operationPlan.noPlansOnDate') }}</p>
                    </div>
                </div>
            </sl-tab-panel>

            <sl-tab-panel name="unplanned">
                <div class="unplanned-section">
                    <Loading v-if="isLoadingUnplanned" />
                    <div v-else>
                        <div v-if="unplannedByDate.length > 0">
                            <div class="regenerate-all-header">
                                <p class="info-message">
                                    <sl-icon name="info-circle" style="margin-right: 0.5rem;"></sl-icon>
                                    Found {{ unplannedVVNs.length }} VVN(s) without plans across {{ unplannedByDate.length }} day(s)
                                </p>
                                <sl-button 
                                    variant="warning" 
                                    size="medium"
                                    @click="openRegenerateAllModal"
                                >
                                    <sl-icon slot="prefix" name="lightning-charge"></sl-icon>
                                    Generate All Missing Plans
                                </sl-button>
                            </div>
                            <div v-for="group in unplannedByDate" :key="group.date" class="date-group">
                                <div class="date-group-header">
                                    <div class="date-with-badge">
                                        <h3 class="date-title">{{ new Date(group.date).toDateString() }}</h3>
                                    </div>
                                    <sl-button 
                                        variant="primary" 
                                        size="medium"
                                        @click="openRegenerationModal(group.date)"
                                    >
                                        <sl-icon slot="prefix" name="arrow-clockwise"></sl-icon>
                                        Generate Plans for This Day
                                    </sl-button>
                                </div>
                                <ul class="vvn-list">
                                    <li v-for="vvn in group.vvns" :key="vvn.notificationId">
                                        <VesselVisitNotificationPrinter 
                                            :notification="vvn"
                                            :link="`/vessel-visit-notifications/view/${vvn.notificationId}`"
                                        />
                                    </li>
                                </ul>
                            </div>
                        </div>
                        <p v-else class="no-data">{{ t('operationPlan.unplannedVVNs.noUnplanned') }}</p>
                    </div>
                </div>
            </sl-tab-panel>
        </sl-tab-group>

        <sl-dialog 
            id="regeneration-modal"
            :label="`Generate Plans for ${selectedDayForRegeneration}`"
            class="regeneration-dialog"
        >
            <div class="modal-content">
                <sl-alert variant="danger" open>
                    <sl-icon slot="icon" name="exclamation-triangle"></sl-icon>
                    <strong>Warning:</strong> Generating plans will <strong>overwrite any existing operation plans</strong> for this day. 
                    This action cannot be undone.
                </sl-alert>

                <div class="algorithm-selection">
                    <EntityDropdown
                        :name="t('scheduling.fields.algorithm.title') || 'Select Scheduling Algorithm'"
                        v-model="selectedAlgorithm"
                        :items="algorithmList"
                        valueKey="value"
                        labelKey="label"
                        :placeholderText="t('scheduling.fields.algorithm.placeholder') || 'Choose an algorithm'"
                        :disabled="isRegenerating"
                        required
                    />
                </div>

                <div class="regeneration-info">
                    <p><strong>Selected Day:</strong> {{ selectedDayForRegeneration }}</p>
                    <p><strong>Algorithm:</strong> {{ algorithmList.find(a => a.value === selectedAlgorithm)?.label }}</p>
                </div>
            </div>

            <div slot="footer">
                <sl-button 
                    variant="default" 
                    @click="closeRegenerationModal"
                    :disabled="isRegenerating"
                    style="margin-right: 1rem;  "
                >
                    Cancel
                </sl-button>
                <sl-button 
                    variant="danger" 
                    @click="confirmRegeneration"
                    :loading="isRegenerating"
                >
                    <sl-icon slot="prefix" name="arrow-clockwise"></sl-icon>
                    Confirm Generation
                </sl-button>
            </div>
        </sl-dialog>

        <!-- Regenerate All Modal -->
        <sl-dialog 
            id="regenerate-all-modal"
            label="Generate All Missing Plans"
            class="regeneration-dialog"
        >
            <div class="modal-content">
                <div class="affected-days">
                    <h4>Affected Days:</h4>
                    <ul class="days-list">
                        <li v-for="group in unplannedByDate" :key="group.date">
                            <sl-icon name="calendar-date" style="margin-right: 0.5rem;"></sl-icon>
                            {{ new Date(group.date).toDateString() }}
                            <sl-badge variant="warning" pill style="margin-left: 0.5rem;">{{ group.vvns.length }} VVN(s)</sl-badge>
                        </li>
                    </ul>
                </div>

                <div class="algorithm-selection">
                    <EntityDropdown
                        :name="t('scheduling.fields.algorithm.title') || 'Select Scheduling Algorithm'"
                        v-model="selectedAlgorithmAll"
                        :items="algorithmList"
                        valueKey="value"
                        labelKey="label"
                        :placeholderText="t('scheduling.fields.algorithm.placeholder') || 'Choose an algorithm'"
                        :disabled="isRegeneratingAll"
                        required
                    />
                </div>

                <div class="regeneration-info">
                    <p><strong>Algorithm:</strong> {{ algorithmList.find(a => a.value === selectedAlgorithmAll)?.label }}</p>
                    <p class="metadata-info">
                        Each generation request will be queued separately. You can review and accept/reject them individually from the Scheduling Queue.
                    </p>
                </div>
            </div>

            <div slot="footer">
                <sl-button 
                    variant="default" 
                    @click="closeRegenerateAllModal"
                    style="margin-right: 1rem;"
                >
                    Cancel
                </sl-button>
                <sl-button 
                    variant="primary" 
                    @click="confirmRegenerateAll"
                    :disabled="!selectedAlgorithmAll"
                >
                    <sl-icon slot="prefix" name="arrow-right"></sl-icon>
                    Continue
                </sl-button>
            </div>
            </sl-dialog>

            <!-- Warning Modal for Regenerate All -->
            <sl-dialog id="regenerate-danger-modal" label="Confirm generation" style="--width: 600px;">
                <div style="padding: 1rem;">
                    <div style="background-color: var(--sl-color-danger-50); padding: 1.5rem; border-radius: var(--sl-border-radius-medium); border-left: 4px solid var(--sl-color-danger-600); margin-bottom: 1.5rem;">
                        <div style="display: flex; align-items: start; gap: 1rem;">
                            <sl-icon name="exclamation-triangle" style="font-size: 2rem; color: var(--sl-color-danger-600); flex-shrink: 0;"></sl-icon>
                            <div>
                                <h3 style="margin: 0 0 0.5rem 0; color: var(--sl-color-danger-900); font-size: 1.1rem;">This action is irreversible</h3>
                                <p style="margin: 0; color: var(--sl-color-danger-800); line-height: 1.6;">
                                    Generating operation plans will <strong>permanently overwrite</strong> any existing plans for the selected days.
                                    This action cannot be undone.
                                </p>
                            </div>
                        </div>
                    </div>

                    <div style="background-color: var(--sl-color-neutral-50); padding: 1rem; border-radius: var(--sl-border-radius-medium); margin-bottom: 1rem;">
                        <p style="margin: 0 0 0.5rem 0; font-weight: 600;">You are about to generate plans for:</p>
                        <ul style="margin: 0.5rem 0 0 1.5rem; color: var(--sl-color-neutral-700);">
                            <li><strong>{{ unplannedByDate.length }}</strong> days</li>
                            <li><strong>{{ unplannedVVNs.length }}</strong> Vessel Visit Notifications</li>
                            <li>Using <strong>{{ algorithmList.find(a => a.value === selectedAlgorithmAll)?.label }}</strong> algorithm</li>
                        </ul>
                    </div>

                    <p style="margin: 1rem 0 0 0; font-size: 0.9rem; color: var(--sl-color-neutral-600);">
                        Do you want to proceed with this operation?
                    </p>
                </div>

                <div slot="footer" style="display: flex; justify-content: flex-end; gap: 0.75rem;">
                    <sl-button variant="default" @click="cancelRegenerateWarning" :disabled="isRegeneratingAll">
                        Cancel
                    </sl-button>
                    <sl-button variant="danger" @click="executeRegenerateAll" :loading="isRegeneratingAll">
                        <sl-icon slot="prefix" name="exclamation-octagon"></sl-icon>
                        Yes, Generate All
                    </sl-button>
                </div>
            </sl-dialog>
        </header>
    </div>
</template>

<style scoped> 
.link {
  text-decoration: none;
  color: inherit;
}

.plans-grid {
    margin-top: 1rem;
}

.plans-list {
    list-style-type: none;
    padding: 0;
    margin: 0;
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(400px, 1fr));
    gap: 1rem;
}

.no-data {
    text-align: center;
    color: var(--sl-color-neutral-500);
    padding: 2rem;
}

.calendar-events {
    margin-top: 1rem;
    margin-left: auto;
    margin-right: auto;
    display: flex;
    align-items: flex-start;
    justify-content: center;
    gap: 5rem;
}

.calendar {
    min-width: 350px;
    height: 600px;
}

.mt-4 {
    flex: 1;
    height: 600px;
    overflow-y: auto;
    scrollbar-width: none;
    padding: 0 20px;
    -ms-overflow-style: none;
}

.mt-4::-webkit-scrollbar {
    display: none;
}

.mt-4 ul {
    list-style-type: none;
    padding: 0;
    margin: 0;
}

.mt-4 li {
    margin-bottom: 1rem;
}

.unplanned-section {
    margin-top: 1rem;
    padding: 1rem;
}

.regenerate-all-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: 1rem;
    margin-bottom: 2rem;
    padding: 1.25rem;
    background: linear-gradient(135deg, var(--sl-color-warning-50) 0%, var(--sl-color-warning-100) 100%);
    border: 2px solid var(--sl-color-warning-300);
    border-radius: var(--sl-border-radius-large);
}

.info-message {
    display: flex;
    align-items: center;
    margin: 0;
    font-weight: 500;
    color: var(--sl-color-warning-800);
}

.unplanned-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 1.5rem;
    padding: 1rem;
    background-color: var(--sl-color-neutral-100);
    border-radius: var(--sl-border-radius-medium);
}

.info-text {
    margin: 0;
    color: var(--sl-color-neutral-700);
}

.vvn-list {
    list-style-type: none;
    padding: 0;
    margin: 0;
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(400px, 1fr));
    gap: 1rem;
}

.vvn-list li {
    text-decoration: none;
}

.vvn-list a {
    text-decoration: none;
    color: inherit;
}

.date-group {
    margin-bottom: 2rem;
    padding: 1.5rem;
    border: 1px solid var(--sl-color-neutral-200);
    border-radius: var(--sl-border-radius-medium);
    background-color: var(--sl-color-neutral-50);
}

.date-group-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: 1rem;
    margin-bottom: 1.5rem;
    padding-bottom: 1rem;
    border-bottom: 2px solid var(--sl-color-neutral-200);
}

.date-with-badge {
    display: flex;
    align-items: center;
    gap: 0.75rem;
}

.date-title {
    margin: 0;
    font-size: 1.25rem;
    font-weight: 600;
    color: var(--sl-color-neutral-900);
    flex: 1;
}

.regeneration-dialog::part(panel) {
    max-width: 800px;
    max-height: 90vh;
}

.modal-content {
    display: flex;
    flex-direction: column;
    gap: 1.5rem;
}

.algorithm-selection {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
}

.algorithm-selection label {
    font-weight: 600;
    color: var(--sl-color-neutral-700);
}

.regeneration-info {
    padding: 1rem;
    background-color: var(--sl-color-neutral-100);
    border-radius: var(--sl-border-radius-medium);
}

.regeneration-info p {
    margin: 0.5rem 0;
}

.affected-days {
    background-color: var(--sl-color-neutral-50);
    padding: 1rem;
    border-radius: var(--sl-border-radius-medium);
    border: 1px solid var(--sl-color-neutral-200);
}

.affected-days h4 {
    margin: 0 0 0.75rem 0;
    color: var(--sl-color-neutral-700);
    font-size: 0.95rem;
    font-weight: 600;
}

.days-list {
    list-style: none;
    padding: 0;
    margin: 0;
    display: flex;
    flex-direction: column;
    gap: 0.75rem;
}

.days-list li {
    display: flex;
    align-items: center;
    padding: 0.65rem 1rem;
    background-color: var(--sl-color-neutral-0);
    border-radius: var(--sl-border-radius-medium);
    font-size: 0.875rem;
    border: 1px solid var(--sl-color-neutral-200);
    white-space: nowrap;
}

.metadata-info {
    margin-top: 1rem;
    padding-top: 1rem;
    border-top: 1px solid var(--sl-color-neutral-200);
    font-size: 0.9rem;
    color: var(--sl-color-neutral-600);
    font-style: italic;
}
</style>