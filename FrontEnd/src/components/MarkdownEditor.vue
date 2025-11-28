<script setup lang="ts">
  import { marked } from 'marked';
  import _ from 'lodash';
import { computed } from 'vue';
  
//   export default {
//     name: 'MarkdownEditor',
//     props: {
//       value: {
//         type: String,
//         default: '# hello'
//       },
//       height: {
//         type: String,
//         default: '100vh'
//       }
//     },
//     computed: {
//       compiledMarkdown() {
//         return marked(this.value, { sanitize: true });
//       }
//     },
//     methods: {
//       update: _.debounce(function(e) {
//         this.$emit('input', e.target.value);
//       }, 300)
//     }
//   };
const props = withDefaults(defineProps<{
  modelValue?: string;
  height?: string;
}>(), {
  modelValue: '# Hello world!',
  height: '100vh'
});

const compiledMarkdown = computed(() => {
    return marked(props.modelValue);
});

const emit = defineEmits<{
    (e: 'update:modelValue', value: string): void;
}>();

const update = _.debounce((e: Event) => {
    const target = e.target as HTMLTextAreaElement;
    emit('update:modelValue', target.value);
}, 300);

</script>
  
<template>
    <div class="editor" :style="{ height: height }">
      <textarea :value="modelValue" @input="update"></textarea>
      <div v-html="compiledMarkdown"></div>
    </div>
</template>

<style scoped>
.editor {
    display: flex;
    gap: 20px;
    height: 100vh;
    padding: 20px;
}

textarea {
    flex: 1;
    font-family: 'Monaco', 'Courier New', monospace;
    font-size: 14px;
    padding: 10px;
    border: 1px solid #ccc;
    border-radius: 4px;
    resize: none;
}

.editor > div {
    flex: 1;
    padding: 10px;
    border: 1px solid #ccc;
    border-radius: 4px;
    overflow-y: auto;
}
</style>