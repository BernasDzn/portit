<script setup lang="ts">
import { computed, ref } from 'vue';
import type { OperationPlanDto } from '@/model/dto/OperationPlanDto';
import type { Staff } from '@/model/Staff';
import ObjectSelector from '@/components/crud/ObjectSelector.vue';

interface Props {
    plan: OperationPlanDto | null;
    operationIndex: number | null;
    availableStaff: any[];
}

const props = defineProps<Props>();
const emit = defineEmits<{
    close: [];
    addStaff: [operationIndex: number, staffSelected: Staff];
    removeStaff: [operationIndex: number, staffSelected: string];
}>();

const formatTime = (dateString: string): string => {
    return new Date(dateString)
        .toLocaleString()
        .split(',')[1]
        .split(':')
        .slice(0, 2)
        .join(':');
};

const operation = computed(() => {
    if (!props.plan || props.operationIndex === null) return null;
    return props.plan.operationSchedule[props.operationIndex];
});

const isLoadOrUnload = computed(() => {
    if (!operation.value) return false;
    const category = operation.value.type.category.value;
    return category === 'LOAD' || category === 'UNLOAD';
});

const assignedStaffIds = computed(() => {
    if (!operation.value) return new Set<string>();
    return new Set(
        operation.value.resources
            .filter(r => r.type === 'Staff')
            .map(r => r.name)
    );
});

const selectedStaff = ref<Staff>(null);

const handleAddStaff = () => {
    if (selectedStaff.value && props.operationIndex !== null) {
        emit('addStaff', props.operationIndex, selectedStaff.value as Staff);
        selectedStaff.value = null;
    }
};

const handleRemoveResource = (staffSelected: string) => {
    if (props.operationIndex !== null) {
        emit('removeStaff', props.operationIndex, staffSelected);
    }
};

const getStaffName = (staffId: string): string => {
    const staff = props.availableStaff.find(s => s.email === staffId);
    return staff ? staff.name : staffId;
};
</script>

<template>
    <sl-drawer label="Operation Details" class="drawer-overview">
        <h3>Operation Details</h3>

        <div v-if="operation">
            <p>
                <strong>Start time:</strong> 
                {{ formatTime(operation.startTime) }}
            </p>
            <p>
                <strong>End time:</strong> 
                {{ formatTime(operation.endTime) }}
            </p>

            <p>
                <strong>Category:</strong> 
                {{ operation.type.description }} operation
            </p>

            <sl-divider></sl-divider>
            <h3>Resources</h3>

            <div class="resources-list">
                <div 
                    v-for="resource in operation.resources"
                    :key="resource.name"
                    class="resource-item"
                >
                    <sl-input 
                        :name="`resource-${resource.name}`" 
                        disabled
                        :value="resource.type === 'Staff' ? getStaffName(resource.name) : resource.name"
                    >
                        <span slot="prefix">{{ resource.type }}</span>
                    </sl-input>
                    <sl-button
                        v-if="resource.type === 'Staff'"
                        variant="danger"
                        size="small"
                        @click="handleRemoveResource(resource.name)"
                    >
                        <sl-icon name="trash"></sl-icon>
                    </sl-button>
                </div>
            </div>

            <div class="add-staff-section">
                <h4>Add Staff Member</h4>
                <p v-if="props.availableStaff.length === 0" class="no-staff-message">
                    <em>All available staff members have been assigned</em>
                </p>
                <div v-else class="add-staff-controls">
                    <ObjectSelector
                        class="field-dropdown"
                        :name="'Select a staff member'"
                        v-model="selectedStaff"
                        :fetch-function="async () => props.availableStaff"
                        :placeholderText="'Select staff member'"
                        labelKey="name"
                    />
                    <sl-button
                        variant="primary"
                        @click="handleAddStaff"
                        :disabled="!selectedStaff"
                    >
                        <sl-icon slot="prefix" name="plus"></sl-icon>
                        Add
                    </sl-button>
                </div>
            </div>

            <sl-divider></sl-divider>
            <h3>Operation Payload</h3>
            
            <div v-if="isLoadOrUnload">
                <sl-input 
                    name="containerId" 
                    label="Container ID:"
                    disabled
                    :value="operation.payload?.containerId || 'Not specified'"
                ></sl-input>
                <br>
                <sl-input 
                    name="storageArea" 
                    label="Storage area:"
                    disabled
                    :value="operation.payload?.storageLocation || 'Not specified'"
                ></sl-input>
            </div>
            <div v-else>
                <em>No payload specifiable for this operation category</em>
            </div>
        </div>

        <sl-button @click="emit('close')" slot="footer" variant="primary">
            Close
        </sl-button>
    </sl-drawer>
</template>

<style scoped>
.resources-list {
    display: flex;
    flex-direction: column;
    gap: 0.75rem;
    margin-bottom: 1.5rem;
}

.resource-item {
    display: flex;
    gap: 0.5rem;
    align-items: center;
}

.resource-item sl-input {
    flex: 1;
}

.add-staff-section {
    margin: 1.5rem 0;
    padding: 1rem;
    background: var(--sl-color-neutral-50);
    border-radius: 4px;
}

.add-staff-section h4 {
    margin: 0 0 1rem 0;
    font-size: 1rem;
    font-weight: 600;
}

.add-staff-controls {
    display: flex;
    gap: 0.5rem;
    align-items: flex-end;
}

.add-staff-controls sl-select {
    flex: 1;
}

.no-staff-message {
    margin-top: 0.5rem;
    margin-bottom: 0;
    color: var(--sl-color-neutral-600);
    font-size: 0.9rem;
}

sl-input {
    margin-bottom: 0.5rem;
}

h3 {
    margin-top: 1rem;
    margin-bottom: 0.75rem;
}

p {
    margin: 0.5rem 0;
}
</style>