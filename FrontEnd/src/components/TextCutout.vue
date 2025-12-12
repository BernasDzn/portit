<script setup lang="ts">
import { computed } from 'vue';


const props = defineProps<{
    text: string;
    maxLength?: number;
}>()

const truncatedText = computed(() => {
    if (props.maxLength && props.text.length > props.maxLength) {
        return props.text.slice(0, props.maxLength) + '...';
    }
    return props.text;
});

</script>

<template>
    <sl-tooltip :content="text" placement="top">
        <span>
            {{ truncatedText }}
        </span>
      </sl-tooltip>
</template>