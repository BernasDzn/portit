<script setup lang="ts">
import ListingBox from '@/components/crud/ListingBox.vue';
import type { Filter, Page } from '@/model/Page';
import { useI18n } from 'vue-i18n';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { ITaskCategoryService } from '@/service/IService/ITaskCategoryService';
import type { TaskCategoryFilter } from '@/model/dto/TaskCategoryDto';
import type TaskCategoryDto from '@/model/dto/TaskCategoryDto';
import TaskCategoryPrinter from '@/components/printers/TaskCategoryPrinter.vue';

const { t } = useI18n();

const taskCategoryService = container.get<ITaskCategoryService>(TYPES.taskCategoryService);

const fetchTaskCategories = async (filtering?: Filter<TaskCategoryFilter>): Promise<Page<TaskCategoryDto>> => {
    return await taskCategoryService.getAllTaskCategories(filtering);
}

</script>

<template>
<div>
    <sl-breadcrumb>
        <sl-breadcrumb-item><RouterLink to="/task-categories/dashboard" class="breadcrumb-link">{{ t('taskCategory.tabs.dashboard') }}</RouterLink></sl-breadcrumb-item>
        <sl-breadcrumb-item>{{ t('taskCategory.tabs.search') }}</sl-breadcrumb-item>
    </sl-breadcrumb>

    <header>
        <h1 class="title">{{ t('taskCategory.title') }}</h1>
        <p class="subtitle">{{ t('taskCategory.subtitle.search') }}</p>

        <ListingBox :fetch-function="fetchTaskCategories" v-slot="{elements}">
            <li v-for="taskCategory in elements" :key="taskCategory.category">
                <TaskCategoryPrinter class="listing-box" :task-category="taskCategory" :link="`/task-categories/view/${taskCategory.category}`"/>
            </li>
        </ListingBox>
    </header>
</div>
</template>
