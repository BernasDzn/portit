<script setup lang="ts">
import { RouterLink } from 'vue-router';
import type { Staff } from '@/model/Staff';
import ActivityTag from '@/components/ActivityTag.vue'


const props = defineProps<{
    staff: Staff;
    link?: string;
}>();

</script>

<template>
    <component :is="props.link ? RouterLink : 'div'" :to="props.link">
        <sl-card class="listing-item">
            <div class="opposed">
                <div>
                    <p>{{ staff.name }}</p>
                    <p class="item-description">{{ staff.mechanographicNumber }}</p>
                </div>
                <ActivityTag :status="staff.status"/>
            </div>
            <div class="email-group">
                <span class="material-icons icon" aria-hidden="true">mail</span>
                <span class="item-description">{{ staff.email }}</span>
            </div>
            <sl-divider></sl-divider>
            <p>Qualifications:</p>
            <ul class="qualification-list">
                <li v-for="qualification in staff.qualifications">
                    <sl-badge v-if="qualification !== undefined" class="list-badge" variant="neutral">{{ qualification.qualificationName }}</sl-badge>
                </li>
            </ul>
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

.email-group {
    display: flex;
    align-items: center;
    gap: 8px;
    margin-top: 8px;
}

.email-group .icon {
    font-size: 20px;
}

.qualification-list {
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