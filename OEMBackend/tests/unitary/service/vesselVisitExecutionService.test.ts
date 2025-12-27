import { VesselVisitExecutionService } from "../../../src/services/vesselVisitExecutionService";
import { VesselVisitExecutionRepository } from "../../../src/repository/vesselVisitExecutionRepository";
import { OperationPlanRepository } from "../../../src/repository/operationPlanRepository";
import { TaskCategoryRepository } from "../../../src/repository/taskCategoryRepository";
import VesselVisitExecution, { OperationWithStatus } from "../../../src/domain/vesselVisitExecution";
import OperationPlan from "../../../src/domain/operationPlan";
import LinkedList from "../../../src/utils/linkedList";

describe('VesselVisitExecutionService', () => {
	let service: VesselVisitExecutionService;
	let mockVveRepo: jest.Mocked<VesselVisitExecutionRepository>;
	let mockOpPlanRepo: jest.Mocked<OperationPlanRepository>;
	let mockTaskCategoryRepo: jest.Mocked<TaskCategoryRepository>;

	beforeEach(() => {
		jest.clearAllMocks();

		service = new VesselVisitExecutionService();

		mockVveRepo = {
			createVesselVisitExecution: jest.fn(),
			updateVesselVisitExecution: jest.fn(),
			getByVVN: jest.fn(),
			count: jest.fn(),
		} as unknown as jest.Mocked<VesselVisitExecutionRepository>;

		mockOpPlanRepo = {
			getByVVN: jest.fn(),
		} as unknown as jest.Mocked<OperationPlanRepository>;

		mockTaskCategoryRepo = {
			getCategoryByCode: jest.fn(),
		} as unknown as jest.Mocked<TaskCategoryRepository>;

		(service as any).vesselVisitExecutionRepository = mockVveRepo;
		(service as any).operationPlanRepository = mockOpPlanRepo;
		(service as any).taskCategoryRepository = mockTaskCategoryRepo;
	});

	describe('createVesselVisitExecution', () => {
		it('should create a new vessel visit execution', async () => {
			const mockPlan = {
				relatedVVN: 'VVN-001',
				operationSchedule: {
					toArray: jest.fn().mockReturnValue([])
				}
			} as any;

			const createdVve = {
				id: 'vve-1',
				code: 'VVE-PORTO-1',
				relatedVVN: 'VVN-001',
				status: 'Open',
				createdBy: 'user-123',
				operationsExecuted: [],
				toDto: jest.fn().mockReturnValue({
					id: 'vve-1',
					code: 'VVE-PORTO-1',
					relatedVVN: 'VVN-001',
					status: 'Open'
				})
			} as any;

			mockVveRepo.getByVVN.mockResolvedValue(null);
			mockOpPlanRepo.getByVVN.mockResolvedValue(mockPlan);
			mockVveRepo.count.mockResolvedValue(0);
			mockVveRepo.createVesselVisitExecution.mockResolvedValue(createdVve);

			const result = await service.createVesselVisitExecution('VVN-001', 'user-123');

			expect(mockVveRepo.getByVVN).toHaveBeenCalledWith('VVN-001');
			expect(mockOpPlanRepo.getByVVN).toHaveBeenCalledWith('VVN-001');
			expect(mockVveRepo.createVesselVisitExecution).toHaveBeenCalled();
			expect(result).toBeDefined();
			expect(result.relatedVVN).toBe('VVN-001');
		});

		it('should throw error when VVE already exists', async () => {
			const existingVve = {
				id: 'vve-existing',
				relatedVVN: 'VVN-002'
			} as any;

			mockVveRepo.getByVVN.mockResolvedValue(existingVve);

			await expect(service.createVesselVisitExecution('VVN-002', 'user-456')).rejects.toThrow(
				'Vessel Visit Execution with related VVN VVN-002 was already opened before.'
			);

			expect(mockOpPlanRepo.getByVVN).not.toHaveBeenCalled();
		});

		it('should throw error when operation plan not found', async () => {
			mockVveRepo.getByVVN.mockResolvedValue(null);
			mockOpPlanRepo.getByVVN.mockResolvedValue(null);

			await expect(service.createVesselVisitExecution('VVN-NOTFOUND', 'user-789')).rejects.toThrow(
				'Operation Plan with VVN VVN-NOTFOUND not found.'
			);

			expect(mockVveRepo.createVesselVisitExecution).not.toHaveBeenCalled();
		});
	});

	describe('closeVesselVisitExecution', () => {
		it('should close vessel visit execution when all operations completed', async () => {
			const mockVve = {
				relatedVVN: 'VVN-003',
				status: 'Open',
				operationsExecuted: [
					{ status: 'Completed' },
					{ status: 'Completed' }
				],
				props: {},
				toDto: jest.fn().mockReturnValue({
					id: 'vve-3',
					relatedVVN: 'VVN-003',
					status: 'Closed'
				})
			} as any;

			mockVveRepo.getByVVN.mockResolvedValue(mockVve);
			mockVveRepo.updateVesselVisitExecution.mockResolvedValue(mockVve);

			const result = await service.closeVesselVisitExecution('VVN-003');

			expect(mockVveRepo.getByVVN).toHaveBeenCalledWith('VVN-003');
			expect(mockVveRepo.updateVesselVisitExecution).toHaveBeenCalled();
			expect(result).toBeDefined();
			expect(result.status).toBe('Closed');
		});

		it('should throw error when VVE not found', async () => {
			mockVveRepo.getByVVN.mockResolvedValue(null);

			await expect(service.closeVesselVisitExecution('VVN-NOTFOUND')).rejects.toThrow(
				'Vessel Visit Execution with VVN VVN-NOTFOUND not found.'
			);
		});

		it('should throw error when VVE already closed', async () => {
			const closedVve = {
				relatedVVN: 'VVN-004',
				status: 'Closed'
			} as any;

			mockVveRepo.getByVVN.mockResolvedValue(closedVve);

			await expect(service.closeVesselVisitExecution('VVN-004')).rejects.toThrow(
				'Vessel Visit Execution with VVN VVN-004 is already closed.'
			);

			expect(mockVveRepo.updateVesselVisitExecution).not.toHaveBeenCalled();
		});

		it('should throw error when not all operations completed', async () => {
			const mockVve = {
				relatedVVN: 'VVN-005',
				status: 'Open',
				operationsExecuted: [
					{ status: 'Completed' },
					{ status: 'InProgress' }
				]
			} as any;

			mockVveRepo.getByVVN.mockResolvedValue(mockVve);

			await expect(service.closeVesselVisitExecution('VVN-005')).rejects.toThrow(
				'Cannot close Vessel Visit Execution with VVN VVN-005 because not all operations are completed.'
			);

			expect(mockVveRepo.updateVesselVisitExecution).not.toHaveBeenCalled();
		});
	});
});
