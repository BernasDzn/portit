import { IncidentTypeDto } from "../../../src/dto/incidentTypeDto";
import { IncidentTypeRepository } from "../../../src/repository/incidentTypeRepository";
import { IncidentTypeService } from "../../../src/services/incidentTypeService";
import IncidentType from "../../../src/domain/incidentType";
import { Page, Pageable } from "../../../src/utils/page";
import { IncidentTypeValidator } from "../../../src/services/validators/incidentTypeValidator";

let service: IncidentTypeService;
let mockRepo: jest.Mocked<IncidentTypeRepository>;
let mockValidator : jest.Mocked<IncidentTypeValidator>;
let mockIncidentType: IncidentType;

beforeEach(() => {
	mockIncidentType = new IncidentType(
		{ name: "Fire Incident", description: "", severity: "Minor" },
		"INC-TEST123"
	);

	// Create mock repository
	mockRepo = {
		create: jest.fn(),
        getById: jest.fn(),
        getAll: jest.fn(),
        update: jest.fn(),
        removeSubtype: jest.fn(),
        deleteById: jest.fn(),
        mapToDto: jest.fn()
	} as unknown as jest.Mocked<IncidentTypeRepository>;

	mockValidator = {
		validateHierarchy: jest.fn()
	} as unknown as jest.Mocked<IncidentTypeValidator>;

	// Create service and inject mock repository
	service = new IncidentTypeService();
	service.incidentTypeRepository = mockRepo;
	service.incidentTypeValidator = mockValidator;
});

describe('createIncidentType', () => {
	it('should create a new incident type', async () => {
		const mockDto: IncidentTypeDto = {
			id: "INC-TEST123",
			description: "",
			severity: "Minor",
			name: "Fire Incident"
		};

		mockRepo.create.mockResolvedValue(mockDto);

		const result = await service.createIncidentType("Fire Incident", "Minor", "");

		expect(mockRepo.create).toHaveBeenCalled();
		expect(result).toEqual(mockDto);
		expect(result.name).toBe("Fire Incident");
	});

	it('should create incident type with proper structure', async () => {
		const mockDto: IncidentTypeDto = {
			id: "INC-TEST456",
			description: "",
			severity: "Major",
			name: "Water Leak"
		};

		mockRepo.create.mockResolvedValue(mockDto);

		const result = await service.createIncidentType("Water Leak", "Major", "");

		const createCall = mockRepo.create.mock.calls[0]?.[0];
		expect(createCall).toBeInstanceOf(IncidentType);
		expect(createCall?.name).toBe("Water Leak");
		expect(result).toEqual(mockDto);
	});

	it('shouldnt allow creation of circular relationships between incident types', async () => {
		
	});


});

describe('getIncidentTypeById', () => {
	it('should return incident type when found', async () => {
		const mockDto: IncidentTypeDto = {
			id: "INC-TEST123",
			description: "",
			severity: "Minor",
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
			id: "INC-PARENT", description: "Parent Type",
			severity: "Major", name: "Parent Type",
			subtypesIds: ["INC-CHILD1", "INC-CHILD2"]
		};

		mockRepo.getById.mockResolvedValue(mockDto);

		const result = await service.getIncidentTypeById("INC-PARENT");

		expect(result?.subtypesIds).toHaveLength(2);
		expect(result?.subtypesIds).toContain("INC-CHILD1");
	});
});

describe('getAllIncidentTypes', () => {
	it('should return all incident types', async () => {
		const mockDtos: IncidentTypeDto[] = [
			{ id: "INC-TYPE1", name: "Type 1", description: "", severity: "Minor" },
			{ id: "INC-TYPE2", name: "Type 2", description: "", severity: "Minor" },
			{ id: "INC-TYPE3", name: "Type 3", description: "", severity: "Minor" }
		];

		let page: Page<IncidentTypeDto> = {
			pageNumber: 1, pageSize: 10, pageCount: 1, items: mockDtos
		};
		
		mockRepo.getAll.mockResolvedValue(page);

		let pageable: Pageable = { pageNumber: 1, pageSize: 10 };
		const result = await service.getAllIncidentTypes(pageable);

		expect(mockRepo.getAll).toHaveBeenCalled();
		expect(result.items).toEqual(mockDtos);
		expect(result.items).toHaveLength(3);
	});

	it('should return empty page when no incident types exist', async () => {
		let emptyPage: Page<IncidentTypeDto> = {
			pageNumber: 1, pageSize: 10, pageCount: 0, items: []
		};
		mockRepo.getAll.mockResolvedValue(emptyPage);

		let pageable: Pageable = { pageNumber: 1, pageSize: 10 };
		const result = await service.getAllIncidentTypes(pageable);

		expect(result.items).toEqual([]);
		expect(result.items).toHaveLength(0);
	});

	it('should return incident types with relationships', async () => {
		const mockDtos: IncidentTypeDto[] = [
			{ id: "INC-PARENT", name: "Parent", description: "", severity: "Major", 
				subtypesIds: ["INC-CHILD1"] 
			},
			{ 
				id: "INC-CHILD1", name: "Child 1", description: "", severity: "Minor", 
				subtypeOfId: "INC-PARENT" 
			}
		];

		let page: Page<IncidentTypeDto> = {
			pageNumber: 1, pageSize: 10, pageCount: 1, items: mockDtos
		};
		mockRepo.getAll.mockResolvedValue(page);

		let pageable: Pageable = { pageNumber: 1, pageSize: 10 };
		const result = await service.getAllIncidentTypes(pageable);

		expect(result.items).toHaveLength(2);
		expect(result.items.pop).toBeDefined();
	});
});

describe('updateIncidentType', () => {
	it('should update incident type name', async () => {
		const mockDto: IncidentTypeDto = {
			id: "INC-TEST123", description: "", severity: "Minor", name: "Updated Name"
		};

		mockRepo.update.mockResolvedValue(mockDto);

		const result = await service.updateIncidentType(
			mockDto.id, mockDto.name, mockDto.description, mockDto.severity
		);

		expect(mockRepo.update).toHaveBeenCalledWith("INC-TEST123", "Updated Name", "", "Minor", undefined);
		expect(result?.name).toBe("Updated Name");
	});

	it('should add children to incident type', async () => {
		const mockDto: IncidentTypeDto = {
			id: "INC-PARENT", name: "Parent", description: "", severity: "Major",
			subtypesIds: ["INC-CHILD1", "INC-CHILD2"]
		};

		mockRepo.update.mockResolvedValue(mockDto);

		const result = await service.updateIncidentType(
			mockDto.id, mockDto.name, mockDto.description, mockDto.severity, 
			["INC-CHILD1", "INC-CHILD2"]
		);

		expect(mockRepo.update).toHaveBeenCalledWith(
			mockDto.id, mockDto.name, mockDto.description, mockDto.severity, 
			["INC-CHILD1", "INC-CHILD2"]
		);
		expect(result?.subtypesIds).toHaveLength(2);
	});

	it('should update both name and children', async () => {
		const mockDto: IncidentTypeDto = {
			id: "INC-TEST", name: "New Name", description: "", severity: "Minor",
			subtypesIds: ["INC-CHILD"]
		};

		mockRepo.update.mockResolvedValue(mockDto);

		const result = await service.updateIncidentType("INC-TEST", "New Name", "", "Minor", ["INC-CHILD"]);

		expect(mockRepo.update).toHaveBeenCalledWith("INC-TEST", "New Name", "", "Minor", ["INC-CHILD"]);
		expect(result?.name).toBe("New Name");
		expect(result?.subtypesIds).toContain("INC-CHILD");
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
			id: "INC-PARENT", name: "Parent", description: "", severity: "Major",
			subtypesIds: ["INC-CHILD2"]
		};

		mockRepo.removeSubtype.mockResolvedValue(mockDto);

		const result = await service.removeSubtype("INC-PARENT", "INC-CHILD1");

		expect(mockRepo.removeSubtype).toHaveBeenCalledWith("INC-PARENT", "INC-CHILD1");
		expect(result?.subtypesIds).not.toContain("INC-CHILD1");
	});

	it('should return null when parent not found', async () => {
		mockRepo.removeSubtype.mockResolvedValue(null);

		const result = await service.removeSubtype("INC-NOTFOUND", "INC-CHILD");

		expect(result).toBeNull();
	});

	it('should handle removing last child', async () => {
		const mockDto: IncidentTypeDto = {
			id: "INC-PARENT", name: "Parent", description: "", severity: "Major",
			subtypesIds: []
		};

		mockRepo.removeSubtype.mockResolvedValue(mockDto);

		const result = await service.removeSubtype("INC-PARENT", "INC-LASTCHILD");

		expect(result?.subtypesIds).toHaveLength(0);
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
