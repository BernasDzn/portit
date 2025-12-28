<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRoute } from 'vue-router';
import TYPES from '@/inversify/types';
import { container } from '@/inversify.config';
import type { IVesselVisitExecutionService } from '@/service/IService/IVesselExecutionService';
import type { OperationWithStatus, VesselVisitExecution } from '@/model/VesselVisitExecution';
import { useAlerts } from '@/composables/alerts';

const {t} = useI18n();
const route = useRoute();
const related_vvn_id = route.params.id as string
const notifications = useAlerts();
const vveService = container.get<IVesselVisitExecutionService>(TYPES.vesselVisitExecutionService);

const operations = ref<Array<OperationWithStatus>>([]);

// Dialog state
const showStartDialog = ref(false);
const startDialogOp = ref<any>(null);
const startDialogTime = ref<string>('');
// Complete dialog state
const showCompleteDialog = ref(false);
const completeDialogOp = ref<any>(null);
const completeDialogTime = ref<string>('');

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
      payload: op.operation.payload || {}
    };
    const e = await vveService.startOperation(related_vvn_id, payload);
    console.log('Operation started:', e);
  } catch (e) {
    console.error('Error starting operation:', e);
    let message = 'Failed to start operation.';
    if (e && typeof e === 'object') {
      // Axios error shape
      if (e.response && e.response.data && e.response.data.message) {
        message += ' ' + e.response.data.message;
      } else if (e.message) {
        message += ' ' + e.message;
      } else {
        message += ' ' + JSON.stringify(e);
      }
    } else {
      message += ' ' + String(e);
    }
    notifications.enqueueNotification(message, notifications.notificationTypes.DANGER);
    return;
  }
  showStartDialog.value = false;
  await fetchOperations();
}

async function confirmCompleteOperation() {
  if (!completeDialogOp.value) return;
  const op = completeDialogOp.value;
  const endTime = new Date(completeDialogTime.value);
  await vveService.completeOperation(related_vvn_id, op.operation.id, endTime);
  showCompleteDialog.value = false;
  await fetchOperations();
}

async function fetchOperations() {
	if (!related_vvn_id) return;
	try {
		const vve : VesselVisitExecution = await vveService.getVesselVisitExecutionByVVN(related_vvn_id);
		operations.value = vve.operationsExecuted || [];
		console.log('Fetched operations:', operations.value);
	} catch (e) {
		operations.value = [];
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

function formatDate(dateStr: Date | string) {
  if (!dateStr) return '';
  const d = new Date(dateStr);
  return d.toLocaleString();
}

onMounted(fetchOperations);

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

    <div v-if="operations.length">
      <h3>Update vessel visit execution</h3>
      <table class="operations-table">
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
              <sl-tag :variant="statusVariant(op.status)" pill>
                <sl-icon :name="statusIcon(op.status)" class="status-icon" style="margin-right:0.4em;" aria-hidden="true"></sl-icon>
                {{ op.status }}
              </sl-tag>
            </td>
            <td>
				<sl-tag variant="info" pill>{{ op.operation.type.category }}</sl-tag>
			</td>
            <td>
              <span v-if="op.status === 'Pending' || op.status === 'Delayed'" class="expected-time">{{ formatDate(op.operation.startTime) }} (expected)</span>
              <span v-else>{{ formatDate(op.operation.startTime) }}</span>
            </td>
            <td>
              <span v-if="op.status !== 'Completed' " class="expected-time">{{ formatDate(op.operation.endTime) }} (expected)</span>
              <span v-else>{{ formatDate(op.operation.endTime) }}</span>
            </td>
            <td>
              <span v-for="(res, rIdx) in op.operation.resources" :key="rIdx" style="margin-right:0.5em; white-space:nowrap;">
				{{ res.name }}
              </span>
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
                <sl-button size="small" variant="default">
                  <sl-icon name="pencil"></sl-icon> Edit
                </sl-button>
              </template>
              <!-- Completed: no actions -->
            </td>
          </tr>
        </tbody>
      </table>
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
  </div>
</template>

<style scoped>
.icon {
    margin: 0;
    margin-right: 1rem;
    font-size: 48px;
}

.operations-table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 1.5rem;
  background: #f8f9fa;
  border: 1px solid #b0b0b0;
  font-size: 1em;
}
.operations-table th, .operations-table td {
  padding: 0.45rem 0.7rem;
  text-align: left;
  border: 1px solid #b0b0b0;
}
.operations-table th {
  background: #e0e0e0;
  color: #222;
  font-weight: 700;
  border-bottom: 2px solid #b0b0b0;
}
.operations-table tr {
  background: #f8f9fa;
}
.operations-table tr:nth-child(even) {
  background: #f0f0f0;
}
.operations-table tr:hover {
  background: #e6f0fa;
}
.operations-table td {
  border-bottom: 1px solid #b0b0b0;
}
.operations-table tr:last-child td {
  border-bottom: 1px solid #b0b0b0;
}
.operations-table .icon-col {
  width: 32px;
  min-width: 32px;
  max-width: 32px;
  text-align: center;
  opacity: 0.7;
  padding-right: 0;
  padding-left: 0.2rem;
}
.status-icon {
  font-size: 1.1em;
  vertical-align: middle;
  color: #555;
}
.expected-time {
  color: #b0b0b0;
  font-style: italic;
}

/* Remove rounded corners and shadow for a more utilitarian look */
.operations-table,
.operations-table th,
.operations-table td {
  border-radius: 0 !important;
  box-shadow: none !important;
}

/* Keep rest of the style for layout */
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