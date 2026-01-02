<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRoute } from 'vue-router';
import TYPES from '@/inversify/types';
import { container } from '@/inversify.config';
import type { IVesselVisitExecutionService } from '@/service/IService/IVesselExecutionService';
import type { OperationWithStatus, VesselVisitExecution } from '@/model/VesselVisitExecution';
import { useAlerts } from '@/composables/alerts';
import type { IStaffService } from '@/service/IService/IStaffService';
import type { ITaskCategoryService } from '@/service/IService/ITaskCategoryService';
import type { Staff } from '@/model/Staff';
import type TaskCategoryDto from '@/model/dto/TaskCategoryDto';

const {t} = useI18n();
const route = useRoute();
const related_vvn_id = route.params.id as string
const notifications = useAlerts();
const vveService = container.get<IVesselVisitExecutionService>(TYPES.vesselVisitExecutionService);
const staffService = container.get<IStaffService>(TYPES.staffService);
const taskCategoryService = container.get<ITaskCategoryService>(TYPES.taskCategoryService);

const operations = ref<Array<OperationWithStatus>>([]);
const complementaryTasks = ref<Array<OperationWithStatus>>([]);

const availableStaff = ref<Staff[]>([]);
const availableCategories = ref<TaskCategoryDto[]>([]);

// Dialog state
const showStartDialog = ref(false);
const startDialogOp = ref<any>(null);
const startDialogTime = ref<string>('');
// Complete dialog state
const showCompleteDialog = ref(false);
const completeDialogOp = ref<any>(null);
const completeDialogTime = ref<string>('');

// Resources dialog state
const showResourcesDialog = ref(false);
const resourcesDialogOp = ref<any>(null);

// Add complementary task dialog state
const showAddTaskDialog = ref(false);
const newTaskCategory = ref<string>('');
const newTaskStaff = ref<string[]>([]);
const newTaskStartTime = ref<string>('');
const newTaskEndTime = ref<string>('');
const newTaskStatus = ref<'Started'>('Started');
const newTaskImpactedOps = ref<string[]>([]);

const resourcesDialogTitle = computed(() => {
  if (!resourcesDialogOp.value) return 'resources for operation';
  const op = resourcesDialogOp.value;
  const idx = operations.value.findIndex((o: any) => o.id === op.id);
  if (idx >= 0) return `Resources for operation #${idx + 1}`;
  if (op.id) return `Resources for operation #${op.id}`;
  return 'Resources  for operation';
});

// Helper function to extract error message from backend responses
function extractErrorMessage(e: unknown, defaultMessage: string): string {
  if (!e || typeof e !== 'object') {
    return defaultMessage + (e ? ' ' + String(e) : '');
  }
  
  const err = e as any;
  
  // Axios error shape: e.response.data may contain the error
  if (err.response?.data) {
    const data = err.response.data;
    // Check for various error message formats from backend
    if (typeof data === 'string') {
      return defaultMessage + ' ' + data;
    }
    if (data.message) {
      return defaultMessage + ' ' + data.message;
    }
    if (data.error) {
      return defaultMessage + ' ' + data.error;
    }
    if (data.title) {
      // .NET Problem Details format
      return defaultMessage + ' ' + data.title + (data.detail ? ': ' + data.detail : '');
    }
    if (data.errors) {
      // Validation errors format
      const errorMessages = Object.values(data.errors).flat().join('; ');
      return defaultMessage + ' ' + errorMessages;
    }
  }
  
  // Standard Error object
  if (err.message) {
    return defaultMessage + ' ' + err.message;
  }
  
  return defaultMessage;
}

// Get the complementary tasks that are blocking a given operation
function getBlockingTasks(operationId: string) {
  console.log('getBlockingTasks called for operationId:', operationId);
  const blocking = complementaryTasks.value.filter((task: any) => {
    const impactedOps = task.impactedOperations || [];
    const isBlocking = (task.status === 'Started' || task.status === 'Delayed') &&
      impactedOps.includes(operationId);
    console.log('Task:', {
      id: task.id,
      operationId: task.operation?.id,
      category: (task.operation.type || task.operation.operationType)?.category,
      status: task.status,
      impactedOps: impactedOps,
      checkingFor: operationId,
      includes: impactedOps.includes(operationId),
      isBlocking
    });
    return isBlocking;
  });
  console.log('Blocking tasks found:', blocking.length);
  return blocking;
}

// Get display name for a single operation by its ID
function getOperationDisplayName(operationId: string) {
  // Check in operations first
  const opIndex = operations.value.findIndex((o: any) => o.operation.id === operationId);
  if (opIndex >= 0) {
    return `Operation #${opIndex + 1}`;
  }
  
  // Check in complementary tasks
  const taskIndex = complementaryTasks.value.findIndex((o: any) => o.operation.id === operationId);
  if (taskIndex >= 0) {
    const taskType = (complementaryTasks.value[taskIndex].operation.type || complementaryTasks.value[taskIndex].operation.operationType).category;
    return `Task #${taskIndex + 1} (${taskType})`;
  }
  
  return 'Unknown Operation';
}

// Get display names for impacted operations
function getImpactedOperationsDisplay(impactedOps: string[]) {
  if (!impactedOps || impactedOps.length === 0) return 'None';
  
  console.log('Getting display for impacted ops:', impactedOps);
  const allOps = [...operations.value, ...complementaryTasks.value];
  console.log('All ops count:', allOps.length);
  
  const display = impactedOps.map(opId => {
    const op = allOps.find((o: any) => o.operation.id === opId);
    console.log('Looking for opId:', opId, 'found:', op ? 'yes' : 'no');
    if (!op) return `Unknown (${opId.substring(0, 8)})`;
    const opType = (op.operation.type || op.operation.operationType).category;
    return opType;
  }).join(', ');
  
  console.log('Display result:', display);
  return display;
}

function openStartDialog(op: any) {
  startDialogOp.value = op;
  const expected = op.operation.startTime instanceof Date ? op.operation.startTime : new Date(op.operation.startTime);
  const pad = (n: number) => n.toString().padStart(2, '0');
  startDialogTime.value = `${expected.getFullYear()}-${pad(expected.getMonth() + 1)}-${pad(expected.getDate())}T${pad(expected.getHours())}:${pad(expected.getMinutes())}`;
  showStartDialog.value = true;
}

function openCompleteDialog(op: any) {
  completeDialogOp.value = op;
  const actualEnd = op.operation.endTime instanceof Date ? op.operation.endTime : new Date(op.operation.endTime);
  const pad = (n: number) => n.toString().padStart(2, '0');
  completeDialogTime.value = `${actualEnd.getFullYear()}-${pad(actualEnd.getMonth() + 1)}-${pad(actualEnd.getDate())}T${pad(actualEnd.getHours())}:${pad(actualEnd.getMinutes())}`;
  showCompleteDialog.value = true;
}

function openResourcesDialog(op: any) {
  resourcesDialogOp.value = op;
  showResourcesDialog.value = true;
}

async function confirmStartOperation() {
  
	if (!startDialogOp.value) {
    notifications.enqueueNotification('No operation selected to start.', notifications.notificationTypes.DANGER);
    return;
  }

  try {
    const op = startDialogOp.value;
    const startTime = new Date(startDialogTime.value).toISOString();
    // Compose payload for backend
    const payload = {
      id: op.operation.id,
      type: op.operation.type.category, // always send the category code
      startTime,
      endTime: op.operation.endTime ? new Date(op.operation.endTime).toISOString() : startTime,
      resources: op.operation.resources.map(r => ({
        name: r.name,
        startTime,
        endTime: op.operation.endTime ? new Date(op.operation.endTime).toISOString() : startTime
      })),
      payload: op.operation.payload || {},
      impactedOperations: op.impactedOperations || []
    };
    const e = await vveService.startOperation(related_vvn_id, payload);
    console.log('Operation started:', e);
    notifications.enqueueNotification('Operation started successfully.', notifications.notificationTypes.SUCCESS);
  } catch (e) {
    console.error('Error starting operation:', e);
    const message = extractErrorMessage(e, 'Failed to start operation.');
    notifications.enqueueNotification(message, notifications.notificationTypes.DANGER);
    return;
  }
  showStartDialog.value = false;
  await fetchOperations();
}

async function confirmCompleteOperation() {
  if (!completeDialogOp.value) {
    notifications.enqueueNotification('No operation selected to complete.', notifications.notificationTypes.DANGER);
    return;
  }
  
  try {
    const op = completeDialogOp.value;
    const endTime = new Date(completeDialogTime.value);
    await vveService.completeOperation(related_vvn_id, op.operation.id, endTime);
    notifications.enqueueNotification('Operation completed successfully.', notifications.notificationTypes.SUCCESS);
    showCompleteDialog.value = false;
    await fetchOperations();
  } catch (e) {
    console.error('Error completing operation:', e);
    const message = extractErrorMessage(e, 'Failed to complete operation.');
    notifications.enqueueNotification(message, notifications.notificationTypes.DANGER);
  }
}

async function fetchOperations() {
	if (!related_vvn_id) return;
	try {
		const vve : VesselVisitExecution = await vveService.getVesselVisitExecutionByVVN(related_vvn_id);
		console.log('Raw VVE response:', vve);
		console.log('VVE type:', typeof vve);
		console.log('operationsExecuted:', vve.operationsExecuted);
		console.log('operationsExecuted type:', typeof vve.operationsExecuted);
		console.log('operationsExecuted length:', vve.operationsExecuted?.length);
		
		const allOps = vve.operationsExecuted || [];
		console.log('allOps:', allOps);
		
		// only load and unload operations
		operations.value = allOps.filter((op: any) => {
      // Handle both 'type' (from API) and 'operationType' (from model)
      const opType = op.operation.type || op.operation.operationType;
      if (!opType) {
        console.warn('Operation missing type:', op);
        return false;
      }
      const category = opType.category.toUpperCase();
      return ['LOAD', 'UNLOAD'].includes(category);
    });
		
		// Complementary tasks are those that are NOT load/unload (for separate display if needed)
		complementaryTasks.value = allOps.filter((op: any) => {
			// Handle both 'type' (from API) and 'operationType' (from model)
			const opType = op.operation.type || op.operation.operationType;
			if (!opType) {
				console.warn('Operation missing type:', op);
				return false;
			}
			const category = opType.category.toUpperCase();
			return !['LOAD', 'UNLOAD'].includes(category);
		});
		
		console.log('Fetched operations:', operations.value);
		console.log('Fetched complementary tasks:', complementaryTasks.value);
		complementaryTasks.value.forEach((task: any, idx: number) => {
			console.log(`Task ${idx}:`, {
				id: task.id,
				operationId: task.operation.id,
				category: (task.operation.type || task.operation.operationType)?.category,
				status: task.status,
				impactedOperations: task.impactedOperations
			});
		});
		operations.value.forEach((op: any, idx: number) => {
			console.log(`Operation ${idx}:`, {
				id: op.id,
				operationId: op.operation.id,
				category: (op.operation.type || op.operation.operationType)?.category,
				status: op.status,
				impactedOperations: op.impactedOperations
			});
		});
	} catch (e) {
		console.error('Error fetching operations:', e);
		const message = extractErrorMessage(e, 'Failed to load operations.');
		notifications.enqueueNotification(message, notifications.notificationTypes.DANGER);
		operations.value = [];
		complementaryTasks.value = [];
	}
}

function statusVariant(status: string) {
  switch (status) {
    case 'Pending': return 'neutral';
    case 'Started': return 'primary';
    case 'Delayed': return 'danger';
    case 'Completed': return 'success';
    default: return 'neutral';
  }
}

function statusIcon(status: string) {
  switch (status) {
    case 'Pending': return 'pause';
    case 'Started': return 'arrow-down';
    case 'Delayed': return 'arrow-right';
    case 'Completed': return 'check2';
    default: return 'question';
  }
}

function openAddTaskDialog() {
  const now = new Date();
  const pad = (n: number) => n.toString().padStart(2, '0');
  newTaskStartTime.value = `${now.getFullYear()}-${pad(now.getMonth() + 1)}-${pad(now.getDate())}T${pad(now.getHours())}:${pad(now.getMinutes())}`;
  newTaskEndTime.value = `${now.getFullYear()}-${pad(now.getMonth() + 1)}-${pad(now.getDate())}T${pad(now.getHours() + 1)}:${pad(now.getMinutes())}`;
  newTaskCategory.value = '';
  newTaskStaff.value = [];
  newTaskImpactedOps.value = [];
  newTaskStatus.value = 'Started';
  showAddTaskDialog.value = true;
}

async function confirmAddTask() {
  if (!newTaskCategory.value) {
    notifications.enqueueNotification('Please select a task category.', notifications.notificationTypes.DANGER);
    return;
  }
  
  if (newTaskStaff.value.length === 0) {
    notifications.enqueueNotification('Please assign at least one staff member.', notifications.notificationTypes.DANGER);
    return;
  }

  try {
    const startTime = new Date(newTaskStartTime.value).toISOString();
    const endTime = new Date(newTaskEndTime.value).toISOString();
    
    const payload = {
      id: new Date().getTime().toString(), // Temporary ID for new operation
      type: newTaskCategory.value, // Send just the category code string
      startTime,
      endTime,
      resources: newTaskStaff.value.map(staffName => ({
        name: staffName,
        type: 'Staff',
        startTime,
        endTime
      })),
      payload: {},
      impactedOperations: newTaskImpactedOps.value
    };
    
    await vveService.startOperation(related_vvn_id, payload);
    
    notifications.enqueueNotification('Complementary task added successfully.', notifications.notificationTypes.SUCCESS);
    showAddTaskDialog.value = false;
    await fetchOperations();
  } catch (e) {
    console.error('Error adding complementary task:', e);
    const message = extractErrorMessage(e, 'Failed to add complementary task.');
    notifications.enqueueNotification(message, notifications.notificationTypes.DANGER);
  }
}

function formatDate(dateStr: Date | string) {
  if (!dateStr) return '';
  const d = new Date(dateStr);
  return d.toLocaleString();
}

async function fetchStaffAndCategories() {
  try {
    // Fetch staff without pagination to avoid backend calculation issues
    const staffPage = await staffService.getStaffs();
    availableStaff.value = staffPage.items;
    
    const categoriesPage = await taskCategoryService.getAllTaskCategories();
    availableCategories.value = categoriesPage.items.filter(cat => 
      !['LOAD', 'UNLOAD'].includes(cat.category.toUpperCase())
    );
  } catch (e) {
    console.error('Error fetching staff and categories:', e);
    const message = extractErrorMessage(e, 'Failed to load staff and task categories.');
    notifications.enqueueNotification(message, notifications.notificationTypes.WARNING);
  }
}

onMounted(() => {
  fetchOperations();
  fetchStaffAndCategories();
});

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
        <RouterLink :to="`/vessel-visit-executions/` + related_vvn_id" class="breadcrumb-link">{{ related_vvn_id }}</RouterLink>
      </sl-breadcrumb-item>
      <sl-breadcrumb-item active>
        <span class="breadcrumb-link">{{ t('execution.tabs.update') }}</span>
      </sl-breadcrumb-item>
    </sl-breadcrumb>

    <div class="data-table">
      <h3>Operations</h3>
      <table class="dt-table" v-if="operations.length > 0">
        <thead>
          <tr>
            <th>#</th>
            <th>Status</th>
            <th>Type</th>
            <th>Start Time</th>
            <th>End Time</th>
            <th>Resources</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(op, idx) in operations" :key="op.id">
            <td>{{ idx + 1 }}</td>
            <td>
              <template v-if="op.status === 'Delayed'">
                <sl-tooltip v-if="getBlockingTasks(op.operation.id).length > 0" placement="top">
                  <div slot="content">
                    <strong>Blocked by:</strong><br>
                    <span v-for="(task, i) in getBlockingTasks(op.operation.id)" :key="i">
                      {{ (task.operation.type || task.operation.operationType).category }}<br>
                    </span>
                  </div>
                  <sl-tag :variant="statusVariant(op.status)" pill>
                    <sl-icon :name="statusIcon(op.status)" class="status-icon" style="margin-right:0.4em;" aria-hidden="true"></sl-icon>
                    {{ op.status }}
                  </sl-tag>
                </sl-tooltip>
                <sl-tag v-else :variant="statusVariant(op.status)" pill>
                  <sl-icon :name="statusIcon(op.status)" class="status-icon" style="margin-right:0.4em;" aria-hidden="true"></sl-icon>
                  {{ op.status }}
                </sl-tag>
              </template>
              <sl-tag v-else :variant="statusVariant(op.status)" pill>
                <sl-icon :name="statusIcon(op.status)" class="status-icon" style="margin-right:0.4em;" aria-hidden="true"></sl-icon>
                {{ op.status }}
              </sl-tag>
            </td>
            <td>
              <sl-tag variant="neutral" pill>{{ (op.operation.type || op.operation.operationType).category }}</sl-tag>
            </td>
            <td>
              <span v-if="op.status === 'Pending' || op.status === 'Delayed'" class="dt-expected">{{ formatDate(op.operation.startTime) }} (expected)</span>
              <span v-else>{{ formatDate(op.operation.startTime) }}</span>
            </td>
            <td>
              <span v-if="op.status !== 'Completed' " class="dt-expected">{{ formatDate(op.operation.endTime) }} (expected)</span>
              <span v-else>{{ formatDate(op.operation.endTime) }}</span>
            </td>
            <td>
              <sl-button size="small" variant="default" @click="openResourcesDialog(op)">
                <sl-icon name="eye"></sl-icon> {{ t('execution.view_resources') }}
              </sl-button>
            </td>
            <td>
              <!-- Actions by status -->
              <template v-if="op.status === 'Pending'">
                <sl-button size="small" variant="primary" @click="openStartDialog(op)">
                  <sl-icon name="play"></sl-icon> Start
                </sl-button>
              </template>
              <template v-else-if="op.status === 'Delayed'">
                <sl-tooltip content="Cannot start: Operation is blocked by complementary tasks" placement="top">
                  <sl-button size="small" variant="primary" disabled>
                    <sl-icon name="play"></sl-icon> Start
                  </sl-button>
                </sl-tooltip>
              </template>
              <template v-else-if="op.status === 'Started'">
                <sl-button size="small" variant="success" @click="openCompleteDialog(op)">
                  <sl-icon name="check2"></sl-icon> Complete
                </sl-button>
              </template>
              <!-- Completed: no actions -->
            </td>
          </tr>
        </tbody>
      </table>
      <div v-else class="dt-empty">
        No operations available.
      </div>
    </div>

    <div class="data-table" style="margin-top: 2rem;">
      <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1rem;">
        <h3>Complementary Tasks</h3>
        <sl-button variant="primary" @click="openAddTaskDialog">
          <sl-icon name="plus-circle"></sl-icon> Add Complementary Task
        </sl-button>
      </div>
      
      <table class="dt-table" v-if="complementaryTasks.length > 0">
        <thead>
          <tr>
            <th>#</th>
            <th>Status</th>
            <th>Type</th>
            <th>Start Time</th>
            <th>End Time</th>
            <th>Resources</th>
            <th>Impacting</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(op, idx) in complementaryTasks" :key="op.id">
            <td>{{ idx + 1 }}</td>
            <td>
              <sl-tag :variant="statusVariant(op.status)" pill>
                <sl-icon :name="statusIcon(op.status)" class="status-icon" style="margin-right:0.4em;" aria-hidden="true"></sl-icon>
                {{ op.status }}
              </sl-tag>
            </td>
            <td>
              <sl-tag variant="neutral" pill>{{ (op.operation.type || op.operation.operationType).category }}</sl-tag>
            </td>
            <td>
              <span v-if="op.status === 'Pending' || op.status === 'Delayed'" class="dt-expected">{{ formatDate(op.operation.startTime) }} (expected)</span>
              <span v-else>{{ formatDate(op.operation.startTime) }}</span>
            </td>
            <td>
              <span v-if="op.status !== 'Completed' " class="dt-expected">{{ formatDate(op.operation.endTime) }} (expected)</span>
              <span v-else>{{ formatDate(op.operation.endTime) }}</span>
            </td>
            <td>
              <sl-button size="small" variant="default" @click="openResourcesDialog(op)">
                <sl-icon name="eye"></sl-icon> {{ t('execution.view_resources') }}
              </sl-button>
            </td>
            <td>
              <template v-if="op.impactedOperations && op.impactedOperations.length > 0">
                <sl-tooltip placement="top">
                  <div slot="content">
                    <strong>Impacting:</strong><br>
                    <span v-for="(impactedId, i) in op.impactedOperations" :key="i">
                      {{ getOperationDisplayName(impactedId) }}<br>
                    </span>
                  </div>
                  <sl-tag variant="warning" size="small" pill>
                    <sl-icon name="exclamation-triangle"></sl-icon>
                    {{ op.impactedOperations.length }}
                  </sl-tag>
                </sl-tooltip>
              </template>
              <span v-else style="color: #666; font-size: 0.875rem;">None</span>
            </td>
            <td>
              <!-- Actions by status -->
              <template v-if="op.status === 'Pending' || op.status === 'Delayed'">
                <sl-button size="small" variant="primary" @click="openStartDialog(op)">
                  <sl-icon name="play"></sl-icon> Start
                </sl-button>
              </template>
              <template v-else-if="op.status === 'Started'">
                <sl-button size="small" variant="success" @click="openCompleteDialog(op)">
                  <sl-icon name="check2"></sl-icon> Complete
                </sl-button>
              </template>
            </td>
          </tr>
        </tbody>
      </table>
      <div v-else class="dt-empty">
        No complementary tasks added yet. Click "Add Complementary Task" to create one.
      </div>
    </div>

    <!-- Complete Operation Dialog -->
    <sl-dialog label="Complete Operation" :open="showCompleteDialog" @sl-after-hide="showCompleteDialog = false">
      <div style="display: flex; flex-direction: column; gap: 1em; min-width: 320px;">
        <sl-input
          id="complete-time-input"
          type="datetime-local"
          v-model="completeDialogTime"
          label="Actual End Time"
          help-text="Set the actual end time for this operation."
          filled
          style="width: 100%;"
        >
          <sl-icon name="clock" slot="prefix"></sl-icon>
        </sl-input>
      </div>
      <div slot="footer" style="display: flex; gap: 0.5em; justify-content: flex-end;">
        <sl-button variant="primary" @click="confirmCompleteOperation">
          <sl-icon name="check2"></sl-icon> Complete
        </sl-button>
        <sl-button variant="default" @click="showCompleteDialog = false">
          <sl-icon name="x"></sl-icon> Cancel
        </sl-button>
      </div>
    </sl-dialog>
    <!-- Start Operation Dialog -->
    <sl-dialog label="Start Operation" :open="showStartDialog" @sl-after-hide="showStartDialog = false">
      <div style="display: flex; flex-direction: column; gap: 1em; min-width: 320px;">
        <sl-input
          id="start-time-input"
          type="datetime-local"
          v-model="startDialogTime"
          label="Actual Start Time"
          help-text="Set the actual start time for this operation."
          filled
          style="width: 100%;"
        >
          <sl-icon name="clock" slot="prefix"></sl-icon>
        </sl-input>
      </div>
      <div slot="footer" style="display: flex; gap: 0.5em; justify-content: flex-end;">
        <sl-button variant="primary" @click="confirmStartOperation">
          <sl-icon name="play"></sl-icon> Start
        </sl-button>
        <sl-button variant="default" @click="showStartDialog = false">
          <sl-icon name="x"></sl-icon> Cancel
        </sl-button>
      </div>
    </sl-dialog>
    <!-- Resources Dialog -->
    <sl-dialog :label="resourcesDialogTitle" :open="showResourcesDialog" @sl-after-hide="showResourcesDialog = false" style="--width: 900px;">
      <div style="width:100%; max-width: 95vw; overflow:auto;">
        <table class="operations-table dt-table" style="width:100%; table-layout: auto;">
          <thead>
            <tr>
              <th>Resource Type</th>
              <th>Resource Name</th>
              <th>Start Time</th>
              <th>End Time</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="!(resourcesDialogOp && resourcesDialogOp.operation && resourcesDialogOp.operation.resources)" class="no-data">
              <td colspan="4">No resources</td>
            </tr>
            <tr v-for="(res, rIdx) in (resourcesDialogOp && resourcesDialogOp.operation ? resourcesDialogOp.operation.resources : [])" :key="rIdx">
              <td>{{ res.type || res.resourceType || (res.resource && res.resource.type) || '-' }}</td>
              <td>{{ res.name || (res.resource && res.resource.name) || '-' }}</td>
              <td>
                <template v-if="res.startTime">
                  <span v-if="resourcesDialogOp.status === 'Pending' || resourcesDialogOp.status === 'Delayed'" class="dt-expected">{{ formatDate(res.startTime) }} (expected)</span>
                  <span v-else>{{ formatDate(res.startTime) }}</span>
                </template>
                <span v-else class="dt-expected">---</span>
              </td>
              <td>
                <template v-if="res.endTime">
                  <span v-if="resourcesDialogOp.status !== 'Completed'" class="dt-expected">{{ formatDate(res.endTime) }} (expected)</span>
                  <span v-else>{{ formatDate(res.endTime) }}</span>
                </template>
                <span v-else class="dt-expected">---</span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      <div slot="footer" style="display: flex; gap: 0.5em; justify-content: flex-end;">
        <sl-button variant="default" @click="showResourcesDialog = false">
          <sl-icon name="x"></sl-icon> Close
        </sl-button>
      </div>
    </sl-dialog>
    <!-- Add Complementary Task Dialog -->
    <sl-dialog 
      label="Add Complementary Task" 
      :open="showAddTaskDialog" 
      @sl-after-hide="showAddTaskDialog = false"
      @sl-request-close="(event) => { if (event.detail.source === 'overlay') event.preventDefault(); }"
      style="--width: 600px;">
      <div style="display: flex; flex-direction: column; gap: 1.5em;">
        
        <!-- Category Selection -->
        <sl-select
          label="Task Category"
          placeholder="Select a task category"
          :value="newTaskCategory"
          @sl-change="(e) => newTaskCategory = e.target.value"
          filled
          required
          @sl-hide.stop
          @sl-after-hide.stop
        >
          <sl-option v-for="cat in availableCategories" :key="cat.category" :value="cat.category">
            {{ cat.name }} ({{ cat.category }})
          </sl-option>
        </sl-select>

        <!-- Staff Assignment -->
        <div>
          <label style="display: block; margin-bottom: 0.5rem; font-weight: 500;">Assigned Staff</label>
          <sl-select
            placeholder="Select staff members"
            :value="newTaskStaff"
            @sl-change="(e) => newTaskStaff = e.target.value"
            multiple
            clearable
            filled
            @sl-hide.stop
            @sl-after-hide.stop
          >
            <sl-option v-for="staff in availableStaff" :key="staff.mechanographicNumber" :value="staff.name">
              {{ staff.name }} ({{ staff.mechanographicNumber }})
            </sl-option>
          </sl-select>
        </div>

        <!-- Start Time -->
        <sl-input
          type="datetime-local"
          :value="newTaskStartTime"
          @sl-input="(e) => newTaskStartTime = e.target.value"
          label="Expected Start Time"
          filled
          required
        >
          <sl-icon name="clock" slot="prefix"></sl-icon>
        </sl-input>

        <!-- End Time -->
        <sl-input
          type="datetime-local"
          :value="newTaskEndTime"
          @sl-input="(e) => newTaskEndTime = e.target.value"
          label="Expected End Time"
          filled
          required
        >
          <sl-icon name="clock" slot="prefix"></sl-icon>
        </sl-input>

        <!-- Impacted Operations -->
        <div>
          <label style="display: block; margin-bottom: 0.5rem; font-weight: 500;">Impacted Operations (Optional)</label>
          <sl-select
            placeholder="Select operations that will be delayed"
            :value="newTaskImpactedOps"
            @sl-change="(e) => newTaskImpactedOps = e.target.value"
            multiple
            clearable
            filled
            @sl-hide.stop
            @sl-after-hide.stop
          >
            <sl-option v-for="(op, idx) in operations" :key="op.id" :value="op.operation.id">
              Operation #{{ idx + 1 }} - {{ (op.operation.type || op.operation.operationType).category }}
            </sl-option>
          </sl-select>
          <small style="color: #666;">Select operations that will be delayed by this complementary task</small>
        </div>

      </div>
      <div slot="footer" style="display: flex; gap: 0.5em; justify-content: flex-end;">
        <sl-button variant="primary" @click="confirmAddTask">
          <sl-icon name="plus-circle"></sl-icon> Create Task
        </sl-button>
        <sl-button variant="default" @click="showAddTaskDialog = false">
          <sl-icon name="x"></sl-icon> Cancel
        </sl-button>
      </div>
    </sl-dialog>
  </div>
</template>

<style scoped>
	.dt-table {
		width: 100%;
		border-collapse: collapse;
	}

	.dt-table thead {
		font-weight: bold;
		color: rgb(107, 105, 115);
	}

	.dt-table th,
	.dt-table td {
		border-bottom: 1px solid #ddd;
		padding: 1rem;
		text-align: left;
	}

	.dt-empty {
		text-align: center;
		color: #666;
		padding: 1rem;
	}

  .dt-expected {
    font-style: italic;
    color: #888;
  }

</style>