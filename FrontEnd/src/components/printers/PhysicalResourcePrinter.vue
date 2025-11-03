<script setup lang="ts">
import { computed } from 'vue';
import { RouterLink } from 'vue-router';

const props = defineProps<{resource: any; link?: string}>();

const getIcons = computed(() => {
    return props.resource.servingDock != undefined ? 'build' : 
        props.resource.averageSpeed != undefined ? 'local_shipping' : 'precision_manufacturing'
});

</script>

<template>
    <component :is="props.link ? RouterLink :'div'" :to="props.link">
        <sl-card class="listing-item">
            <div class="opposed">
                <div>
                <!-- {{ 
                        resource
                     }} -->
                    <div class="flex">
                        <p>{{ resource.description }}</p>
                        <sl-tag 
                            size="small" 
                            :variant="['success', 'warning', 'danger'][resource.status]"
                        >
                            {{ ["Available", "Maintenance", "Out of service"][resource.status] }}
                        </sl-tag>
                    </div>
                    <p class="item-description">
                        {{ resource.code }} <br>
                        Setup time: {{resource.setupTimeInMinutes}}m
                    </p>
                </div>
                <span class="material-icons icon" aria-hidden="true">build</span>
            </div>
            <sl-divider></sl-divider>
            <p>Required qualifications:</p>
            <ul class="qualification-list">
                <li v-for="q in props.resource.qualifications">
                    <sl-badge v-if="q !== undefined" class="list-badge" variant="neutral">{{ q.qualificationName }}</sl-badge>
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

.qualification-list {
    padding: 0;
    display: flex;
    gap: 8px;
    flex-wrap: wrap;
}

.flex {
    display: flex;
    align-items: center;
    gap: 10px;
}

.list-badge::part(base) {
    border-radius: var(--sl-border-radius-medium);
    background-color: var(--sl-color-neutral-200);
    color: var(--sl-color-neutral-800);
}

</style>