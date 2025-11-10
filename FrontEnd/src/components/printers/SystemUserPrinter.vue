<script setup lang="ts">
import type { SystemUser } from '@/model/SystemUser';
import { RouterLink } from 'vue-router';
import { computed } from 'vue';
import {useI18n} from 'vue-i18n';

const { t } = useI18n();

const props = defineProps<{ systemUser: SystemUser | any; link?: string }>();

const email = computed(() => props.systemUser?.emailAddress ?? props.systemUser?.email ?? '');

const ROLE_LABELS: Record<string, string> = {
    ADMIN: 'user.fields.role.options.admin',
    PORT_AUTHORITY_OFFICER: 'user.fields.role.options.pao',
    SAO_REPRESENTATIVE: 'user.fields.role.options.saoRep',
    LOGISTICS_OPERATOR: 'user.fields.role.options.logisticsOperator'
};

// Numeric role mapping to match backend enum ordering (Administrator = 0, ...)
const NUM_ROLE_LABELS: Record<number, string> = {
    0: 'user.fields.role.options.admin',
    1: 'user.fields.role.options.pao',
    2: 'user.fields.role.options.saoRep',
    3: 'user.fields.role.options.logisticsOperator'
};

const roleLabel = computed(() => {
    const r = props.systemUser?.role;
    if (r === undefined || r === null || r === '') return '';
    
    // If backend sends numeric enum values, map them first
    if (typeof r === 'number') {
        const label = NUM_ROLE_LABELS[r];
        return label ? t(label) : String(r);
    }
    
    // Direct match (key)
    if (ROLE_LABELS[r]) return t(ROLE_LABELS[r]);
    
    // Direct match by value (maybe already a friendly string)
    for (const v of Object.values(ROLE_LABELS)) {
        if (String(v).toLowerCase() === String(r).toLowerCase()) return t(v);
    }
    
    // Try to normalize incoming role like 'Port Authority Officer', 'PORTAUTHORITYOFFICER', 'port_authority_officer'
    const normalized = String(r).toUpperCase().replace(/[^A-Z0-9]/g, '');
    for (const key of Object.keys(ROLE_LABELS)) {
        if (key.replace(/_/g, '') === normalized) return t(ROLE_LABELS[key]);
    }
    
    return String(r);
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
                        {{ t('user.fields.isActive.title') }}: {{ props.systemUser.isActive ? t('user.fields.isActive.options.active') : t('user.fields.isActive.options.inactive') }}
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