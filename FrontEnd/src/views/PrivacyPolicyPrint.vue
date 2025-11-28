<script setup lang="ts">
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { IPrivacyPolicyService } from '@/service/IService/IPrivacyPolicyService';
import { onMounted, ref } from 'vue';
import { marked } from 'marked';

const privacyPolicyService = container.get<IPrivacyPolicyService>(TYPES.privacyPolicyService);
const privacyPolicyHtml = ref<string>('');

onMounted(async () => {
    try {
        const data = await privacyPolicyService.getActivePrivacyPolicy();
        privacyPolicyHtml.value = await marked(data.content) as string;
        
        setTimeout(() => {
            window.print();
            
            setTimeout(() => {
                if (!window.matchMedia('print').matches) {
                    window.close();
                }
            }, 1000);
        }, 500);
    } catch (error) {
        privacyPolicyHtml.value = '<p>Failed to load privacy policy.</p>';
    }
});

window.onafterprint = () => {
    window.close();
};

window.addEventListener('focus', () => {
    setTimeout(() => {
        window.close();
    }, 100);
});
</script>

<template>
    <div class="print-container">
        <h1>Privacy Policy</h1>
        <div class="content" v-html="privacyPolicyHtml"></div>
    </div>
</template>

<style scoped>
.print-container {
    font-family: Arial, sans-serif;
    line-height: 1.6;
    max-width: 800px;
    margin: 0 auto;
    padding: 20px;
}

h1 {
    font-size: 2em;
    margin-top: 0;
    margin-bottom: 1em;
    font-weight: 600;
}

.content :deep(h1),
.content :deep(h2),
.content :deep(h3),
.content :deep(h4),
.content :deep(h5),
.content :deep(h6) {
    margin-top: 1em;
    margin-bottom: 0.5em;
    font-weight: 600;
}

.content :deep(h1) { font-size: 2em; }
.content :deep(h2) { font-size: 1.5em; }
.content :deep(h3) { font-size: 1.25em; }

.content :deep(p) {
    margin-bottom: 1em;
}

.content :deep(ul),
.content :deep(ol) {
    margin-bottom: 1em;
    padding-left: 2em;
}

.content :deep(code) {
    background-color: #f4f4f4;
    padding: 2px 4px;
    border-radius: 3px;
    font-family: monospace;
}

.content :deep(pre) {
    background-color: #f4f4f4;
    padding: 10px;
    border-radius: 5px;
    overflow-x: auto;
}

.content :deep(blockquote) {
    border-left: 4px solid #ddd;
    padding-left: 1em;
    margin-left: 0;
    color: #666;
}

@media print {
    .print-container {
        padding: 0;
    }
}
</style>
