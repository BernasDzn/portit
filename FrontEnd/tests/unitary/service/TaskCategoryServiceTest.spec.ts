import { describe, it, expect, vi, beforeEach } from 'vitest';
import { TaskCategoryService } from '@/service/TaskCategoryService';
import type { IHttpService, Response } from '@/service/IService/IHttpService';
import type TaskCategoryDto from '@/model/dto/TaskCategoryDto';
import type { Page } from '@/model/Page';
import { TaskCategory } from '@/model/TaskCategory';

describe('TaskCategoryService', () => {
    let taskCategoryService: TaskCategoryService;
    let mockHttpService: IHttpService;

    const mockTaskCategoryDto: TaskCategoryDto = {
        name: 'Loading',
        category: 'LOAD',
        description: 'Loading operations'
    };

    const mockTaskCategory = new TaskCategory(mockTaskCategoryDto);

    const mockPage: Page<TaskCategoryDto> = {
        items: [mockTaskCategoryDto],
        pageNumber: 1,
        pageSize: 10,
        pageCount: 1,
    };

    beforeEach(() => {
        mockHttpService = {
            get: vi.fn(),
            getWithoutCredentials: vi.fn(),
            post: vi.fn(),
            put: vi.fn(),
            patch: vi.fn(),
            delete: vi.fn(),
        };

        taskCategoryService = new TaskCategoryService(mockHttpService);
    });

    describe('getAllTaskCategories', () => {
        it('should return page of task categories without filter', async () => {
            vi.mocked(mockHttpService.get).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: mockPage,
            } as Response<Page<TaskCategoryDto>>);

            const result = await taskCategoryService.getAllTaskCategories();

            expect(mockHttpService.get).toHaveBeenCalledWith('/oem/task-categories?');
            expect(result).toEqual(mockPage);
        });

        it('should return page of task categories with filter', async () => {
            vi.mocked(mockHttpService.get).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: mockPage,
            } as Response<Page<TaskCategoryDto>>);

            const result = await taskCategoryService.getAllTaskCategories({
                filter: { name: 'Loading' },
                pageNumber: 1,
                pageSize: 10
            });

            expect(mockHttpService.get).toHaveBeenCalledWith(expect.stringContaining('/oem/task-categories?'));
            expect(result).toEqual(mockPage);
        });

        it('should throw error when getAllTaskCategories fails', async () => {
            vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Network error'));

            await expect(taskCategoryService.getAllTaskCategories()).rejects.toThrow('Network error');
        });
    });

    describe('getTaskCategoryByCode', () => {
        it('should return task category by code', async () => {
            vi.mocked(mockHttpService.get).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: mockTaskCategoryDto,
            } as Response<TaskCategoryDto>);

            const result = await taskCategoryService.getTaskCategoryByCode('LOAD');

            expect(mockHttpService.get).toHaveBeenCalledWith('/oem/task-categories/code/LOAD');
            expect(result).toBeDefined();
        });

        it('should return undefined when task category not found', async () => {
            vi.mocked(mockHttpService.get).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: undefined,
            } as Response<TaskCategoryDto | undefined>);

            const result = await taskCategoryService.getTaskCategoryByCode('NOTFOUND');

            expect(result).toBeUndefined();
        });

        it('should throw error when getTaskCategoryByCode fails', async () => {
            vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Network error'));

            await expect(taskCategoryService.getTaskCategoryByCode('LOAD')).rejects.toThrow('Network error');
        });
    });

    describe('createTaskCategory', () => {
        it('should create task category successfully', async () => {
            vi.mocked(mockHttpService.post).mockResolvedValue({
                status: 201,
                statusText: 'Created',
                data: mockTaskCategoryDto,
            } as Response<TaskCategoryDto>);

            const result = await taskCategoryService.createTaskCategory(mockTaskCategoryDto);

            expect(mockHttpService.post).toHaveBeenCalledWith('/oem/task-categories', mockTaskCategoryDto);
            expect(result).toEqual(mockTaskCategoryDto);
        });

        it('should throw error when createTaskCategory fails', async () => {
            vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Creation failed'));

            await expect(taskCategoryService.createTaskCategory(mockTaskCategoryDto)).rejects.toThrow('Creation failed');
        });
    });

    describe('updateTaskCategory', () => {
        it('should update task category successfully', async () => {
            const updatedDto = { ...mockTaskCategoryDto, description: 'Updated description' };

            vi.mocked(mockHttpService.put).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: updatedDto,
            } as Response<TaskCategoryDto>);

            const result = await taskCategoryService.updateTaskCategory(updatedDto);

            expect(mockHttpService.put).toHaveBeenCalledWith(
                `/oem/task-categories/${mockTaskCategoryDto.category}`,
                updatedDto
            );
            expect(result).toEqual(updatedDto);
        });

        it('should throw error when updateTaskCategory fails', async () => {
            vi.mocked(mockHttpService.put).mockRejectedValue(new Error('Update failed'));

            await expect(taskCategoryService.updateTaskCategory(mockTaskCategoryDto)).rejects.toThrow('Update failed');
        });
    });
});
