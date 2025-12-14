import OperationPlanController from '../../../src/controllers/operationPlanController';
import { OperationPlanService } from '../../../src/services/operationPlanService';
import { Request, Response, NextFunction } from 'express';

let controller: OperationPlanController;
let mockService: jest.Mocked<OperationPlanService>;
let mockRequest: Partial<Request>;
let mockResponse: Partial<Response>;
let mockNext: jest.Mock;

beforeEach(() => {
	mockService = new OperationPlanService() as jest.Mocked<OperationPlanService>;
	controller = new OperationPlanController(mockService);
	
	mockRequest = {
		query: {}
	};
	
	mockResponse = {
		status: jest.fn().mockReturnThis(),
		json: jest.fn().mockReturnThis()
	};
	
	mockNext = jest.fn();
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

		await controller.getPlans(
			mockRequest as Request,
			mockResponse as Response,
			mockNext
		);

		expect(mockService.getAll).toHaveBeenCalledWith({ pageNumber: 1, pageSize: 10 });
		expect(mockResponse.status).toHaveBeenCalledWith(200);
		expect(mockResponse.json).toHaveBeenCalledWith(mockPlans);
	});

	it('should return paginated operation plans with custom pagination', async () => {
		mockRequest.query = { pageNumber: '2', pageSize: '5' };
		
		const mockPlans = {
			pageNumber: 2,
			pageSize: 5,
			pageCount: 3,
			items: [
				{ id: '3', relatedVVN: 'VVN003', dock: 'Dock3' }
			]
		};

		mockService.getAll = jest.fn().mockResolvedValue(mockPlans);

		await controller.getPlans(
			mockRequest as Request,
			mockResponse as Response,
			mockNext
		);

		expect(mockService.getAll).toHaveBeenCalledWith({ pageNumber: 2, pageSize: 5 });
		expect(mockResponse.status).toHaveBeenCalledWith(200);
		expect(mockResponse.json).toHaveBeenCalledWith(mockPlans);
	});

	it('should return 500 when getAll fails due to an internal error', async () => {
		const error = new Error('Database connection failed');
		mockService.getAll = jest.fn().mockRejectedValue(error);

		await controller.getPlans(
			mockRequest as Request,
			mockResponse as Response,
			mockNext
		);

		expect(mockService.getAll).toHaveBeenCalledWith({ pageNumber: 1, pageSize: 10 });
		expect(mockResponse.status).toHaveBeenCalledWith(500);
		expect(mockResponse.json).toHaveBeenCalledWith({ message: 'Error retrieving operation plans' });
		expect(mockNext).toHaveBeenCalledWith(error);
	});

	it('should handle invalid pagination parameters gracefully', async () => {
		mockRequest.query = { pageNumber: 'invalid', pageSize: 'invalid' };
		
		const mockPlans = {
			pageNumber: 1,
			pageSize: 10,
			pageCount: 1,
			items: []
		};

		mockService.getAll = jest.fn().mockResolvedValue(mockPlans);

		await controller.getPlans(
			mockRequest as Request,
			mockResponse as Response,
			mockNext
		);

		expect(mockService.getAll).toHaveBeenCalledWith({ pageNumber: 1, pageSize: 10 });
		expect(mockResponse.status).toHaveBeenCalledWith(200);
	});
});

describe('getPlanById', () => {
	it('should return operation plan when found', async () => {
		const mockPlan = { id: '1', relatedVVN: 'VVN001', dock: 'Dock1' };
		mockRequest.params = { id: '1' };

		mockService.getById = jest.fn().mockResolvedValue(mockPlan);

		await controller.getPlanById(
			mockRequest as Request,
			mockResponse as Response,
			mockNext
		);

		expect(mockService.getById).toHaveBeenCalledWith('1');
		expect(mockResponse.status).toHaveBeenCalledWith(200);
		expect(mockResponse.json).toHaveBeenCalledWith(mockPlan);
	});

	it('should return 404 when operation plan not found', async () => {
		mockRequest.params = { id: '999' };

		mockService.getById = jest.fn().mockResolvedValue(null);

		await controller.getPlanById(
			mockRequest as Request,
			mockResponse as Response,
			mockNext
		);

		expect(mockService.getById).toHaveBeenCalledWith('999');
		expect(mockResponse.status).toHaveBeenCalledWith(404);
		expect(mockResponse.json).toHaveBeenCalledWith('Operation plan not found');
	});

	it('should return 500 when getById fails due to an internal error', async () => {
		const error = new Error('Database error');
		mockRequest.params = { id: '1' };

		mockService.getById = jest.fn().mockRejectedValue(error);

		await controller.getPlanById(
			mockRequest as Request,
			mockResponse as Response,
			mockNext
		);

		expect(mockService.getById).toHaveBeenCalledWith('1');
		expect(mockResponse.status).toHaveBeenCalledWith(500);
		expect(mockResponse.json).toHaveBeenCalledWith({ message: 'Error retrieving operation plan by ID' });
		expect(mockNext).toHaveBeenCalledWith(error);
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
			},
			{
				date: '2024-10-02',
				plans: [
					{ id: '2', relatedVVN: 'VVN002', dock: 'Dock2' }
				]
			}
		];

		mockService.getByDateGrouped = jest.fn().mockResolvedValue(mockGroupedPlans);

		await controller.getPlansByDate(
			mockRequest as Request,
			mockResponse as Response,
			mockNext
		);

		expect(mockService.getByDateGrouped).toHaveBeenCalled();
		expect(mockResponse.status).toHaveBeenCalledWith(200);
		expect(mockResponse.json).toHaveBeenCalledWith(mockGroupedPlans);
	});

	it('should return 500 when getByDateGrouped fails due to an internal error', async () => {
		const error = new Error('Database error');

		mockService.getByDateGrouped = jest.fn().mockRejectedValue(error);

		await controller.getPlansByDate(
			mockRequest as Request,
			mockResponse as Response,
			mockNext
		);

		expect(mockService.getByDateGrouped).toHaveBeenCalled();
		expect(mockResponse.status).toHaveBeenCalledWith(500);
		expect(mockNext).toHaveBeenCalledWith(error);
	});
});

describe('getNotificationWithoutPlan', () => {
	it('should return list of VVN IDs without operation plans', async () => {
		const mockVVNIds = ['VVN003', 'VVN004'];

		mockService.getNotificationsWithoutPlan = jest.fn().mockResolvedValue(mockVVNIds);

		await controller.getNotificationWithoutPlan(
			mockRequest as Request,
			mockResponse as Response,
			mockNext
		);

		expect(mockService.getNotificationsWithoutPlan).toHaveBeenCalled();
		expect(mockResponse.status).toHaveBeenCalledWith(200);
		expect(mockResponse.json).toHaveBeenCalledWith(mockVVNIds);
	});

	it('should return 500 when getNotificationsWithoutPlan fails due to an internal error', async () => {
		const error = new Error('External service error');

		mockService.getNotificationsWithoutPlan = jest.fn().mockRejectedValue(error);

		await controller.getNotificationWithoutPlan(
			mockRequest as Request,
			mockResponse as Response,
			mockNext
		);

		expect(mockService.getNotificationsWithoutPlan).toHaveBeenCalled();
		expect(mockResponse.status).toHaveBeenCalledWith(500);
		expect(mockResponse.json).toHaveBeenCalledWith({ message: 'Error retrieving notifications without plan' });
		expect(mockNext).toHaveBeenCalledWith(error);
	});
});