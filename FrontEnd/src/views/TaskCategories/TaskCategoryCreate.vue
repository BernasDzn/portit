<script setup lang="ts">
import { ref } from 'vue';
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import { useI18n } from 'vue-i18n';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { ITaskCategoryService } from '@/service/IService/ITaskCategoryService';
import type TaskCategoryDto from '@/model/dto/TaskCategoryDto';
import { TaskCategory } from '@/model/TaskCategory';

const { t } = useI18n();

const taskCategory = ref<TaskCategoryDto>({
    name: '',
    category: '',
    description: ''
});

const taskCategoryService = container.get<ITaskCategoryService>(TYPES.taskCategoryService);

const submitTaskCategory = (obj:any) => {
    var taskCategoryObj = new TaskCategory(obj);
    return taskCategoryService.createTaskCategory(taskCategoryObj.toDto());
};

</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item>
                <RouterLink to="/task-categories/dashboard" class="breadcrumb-link">{{ t('taskCategory.tabs.dashboard') }}</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ t('taskCategory.tabs.create') }}</sl-breadcrumb-item>
        </sl-breadcrumb>
        <h1 class="title">{{ t('taskCategory.tabs.create') }}</h1>
        <p class="subtitle">{{ t('taskCategory.subtitle.create') }}</p>

        <EntityForm :object="taskCategory" :submit-function="submitTaskCategory">
            <div class="form">
                <div class="general-info">
                    <p class="section-title">{{ t('taskCategory.generalFields') }}</p>
                    <FormField class="field" inputId="task-category-name"
                        :name="t('taskCategory.fields.name.title') + '*'" v-model="taskCategory.name"
                        :placeholderText="t('taskCategory.fields.name.placeholder')" required />
                    <FormField class="field" inputId="task-category-category"
                        :name="t('taskCategory.fields.category.title') + '*'" v-model="taskCategory.category"
                        :placeholderText="t('taskCategory.fields.category.placeholder')" required />
                    <FormField class="field" inputId="task-category-description" :type="'textarea'"
                        :name="t('taskCategory.fields.description.title') + '*'" v-model="taskCategory.description"
                        :placeholderText="t('taskCategory.fields.description.placeholder')" required />
                </div>
            </div>
        </EntityForm>
    </div>
</template>

<style scoped>
.form {
    display: flex;
    flex-direction: row;
    flex-wrap: wrap;
}

.field {
    margin-bottom: 1rem;
    width: 30rem;
    padding: 0.5rem;
}

.section-title {
    font-size: 0.8rem;
    margin-bottom: 1rem;
    color: var(--sl-color-neutral-400);
}
</style>
