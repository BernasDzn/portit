import { Service } from "typedi";
import { ComplementaryTaskRepository } from "../repository/complementaryTaskRepository";
import { ComplementaryTaskDto, CreateComplementaryTaskDto, UpdateComplementaryTaskDto } from "../dto/complementaryTaskDto";
import { ComplementaryTaskFilter } from "../dto/filters/complementaryTaskFilter";
import { Page, Pageable } from "../utils/page";
import { TaskStatus } from "../domain/complementaryTask";

@Service("complementaryTaskService")
export class ComplementaryTaskService {

	private repository: ComplementaryTaskRepository;

	constructor() {
		this.repository = new ComplementaryTaskRepository();
	}

	async create(dto: CreateComplementaryTaskDto): Promise<ComplementaryTaskDto> {
		// Validate that start timestamp is not in the future beyond a reasonable threshold
		const now = new Date();
		if (dto.startTimestamp > new Date(now.getTime() + 24 * 60 * 60 * 1000)) {
			throw new Error("Start timestamp cannot be more than 24 hours in the future");
		}

		// Validate end timestamp if provided
		if (dto.endTimestamp && dto.endTimestamp < dto.startTimestamp) {
			throw new Error("End timestamp must be after start timestamp");
		}

		// Auto-set status to completed if end timestamp is provided
		if (dto.endTimestamp && !dto.status) {
			dto.status = TaskStatus.COMPLETED;
		}

		return await this.repository.create(dto);
	}

	async findById(id: string): Promise<ComplementaryTaskDto | null> {
		return await this.repository.findById(id);
	}

	async findAll(filter: ComplementaryTaskFilter, pageable: Pageable): Promise<Page<ComplementaryTaskDto>> {
		return await this.repository.findAll(filter, pageable);
	}

	async update(id: string, dto: UpdateComplementaryTaskDto): Promise<ComplementaryTaskDto | null> {
		const existing = await this.repository.findById(id);
		if (!existing) {
			throw new Error(`Complementary task with id ${id} not found`);
		}

		// Validate timestamps if they are being updated
		const startTimestamp = dto.startTimestamp || existing.startTimestamp;
		const endTimestamp = dto.endTimestamp || existing.endTimestamp;

		if (endTimestamp && endTimestamp < startTimestamp) {
			throw new Error("End timestamp must be after start timestamp");
		}

		return await this.repository.update(id, dto);
	}

	async delete(id: string): Promise<boolean> {
		const existing = await this.repository.findById(id);
		if (!existing) {
			throw new Error(`Complementary task with id ${id} not found`);
		}

		return await this.repository.delete(id);
	}

	async completeTask(id: string, endTimestamp?: Date): Promise<ComplementaryTaskDto | null> {
		const existing = await this.repository.findById(id);
		if (!existing) {
			throw new Error(`Complementary task with id ${id} not found`);
		}

		if (existing.status === TaskStatus.COMPLETED) {
			throw new Error("Task is already completed");
		}

		const completionTime = endTimestamp || new Date();

		if (completionTime < existing.startTimestamp) {
			throw new Error("Completion time cannot be before start timestamp");
		}

		return await this.repository.completeTask(id, completionTime);
	}

	async findOngoingTasksImpactingOperations(vesselVisitEventId?: string): Promise<ComplementaryTaskDto[]> {
		return await this.repository.findOngoingTasksImpactingOperations(vesselVisitEventId);
	}

	async getTasksByVessel(vesselVisitEventId: string, pageable: Pageable): Promise<Page<ComplementaryTaskDto>> {
		const filter: ComplementaryTaskFilter = { vesselVisitEventId };
		return await this.repository.findAll(filter, pageable);
	}

	async getTasksByDateRange(startDate: Date, endDate: Date, pageable: Pageable): Promise<Page<ComplementaryTaskDto>> {
		const filter: ComplementaryTaskFilter = { startDate, endDate };
		return await this.repository.findAll(filter, pageable);
	}

	async getOngoingTasks(pageable: Pageable): Promise<Page<ComplementaryTaskDto>> {
		const filter: ComplementaryTaskFilter = { status: TaskStatus.ONGOING };
		return await this.repository.findAll(filter, pageable);
	}
}
