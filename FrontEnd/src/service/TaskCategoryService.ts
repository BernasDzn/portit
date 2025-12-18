import { inject, injectable } from 'inversify';
import { TYPES } from '@/inversify/types';
import type { IHttpService } from './IService/IHttpService';
import type { Page } from '@/model/Page';
import type TaskCategoryDto from '@/model/dto/TaskCategoryDto';
import type { ITaskCategoryService } from './IService/ITaskCategoryService';
import { TaskCategory } from '@/model/TaskCategory';

@injectable()
export class TaskCategoryService implements ITaskCategoryService {

    constructor(
        @inject(TYPES.api)
        private http: IHttpService
    ) { }

    async getAllTaskCategories(): Promise<Page<TaskCategoryDto>> {        
        const res = await this.http.get<Page<TaskCategoryDto>>(`/oem/task-categories`);
        return res.data;
    }

    async getTaskCategoryByCode(code: string): Promise<TaskCategoryDto | undefined> {
        const res = await this.http.get<TaskCategoryDto>(`/oem/task-categories/code/${code}`);
        return res.data ? TaskCategory.fromDto(res.data) : undefined;
    }

    async createTaskCategory(taskCategory: TaskCategoryDto): Promise<TaskCategoryDto> {
        const res = await this.http.post<TaskCategoryDto>('/oem/task-categories', taskCategory);
        return res.data;
    }

    async updateTaskCategory(id: string, taskCategory: TaskCategory): Promise<TaskCategoryDto> {
        const res = await this.http.patch<TaskCategoryDto>(`/oem/task-categories/${id}`, taskCategory.toDto());
        return res.data;
    }
}
