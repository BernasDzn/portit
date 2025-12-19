<script setup lang="ts">
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import { ref, onMounted } from 'vue';
import { useRoute, RouterLink } from 'vue-router';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import Loading from '@/components/Loading.vue';
import { useI18n } from 'vue-i18n';
import type TaskCategoryDto from '@/model/dto/TaskCategoryDto';
import type { ITaskCategoryService } from '@/service/IService/ITaskCategoryService';
import { TaskCategory } from '@/model/TaskCategory';

const { t } = useI18n();
const taskCategoryService = container.get<ITaskCategoryService>(TYPES.taskCategoryService);

const route = useRoute();
const taskCategoryId = String(route.params.id || '');

let taskCategory = ref({
    name: '',
    category: '',
    description: ''
});

const loading = ref(true);

onMounted(async () => {
    loading.value = true;

    try {
        const data = await taskCategoryService.getTaskCategoryByCode(taskCategoryId);
        if (!data) return;
        
        taskCategory.value = {
            name: data.name,
            category: data.category,
            description: data.description
        };
    } catch (err) {
        console.error('Failed to load task category', err);
    } finally {
        loading.value = false;
    }
});

const updateTaskCategory = async (obj: TaskCategoryDto) => {
    const tc = new TaskCategory(obj);
    return taskCategoryService.updateTaskCategory(tc.toDto());
};

</script>

<template>
    <div class="incident-type-edit">
        <sl-breadcrumb>
            <sl-breadcrumb-item>
                <RouterLink to="/task-categories/dashboard" class="link">{{ t('taskCategory.tabs.dashboard') }}</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                <RouterLink to="/task-categories/search" class="link">{{ t('taskCategory.tabs.search') }}</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                <RouterLink :to="taskCategory.category ? `/task-categories/view/${taskCategory.category}` : '/task-categories/search'" class="link">
                    {{ taskCategory.category || t('taskCategory.title') }}
                </RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ t('buttons.edit') }}</sl-breadcrumb-item>
        </sl-breadcrumb>

        <h1 class="title">{{ t('taskCategory.tabs.edit') }}</h1>
        <p class="subtitle">{{ t('taskCategory.subtitle.edit') }}</p>

        <Loading v-if="loading" />
        <EntityForm :object="taskCategory" editing-id="taskCategoryId" :submit-function="updateTaskCategory" v-else>
            <div class="form">
                <div class="general-info">
                    <p class="section-title">{{ t('taskCategory.generalFields') }}</p>
                    <FormField class="field" inputId="task-category-category"
                        :name="t('taskCategory.fields.category.title') + '*'" v-model="taskCategory.category"
                        :placeholderText="t('taskCategory.fields.category.placeholder')" required :enabled="false" />
                    <FormField class="field" inputId="task-category-name"
                        :name="t('taskCategory.fields.name.title') + '*'" v-model="taskCategory.name"
                        :placeholderText="t('taskCategory.fields.name.placeholder')" required />
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

.fields-dropdown {
    display: flex;
    flex-direction: row;
    flex-wrap: wrap;
    gap: 1rem;
}

.field-dropdown {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    margin-bottom: 1rem;
    margin-top: 0.5rem;
    margin-left: 0.5rem;
    max-width: 30rem;
}

.section-title {
    font-size: 0.8rem;
    margin-bottom: 1rem;
    color: var(--sl-color-neutral-400);
}

.section-divider {
    display: block;
    width: 100%;
    height: 1px;
    background-color: var(--sl-color-neutral-200);
    margin: 1.5rem 0;
}
</style>
