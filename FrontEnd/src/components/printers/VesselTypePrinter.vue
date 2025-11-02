<script setup lang="ts">
import type { VesselType } from '@/model/VesselType';
import { computed } from 'vue';
const props = defineProps<{vtype: VesselType}>();

</script>

<template>
    <sl-card class="listing-item">
        <div class="opposed">
            <div>
                <p>{{ vtype.name }}</p>
                <p class="item-description">
                    {{ vtype.description }}<br/>
                </p>
            </div>
            <span class="material-icons icon" aria-hidden="true">directions_boat</span>
        </div>
        <slot></slot>
    </sl-card>
</template>

<style scoped>

.icon {
    font-size: 35px;
    color: var(--accent-1);
}

</style>