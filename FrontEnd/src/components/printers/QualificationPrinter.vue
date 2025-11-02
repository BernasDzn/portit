<script setup lang="ts">
import type { Qualification } from '@/model/Qualifications';
import { RouterLink } from 'vue-router';

const props = defineProps<{qualification: Qualification; link?: string}>();

</script>

<template>
    <component :is="props.link ? RouterLink :'div'" :to="props.link">
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
    </component>
</template>

<style scoped>

.icon {
    font-size: 35px;
    color: var(--accent-1);
}

</style>