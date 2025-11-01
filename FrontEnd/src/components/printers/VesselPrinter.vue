<script setup lang="ts">
import type { Vessel } from '@/model/Vessels';


const props = defineProps<{vessel: Vessel}>();

</script>

<template>
    <sl-card class="listing-item">
        <div class="opposed">
            <div>
                <p>{{ vessel.name }}</p>
                <p class="item-description">{{ vessel.imo }}</p>
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