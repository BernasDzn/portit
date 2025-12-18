import type { TaskCategoryFilter } from '@/model/dto/TaskCategoryDto';
import type TaskCategoryDto from '@/model/dto/TaskCategoryDto';
import type { Page } from '@/model/Page';
import type { Filter } from 'mongodb';

export interface ITaskCategoryService {
    getAllTaskCategories(filtering?: Filter<TaskCategoryFilter>): Promise<Page<TaskCategoryDto>>;
    getTaskCategoryByCode(code: string): Promise<TaskCategoryDto | undefined>;
    createTaskCategory(taskCategory: TaskCategoryDto): Promise<TaskCategoryDto>;
    updateTaskCategory(id: string, taskCategory: TaskCategoryDto): Promise<TaskCategoryDto>;
}
