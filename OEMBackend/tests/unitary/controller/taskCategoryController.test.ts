import { TaskCategoryController } from '../../../src/controllers/taskCategoryController';
import { TaskCategoryService } from '../../../src/services/taskCategoryService';
import { CreateTaskCategoryDto, TaskCategoryDto } from '../../../src/dto/taskCategoryDto';
import { Page } from '../../../src/utils/page';

jest.mock('../../../src/services/taskCategoryService');

describe('TaskCategoryController', () => {
	let controller: TaskCategoryController;
	let mockService: jest.Mocked<TaskCategoryService>;

	beforeEach(() => {
		jest.clearAllMocks();

		mockService = new TaskCategoryService() as jest.Mocked<TaskCategoryService>;
		controller = new TaskCategoryController();
		(controller as any).taskCategoryService = mockService;
	});

	describe('createCategory', () => {
		it('should create a task category and return 201 status', async () => {
			const createDto: CreateTaskCategoryDto = {
				category: 'MAINT',
				name: 'Maintenance',
				description: 'Maintenance tasks'
			};

			const createdDto: TaskCategoryDto = {
				id: 'cat-1',
				category: 'MAINT',
				name: 'Maintenance',
				description: 'Maintenance tasks'
			};

			mockService.createCategory = jest.fn().mockResolvedValue(createdDto);
			controller.setStatus = jest.fn();

			const result = await controller.createCategory(createDto);

			expect(mockService.createCategory).toHaveBeenCalledWith(createDto);
			expect(controller.setStatus).toHaveBeenCalledWith(201);
			expect(result).toEqual(createdDto);
		});

		it('should throw error when category already exists', async () => {
			const createDto: CreateTaskCategoryDto = {
				category: 'LOAD',
				name: 'Loading',
				description: 'Loading tasks'
			};

			mockService.createCategory = jest.fn().mockRejectedValue(
				new Error('Task Category with code LOAD already exists.')
			);

			await expect(controller.createCategory(createDto)).rejects.toThrow();
		});
	});

	describe('updateCategory', () => {
		it('should update a task category', async () => {
			const updateDto: CreateTaskCategoryDto = {
				category: 'INSP',
				name: 'Inspection Updated',
				description: 'Updated inspection tasks'
			};

			const updatedDto: TaskCategoryDto = {
				id: 'cat-3',
				category: 'INSP',
				name: 'Inspection Updated',
				description: 'Updated inspection tasks'
			};

			mockService.updateCategory = jest.fn().mockResolvedValue(updatedDto);

			const result = await controller.updateCategory('INSP', updateDto);

			expect(mockService.updateCategory).toHaveBeenCalledWith('INSP', updateDto);
			expect(result).toEqual(updatedDto);
		});

		it('should return 404 when category not found', async () => {
			const updateDto: CreateTaskCategoryDto = {
				category: 'NOTF',
				name: 'Not Found',
				description: 'Does not exist'
			};

			mockService.updateCategory = jest.fn().mockResolvedValue(null);
			controller.setStatus = jest.fn();

			const result = await controller.updateCategory('NOTF', updateDto);

			expect(controller.setStatus).toHaveBeenCalledWith(404);
			expect(result).toEqual({ message: 'Task category not found' });
		});
	});

	describe('getCategoryByCode', () => {
		it('should return category by code', async () => {
			const categoryDto: TaskCategoryDto = {
				id: 'cat-4',
				category: 'UNLD',
				name: 'Unloading',
				description: 'Unloading tasks'
			};

			mockService.getCategoryByCode = jest.fn().mockResolvedValue(categoryDto);

			const result = await controller.getCategoryByCode('UNLD');

			expect(mockService.getCategoryByCode).toHaveBeenCalledWith('UNLD');
			expect(result).toEqual(categoryDto);
		});

		it('should return 404 when category not found', async () => {
			mockService.getCategoryByCode = jest.fn().mockResolvedValue(null);
			controller.setStatus = jest.fn();

			const result = await controller.getCategoryByCode('NOTFOUND');

			expect(controller.setStatus).toHaveBeenCalledWith(404);
			expect(result).toEqual({ message: 'Task category not found' });
		});
	});

	describe('getAllCategories', () => {
		it('should return paginated categories with defaults', async () => {
			const mockPage: Page<TaskCategoryDto> = {
				pageNumber: 0,
				pageSize: 10,
				pageCount: 1,
				items: [
					{
						id: 'cat-1',
						category: 'CAT1',
						name: 'Category 1',
						description: 'First category'
					}
				]
			};

			mockService.getAllCategories = jest.fn().mockResolvedValue(mockPage);

			const result = await controller.getAllCategories();

			expect(mockService.getAllCategories).toHaveBeenCalledWith({
				pageNumber: 0,
				pageSize: 10,
				name: undefined
			});
			expect(result).toEqual(mockPage);
		});

		it('should return paginated categories with custom parameters', async () => {
			const mockPage: Page<TaskCategoryDto> = {
				pageNumber: 2,
				pageSize: 20,
				pageCount: 2,
				items: []
			};

			mockService.getAllCategories = jest.fn().mockResolvedValue(mockPage);

			const result = await controller.getAllCategories(2, 20, 'test');

			expect(mockService.getAllCategories).toHaveBeenCalledWith({
				pageNumber: 2,
				pageSize: 20,
				name: 'test'
			});
			expect(result).toEqual(mockPage);
		});

		it('should handle empty results', async () => {
			const emptyPage: Page<TaskCategoryDto> = {
				pageNumber: 0,
				pageSize: 10,
				pageCount: 0,
				items: []
			};

			mockService.getAllCategories = jest.fn().mockResolvedValue(emptyPage);

			const result = await controller.getAllCategories();

			expect(result.items).toHaveLength(0);
			expect(result.pageCount).toBe(0);
		});
	});
});
