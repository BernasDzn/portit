<script setup lang="ts">
// @ts-ignore: missing type declarations for 'vue-i18n' in this project
import { useI18n } from 'vue-i18n'
import { RouterLink } from 'vue-router';
import type { StorageArea } from '@/model/StorageArea';

const { t } = useI18n();

const props = defineProps<{
    storageArea: StorageArea;
    link?: string;
}>();

const statuses = [
	"Yard", "Warehouse"
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
							<span class="item-description">{{ storageArea.dockServices.length }} {{t('storageArea.printer.docks_serviced')}}</span>
						</div>
					</div>
                </div>
                <sl-tag>{{ statuses[storageArea.type] }}</sl-tag>
            </div>
			<sl-divider></sl-divider>
            <div class="email-group" style="display: flex; flex-wrap: wrap;">
                <div style="display:flex; flex: 100%; justify-content: space-between;">
					<span class="item-description">{{ t('storageArea.printer.capacity_usage') }}</span>
					<span>{{ (storageArea.currentOccupancy / storageArea.capacity * 100).toPrecision(2) }} %</span>
				</div>
				<div style="width: 100%;">
					<sl-range min="0" :value="storageArea.currentOccupancy" :max="storageArea.capacity" 
						:style="
						storageArea.currentOccupancy / storageArea.capacity > 0.6 ?
						'--track-color-active: var(--sl-color-warning-600);' + '--track-color-inactive: var(--sl-color-neutral-300);' +
						'--thumb-size: 0px;' + 'width: 100%;'
						:
						'--track-color-active: var(--sl-color-success-600);' + '--track-color-inactive: var(--sl-color-neutral-300);' +
						'--thumb-size: 0px;' + 'width: 100%;'
						"
					>
					</sl-range>
				</div>
				<div class="capacity-below" style="display:flex; flex: 100%; justify-content: space-between;">
					<span>{{ storageArea.currentOccupancy }} / {{ storageArea.capacity }} {{ t('storageArea.printer.units') }}</span>
					<span>{{ storageArea.capacity - storageArea.currentOccupancy }} {{ t('storageArea.printer.available') }}</span>
				</div>
            </div>
        </sl-card>
    </component>
</template>

<style scoped>

.capacity-below{
	color: var(--sl-color-neutral-400);
	font-size: 0.75rem;
}

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