import { TaskCategory } from "../../../src/domain/taskCategory";

describe('TaskCategory', () => {
	describe('constructor', () => {
		it('should create a task category with all properties', () => {
			const taskCategory = new TaskCategory({
				id: 'cat-1',
				category: 'MAINT',
				name: 'Maintenance Tasks',
				description: 'Regular maintenance activities'
			});

			expect(taskCategory.id).toBe('cat-1');
			expect(taskCategory.name).toBe('Maintenance Tasks');
			expect(taskCategory.description).toBe('Regular maintenance activities');
			expect(taskCategory.category).toBeDefined();
		});

		it('should create a task category with undefined id', () => {
			const taskCategory = new TaskCategory({
				id: undefined,
				category: 'REPAIR',
				name: 'Repair Tasks',
				description: 'Emergency repairs'
			});

			expect(taskCategory.id).toBeUndefined();
			expect(taskCategory.name).toBe('Repair Tasks');
			expect(taskCategory.description).toBe('Emergency repairs');
		});

		it('should create IdCode from category string', () => {
			const taskCategory = new TaskCategory({
				id: 'cat-2',
				category: 'INSP',
				name: 'Inspection Tasks',
				description: 'Quality inspections'
			});

			expect(taskCategory.category).toBeDefined();
			expect(taskCategory.category.getValue()).toBe('INSP');
		});
	});

	describe('toDto', () => {
		it('should convert task category to DTO', () => {
			const taskCategory = new TaskCategory({
				id: 'cat-100',
				category: 'LOAD',
				name: 'Loading Operations',
				description: 'Container loading tasks'
			});

			const dto = taskCategory.toDto();

			expect(dto.id).toBe('cat-100');
			expect(dto.name).toBe('Loading Operations');
			expect(dto.category).toBe('LOAD');
			expect(dto.description).toBe('Container loading tasks');
		});

		it('should handle undefined id in DTO', () => {
			const taskCategory = new TaskCategory({
				id: undefined,
				category: 'UNLD',
				name: 'Unloading Operations',
				description: 'Container unloading tasks'
			});

			const dto = taskCategory.toDto();

			expect(dto.id).toBeUndefined();
			expect(dto.name).toBe('Unloading Operations');
			expect(dto.category).toBe('UNLD');
		});
	});
});
