import { SchedulingRequestService } from "../../../src/services/schedulingRequestService";
import { workQueueRepository } from "../../../src/repository/workQueueRepository";
import { OperationPlanService } from "../../../src/services/operationPlanService";

jest.mock('../../../src/repository/workQueueRepository');
jest.mock('../../../src/services/operationPlanService');

describe('SchedulingRequestService', () => {
	let service: SchedulingRequestService;

	beforeEach(() => {
		jest.clearAllMocks();
		service = new SchedulingRequestService();
	});

	describe('scheduleRequest', () => {
		it('should queue a scheduling request', async () => {
			(workQueueRepository.enqueueRequest as jest.Mock).mockResolvedValue(0);

			const result = await service.scheduleRequest('2024-12-27', 'greedy', 5, 'user-123');

			expect(workQueueRepository.enqueueRequest).toHaveBeenCalledWith(
				'2024-12-27',
				'greedy',
				5,
				0,
				'user-123'
			);
			expect(result.message).toContain('Your scheduling request has been queued');
			expect(result.message).toContain('number 1');
		});

		it('should return correct queue position', async () => {
			(workQueueRepository.enqueueRequest as jest.Mock).mockResolvedValue(3);

			const result = await service.scheduleRequest('2024-12-28', 'genetic', 3, 'user-456');

			expect(result.message).toContain('number 4');
		});

		it('should handle default daysAhead parameter', async () => {
			(workQueueRepository.enqueueRequest as jest.Mock).mockResolvedValue(0);

			await service.scheduleRequest('2024-12-29', 'tabu', undefined, 'user-789');

			expect(workQueueRepository.enqueueRequest).toHaveBeenCalledWith(
				'2024-12-29',
				'tabu',
				2,
				0,
				'user-789'
			);
		});
	});

	describe('getQueueState', () => {
		it('should return current queue state', async () => {
			const mockQueueItems = [
				{
					id: 'req-1',
					status: 'pending',
					priority: 1,
					issuer: 'user-1'
				},
				{
					id: 'req-2',
					status: 'processing',
					priority: 2,
					issuer: 'user-2'
				}
			];

			(workQueueRepository.getQueueState as jest.Mock).mockResolvedValue(mockQueueItems);

			const result = await service.getQueueState();

			expect(workQueueRepository.getQueueState).toHaveBeenCalled();
			expect(result).toHaveLength(2);
			expect(result[0].id).toBe('req-1');
		});

		it('should handle empty queue', async () => {
			(workQueueRepository.getQueueState as jest.Mock).mockResolvedValue([]);

			const result = await service.getQueueState();

			expect(result).toEqual([]);
		});
	});

	describe('acceptRequest', () => {
		it('should accept a completed scheduling request', async () => {
			const mockItem = {
				id: 'req-1',
				status: 'completed',
				issuer: 'user-123',
				result: { scheduled: true }
			};

			(workQueueRepository.getById as jest.Mock).mockResolvedValue(mockItem);
			(OperationPlanService as jest.Mock).mockImplementation(() => ({
				createPlans: jest.fn().mockResolvedValue([{ id: 'plan-1' }])
			}));
			(workQueueRepository.finishRequest as jest.Mock).mockResolvedValue(undefined);

			const result = await service.acceptRequest('req-1', 'user-123', 'token-123');

			expect(workQueueRepository.getById).toHaveBeenCalledWith('req-1');
			expect(workQueueRepository.finishRequest).toHaveBeenCalledWith('req-1', 'accepted');
			expect(result).toBeDefined();
			expect(result?.id).toBe('req-1');
		});

		it('should throw error when request not found', async () => {
			(workQueueRepository.getById as jest.Mock).mockResolvedValue(null);

			await expect(service.acceptRequest('req-notfound', 'user-123', 'token')).rejects.toThrow(
				'No scheduling request found with ID req-notfound'
			);
		});

		it('should throw error when request not completed', async () => {
			const mockItem = {
				id: 'req-2',
				status: 'pending',
				issuer: 'user-123'
			};

			(workQueueRepository.getById as jest.Mock).mockResolvedValue(mockItem);

			await expect(service.acceptRequest('req-2', 'user-123', 'token')).rejects.toThrow(
				'Scheduling is not completed. Current status: pending'
			);
		});

		it('should throw error when request issued by different user', async () => {
			const mockItem = {
				id: 'req-3',
				status: 'completed',
				issuer: 'user-456'
			};

			(workQueueRepository.getById as jest.Mock).mockResolvedValue(mockItem);

			await expect(service.acceptRequest('req-3', 'user-123', 'token')).rejects.toThrow(
				'Scheduling request was not issued by you'
			);
		});

		it('should throw error when no schedule data found', async () => {
			const mockItem = {
				id: 'req-4',
				status: 'completed',
				issuer: 'user-123',
				result: null
			};

			(workQueueRepository.getById as jest.Mock).mockResolvedValue(mockItem);

			await expect(service.acceptRequest('req-4', 'user-123', 'token')).rejects.toThrow(
				'No schedule data found for request ID req-4'
			);
		});

		it('should throw error when plan creation fails', async () => {
			const mockItem = {
				id: 'req-5',
				status: 'completed',
				issuer: 'user-123',
				result: { scheduled: true }
			};

			(workQueueRepository.getById as jest.Mock).mockResolvedValue(mockItem);
			(OperationPlanService as jest.Mock).mockImplementation(() => ({
				createPlans: jest.fn().mockResolvedValue(null)
			}));

			await expect(service.acceptRequest('req-5', 'user-123', 'token')).rejects.toThrow(
				'Failed to save operation plans for request ID req-5'
			);
		});
	});

	describe('rejectRequest', () => {
		it('should reject a completed scheduling request', async () => {
			const mockItem = {
				id: 'req-6',
				status: 'completed',
				issuer: 'user-123'
			};

			(workQueueRepository.getById as jest.Mock).mockResolvedValue(mockItem);
			(workQueueRepository.finishRequest as jest.Mock).mockResolvedValue(undefined);

			const result = await service.rejectRequest('req-6', 'user-123');

			expect(workQueueRepository.getById).toHaveBeenCalledWith('req-6');
			expect(workQueueRepository.finishRequest).toHaveBeenCalledWith('req-6', 'rejected');
			expect(result).toBeDefined();
			expect(result?.id).toBe('req-6');
		});

		it('should throw error when request not found', async () => {
			(workQueueRepository.getById as jest.Mock).mockResolvedValue(null);

			await expect(service.rejectRequest('req-notfound', 'user-123')).rejects.toThrow(
				'No scheduling request found with ID req-notfound'
			);
		});

		it('should throw error when request not completed', async () => {
			const mockItem = {
				id: 'req-7',
				status: 'processing',
				issuer: 'user-123'
			};

			(workQueueRepository.getById as jest.Mock).mockResolvedValue(mockItem);

			await expect(service.rejectRequest('req-7', 'user-123')).rejects.toThrow(
				'Scheduling request ID req-7 is not yet completed. Current status: processing'
			);
		});

		it('should throw error when request issued by different user', async () => {
			const mockItem = {
				id: 'req-8',
				status: 'completed',
				issuer: 'user-456'
			};

			(workQueueRepository.getById as jest.Mock).mockResolvedValue(mockItem);

			await expect(service.rejectRequest('req-8', 'user-123')).rejects.toThrow(
				'Scheduling request ID req-8 was not issued by you'
			);
		});
	});
});
