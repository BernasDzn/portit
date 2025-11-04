<script setup lang="ts">
import type { SystemUser } from '@/model/SystemUser';
import { RouterLink } from 'vue-router';
import { computed } from 'vue';

const props = defineProps<{ systemUser: SystemUser | any; link?: string }>();

const email = computed(() => props.systemUser?.emailAddress ?? props.systemUser?.email ?? '');
</script>

<template>
    <component :is="props.link ? RouterLink : 'div'" :to="props.link">
        <sl-card class="listing-item">
            <div class="opposed">
                <div>
                    <p class="user-identification">
                        {{ email }}
                        <sl-tag size="small" variant="neutral">
                            {{ props.systemUser.role }}
                        </sl-tag>
                    </p>
                    <p class="item-description">
                        Active: {{ props.systemUser.isActive ? 'Yes' : 'No' }}
                    </p>
                </div>
                <span class="material-icons icon" aria-hidden="true">person</span>
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

.user-identification {
    display: flex;
    align-items: center;
    gap: 15px;
}

</style>    