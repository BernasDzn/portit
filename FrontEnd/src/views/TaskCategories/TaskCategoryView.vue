<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import type { IncidentType, IncidentTypeDto } from '@/model/IncidentType';
import EntityView from '@/components/crud/EntityView.vue';
import IncidentTypePrinter from '@/components/printers/IncidentTypePrinter.vue';
import type { IIncidentTypeService } from '@/service/IService/IIncidentTypeService';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import { useI18n } from 'vue-i18n';
import type TaskCategoryDto from '@/model/dto/TaskCategoryDto';
import type { ITaskCategoryService } from '@/service/IService/ITaskCategoryService';

const { t } = useI18n();
const route = useRoute();
const router = useRouter();

const taskCategoryService = container.get<ITaskCategoryService>(TYPES.taskCategoryService);
const taskCategoryId = computed(() => route.params.id as string);

const fetchTaskCategory = async (): Promise<TaskCategoryDto | undefined> => {
    return await taskCategoryService.getTaskCategoryByCode(taskCategoryId.value);
};

</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item>
                <RouterLink to="/task-categories/dashboard" class="breadcrumb-link">{{ t('taskCategory.tabs.dashboard') }}</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                <RouterLink to="/task-categories/search" class="breadcrumb-link">{{ t('taskCategory.tabs.search') }}</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ taskCategoryId }}</sl-breadcrumb-item>
        </sl-breadcrumb>

        <EntityView :fetch-function="fetchTaskCategory" v-slot="entity">
            <div>
                <div class="opposed">
                    <div class="view-header">
                        <span class="material-icons icon" aria-hidden="true">category</span>
                        <div>
                            <h2 class="title">{{ entity.element.name }}</h2>
                        </div>
                    </div>
                    <div style="display: flex; gap: 0.5rem;">
                        <RouterLink :to="`/task-categories/edit/${entity.element.category}`">
                            <sl-button variant="default" size="large">
                                <sl-icon slot="prefix" name="pencil"></sl-icon>
                                {{ t('buttons.edit') }}
                            </sl-button>
                        </RouterLink>
                    </div>
                </div>
                <div class="viewing-content">
                    <sl-card class="info-card" style="flex: 100%;">
                        <p>{{ t('taskCategory.taskCategoryInformation') }}</p>
                        <div class="info-grid">
                            <div class="info-block">
                                <span class="label">{{ t('taskCategory.fields.category.title') }}</span>
                                <p>{{ entity.element.category }}</p>
                            </div>
                            <div class="info-block">
                                <span class="label">{{ t('taskCategory.fields.name.title') }}</span>
                                <p>{{ entity.element.name }}</p>
                            </div>
                            <div class="info-block" style="flex: 100%;">
                                <span class="label">{{ t('taskCategory.fields.description.title') }}</span>
                                <p>{{ entity.element.description }}</p>
                            </div>
                        </div>
                    </sl-card>
                </div>
            </div>
        </EntityView>
    </div>
</template>

<style scoped>
.info-block {
    flex: 1 1 45%;
    min-width: 200px;
}

.viewing-content {
    display: flex;
    flex-wrap: wrap;
    gap: 1rem;
}

.info-grid {
    display: flex;
    flex-wrap: wrap;
    gap: 1rem;
}

.label {
    font-weight: 600;
    color: var(--sl-color-neutral-600);
    font-size: 0.9rem;
}
</style>
