<script setup lang="ts">
import type { Staff } from '@/model/Staff';

const props = defineProps<{staff: Staff}>();

</script>

<template>
    <sl-card class="listing-item">
        <div class="opposed">
            <div>
                <p>{{ staff.name }}</p>
                <p class="item-description">{{ staff.mechanographicNumber }}</p>
            </div>
            <span class="material-icons icon" aria-hidden="true">person</span>
        </div>
        <!-- For view details in the future maybe?  -->
        <slot></slot>
    </sl-card>
</template>

<style scoped>

.icon {
    font-size: 35px;
    color: var(--accent-1);
}

</style>