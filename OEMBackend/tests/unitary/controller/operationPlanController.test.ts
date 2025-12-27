import { OperationPlanController } from '../../../src/controllers/operationPlanController';
import { OperationPlanService } from '../../../src/services/operationPlanService';

// Mock the OperationPlanService
jest.mock('../../../src/services/operationPlanService');

describe('OperationPlanController', () => {
	let controller: OperationPlanController;
	let mockService: jest.Mocked<OperationPlanService>;

	beforeEach(() => {
		jest.clearAllMocks();
		mockService = new OperationPlanService() as jest.Mocked<OperationPlanService>;
		controller = new OperationPlanController();
		(controller as any).operationPlanService = mockService;
	});

	describe('getPlans', () => {
		it('should return paginated operation plans with default pagination', async () => {
			const mockPlans = {
				pageNumber: 1,
				pageSize: 10,
				pageCount: 1,
				items: [
					{ id: '1', relatedVVN: 'VVN001', dock: 'Dock1' },
					{ id: '2', relatedVVN: 'VVN002', dock: 'Dock2' }
				]
			};

			mockService.getAll = jest.fn().mockResolvedValue(mockPlans);

			const result = await controller.getPlans();

			expect(mockService.getAll).toHaveBeenCalledWith({
				pageNumber: 1,
				pageSize: 10,
				startDate: undefined,
				endDate: undefined
			});
			expect(result).toEqual(mockPlans);
		});

		it('should return paginated operation plans with custom pagination', async () => {
			const mockPlans = {
				pageNumber: 2,
				pageSize: 5,
				pageCount: 3,
				items: [
					{ id: '3', relatedVVN: 'VVN003', dock: 'Dock3' }
				]
			};

			mockService.getAll = jest.fn().mockResolvedValue(mockPlans);

			const result = await controller.getPlans(2, 5);

			expect(mockService.getAll).toHaveBeenCalledWith({
				pageNumber: 2,
				pageSize: 5,
				startDate: undefined,
				endDate: undefined
			});
			expect(result).toEqual(mockPlans);
		});

		it('should return operation plans with date filters', async () => {
			const mockPlans = {
				pageNumber: 1,
				pageSize: 10,
				pageCount: 1,
				items: []
			};

			mockService.getAll = jest.fn().mockResolvedValue(mockPlans);

			const result = await controller.getPlans(1, 10, '2024-10-01', '2024-10-31');

			expect(mockService.getAll).toHaveBeenCalledWith({
				pageNumber: 1,
				pageSize: 10,
				startDate: '2024-10-01',
				endDate: '2024-10-31'
			});
			expect(result).toEqual(mockPlans);
		});
	});

	describe('getPlanById', () => {
		it('should return operation plan when found', async () => {
			const mockPlan = {
				id: '1',
				relatedVVN: 'VVN001',
				dock: 'Dock1'
			};

			mockService.getById = jest.fn().mockResolvedValue(mockPlan);

			const result = await controller.getPlanById('1');

			expect(mockService.getById).toHaveBeenCalledWith('1');
			expect(result).toEqual(mockPlan);
		});

		it('should return 404 when operation plan not found', async () => {
			mockService.getById = jest.fn().mockResolvedValue(null);

			const result = await controller.getPlanById('999');

			expect(mockService.getById).toHaveBeenCalledWith('999');
			expect(result).toEqual({ message: 'Operation plan not found' });
		});
	});

	describe('getPlansByDate', () => {
		it('should return operation plans grouped by date', async () => {
			const mockGroupedPlans = [
				{
					date: '2024-10-01',
					plans: [
						{ id: '1', relatedVVN: 'VVN001', dock: 'Dock1' }
					]
				}
			];

			mockService.getByDateGrouped = jest.fn().mockResolvedValue(mockGroupedPlans);

			const result = await controller.getPlansByDate();

			expect(mockService.getByDateGrouped).toHaveBeenCalled();
			expect(result).toEqual(mockGroupedPlans);
		});
	});
});