import { IncidentTypeController } from '../../../src/controllers/incidentTypeController';
import { IncidentTypeService } from '../../../src/services/incidentTypeService';
import { PartialIncidentTypeDto, IncidentTypeDto } from '../../../src/dto/incidentTypeDto';
import { NotFoundError } from '../../../src/core/infra/extraErrors';

// Mock the IncidentTypeService
jest.mock('../../../src/services/incidentTypeService');

describe('IncidentTypeController', () => {
	let controller: IncidentTypeController;
	let mockService: jest.Mocked<IncidentTypeService>;

	beforeEach(() => {
		// Clear all mocks before each test
		jest.clearAllMocks();
		
		// Get the mocked service
		mockService = new IncidentTypeService() as jest.Mocked<IncidentTypeService>;
		
		// Create controller instance
		controller = new IncidentTypeController();
		
		// Replace the service in the controller with our mock
		(controller as any).incidentTypeService = mockService;
	});

	describe('count', () => {
		it('should return the count of incident types', async () => {
			mockService.count = jest.fn().mockResolvedValue(5);

			const result = await controller.count();

			expect(mockService.count).toHaveBeenCalled();
			expect(result).toEqual({ count: 5 });
		});
	});

	describe('getPaged', () => {
		it('should return paginated incident types', async () => {
			const mockPage = {
				pageNumber: 1,
				pageSize: 10,
				pageCount: 1,
				items: [
					{ id: 'INC-TYPE1', name: 'Type 1' },
					{ id: 'INC-TYPE2', name: 'Type 2' }
				]
			};

			mockService.getPaged = jest.fn().mockResolvedValue(mockPage);

			const result = await controller.getPaged(1, 10);

			const callArg = (mockService.getPaged).mock.calls[0][0];
			expect(callArg.pageNumber).toBe(1);
			expect(callArg.pageSize).toBe(10);
			expect(result).toEqual(mockPage);
		});

		it('should use default pagination if not provided', async () => {
			const mockPage = {
				pageNumber: 1,
				pageSize: 10,
				pageCount: 1,
				items: []
			};

			mockService.getPaged = jest.fn().mockResolvedValue(mockPage);

			await controller.getPaged();

			expect(mockService.getPaged).toHaveBeenCalledWith({ pageNumber: 1, pageSize: 10 });
		});
	});

	describe('getById', () => {
		it('should return incident type when found', async () => {
			const mockIncidentType: IncidentTypeDto = {
				id: 'INC-TEST123',
				name: 'Fire Incident',
				description: 'A fire',
				severity: 'CRITICAL'
			};

			mockService.getById = jest.fn().mockResolvedValue(mockIncidentType);

			const result = await controller.getById('INC-TEST123');

			expect(mockService.getById).toHaveBeenCalledWith('INC-TEST123');
			expect(result).toEqual(mockIncidentType);
		});

		it('should handle not found error gracefully', async () => {
			const error = new NotFoundError('IncidentType with bid INC-NOTFOUND not found.');
			mockService.getById = jest.fn().mockRejectedValue(error);

			const result = await controller.getById('INC-NOTFOUND');

			expect(result).toEqual({ message: error.message });
		});

		it('should return 500 for other errors', async () => {
			const error = new Error('Database error');
			mockService.getById = jest.fn().mockRejectedValue(error);

			const result = await controller.getById('INC-TEST');

			expect(result).toEqual({ message: 'Internal server error' });
		});
	});

	describe('createIncidentType', () => {
		it('should create a new incident type successfully', async () => {
			const body: PartialIncidentTypeDto = {
				name: 'Test Incident',
				description: 'A test incident',
				severity: 'MINOR'
			};

			const mockCreated: IncidentTypeDto = {
				id: 'INC-TEST123',
				name: 'Test Incident',
				description: 'A test incident',
				severity: 'MINOR'
			};

			mockService.create = jest.fn().mockResolvedValue(mockCreated);

			const result = await controller.createIncidentType(body);

			expect(mockService.create).toHaveBeenCalledWith(body);
			expect(result).toEqual(mockCreated);
		});

		it('should handle errors during creation', async () => {
			const body: PartialIncidentTypeDto = {
				name: 'Test Type',
				description: '',
				severity: 'MINOR'
			};

			const error = new Error('Database error');
			mockService.create = jest.fn().mockRejectedValue(error);

			await expect(controller.createIncidentType(body)).rejects.toThrow('Database error');
		});
	});

	describe('updateIncidentType', () => {
		it('should update incident type successfully', async () => {
			const body: PartialIncidentTypeDto = {
				name: 'Updated Name',
				description: 'Updated description',
				severity: 'MAJOR'
			};

			const mockUpdated: IncidentTypeDto = {
				id: 'INC-TEST123',
				...body
			};

			mockService.update = jest.fn().mockResolvedValue(mockUpdated);

			const result = await controller.updateIncidentType('INC-TEST123', body);

			expect(mockService.update).toHaveBeenCalledWith('INC-TEST123', body);
			expect(result).toEqual(mockUpdated);
		});

		it('should return not found when incident type does not exist', async () => {
			const body: PartialIncidentTypeDto = {
				name: 'Updated Name'
			};

			mockService.update = jest.fn().mockResolvedValue(null);

			const result = await controller.updateIncidentType('INC-NOTFOUND', body);

			expect(result).toEqual({ message: 'Incident type not found' });
		});
	});
});