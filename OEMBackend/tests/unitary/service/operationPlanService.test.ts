import { OperationPlanDto } from "../../../src/dto/operationPlanDto";
import { OperationPlanRepository } from "../../../src/repository/operationPlanRepository";
import { OperationPlanService } from "../../../src/services/operationPlanService";
import OperationPlan from "../../../src/domain/operationPlan";
import LinkedList from "../../../src/utils/linkedList";
import Operation from "../../../src/domain/value/operation";
import { IdCode } from "../../../src/domain/value/idcode";
import { TaskCategory } from "../../../src/domain/taskCategory";
import OperationPlanMetadata from "../../../src/domain/value/operationPlanMetadata";
import { Page } from "../../../src/utils/page";

let service: OperationPlanService;
let mockRepo: jest.Mocked<OperationPlanRepository>;
let mockOperationPlan: OperationPlan;

beforeEach(() => {
	let operationSchedule = new LinkedList<Operation>();
	operationSchedule.insertInBegin(
		new Operation({
			operationType: new TaskCategory(
				{ id: "TK1", category: "op_type_1", description: "Loading", name: "Loading" },
			),
			startTime: new Date("2024-10-01T08:00:00Z"),
			endTime: new Date("2024-10-01T10:00:00Z"),
			resources: []
		})
	);

	let operationPlanMetadata = new OperationPlanMetadata({
		createdBy: "tester",
		createdAt: new Date("2024-09-01T12:00:00Z"),
		algorithmUsed: "optimal"
	});

	mockOperationPlan = new OperationPlan(
		{
			relatedVVN: "VVN001",
			dock: "Dock1",
			operationSchedule: operationSchedule,
			metadata: operationPlanMetadata
		},
		"plan-test"
	);

	// Create mock repository
	mockRepo = {
		create: jest.fn(),
		getAll: jest.fn(),
		getById: jest.fn(),
		getByDateGrouped: jest.fn(),
		getNotificationsWithoutPlan: jest.fn(),
		deleteById: jest.fn(),
		getContainersOfNotification: jest.fn()
	} as jest.Mocked<OperationPlanRepository>;

	// Create service and inject mock repository
	service = new OperationPlanService();
	service.operationPlanRepository = mockRepo;
});

describe('getAll', () => {
	it('should return paginated operation plans', async () => {
		const mockPlans: Page<OperationPlanDto> = {
			pageNumber: 1,
			pageSize: 10,
			pageCount: 1,
			items: [mockOperationPlan.toDto()]
		};

		mockRepo.getAll.mockResolvedValue(mockPlans);

		const result = await service.getAll({ pageNumber: 1, pageSize: 10 });

		expect(mockRepo.getAll).toHaveBeenCalledWith({ pageNumber: 1, pageSize: 10 });
		expect(result).toEqual(mockPlans);
	});

	it('should handle empty results', async () => {
		const emptyPage: Page<OperationPlanDto> = {
			pageNumber: 1,
			pageSize: 10,
			pageCount: 0,
			items: []
		};

		mockRepo.getAll.mockResolvedValue(emptyPage);

		const result = await service.getAll({ pageNumber: 1, pageSize: 10 });

		expect(result.items).toHaveLength(0);
		expect(result.pageCount).toBe(0);
	});

	it('should handle different page sizes', async () => {
		const mockPlans: Page<OperationPlanDto> = {
			pageNumber: 2,
			pageSize: 5,
			pageCount: 3,
			items: [mockOperationPlan.toDto()]
		};

		mockRepo.getAll.mockResolvedValue(mockPlans);

		const result = await service.getAll({ pageNumber: 2, pageSize: 5 });

		expect(mockRepo.getAll).toHaveBeenCalledWith({ pageNumber: 2, pageSize: 5 });
		expect(result.pageNumber).toBe(2);
		expect(result.pageSize).toBe(5);
	});
});

describe('getById', () => {
	it('should return operation plan when found', async () => {
		const planDto = mockOperationPlan.toDto();
		mockRepo.getById.mockResolvedValue(planDto);

		const result = await service.getById('plan-test');

		expect(mockRepo.getById).toHaveBeenCalledWith('plan-test');
		expect(result).toEqual(planDto);
	});

	it('should return null when operation plan not found', async () => {
		mockRepo.getById.mockResolvedValue(null);

		const result = await service.getById('non-existent-id');

		expect(mockRepo.getById).toHaveBeenCalledWith('non-existent-id');
		expect(result).toBeNull();
	});
});

describe('getByDateGrouped', () => {
	it('should return operation plans grouped by date', async () => {
		const groupedPlans = [
			{
				date: '2024-10-01',
				plans: [mockOperationPlan.toDto()]
			},
			{
				date: '2024-10-02',
				plans: [mockOperationPlan.toDto()]
			}
		];

		mockRepo.getByDateGrouped.mockResolvedValue(groupedPlans);

		const result = await service.getByDateGrouped();

		expect(mockRepo.getByDateGrouped).toHaveBeenCalled();
		expect(result).toEqual(groupedPlans);
		expect(result).toHaveLength(2);
	});

	it('should return empty array when no plans exist', async () => {
		mockRepo.getByDateGrouped.mockResolvedValue([]);

		const result = await service.getByDateGrouped();

		expect(result).toEqual([]);
	});
});

describe('getNotificationsWithoutPlan', () => {
	it('should return list of VVN IDs without plans', async () => {
		const unplannedVvns = ['VVN003', 'VVN004', 'VVN005'];
		const mockToken = 'mock-jwt-token';

		mockRepo.getNotificationsWithoutPlan.mockResolvedValue(unplannedVvns);

		const result = await service.getNotificationsWithoutPlan(mockToken);

		expect(mockRepo.getNotificationsWithoutPlan).toHaveBeenCalledWith(mockToken);
		expect(result).toEqual(unplannedVvns);
		expect(result).toHaveLength(3);
	});

	it('should return empty array when all notifications have plans', async () => {
		const mockToken = 'mock-jwt-token';
		mockRepo.getNotificationsWithoutPlan.mockResolvedValue([]);

		const result = await service.getNotificationsWithoutPlan(mockToken);

		expect(result).toEqual([]);
	});
});