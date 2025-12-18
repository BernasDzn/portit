import type TaskCategoryDto from '@/model/dto/TaskCategoryDto';
import type { Filter, Page } from '@/model/Page';

export interface ITaskCategoryService {
    getAllTaskCategories(): Promise<Page<TaskCategoryDto>>;
    getTaskCategoryByCode(code: string): Promise<TaskCategoryDto | undefined>;
    createTaskCategory(taskCategory: TaskCategoryDto): Promise<TaskCategoryDto>;
    updateTaskCategory(id: string, taskCategory: TaskCategoryDto): Promise<TaskCategoryDto>;
}
