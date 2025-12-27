import { TaskCategoryService } from "../../../src/services/taskCategoryService";
import { TaskCategoryRepository } from "../../../src/repository/taskCategoryRepository";
import { TaskCategory } from "../../../src/domain/taskCategory";
import { CreateTaskCategoryDto } from "../../../src/dto/taskCategoryDto";
import { Page } from "../../../src/utils/page";

describe('TaskCategoryService', () => {
	let service: TaskCategoryService;
	let mockRepo: jest.Mocked<TaskCategoryRepository>;

	beforeEach(() => {
		jest.clearAllMocks();

		service = new TaskCategoryService();

		mockRepo = {
			createCategory: jest.fn(),
			updateCategory: jest.fn(),
			getCategoryByCode: jest.fn(),
			getAllCategories: jest.fn(),
		} as unknown as jest.Mocked<TaskCategoryRepository>;

		(service as any).taskCategoryRepository = mockRepo;
	});

	describe('createCategory', () => {
		it('should create a new task category', async () => {
			const createDto: CreateTaskCategoryDto = {
				category: 'MAINT',
				name: 'Maintenance',
				description: 'Maintenance tasks'
			};

			const createdCategory = new TaskCategory({
				id: 'cat-1',
				category: 'MAINT',
				name: 'Maintenance',
				description: 'Maintenance tasks'
			});

			mockRepo.getCategoryByCode.mockResolvedValue(null);
			mockRepo.createCategory.mockResolvedValue(createdCategory);

			const result = await service.createCategory(createDto);

			expect(mockRepo.getCategoryByCode).toHaveBeenCalledWith('MAINT');
			expect(mockRepo.createCategory).toHaveBeenCalled();
			expect(result).toBeDefined();
			expect(result.name).toBe('Maintenance');
		});

		it('should throw error when category already exists', async () => {
			const createDto: CreateTaskCategoryDto = {
				category: 'LOAD',
				name: 'Loading',
				description: 'Loading tasks'
			};

			const existingCategory = new TaskCategory({
				id: 'cat-2',
				category: 'LOAD',
				name: 'Loading',
				description: 'Loading tasks'
			});

			mockRepo.getCategoryByCode.mockResolvedValue(existingCategory);

			await expect(service.createCategory(createDto)).rejects.toThrow(
				'Task Category with code LOAD already exists.'
			);

			expect(mockRepo.getCategoryByCode).toHaveBeenCalledWith('LOAD');
			expect(mockRepo.createCategory).not.toHaveBeenCalled();
		});
	});

	describe('updateCategory', () => {
		it('should update an existing task category', async () => {
			const updateDto: CreateTaskCategoryDto = {
				category: 'INSP',
				name: 'Inspection Updated',
				description: 'Updated inspection tasks'
			};

			const existingCategory = new TaskCategory({
				id: 'cat-3',
				category: 'INSP',
				name: 'Inspection',
				description: 'Inspection tasks'
			});

			const updatedCategory = new TaskCategory({
				id: 'cat-3',
				category: 'INSP',
				name: 'Inspection Updated',
				description: 'Updated inspection tasks'
			});

			mockRepo.getCategoryByCode.mockResolvedValue(existingCategory);
			mockRepo.updateCategory.mockResolvedValue(updatedCategory);

			const result = await service.updateCategory('INSP', updateDto);

			expect(mockRepo.getCategoryByCode).toHaveBeenCalledWith('INSP');
			expect(mockRepo.updateCategory).toHaveBeenCalled();
			expect(result).toBeDefined();
			expect(result?.name).toBe('Inspection Updated');
		});

		it('should throw error when category does not exist', async () => {
			const updateDto: CreateTaskCategoryDto = {
				category: 'XXXX',
				name: 'Non-existent',
				description: 'Does not exist'
			};

			mockRepo.getCategoryByCode.mockResolvedValue(null);

			await expect(service.updateCategory('XXXX', updateDto)).rejects.toThrow(
				'Task Category with code XXXX does not exist.'
			);

			expect(mockRepo.updateCategory).not.toHaveBeenCalled();
		});

		it('should return null when update fails', async () => {
			const updateDto: CreateTaskCategoryDto = {
				category: 'FAIL',
				name: 'Fail',
				description: 'Should fail'
			};

			const existingCategory = new TaskCategory({
				id: 'cat-fail',
				category: 'FAIL',
				name: 'Fail',
				description: 'Should fail'
			});

			mockRepo.getCategoryByCode.mockResolvedValue(existingCategory);
			mockRepo.updateCategory.mockResolvedValue(null);

			const result = await service.updateCategory('FAIL', updateDto);

			expect(result).toBeNull();
		});
	});

	describe('getCategoryByCode', () => {
		it('should return category when found', async () => {
			const category = new TaskCategory({
				id: 'cat-4',
				category: 'UNLD',
				name: 'Unloading',
				description: 'Unloading tasks'
			});

			mockRepo.getCategoryByCode.mockResolvedValue(category);

			const result = await service.getCategoryByCode('UNLD');

			expect(mockRepo.getCategoryByCode).toHaveBeenCalledWith('UNLD');
			expect(result).toBeDefined();
			expect(result?.name).toBe('Unloading');
		});

		it('should return null when category not found', async () => {
			mockRepo.getCategoryByCode.mockResolvedValue(null);

			const result = await service.getCategoryByCode('NOTF');

			expect(mockRepo.getCategoryByCode).toHaveBeenCalledWith('NOTF');
			expect(result).toBeNull();
		});
	});

	describe('getAllCategories', () => {
		it('should return paginated list of categories', async () => {
			const mockPage: Page<any> = {
				pageNumber: 1,
				pageSize: 10,
				pageCount: 1,
				items: [
					{
						id: 'cat-5',
						name: 'Category 1',
						category: 'CAT1',
						description: 'First category'
					}
				]
			};

			mockRepo.getAllCategories.mockResolvedValue(mockPage);

			const result = await service.getAllCategories({ pageNumber: 1, pageSize: 10 });

			expect(mockRepo.getAllCategories).toHaveBeenCalledWith({ pageNumber: 1, pageSize: 10 });
			expect(result.items).toHaveLength(1);
			expect(result.pageCount).toBe(1);
		});

		it('should handle empty results', async () => {
			const emptyPage: Page<any> = {
				pageNumber: 1,
				pageSize: 10,
				pageCount: 0,
				items: []
			};

			mockRepo.getAllCategories.mockResolvedValue(emptyPage);

			const result = await service.getAllCategories({ pageNumber: 1, pageSize: 10 });

			expect(result.items).toHaveLength(0);
			expect(result.pageCount).toBe(0);
		});
	});
});
