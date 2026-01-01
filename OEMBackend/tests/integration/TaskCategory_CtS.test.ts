import { TaskCategoryController } from "../../src/controllers/taskCategoryController";
import { TaskCategoryService } from "../../src/services/taskCategoryService";

describe('TaskCategory Controller->Service integration', () => {
    let controller: TaskCategoryController;
    let service: TaskCategoryService;
    let mockRepo: any;

    beforeEach(() => {
        service = new TaskCategoryService();
        controller = new TaskCategoryController();

        mockRepo = { count: jest.fn(), getPaged: jest.fn(), getById: jest.fn(), save: jest.fn(), update: jest.fn() };
        (service as any).taskCategoryRepository = mockRepo;
        (controller as any).taskCategoryService = service;
    });

    describe('getAllCategories', () => {
        it('forwards to service', async () => {
            const page = { pageNumber: 0, pageSize: 10, pageCount: 1, items: [] };
            mockRepo.getAllCategories = jest.fn().mockResolvedValue(page);
            (service as any).getAllCategories = jest.fn().mockImplementation((f: any) => mockRepo.getAllCategories(f));
            const res = await controller.getAllCategories();
            expect(res).toEqual(page);
        });
    });

    describe('createCategory', () => {
        it('returns created category', async () => {
            const body = { name: 'cat', description: 'd' } as any;
            (service as any).createCategory = jest.fn().mockResolvedValue({ id: 'C1', ...body });
            const res = await controller.createCategory(body);
            expect(res).toEqual({ id: 'C1', ...body });
        });
    });

    describe('updateCategory and getCategoryByCode', () => {
        it('updateCategory returns 404 when not found', async () => {
            (service as any).updateCategory = jest.fn().mockResolvedValue(null);
            const res = await controller.updateCategory('NA', { name: 'u' } as any);
            expect(res).toEqual({ message: 'Task category not found' });
        });

        it('updateCategory returns updated category', async () => {
            (service as any).updateCategory = jest.fn().mockResolvedValue({ id: 'C2', name: 'u' });
            const res = await controller.updateCategory('C2', { name: 'u' } as any);
            expect(res).toEqual({ id: 'C2', name: 'u' });
        });

        it('getCategoryByCode returns 404 when not found', async () => {
            (service as any).getCategoryByCode = jest.fn().mockResolvedValue(null);
            const res = await controller.getCategoryByCode('X');
            expect(res).toEqual({ message: 'Task category not found' });
        });

        it('getCategoryByCode returns category when found', async () => {
            (service as any).getCategoryByCode = jest.fn().mockResolvedValue({ code: 'X', name: 'n' });
            const res = await controller.getCategoryByCode('X');
            expect(res).toEqual({ code: 'X', name: 'n' });
        });
    });

});
