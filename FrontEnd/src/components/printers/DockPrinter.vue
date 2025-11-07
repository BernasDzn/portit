<script setup lang="ts">
import type { Dock } from '@/model/Dock';
import { computed } from 'vue';
import { RouterLink } from 'vue-router';
import { useI18n } from 'vue-i18n';
import type { StorageArea } from '@/model/StorageArea';

const { t } = useI18n();

const props = withDefaults(defineProps<{
    dock: Dock;
    link?: string;
    showDetails?: boolean;
    distance_string?: string;
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
            <div v-if="distance_string">
                <div class="icon-group">
                    <span class="material-icons sec_icon" aria-hidden="true">route</span>
                    <span class="item-description">{{ props.distance_string }}</span>
                </div>
                <sl-divider></sl-divider>
            </div>
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

.icon-group {
    display: flex;
    align-items: center;
    gap: 4px;
    margin-bottom: 8px;
}

</style>