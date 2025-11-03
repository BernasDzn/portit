<script setup lang="ts">
import type { VesselType } from '@/model/VesselType';
import { computed } from 'vue';
import { RouterLink } from 'vue-router';
const props = defineProps<{
    vtype: VesselType,
    link?: string
}>();

</script>

<template>
    <component :is="props.link ? RouterLink : 'div'" :to="props.link">
        <sl-card class="listing-item">
            <div class="opposed">
                <div>
                    <p>{{ vtype.name }}</p>
                    <p class="item-description">
                        {{ vtype.description }}<br />
                        Capacity: {{ vtype.capacity }}<br />
                    </p>
                </div>
                <span class="material-icons icon" aria-hidden="true">sailing</span>
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
.vessel-types-list {
    padding: 0;
    display: flex;
    gap: 8px;
    flex-wrap: wrap;
}
</style>