<script setup lang="ts">

const status = [
    { label: 'Available', variant: 'success' },
    { label: 'Unavailable', variant: 'danger' },
    { label: 'Temporarily reassigned', variant: 'warning' }
];

const props = defineProps<{
    status: number | undefined
}>();

</script>

<template>
    <sl-tag v-if="props.status !== undefined" :variant="status[props.status]?.variant">{{status[props.status]?.label}}</sl-tag>
</template>