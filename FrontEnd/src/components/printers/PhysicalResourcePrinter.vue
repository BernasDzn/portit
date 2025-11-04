<script setup lang="ts">
import { computed } from 'vue';
import { RouterLink } from 'vue-router';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();

const props = defineProps<{resource: any; link?: string}>();

</script>

<template>
    <component :is="props.link ? RouterLink :'div'" :to="props.link">
        <sl-card class="listing-item">
            <div class="opposed">
                <div>
                <!-- {{ 
                        resource
                     }} -->
                    <div>
                        <p>{{ resource.description }}</p>
                    </div>
                </div>
                <sl-tag 
                    :variant="['success', 'warning', 'danger'][resource.status]"
                >
                    {{ [t('physicalResource.fields.status.options.available'), t('physicalResource.fields.status.options.maintenance'), t('physicalResource.fields.status.options.outOfService')][resource.status] }}
                </sl-tag>
            </div>
            <p class="item-description">
                {{ resource.code }} <br>
            </p>
            <sl-divider></sl-divider>
            <p>{{ t('physicalResource.requiredQualifications') }}</p>
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