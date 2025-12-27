import { PartialIncidentTypeDto, IncidentTypeDto } from "../../../src/dto/incidentTypeDto";
import { IncidentTypeRepository } from "../../../src/repository/incidentTypeRepository";
import { IncidentTypeService } from "../../../src/services/incidentTypeService";
import IncidentType from "../../../src/domain/incidentType";
import { Pageable, Page } from "../../../src/utils/page";
import { NotFoundError } from "../../../src/core/infra/extraErrors";

describe('IncidentTypeService', () => {
	let service: IncidentTypeService;
	let mockRepo: jest.Mocked<IncidentTypeRepository>;

	beforeEach(() => {
		jest.clearAllMocks();
		
		// Create service
		service = new IncidentTypeService();
		
		// Mock the repository
		mockRepo = {
			count: jest.fn(),
			getPaged: jest.fn(),
			getById: jest.fn(),
			save: jest.fn(),
			update: jest.fn(),
			removeSubtype: jest.fn(),
			deleteById: jest.fn(),
		} as unknown as jest.Mocked<IncidentTypeRepository>;
		
		// Replace the repository in the service with our mock
		(service as any).incidentTypeRepository = mockRepo;
	});

	describe('count', () => {
		it('should return the count of incident types', async () => {
			mockRepo.count.mockResolvedValue(5);

			const result = await service.count();

			expect(mockRepo.count).toHaveBeenCalled();
			expect(result).toBe(5);
		});
	});

	describe('getPaged', () => {
		it('should return paginated incident types', async () => {
			const pageable: Pageable = { pageNumber: 1, pageSize: 10 };
			const mockIncidentType = new IncidentType({
				name: 'Type 1',
				description: 'Description 1',
				severity: 'Minor'
			});

			const mockPage: Page<IncidentType> = {
				pageNumber: 1,
				pageSize: 10,
				pageCount: 1,
				items: [mockIncidentType]
			};

			mockRepo.getPaged.mockResolvedValue(mockPage);

			const result = await service.getPaged(pageable);

			expect(mockRepo.getPaged).toHaveBeenCalledWith(pageable);
			expect(result).toBeDefined();
			expect(result.items).toBeDefined();
			expect(result.items.length).toBe(1);
		});
	});

	describe('getById', () => {
		it('should return incident type when found', async () => {
			const incidentType = new IncidentType({
				name: 'Fire Incident',
				description: 'A fire',
				severity: 'Critical'
			});

			mockRepo.getById.mockResolvedValue(incidentType);

			const result = await service.getById('INC-TEST123');

			expect(mockRepo.getById).toHaveBeenCalledWith('INC-TEST123');
			expect(result).toBeDefined();
			expect(result.name).toBe('Fire Incident');
		});

		it('should throw NotFoundError when incident type not found', async () => {
			mockRepo.getById.mockRejectedValue(new NotFoundError('Not found'));

			await expect(service.getById('INC-NOTFOUND')).rejects.toThrow();
		});
	});

	describe('create', () => {
		it('should create a new incident type', async () => {
			const body: PartialIncidentTypeDto = {
				name: 'Fire Incident',
				description: 'A fire',
				severity: 'Critical'
			};

			const created = new IncidentType({
				name: 'Fire Incident',
				description: 'A fire',
				severity: 'Critical'
			});

			mockRepo.save.mockResolvedValue(created);

			const result = await service.create(body);

			expect(mockRepo.save).toHaveBeenCalled();
			expect(result).toBeDefined();
			expect(result.name).toBe('Fire Incident');
		});

		it('should throw error when parent not found', async () => {
			const body: PartialIncidentTypeDto = {
				name: 'Child Incident',
				description: 'Child',
				severity: 'Major',
				subtypeOf: 'INC-PARENT'
			};

			mockRepo.getById.mockRejectedValue(new Error('Parent not found'));

			await expect(service.create(body)).rejects.toThrow();
		});
	});

	describe('update', () => {
		it('should update incident type successfully', async () => {
			const existing = new IncidentType({
				name: 'Old Name',
				description: 'Old description',
				severity: 'Minor'
			});

			const updated = new IncidentType({
				name: 'New Name',
				description: 'New description',
				severity: 'Major'
			});

			mockRepo.getById.mockResolvedValue(existing);
			mockRepo.update.mockResolvedValue(updated);

			const result = await service.update('INC-TEST', {
				name: 'New Name',
				description: 'New description',
				severity: 'Major'
			});

			expect(mockRepo.getById).toHaveBeenCalledWith('INC-TEST');
			expect(mockRepo.update).toHaveBeenCalled();
			expect(result).toBeDefined();
			expect(result.name).toBe('New Name');
		});

		it('should throw NotFoundError when incident type does not exist', async () => {
			mockRepo.getById.mockRejectedValue(new NotFoundError('Not found'));

			await expect(service.update('INC-NOTFOUND', {
				name: 'New Name'
			})).rejects.toThrow();
		});
	});
});
