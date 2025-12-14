import { IncidentTypeDto } from "../../../src/dto/incidentTypeDto";
import { IncidentTypeRepository } from "../../../src/repository/incidentTypeRepository";
import { IncidentTypeService } from "../../../src/services/incidentTypeService";
import IncidentType from "../../../src/domain/incidentType";

let service: IncidentTypeService;
let mockRepo: jest.Mocked<IncidentTypeRepository>;
let mockIncidentType: IncidentType;

beforeEach(() => {
	mockIncidentType = new IncidentType(
		{ name: "Fire Incident" },
		"INC-TEST123"
	);

	// Create mock repository
	mockRepo = {
		create: jest.fn(),
		getById: jest.fn(),
		getAll: jest.fn(),
		update: jest.fn(),
		removeChild: jest.fn(),
		deleteById: jest.fn()
	} as jest.Mocked<IncidentTypeRepository>;

	// Create service and inject mock repository
	service = new IncidentTypeService();
	service.incidentTypeRepository = mockRepo;
});

describe('IncidentTypeService', () => {
	describe('createIncidentType', () => {
		it('should create a new incident type', async () => {
			const mockDto: IncidentTypeDto = {
				id: "INC-TEST123",
				name: "Fire Incident"
			};

			mockRepo.create.mockResolvedValue(mockDto);

			const result = await service.createIncidentType("Fire Incident");

			expect(mockRepo.create).toHaveBeenCalled();
			expect(result).toEqual(mockDto);
			expect(result.name).toBe("Fire Incident");
		});

		it('should create incident type with proper structure', async () => {
			const mockDto: IncidentTypeDto = {
				id: "INC-TEST456",
				name: "Water Leak"
			};

			mockRepo.create.mockResolvedValue(mockDto);

			const result = await service.createIncidentType("Water Leak");

			const createCall = mockRepo.create.mock.calls[0]?.[0];
			expect(createCall).toBeInstanceOf(IncidentType);
			expect(createCall?.name).toBe("Water Leak");
			expect(result).toEqual(mockDto);
		});
	});

	describe('getIncidentTypeById', () => {
		it('should return incident type when found', async () => {
			const mockDto: IncidentTypeDto = {
				id: "INC-TEST123",
				name: "Fire Incident"
			};

			mockRepo.getById.mockResolvedValue(mockDto);

			const result = await service.getIncidentTypeById("INC-TEST123");

			expect(mockRepo.getById).toHaveBeenCalledWith("INC-TEST123");
			expect(result).toEqual(mockDto);
		});

		it('should return null when incident type not found', async () => {
			mockRepo.getById.mockResolvedValue(null);

			const result = await service.getIncidentTypeById("INC-NOTFOUND");

			expect(mockRepo.getById).toHaveBeenCalledWith("INC-NOTFOUND");
			expect(result).toBeNull();
		});

		it('should return incident type with parent and children', async () => {
			const mockDto: IncidentTypeDto = {
				id: "INC-PARENT",
				name: "Parent Type",
				childrenIds: ["INC-CHILD1", "INC-CHILD2"]
			};

			mockRepo.getById.mockResolvedValue(mockDto);

			const result = await service.getIncidentTypeById("INC-PARENT");

			expect(result?.childrenIds).toHaveLength(2);
			expect(result?.childrenIds).toContain("INC-CHILD1");
		});
	});

	describe('getAllIncidentTypes', () => {
		it('should return all incident types', async () => {
			const mockDtos: IncidentTypeDto[] = [
				{ id: "INC-TYPE1", name: "Type 1" },
				{ id: "INC-TYPE2", name: "Type 2" },
				{ id: "INC-TYPE3", name: "Type 3" }
			];

			mockRepo.getAll.mockResolvedValue(mockDtos);

			const result = await service.getAllIncidentTypes();

			expect(mockRepo.getAll).toHaveBeenCalled();
			expect(result).toEqual(mockDtos);
			expect(result).toHaveLength(3);
		});

		it('should return empty array when no incident types exist', async () => {
			mockRepo.getAll.mockResolvedValue([]);

			const result = await service.getAllIncidentTypes();

			expect(result).toEqual([]);
			expect(result).toHaveLength(0);
		});

		it('should return incident types with relationships', async () => {
			const mockDtos: IncidentTypeDto[] = [
				{ 
					id: "INC-PARENT", 
					name: "Parent", 
					childrenIds: ["INC-CHILD1"] 
				},
				{ 
					id: "INC-CHILD1", 
					name: "Child 1", 
					parentId: "INC-PARENT" 
				}
			];

			mockRepo.getAll.mockResolvedValue(mockDtos);

			const result = await service.getAllIncidentTypes();

			expect(result).toHaveLength(2);
			expect(result[0]?.childrenIds).toBeDefined();
			expect(result[1]?.parentId).toBeDefined();
		});
	});

	describe('updateIncidentType', () => {
		it('should update incident type name', async () => {
			const mockDto: IncidentTypeDto = {
				id: "INC-TEST123",
				name: "Updated Name"
			};

			mockRepo.update.mockResolvedValue(mockDto);

			const result = await service.updateIncidentType("INC-TEST123", "Updated Name");

			expect(mockRepo.update).toHaveBeenCalledWith("INC-TEST123", "Updated Name", undefined);
			expect(result?.name).toBe("Updated Name");
		});

		it('should add children to incident type', async () => {
			const mockDto: IncidentTypeDto = {
				id: "INC-PARENT",
				name: "Parent",
				childrenIds: ["INC-CHILD1", "INC-CHILD2"]
			};

			mockRepo.update.mockResolvedValue(mockDto);

			const result = await service.updateIncidentType(
				"INC-PARENT", 
				undefined, 
				["INC-CHILD1", "INC-CHILD2"]
			);

			expect(mockRepo.update).toHaveBeenCalledWith(
				"INC-PARENT", 
				undefined, 
				["INC-CHILD1", "INC-CHILD2"]
			);
			expect(result?.childrenIds).toHaveLength(2);
		});

		it('should update both name and children', async () => {
			const mockDto: IncidentTypeDto = {
				id: "INC-TEST",
				name: "New Name",
				childrenIds: ["INC-CHILD"]
			};

			mockRepo.update.mockResolvedValue(mockDto);

			const result = await service.updateIncidentType("INC-TEST", "New Name", ["INC-CHILD"]);

			expect(mockRepo.update).toHaveBeenCalledWith("INC-TEST", "New Name", ["INC-CHILD"]);
			expect(result?.name).toBe("New Name");
			expect(result?.childrenIds).toContain("INC-CHILD");
		});

		it('should return null when incident type not found', async () => {
			mockRepo.update.mockResolvedValue(null);

			const result = await service.updateIncidentType("INC-NOTFOUND", "New Name");

			expect(result).toBeNull();
		});
	});

	describe('removeChild', () => {
		it('should remove child from incident type', async () => {
			const mockDto: IncidentTypeDto = {
				id: "INC-PARENT",
				name: "Parent",
				childrenIds: ["INC-CHILD2"]
			};

			mockRepo.removeChild.mockResolvedValue(mockDto);

			const result = await service.removeChild("INC-PARENT", "INC-CHILD1");

			expect(mockRepo.removeChild).toHaveBeenCalledWith("INC-PARENT", "INC-CHILD1");
			expect(result?.childrenIds).not.toContain("INC-CHILD1");
		});

		it('should return null when parent not found', async () => {
			mockRepo.removeChild.mockResolvedValue(null);

			const result = await service.removeChild("INC-NOTFOUND", "INC-CHILD");

			expect(result).toBeNull();
		});

		it('should handle removing last child', async () => {
			const mockDto: IncidentTypeDto = {
				id: "INC-PARENT",
				name: "Parent",
				childrenIds: []
			};

			mockRepo.removeChild.mockResolvedValue(mockDto);

			const result = await service.removeChild("INC-PARENT", "INC-LASTCHILD");

			expect(result?.childrenIds).toHaveLength(0);
		});
	});

	describe('deleteIncidentType', () => {
		it('should delete incident type successfully', async () => {
			mockRepo.deleteById.mockResolvedValue(true);

			const result = await service.deleteIncidentType("INC-TEST123");

			expect(mockRepo.deleteById).toHaveBeenCalledWith("INC-TEST123");
			expect(result).toBe(true);
		});

		it('should return false when incident type not found', async () => {
			mockRepo.deleteById.mockResolvedValue(false);

			const result = await service.deleteIncidentType("INC-NOTFOUND");

			expect(mockRepo.deleteById).toHaveBeenCalledWith("INC-NOTFOUND");
			expect(result).toBe(false);
		});

		it('should delete incident type with children', async () => {
			mockRepo.deleteById.mockResolvedValue(true);

			const result = await service.deleteIncidentType("INC-PARENT");

			expect(mockRepo.deleteById).toHaveBeenCalledWith("INC-PARENT");
			expect(result).toBe(true);
		});
	});
});
