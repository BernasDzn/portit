<script setup lang="ts">
// @ts-ignore: missing type declarations for 'vue-i18n' in this project
import { useI18n } from 'vue-i18n'
import { RouterLink } from 'vue-router';
import type { StorageArea } from '@/model/StorageArea';

import StorageCapacityPrinter from '@/components/StorageCapacityPrinter.vue';

const { t } = useI18n();

const props = defineProps<{
    storageArea: StorageArea;
    link?: string;
}>();

const statuses = [
	"storageArea.fields.type.options.yard", "storageArea.fields.type.options.warehouse"
];

</script>

<template>
    <component :is="props.link ? RouterLink : 'div'" :to="props.link">
        <sl-card class="listing-item">
            <div class="opposed">
                <div>
                    <p>{{ storageArea.nameCode }}</p>
                    <div class="item-description">
						<div class="email-group">
							<span class="material-icons icon" aria-hidden="true">location_on</span>
							<span class="item-description">{{ storageArea.location }}</span>
						</div>
					</div>
					<div class="item-description">
						<div class="email-group">
							<span class="material-icons icon" aria-hidden="true">anchor</span>
							<span v-if="storageArea.type == 0" class="item-description">
                                {{ storageArea.dockServices.length }} {{t('storageArea.printer.docks_serviced')}}
                            </span>
                            <span v-else class="item-description">
                                {{ t('storageArea.printer.all_docks_serviced') }}
                            </span>
						</div>
					</div>
                </div>
                <sl-tag>{{ t(statuses[storageArea.type]!) }}</sl-tag>
            </div>
			<sl-divider></sl-divider>
            <StorageCapacityPrinter :storage-area="storageArea" />
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