<script setup lang="ts">
import type { Qualification } from '@/model/Qualifications';


const props = defineProps<{qualification: Qualification}>();

</script>

<template>
    <sl-card class="listing-item">
        <div class="opposed">
            <div>
                <p>{{ qualification.qualificationName }}</p>
                <p class="item-description">{{ qualification.idCode }}</p>
            </div>
            <span class="material-icons icon" aria-hidden="true">workspace_premium</span>
        </div>
        <!-- For view details in the future maybe?  -->
        <slot></slot>
    </sl-card>
</template>

<style scoped>

.icon {
    font-size: 35px;
    color: var(--sl-color-indigo-700);
}

</style>