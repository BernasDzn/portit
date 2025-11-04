<script setup lang="ts">
import type { SystemUser } from '@/model/SystemUser';
import { RouterLink } from 'vue-router';
import { computed } from 'vue';

const props = defineProps<{ systemUser: SystemUser | any; link?: string }>();

const email = computed(() => props.systemUser?.emailAddress ?? props.systemUser?.email ?? '');

const ROLE_LABELS: Record<string, string> = {
    ADMIN: 'Administrator',
    PORT_AUTHORITY_OFFICER: 'Port Authority Officer',
    SAO_REPRESENTATIVE: 'SAO Representative',
    LOGISTICS_OPERATOR: 'Logistics Operator'
};

// Numeric role mapping to match backend enum ordering (Administrator = 0, ...)
const NUM_ROLE_LABELS: Record<number, string> = {
    0: 'Administrator',
    1: 'Port Authority Officer',
    2: 'SAO Representative',
    3: 'Logistics Operator'
};

const roleLabel = computed(() => {
    const r = props.systemUser?.role;
    if (r === undefined || r === null || r === '') return '';
    // If backend sends numeric enum values, map them first
    if (typeof r === 'number') {
        return NUM_ROLE_LABELS[r] ?? String(r);
    }
    // Direct match (key)
    if (ROLE_LABELS[r]) return ROLE_LABELS[r];
    // Direct match by value (maybe already a friendly string)
    for (const v of Object.values(ROLE_LABELS)) {
        if (String(v).toLowerCase() === String(r).toLowerCase()) return v;
    }
    // Try to normalize incoming role like 'Port Authority Officer', 'PORTAUTHORITYOFFICER', 'port_authority_officer'
    const normalized = String(r).toUpperCase().replace(/[^A-Z0-9]/g, '');
    for (const key of Object.keys(ROLE_LABELS)) {
        if (key.replace(/_/g, '') === normalized) return ROLE_LABELS[key];
    }
    return r;
});

</script>

<template>
    <component :is="props.link ? RouterLink : 'div'" :to="props.link">
        <sl-card class="listing-item">
            <div class="opposed">
                <div>
                    <p class="user-identification">
                        {{ email }}
                        <sl-tag size="small" variant="neutral">
                            {{ roleLabel }}
                        </sl-tag>
                    </p>
                    <p class="item-description">
                        Active: {{ props.systemUser.isActive ? 'Yes' : 'No' }}
                    </p>
                </div>
                <span class="material-icons icon" aria-hidden="true">person</span>
            </div>
            <slot></slot>
        </sl-card>
    </component>
</template>

<style scoped>
.icon {
    font-size: 35px;
    color: var(--accent-1);
}

.user-identification {
    display: flex;
    align-items: center;
    gap: 15px;
}

</style>    