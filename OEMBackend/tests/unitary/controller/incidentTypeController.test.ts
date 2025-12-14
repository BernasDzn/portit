import IncidentTypeController from '../../../src/controllers/incidentTypeController';
import { IncidentTypeService } from '../../../src/services/incidentTypeService';
import { Request, Response, NextFunction } from 'express';

let controller: IncidentTypeController;
let mockService: jest.Mocked<IncidentTypeService>;
let mockRequest: Partial<Request>;
let mockResponse: Partial<Response>;
let mockNext: jest.Mock;

beforeEach(() => {
	mockService = new IncidentTypeService() as jest.Mocked<IncidentTypeService>;
	controller = new IncidentTypeController(mockService);
	
	mockRequest = {
		body: {},
		params: {},
		query: {}
	};
	
	mockResponse = {
		status: jest.fn().mockReturnThis(),
		json: jest.fn().mockReturnThis()
	};
	
	mockNext = jest.fn();
});

describe('IncidentTypeController', () => {
	describe('createIncidentType', () => {
		it('should create a new incident type successfully', async () => {
			const mockIncidentType = {
				id: 'INC-TEST123',
				name: 'Test Incident'
			};

			mockRequest.body = { name: 'Test Incident' };
			mockService.createIncidentType = jest.fn().mockResolvedValue(mockIncidentType);

			await controller.createIncidentType(
				mockRequest as Request,
				mockResponse as Response,
				mockNext
			);

			expect(mockService.createIncidentType).toHaveBeenCalledWith('Test Incident', undefined, undefined);
			expect(mockResponse.status).toHaveBeenCalledWith(201);
			expect(mockResponse.json).toHaveBeenCalledWith(mockIncidentType);
		});

		it('should handle errors during creation', async () => {
			const error = new Error('Database error');
			mockRequest.body = { name: 'Test Type' };
			mockService.createIncidentType = jest.fn().mockRejectedValue(error);

			await controller.createIncidentType(
				mockRequest as Request,
				mockResponse as Response,
				mockNext
			);

			expect(mockNext).toHaveBeenCalledWith(error);
		});
	});

	describe('getIncidentTypeById', () => {
		it('should return incident type when found', async () => {
			const mockIncidentType = {
				id: 'INC-TEST123',
				name: 'Fire Incident'
			};

			mockRequest.params = { id: 'INC-TEST123' };
			mockService.getIncidentTypeById = jest.fn().mockResolvedValue(mockIncidentType);

			await controller.getIncidentTypeById(
				mockRequest as Request,
				mockResponse as Response,
				mockNext
			);

			expect(mockService.getIncidentTypeById).toHaveBeenCalledWith('INC-TEST123');
			expect(mockResponse.status).toHaveBeenCalledWith(200);
			expect(mockResponse.json).toHaveBeenCalledWith(mockIncidentType);
		});

		it('should return 404 when incident type not found', async () => {
			mockRequest.params = { id: 'INC-NOTFOUND' };
			mockService.getIncidentTypeById = jest.fn().mockResolvedValue(null);

			await controller.getIncidentTypeById(
				mockRequest as Request,
				mockResponse as Response,
				mockNext
			);

			expect(mockService.getIncidentTypeById).toHaveBeenCalledWith('INC-NOTFOUND');
			expect(mockResponse.status).toHaveBeenCalledWith(404);
		});

		it('should handle errors during retrieval', async () => {
			const error = new Error('Database error');
			mockRequest.params = { id: 'INC-TEST123' };
			mockService.getIncidentTypeById = jest.fn().mockRejectedValue(error);

			await controller.getIncidentTypeById(
				mockRequest as Request,
				mockResponse as Response,
				mockNext
			);

			expect(mockNext).toHaveBeenCalledWith(error);
		});
	});

	describe('getAllIncidentTypes', () => {
		it('should return all incident types', async () => {
			const mockIncidentTypes = [
				{ id: 'INC-TYPE1', name: 'Type 1' },
				{ id: 'INC-TYPE2', name: 'Type 2' },
				{ id: 'INC-TYPE3', name: 'Type 3' }
			];

			mockService.getAllIncidentTypes = jest.fn().mockResolvedValue(mockIncidentTypes);

			await controller.getAllIncidentTypes(
				mockRequest as Request,
				mockResponse as Response,
				mockNext
			);

			expect(mockService.getAllIncidentTypes).toHaveBeenCalled();
			expect(mockResponse.status).toHaveBeenCalledWith(200);
			expect(mockResponse.json).toHaveBeenCalledWith(mockIncidentTypes);
		});

		it('should return empty array when no incident types exist', async () => {
			mockService.getAllIncidentTypes = jest.fn().mockResolvedValue([]);

			await controller.getAllIncidentTypes(
				mockRequest as Request,
				mockResponse as Response,
				mockNext
			);

			expect(mockResponse.status).toHaveBeenCalledWith(200);
			expect(mockResponse.json).toHaveBeenCalledWith([]);
		});

		it('should handle errors during retrieval', async () => {
			const error = new Error('Database error');
			mockService.getAllIncidentTypes = jest.fn().mockRejectedValue(error);

			await controller.getAllIncidentTypes(
				mockRequest as Request,
				mockResponse as Response,
				mockNext
			);

			expect(mockNext).toHaveBeenCalledWith(error);
		});
	});

	describe('updateIncidentType', () => {
		it('should update incident type name', async () => {
			const mockUpdatedType = {
				id: 'INC-TEST123',
				name: 'Updated Name'
			};

			mockRequest.params = { id: 'INC-TEST123' };
			mockRequest.body = { name: 'Updated Name' };
			mockService.updateIncidentType = jest.fn().mockResolvedValue(mockUpdatedType);

			await controller.updateIncidentType(
				mockRequest as Request,
				mockResponse as Response,
				mockNext
			);

			expect(mockService.updateIncidentType).toHaveBeenCalledWith('INC-TEST123', 'Updated Name', undefined);
			expect(mockResponse.status).toHaveBeenCalledWith(200);
			expect(mockResponse.json).toHaveBeenCalledWith(mockUpdatedType);
		});

		it('should add children to incident type', async () => {
			const mockUpdatedType = {
				id: 'INC-PARENT',
				name: 'Parent',
				subtypesIds: ['INC-CHILD1', 'INC-CHILD2']
			};

			mockRequest.params = { id: 'INC-PARENT' };
			mockRequest.body = { subtypesIds: ['INC-CHILD1', 'INC-CHILD2'] };
			mockService.updateIncidentType = jest.fn().mockResolvedValue(mockUpdatedType);

			await controller.updateIncidentType(
				mockRequest as Request,
				mockResponse as Response,
				mockNext
			);

			expect(mockService.updateIncidentType).toHaveBeenCalledWith(
				'INC-PARENT', 
				undefined, 
				['INC-CHILD1', 'INC-CHILD2']
			);
			expect(mockResponse.status).toHaveBeenCalledWith(200);
		});

		it('should update both name and children', async () => {
			const mockUpdatedType = {
				id: 'INC-TEST',
				name: 'New Name',
				subtypesIds: ['INC-CHILD']
			};

			mockRequest.params = { id: 'INC-TEST' };
			mockRequest.body = { name: 'New Name', subtypesIds: ['INC-CHILD'] };
			mockService.updateIncidentType = jest.fn().mockResolvedValue(mockUpdatedType);

			await controller.updateIncidentType(
				mockRequest as Request,
				mockResponse as Response,
				mockNext
			);

			expect(mockService.updateIncidentType).toHaveBeenCalledWith('INC-TEST', 'New Name', ['INC-CHILD']);
			expect(mockResponse.status).toHaveBeenCalledWith(200);
		});

		it('should return 404 when incident type not found', async () => {
			mockRequest.params = { id: 'INC-NOTFOUND' };
			mockRequest.body = { name: 'New Name' };
			mockService.updateIncidentType = jest.fn().mockResolvedValue(null);

			await controller.updateIncidentType(
				mockRequest as Request,
				mockResponse as Response,
				mockNext
			);

			expect(mockResponse.status).toHaveBeenCalledWith(404);
		});

		it('should handle errors during update', async () => {
			const error = new Error('Update error');
			mockRequest.params = { id: 'INC-TEST' };
			mockRequest.body = { name: 'New Name' };
			mockService.updateIncidentType = jest.fn().mockRejectedValue(error);

			await controller.updateIncidentType(
				mockRequest as Request,
				mockResponse as Response,
				mockNext
			);

			expect(mockNext).toHaveBeenCalledWith(error);
		});
	});

	describe('removeChild', () => {
		it('should remove child from incident type', async () => {
			const mockUpdatedType = {
				id: 'INC-PARENT',
				name: 'Parent',
				subtypesIds: ['INC-CHILD2']
			};

			mockRequest.params = { id: 'INC-PARENT', subtypeId: 'INC-CHILD1' };
			mockService.removeSubtype = jest.fn().mockResolvedValue(mockUpdatedType);

			await controller.removeSubtype(
				mockRequest as Request,
				mockResponse as Response,
				mockNext
			);

			expect(mockService.removeSubtype).toHaveBeenCalledWith('INC-PARENT', 'INC-CHILD1');
			expect(mockResponse.status).toHaveBeenCalledWith(200);
			expect(mockResponse.json).toHaveBeenCalledWith(mockUpdatedType);
		});

		it('should return 404 when incident type not found', async () => {
			mockRequest.params = { id: 'INC-NOTFOUND', childId: 'INC-CHILD' };
			mockService.removeSubtype = jest.fn().mockResolvedValue(null);

			await controller.removeSubtype(
				mockRequest as Request,
				mockResponse as Response,
				mockNext
			);

			expect(mockResponse.status).toHaveBeenCalledWith(404);
		});

		it('should handle errors during child removal', async () => {
			const error = new Error('Removal error');
			mockRequest.params = { id: 'INC-PARENT', childId: 'INC-CHILD' };
			mockService.removeSubtype = jest.fn().mockRejectedValue(error);

			await controller.removeSubtype(
				mockRequest as Request,
				mockResponse as Response,
				mockNext
			);

			expect(mockNext).toHaveBeenCalledWith(error);
		});
	});

	describe('deleteIncidentType', () => {
		it('should delete incident type successfully', async () => {
			mockRequest.params = { id: 'INC-TEST123' };
			mockService.deleteIncidentType = jest.fn().mockResolvedValue(true);

			await controller.deleteIncidentType(
				mockRequest as Request,
				mockResponse as Response,
				mockNext
			);

			expect(mockService.deleteIncidentType).toHaveBeenCalledWith('INC-TEST123');
			expect(mockResponse.status).toHaveBeenCalledWith(200);
			expect(mockResponse.json).toHaveBeenCalledWith('Incident type deleted successfully');
		});

		it('should return 404 when incident type not found', async () => {
			mockRequest.params = { id: 'INC-NOTFOUND' };
			mockService.deleteIncidentType = jest.fn().mockResolvedValue(false);

			await controller.deleteIncidentType(
				mockRequest as Request,
				mockResponse as Response,
				mockNext
			);

			expect(mockService.deleteIncidentType).toHaveBeenCalledWith('INC-NOTFOUND');
			expect(mockResponse.status).toHaveBeenCalledWith(404);
		});

		it('should handle errors during deletion', async () => {
			const error = new Error('Deletion error');
			mockRequest.params = { id: 'INC-TEST123' };
			mockService.deleteIncidentType = jest.fn().mockRejectedValue(error);

			await controller.deleteIncidentType(
				mockRequest as Request,
				mockResponse as Response,
				mockNext
			);

			expect(mockNext).toHaveBeenCalledWith(error);
		});
	});
});
