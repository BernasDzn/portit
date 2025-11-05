<script setup lang="ts">
import type { Dock } from '@/model/Dock';
import { computed } from 'vue';
import { RouterLink } from 'vue-router';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();

const props = withDefaults(defineProps<{
    dock: Dock;
    link?: string;
    showDetails?: boolean;
}>(), {
    showDetails: true
});

const typesDisplay = computed(() => {
    return props.dock.supportedVesselTypes.map((type: any) => type.name) ?? [];
});
</script>

<template>
    <component :is="props.link ? RouterLink : 'div'" :to="props.link">
        <sl-card class="listing-item">
            <div class="opposed">
                <div>
                    <p>{{ dock.name }}</p>
                    <p class="item-description">
                        {{ dock.code }}<br />
                        {{ dock.location }}<br />
                    </p>
                </div>
                <span class="material-icons icon" aria-hidden="true">anchor</span>
            </div>
            <div class="details" v-if="props.showDetails">
                <sl-divider></sl-divider>
                <p>{{ t('dock.fields.supportedVesselTypes.title') }}:</p>
                <ul class="vessel-types-list">
                    <li v-for="vtype in typesDisplay">
                        <sl-badge v-if="vtype !== undefined" class="list-badge" variant="neutral">{{ vtype }}</sl-badge>
                    </li>
                </ul>
                <slot></slot>
            </div>
        </sl-card>
    </component>
</template>

<style scoped>
.icon {
    font-size: 35px;
    color: var(--accent-1);
}
.vessel-types-list {
    padding: 0;
    display: flex;
    gap: 8px;
    flex-wrap: wrap;
}

.list-badge::part(base) {
    border-radius: var(--sl-border-radius-medium);
    background-color: var(--sl-color-neutral-200);
    color: var(--sl-color-neutral-800);
}
</style>