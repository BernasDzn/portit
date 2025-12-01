<script setup lang="ts">
import { useI18n } from 'vue-i18n';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { IOperationPlanService } from '@/service/IService/IOperationPlanService';
import PlansByDay from '@/components/PlansByDay.vue';

const oemService = container.get<IOperationPlanService>(TYPES.operationPlanService);

const { t, locale } = useI18n();

</script>

<template>
  <div>

    <sl-breadcrumb>
      <sl-breadcrumb-item><RouterLink to="/scheduling-dashboard" class="breadcrumb-link">{{ t('scheduling.tabs.dashboard') }}</RouterLink></sl-breadcrumb-item>
      <sl-breadcrumb-item>{{ t('scheduling.tabs.search') }}</sl-breadcrumb-item>
    </sl-breadcrumb>

    <header>
        <h1 class="title">{{ t('scheduling.tabs.search') }}</h1>
        <p class="subtitle">{{ t('scheduling.subtitles.search') }}</p>

        

    </header>
  </div>
</template>

<style scoped> 
.link {
  text-decoration: none;
  color: inherit;
}
</style>