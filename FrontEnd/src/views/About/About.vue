<script setup lang="ts">
import { useI18n } from 'vue-i18n';

const { t } = useI18n();

</script>

<template>
    <div>
        <h1 class="title">{{ t('about.title') }}</h1>
        
    </div>
</template>