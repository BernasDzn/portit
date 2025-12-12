<script setup lang="ts">
import type { OperationPlanDto } from '@/model/dto/OperationPlanDto';
import { computed } from 'vue';

interface Props {
    plan: OperationPlanDto | null;
    operationIndex: number | null;
}

const props = defineProps<Props>();
const emit = defineEmits<{
    close: [];
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

            <div class="resources-grid">
                <sl-input 
                    v-for="resource in operation.resources"
                    :key="resource.name"
                    :name="`resource-${resource.name}`" 
                    disabled
                    :value="`${resource.name} (${resource.type})`"
                ></sl-input>
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
.resources-grid {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: 1rem;
    margin-bottom: 1rem;
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