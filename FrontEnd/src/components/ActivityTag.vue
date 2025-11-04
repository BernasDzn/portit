<script setup lang="ts">
import { useI18n } from 'vue-i18n';

const { t } = useI18n();

const status = [
    { label: 'staff.fields.status.options.available', variant: 'success' },
    { label: 'staff.fields.status.options.unavailable', variant: 'danger' },
    { label: 'staff.fields.status.options.temporarilyReassigned', variant: 'warning' }
];

const props = defineProps<{
    status: number | undefined
}>();

</script>

<template>
    <sl-tag v-if="props.status !== undefined" :variant="status[props.status]?.variant">{{t(status[props.status]?.label as string)}}</sl-tag>
</template>