<script setup lang="ts">
	import type { StorageArea } from '@/model/StorageArea';
	// @ts-ignore: missing type declarations for 'vue-i18n' in this project
	import { useI18n } from 'vue-i18n';

	const { t } = useI18n();

	const props = defineProps<{
		storageArea: StorageArea;
	}>();

</script>

<template>
	<div class="email-group" style="display: flex; flex-wrap: wrap;">
		<div style="display:flex; flex: 100%; justify-content: space-between;">
			<span class="item-description">{{ t('storageArea.printer.capacity_usage') }}</span>
			<span>{{ ((storageArea.currentOccupancy / storageArea.capacity) * 100).toFixed(2) }}%</span>
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
</template>

<style scoped>
	.capacity-below{
		color: var(--sl-color-neutral-400);
		font-size: 0.75rem;
	}
</style>