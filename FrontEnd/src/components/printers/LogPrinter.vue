<script setup lang="ts">
import type { Logs } from '@/model/Logs';
import type { Qualification } from '@/model/Qualifications';
import { RouterLink } from 'vue-router';

const props = defineProps<{log: Logs;}>();
</script>

<template>
    <div>
        <sl-card class="listing-item">
            <div class="opposed">
                <div>
                    <p>{{ log.timestamp }}</p>
                    <p class="item-description">{{ log.message }}</p>
                </div>
                <div>
                    <span class="icon material-icons">
                        {{
                            log.level === 'INF' ? 'info' :
                            log.level === 'WRN' ? 'warning' :
                            log.level === 'ERR' ? 'error' : 'help'
                        }}
                    </span>
                </div>
            </div>
            <!-- For view details in the future maybe?  -->
            <slot></slot>
        </sl-card>
    </div>
</template>

<style scoped>

.icon {
    color: var(--accent-1);
}

</style>