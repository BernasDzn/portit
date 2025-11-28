<template>
    <div class="markdown-preview" v-html="compiledMarkdown"></div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { marked } from 'marked';

const props = defineProps<{
    markdown: string;
}>();

const compiledMarkdown = computed(() => {
    return marked(props.markdown);
});
</script>
  
<style scoped>
.markdown-preview {
    padding: 10px;
}
</style>