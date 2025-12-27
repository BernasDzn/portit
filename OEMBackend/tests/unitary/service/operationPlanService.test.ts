/// <reference types="jest" />
import { OperationPlanDto } from "../../../src/dto/operationPlanDto";
import { OperationPlanRepository } from "../../../src/repository/operationPlanRepository";
import { OperationPlanService } from "../../../src/services/operationPlanService";
import { PlanFilter } from "../../../src/dto/filters/planFilter";
import { Page } from "../../../src/utils/page";
import OperationPlan from "../../../src/domain/operationPlan";

describe('OperationPlanService', () => {
	let service: OperationPlanService;
	let mockRepo: jest.Mocked<OperationPlanRepository>;

	beforeEach(() => {
		jest.clearAllMocks();
		
		// Create service
		service = new OperationPlanService();
		
		// Create mock repository
		mockRepo = {
			getAll: jest.fn(),
			getById: jest.fn(),
			getByDateGrouped: jest.fn(),
			getNotificationsWithoutPlan: jest.fn(),
			deleteById: jest.fn(),
			getContainersOfNotification: jest.fn(),
			save: jest.fn(),
			update: jest.fn()
		} as unknown as jest.Mocked<OperationPlanRepository>;

		// Replace the repository in the service with our mock
		(service as any).operationPlanRepository = mockRepo;
	});

	describe('getAll', () => {
		it('should return paginated operation plans', async () => {
			const mockPlan = {
				toDto: jest.fn().mockReturnValue({
					id: 'plan-1',
					relatedVVN: 'VVN001',
					dock: 'Dock1'
				})
			} as any;

			const mockPage: Page<OperationPlan> = {
				pageNumber: 1,
				pageSize: 10,
				pageCount: 1,
				items: [mockPlan]
			};

			mockRepo.getAll.mockResolvedValue(mockPage);

			const result = await service.getAll({ pageNumber: 1, pageSize: 10 });

			expect(mockRepo.getAll).toHaveBeenCalledWith({ pageNumber: 1, pageSize: 10 });
			expect(result).toBeDefined();
			expect(result.items).toHaveLength(1);
		});

		it('should handle empty results', async () => {
			const emptyPage: Page<OperationPlan> = {
				pageNumber: 1,
				pageSize: 10,
				pageCount: 0,
				items: []
			};

			mockRepo.getAll.mockResolvedValue(emptyPage);

			const result = await service.getAll({ pageNumber: 1, pageSize: 10 });

			expect(result.items).toHaveLength(0);
			expect(result.pageCount).toBe(0);
		});

		it('should handle different page sizes', async () => {
			const mockPlan = {
				toDto: jest.fn().mockReturnValue({
					id: 'plan-1',
					relatedVVN: 'VVN001',
					dock: 'Dock1'
				})
			} as any;

			const mockPage: Page<OperationPlan> = {
				pageNumber: 2,
				pageSize: 5,
				pageCount: 3,
				items: [mockPlan]
			};

			mockRepo.getAll.mockResolvedValue(mockPage);

			const result = await service.getAll({ pageNumber: 2, pageSize: 5 });

			expect(mockRepo.getAll).toHaveBeenCalledWith({ pageNumber: 2, pageSize: 5 });
			expect(result.pageNumber).toBe(2);
			expect(result.pageSize).toBe(5);
		});
	});

	describe('getById', () => {
		it('should return operation plan when found', async () => {
			const mockPlan = {
				toDto: jest.fn().mockReturnValue({
					id: 'plan-1',
					relatedVVN: 'VVN001',
					dock: 'Dock1'
				})
			} as any;

			mockRepo.getById.mockResolvedValue(mockPlan);

			const result = await service.getById('plan-1');

			expect(mockRepo.getById).toHaveBeenCalledWith('plan-1');
			expect(result).toBeDefined();
			expect(result?.id).toBe('plan-1');
		});

		it('should return null when operation plan not found', async () => {
			mockRepo.getById.mockResolvedValue(null);

			const result = await service.getById('non-existent-id');

			expect(mockRepo.getById).toHaveBeenCalledWith('non-existent-id');
			expect(result).toBeNull();
		});
	});

	describe('getByDateGrouped', () => {
		it('should return operation plans grouped by date', async () => {
			const groupedPlans = [
				{
					date: '2024-10-01',
					plans: [
						{
							id: 'plan-1',
							relatedVVN: 'VVN001',
							dock: 'Dock1'
						}
					]
				}
			];

			mockRepo.getByDateGrouped.mockResolvedValue(groupedPlans);

			const result = await service.getByDateGrouped();

			expect(mockRepo.getByDateGrouped).toHaveBeenCalled();
			expect(result).toBeDefined();
			expect(result).toHaveLength(1);
		});
	});
});