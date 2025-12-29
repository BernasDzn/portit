import { SchedulingRequestController } from '../../../src/controllers/schedulingRequestController';
import { SchedulingRequestService } from '../../../src/services/schedulingRequestService';
import { WorkQueueItemDto } from '../../../src/dto/workQueueItemDto';

jest.mock('../../../src/services/schedulingRequestService');

describe('SchedulingRequestController', () => {
	let controller: SchedulingRequestController;
	let mockService: jest.Mocked<SchedulingRequestService>;
	let mockreq : ExpressRequest;

	beforeEach(() => {
		jest.clearAllMocks();

		mockService = new SchedulingRequestService() as jest.Mocked<SchedulingRequestService>;
		controller = new SchedulingRequestController();
		(controller as any).schedulingRequestService = mockService;
		mockreq = {user : { token: 'valid-token', emailAddress: 'system' }};
	});

	describe('scheduleRequest', () => {
		it('should schedule a request with all required parameters', async () => {
			const mockResult: WorkQueueItemDto = {
				id: 'wqi-1',
				operationPlanVVN: 'VVN-001',
				algorithmType: 'greedy',
				daysAhead: 1,
				dateToProcess: new Date(),
				result: 'Pending'
			};

			mockService.scheduleRequest = jest.fn().mockResolvedValue(mockResult);

			const result = await controller.scheduleRequest('VVN-001', 'greedy', 2, mockreq);

			expect(mockService.scheduleRequest).toHaveBeenCalledWith('VVN-001', 'greedy', 1, 'system');
			expect(result).toEqual(mockResult);
		});

		it('should return error when day is missing', async () => {
			controller.setStatus = jest.fn();

			const result = await controller.scheduleRequest('', 'greedy', 2);

			expect(controller.setStatus).toHaveBeenCalledWith(400);
			expect(result).toEqual({ message: 'Missing required query parameters: day and alg' });
		});

		it('should return error when algorithm is missing', async () => {
			controller.setStatus = jest.fn();

			const result = await controller.scheduleRequest('VVN-001', '', 2);

			expect(controller.setStatus).toHaveBeenCalledWith(400);
			expect(result).toEqual({ message: 'Missing required query parameters: day and alg' });
		});

		it('should use default daysAhead value of 2', async () => {
			const mockResult: WorkQueueItemDto = {
				id: 'wqi-2',
				operationPlanVVN: 'VVN-002',
				algorithmType: 'genetic',
				daysAhead: 1,
				dateToProcess: new Date(),
				result: 'Pending'
			};

			mockService.scheduleRequest = jest.fn().mockResolvedValue(mockResult);

			await controller.scheduleRequest('VVN-002', 'genetic', 2, mockreq);

			expect(mockService.scheduleRequest).toHaveBeenCalledWith('VVN-002', 'genetic', 1, 'system');
		});

		it('should override daysAhead parameter to 1 regardless of input', async () => {
			const mockResult: WorkQueueItemDto = {
				id: 'wqi-3',
				operationPlanVVN: 'VVN-003',
				algorithmType: 'tabu',
				daysAhead: 1,
				dateToProcess: new Date(),
				result: 'Pending'
			};

			mockService.scheduleRequest = jest.fn().mockResolvedValue(mockResult);

			await controller.scheduleRequest('VVN-003', 'tabu', 10, mockreq);

			expect(mockService.scheduleRequest).toHaveBeenCalledWith('VVN-003', 'tabu', 1, 'system');
		});
	});

	describe('getQueueState', () => {
		it('should return queue state with pending items', async () => {
			const mockQueueState = {
				pendingCount: 3,
				processingCount: 1,
				totalCount: 4,
				items: [
					{
						id: 'wqi-1',
						operationPlanVVN: 'VVN-001',
						algorithmType: 'greedy',
						daysAhead: 1,
						dateToProcess: new Date(),
						result: 'Pending'
					}
				]
			};

			mockService.getQueueState = jest.fn().mockResolvedValue(mockQueueState);

			const result = await controller.getQueueState();

			expect(mockService.getQueueState).toHaveBeenCalled();
			expect(result).toEqual(mockQueueState);
			expect(result.pendingCount).toBe(3);
		});

		it('should return empty queue state', async () => {
			const emptyQueueState = {
				pendingCount: 0,
				processingCount: 0,
				totalCount: 0,
				items: []
			};

			mockService.getQueueState = jest.fn().mockResolvedValue(emptyQueueState);

			const result = await controller.getQueueState();

			expect(result.totalCount).toBe(0);
			expect(result.items).toHaveLength(0);
		});
	});

	describe('acceptRequest', () => {
		it('should accept a scheduling request with id parameter', async () => {
			const mockResult: WorkQueueItemDto = {
				id: 'wqi-2',
				operationPlanVVN: 'VVN-002',
				algorithmType: 'genetic',
				daysAhead: 1,
				dateToProcess: new Date(),
				result: 'Processing'
			};

			mockService.acceptRequest = jest.fn().mockResolvedValue(mockResult);

			const result = await controller.acceptRequest('wqi-2', mockreq);

			expect(mockService.acceptRequest).toHaveBeenCalledWith('wqi-2', 'system', 'valid-token');
			expect(result).toEqual(mockResult);
		});

		it('should return error when id is missing', async () => {
			controller.setStatus = jest.fn();

			const result = await controller.acceptRequest('');

			expect(controller.setStatus).toHaveBeenCalledWith(400);
			expect(result).toEqual({ message: 'Missing required query parameter: id' });
		});

		it('should pass correct parameters to service', async () => {
			const mockResult: WorkQueueItemDto = {
				id: 'wqi-3',
				operationPlanVVN: 'VVN-003',
				algorithmType: 'tabu',
				daysAhead: 1,
				dateToProcess: new Date(),
				result: 'Processing'
			};

			mockService.acceptRequest = jest.fn().mockResolvedValue(mockResult);

			await controller.acceptRequest('wqi-3', mockreq);

			expect(mockService.acceptRequest).toHaveBeenCalledWith('wqi-3', 'system', 'valid-token');
		});
	});

	describe('rejectRequest', () => {
		it('should reject a scheduling request with id parameter', async () => {
			const mockResult: WorkQueueItemDto = {
				id: 'wqi-4',
				operationPlanVVN: 'VVN-004',
				algorithmType: 'greedy',
				daysAhead: 1,
				dateToProcess: new Date(),
				result: 'Rejected'
			};

			mockService.rejectRequest = jest.fn().mockResolvedValue(mockResult);

			const result = await controller.rejectRequest('wqi-4', mockreq);

			expect(mockService.rejectRequest).toHaveBeenCalledWith('wqi-4', 'system');
			expect(result).toEqual(mockResult);
		});

		it('should return error when id is missing', async () => {
			controller.setStatus = jest.fn();

			const result = await controller.rejectRequest('');

			expect(controller.setStatus).toHaveBeenCalledWith(400);
			expect(result).toEqual({ message: 'Missing required query parameter: id' });
		});

		it('should pass correct parameters to service', async () => {
			const mockResult: WorkQueueItemDto = {
				id: 'wqi-5',
				operationPlanVVN: 'VVN-005',
				algorithmType: 'simulated_annealing',
				daysAhead: 1,
				dateToProcess: new Date(),
				result: 'Rejected'
			};

			mockService.rejectRequest = jest.fn().mockResolvedValue(mockResult);

			await controller.rejectRequest('wqi-5' , mockreq);

			expect(mockService.rejectRequest).toHaveBeenCalledWith('wqi-5', 'system');
		});

		it('should handle rejection of active request', async () => {
			const mockResult: WorkQueueItemDto = {
				id: 'wqi-6',
				operationPlanVVN: 'VVN-006',
				algorithmType: 'genetic',
				daysAhead: 1,
				dateToProcess: new Date(),
				result: 'Rejected'
			};

			mockService.rejectRequest = jest.fn().mockResolvedValue(mockResult);

			const result = await controller.rejectRequest('wqi-6' , mockreq);

			expect(result.result).toBe('Rejected');
		});
	});
});
