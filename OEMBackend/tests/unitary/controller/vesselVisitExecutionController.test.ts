import { VesselVisitExecutionController } from '../../../src/controllers/vesselVisitExecutionController';
import { VesselVisitExecutionService } from '../../../src/services/vesselVisitExecutionService';
import { VesselVisitExecutionDto } from '../../../src/dto/vesselVisitExecutionDto';
import { Page } from '../../../src/utils/page';

jest.mock('../../../src/services/vesselVisitExecutionService');

describe('VesselVisitExecutionController', () => {
	let controller: VesselVisitExecutionController;
	let mockService: jest.Mocked<VesselVisitExecutionService>;
	let mockreq : ExpressRequest;

	beforeEach(() => {
		jest.clearAllMocks();

		mockService = new VesselVisitExecutionService() as jest.Mocked<VesselVisitExecutionService>;
		controller = new VesselVisitExecutionController();
		mockreq = {user : { token: 'valid-token', emailAddress: 'system' }};
		(controller as any).vesselVisitExecutionService = mockService;
	});

	describe('openVesselVisitExecution', () => {
		it('should create and open a vessel visit execution with 201 status', async () => {
			const mockVve: VesselVisitExecutionDto = {
				id: 'vve-1',
				code: 'VVE-PORTO-1',
				relatedVVN: 'VVN-001',
				status: 'Open',
				createdBy: 'system',
				operationsExecuted: [],
				dateOpen: new Date(),
				dateClosed: undefined
			};

			mockService.createVesselVisitExecution = jest.fn().mockResolvedValue(mockVve);
			controller.setStatus = jest.fn();

			const result = await controller.openVesselVisitExecution('VVN-001', mockreq);

			expect(mockService.createVesselVisitExecution).toHaveBeenCalledWith('VVN-001', 'system');
			expect(controller.setStatus).toHaveBeenCalledWith(201);
			expect(result).toEqual(mockVve);
		});

		it('should throw error when VVE already exists', async () => {
			mockService.createVesselVisitExecution = jest.fn().mockRejectedValue(
				new Error('Vessel Visit Execution with related VVN VVN-002 was already opened before.')
			);

			await expect(controller.openVesselVisitExecution('VVN-002')).rejects.toThrow();
		});

		it('should throw error when operation plan not found', async () => {
			mockService.createVesselVisitExecution = jest.fn().mockRejectedValue(
				new Error('Operation Plan with VVN VVN-NOTFOUND not found.')
			);

			await expect(controller.openVesselVisitExecution('VVN-NOTFOUND')).rejects.toThrow();
		});
	});

	describe('closeVesselVisitExecution', () => {
		it('should close a vessel visit execution', async () => {
			const mockVve: VesselVisitExecutionDto = {
				id: 'vve-2',
				code: 'VVE-PORTO-2',
				relatedVVN: 'VVN-003',
				status: 'Closed',
				createdBy: 'system',
				operationsExecuted: [],
				dateOpen: new Date(),
				dateClosed: new Date()
			};

			mockService.closeVesselVisitExecution = jest.fn().mockResolvedValue(mockVve);

			const result = await controller.closeVesselVisitExecution('VVN-003');

			expect(mockService.closeVesselVisitExecution).toHaveBeenCalledWith('VVN-003');
			expect(result).toEqual(mockVve);
			expect(result.status).toBe('Closed');
		});

		it('should throw error when VVE not found', async () => {
			mockService.closeVesselVisitExecution = jest.fn().mockRejectedValue(
				new Error('Vessel Visit Execution with VVN VVN-NOTFOUND not found.')
			);

			await expect(controller.closeVesselVisitExecution('VVN-NOTFOUND')).rejects.toThrow();
		});

		it('should throw error when not all operations completed', async () => {
			mockService.closeVesselVisitExecution = jest.fn().mockRejectedValue(
				new Error('Cannot close Vessel Visit Execution because not all operations are completed.')
			);

			await expect(controller.closeVesselVisitExecution('VVN-004')).rejects.toThrow();
		});
	});

	describe('startOperation', () => {
		it('should start an operation on vessel visit execution', async () => {
			const operation = {
				id: 'op-1',
				type: 'LOAD',
				startTime: new Date()
			};

			const mockVve: VesselVisitExecutionDto = {
				id: 'vve-3',
				code: 'VVE-PORTO-3',
				relatedVVN: 'VVN-005',
				status: 'Open',
				createdBy: 'system',
				operationsExecuted: [],
				dateOpen: new Date()
			};

			mockService.startOperation = jest.fn().mockResolvedValue(mockVve);

			const result = await controller.startOperation('VVN-005', operation);

			expect(mockService.startOperation).toHaveBeenCalledWith('VVN-005', operation);
			expect(result).toEqual(mockVve);
		});
	});

	describe('completeOperation', () => {
		it('should complete an operation on vessel visit execution', async () => {
			const endTime = new Date();
			const body = { endTime: endTime.toISOString() };

			const mockVve: VesselVisitExecutionDto = {
				id: 'vve-4',
				code: 'VVE-PORTO-4',
				relatedVVN: 'VVN-006',
				status: 'Open',
				createdBy: 'system',
				operationsExecuted: [],
				dateOpen: new Date()
			};

			mockService.completeOperation = jest.fn().mockResolvedValue(mockVve);

			const result = await controller.completeOperation('VVN-006', 'op-2', body);

			expect(mockService.completeOperation).toHaveBeenCalledWith(
				'VVN-006',
				'op-2',
				expect.any(Date)
			);
			expect(result).toEqual(mockVve);
		});
	});

	describe('getVesselVisitExecution', () => {
		it('should return vessel visit execution by VVN', async () => {
			const mockVve: VesselVisitExecutionDto = {
				id: 'vve-5',
				code: 'VVE-PORTO-5',
				relatedVVN: 'VVN-007',
				status: 'Open',
				createdBy: 'system',
				operationsExecuted: [],
				dateOpen: new Date()
			};

			mockService.getVesselVisitExecutionByVVN = jest.fn().mockResolvedValue(mockVve);

			const result = await controller.getVesselVisitExecution('VVN-007');

			expect(mockService.getVesselVisitExecutionByVVN).toHaveBeenCalledWith('VVN-007');
			expect(result).toEqual(mockVve);
		});

		it('should return 404 when vessel visit execution not found', async () => {
			mockService.getVesselVisitExecutionByVVN = jest.fn().mockResolvedValue(null);
			controller.setStatus = jest.fn();

			const result = await controller.getVesselVisitExecution('VVN-NOTFOUND');

			expect(controller.setStatus).toHaveBeenCalledWith(404);
			expect(result).toEqual({
				message: 'Vessel Visit Execution with VVN VVN-NOTFOUND not found.'
			});
		});
	});

	describe('countVesselVisitExecutions', () => {
		it('should return count of vessel visit executions', async () => {
			mockService.count = jest.fn().mockResolvedValue(5);

			const result = await controller.countVesselVisitExecutions();

			expect(mockService.count).toHaveBeenCalled();
			expect(result).toEqual({ count: 5 });
		});

		it('should return zero count when no executions exist', async () => {
			mockService.count = jest.fn().mockResolvedValue(0);

			const result = await controller.countVesselVisitExecutions();

			expect(result).toEqual({ count: 0 });
		});
	});

	describe('getAllVesselVisitExecutions', () => {
		it('should return paginated vessel visit executions with defaults', async () => {
			const mockPage: Page<VesselVisitExecutionDto> = {
				pageNumber: 1,
				pageSize: 10,
				pageCount: 1,
				items: [
					{
						id: 'vve-6',
						code: 'VVE-PORTO-6',
						relatedVVN: 'VVN-008',
						status: 'Open',
						createdBy: 'system',
						operationsExecuted: [],
						dateOpen: new Date()
					}
				]
			};

			mockService.getAllVesselVisitExecutions = jest.fn().mockResolvedValue(mockPage);

			const result = await controller.getAllVesselVisitExecutions();

			expect(mockService.getAllVesselVisitExecutions).toHaveBeenCalledWith({pageNumber: 1, pageSize: 10});
			expect(result).toEqual(mockPage);
			expect(result.items).toHaveLength(1);
		});

		it('should return paginated vessel visit executions with custom parameters', async () => {
			const mockPage: Page<VesselVisitExecutionDto> = {
				pageNumber: 2,
				pageSize: 20,
				pageCount: 2,
				items: []
			};

			mockService.getAllVesselVisitExecutions = jest.fn().mockResolvedValue(mockPage);

			const result = await controller.getAllVesselVisitExecutions(2, 20);

			expect(mockService.getAllVesselVisitExecutions).toHaveBeenCalledWith({pageNumber: 2, pageSize: 20});
			expect(result).toEqual(mockPage);
		});

		it('should handle empty results', async () => {
			const emptyPage: Page<VesselVisitExecutionDto> = {
				pageNumber: 1,
				pageSize: 10,
				pageCount: 0,
				items: []
			};

			mockService.getAllVesselVisitExecutions = jest.fn().mockResolvedValue(emptyPage);

			const result = await controller.getAllVesselVisitExecutions();

			expect(result.items).toHaveLength(0);
			expect(result.pageCount).toBe(0);
		});
	});
});
