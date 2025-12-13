<script setup lang="ts">
import { computed } from 'vue';
import type { OperationPlanDto } from '@/model/dto/OperationPlanDto';
import type { STSCrane } from '@/model/PhysicalResource';
import type { Staff } from '@/model/Staff';

interface Props {
    plan: OperationPlanDto | null;
    allSTSCranes: STSCrane[];
    allStaff: Staff[];
}

const props = defineProps<Props>();

const allocatedCranes = computed(() => {
    if (!props.plan) return [];
    
    const craneNames = new Set<string>();
    props.plan.operationSchedule.forEach(op => {
        op.resources.forEach(resource => {
            if (resource.type !== 'Staff') {
                craneNames.add(resource.name);
            }
        });
    });
    
    return props.allSTSCranes.filter(crane => craneNames.has(crane.code));
});

const allocatedStaff = computed(() => {
    if (!props.plan) return [];
    
    const staffEmails = new Set<string>();
    props.plan.operationSchedule.forEach(op => {
        op.resources.forEach(resource => {
            if (resource.type === 'Staff') {
                staffEmails.add(resource.name);
            }
        });
    });
    
    return props.allStaff.filter(staff => staffEmails.has(staff.email));
});

const getCraneOperationCount = (craneCode: string): number => {
    if (!props.plan) return 0;
    
    return props.plan.operationSchedule.filter(op =>
        op.resources.some(r => r.name === craneCode && r.type !== 'Staff')
    ).length;
};

const getStaffOperationCount = (staffEmail: string): number => {
    if (!props.plan) return 0;
    
    return props.plan.operationSchedule.filter(op =>
        op.resources.some(r => r.name === staffEmail && r.type === 'Staff')
    ).length;
};
</script>

<template>
    <div class="allocated-resources">
        <!-- Cranes Column -->
        <div class="resource-column">
            <div class="column-header">
                <span class="material-icons icon" aria-hidden="true">build</span>
                <h4>Allocated Cranes</h4>
                <sl-badge variant="primary" pill>{{ allocatedCranes.length }}</sl-badge>
            </div>
            
            <div v-if="allocatedCranes.length === 0" class="empty-state">
                <p>No cranes allocated</p>
            </div>
            
            <div v-else class="resource-list">
                <div 
                    v-for="crane in allocatedCranes" 
                    :key="crane.code"
                    class="resource-card"
                >
                    <div class="resource-info">
                        <div class="resource-name">
                            <span class="material-icons resource-icon" aria-hidden="true">build</span>
                            {{ crane.code }}
                        </div>
                        <div class="resource-details">
                            <span class="detail-label">Capacity:</span>
                            <span>{{ crane.liftingCapacity }}t</span>
                        </div>
                    </div>
                    <sl-badge variant="neutral">
                        {{ getCraneOperationCount(crane.code) }} ops
                    </sl-badge>
                </div>
            </div>
        </div>

        <!-- Staff Column -->
        <div class="resource-column">
            <div class="column-header">
                <sl-icon name="people"></sl-icon>
                <h4>Allocated Staff</h4>
                <sl-badge variant="primary" pill>{{ allocatedStaff.length }}</sl-badge>
            </div>
            
            <div v-if="allocatedStaff.length === 0" class="empty-state">
                <p>No staff allocated</p>
            </div>
            
            <div v-else class="resource-list">
                <div 
                    v-for="staff in allocatedStaff" 
                    :key="staff.email"
                    class="resource-card"
                >
                    <div class="resource-info">
                        <div class="resource-name">
                            <sl-icon name="person" class="resource-icon"></sl-icon>
                            {{ staff.name }}
                        </div>
                        <div class="resource-details">
                            <span>{{ staff.name || 'Staff' }}</span>
                        </div>
                    </div>
                    <sl-badge variant="neutral">
                        {{ getStaffOperationCount(staff.email) }} ops
                    </sl-badge>
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped>
.allocated-resources {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 1.5rem;
    margin-bottom: 2rem;
}

.resource-column {
    background: var(--sl-color-neutral-0);
    border: 1px solid var(--sl-color-neutral-200);
    border-radius: 8px;
    padding: 1.5rem;
}

.column-header {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    margin-bottom: 1rem;
    padding-bottom: 1rem;
    border-bottom: 2px solid var(--sl-color-neutral-200);
}

.column-header sl-icon {
    font-size: 1.5rem;
    color: var(--sl-color-primary-600);
}

.column-header h4 {
    margin: 0;
    font-size: 1.1rem;
    font-weight: 600;
    flex: 1;
}

.empty-state {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    padding: 2rem;
    text-align: center;
    color: var(--sl-color-neutral-600);
}

.empty-state p {
    margin-top: 0.5rem;
    font-size: 0.9rem;
}

.resource-list {
    display: flex;
    flex-direction: column;
    gap: 0.75rem;
}

.resource-card {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 1rem;
    background: var(--sl-color-neutral-50);
    border: 1px solid var(--sl-color-neutral-200);
    border-radius: 6px;
    transition: all 0.2s ease;
}

.resource-card:hover {
    background: var(--sl-color-neutral-100);
    border-color: var(--sl-color-primary-300);
    transform: translateY(-1px);
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
}

.resource-info {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
    flex: 1;
}

.resource-name {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    font-weight: 600;
    font-size: 0.95rem;
    color: var(--sl-color-neutral-900);
}

.resource-icon {
    font-size: 1rem;
    color: var(--sl-color-primary-600);
}

.resource-details {
    display: flex;
    gap: 0.5rem;
    font-size: 0.85rem;
    color: var(--sl-color-neutral-600);
}

.detail-label {
    font-weight: 500;
}

.icon {
    font-size: 1.5rem;
    color: var(--sl-color-primary-600);
}

@media (max-width: 968px) {
    .allocated-resources {
        grid-template-columns: 1fr;
    }
}
</style>