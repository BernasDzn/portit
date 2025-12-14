import ComplementaryTask, { TaskStatus } from "../domain/complementaryTask";
import { ComplementaryTaskDto, CreateComplementaryTaskDto, UpdateComplementaryTaskDto } from "../dto/complementaryTaskDto";
import { ComplementaryTaskFilter } from "../dto/filters/complementaryTaskFilter";
import ComplementaryTaskModel from "../schemas/complementaryTaskSchema";
import { Page, Pageable } from "../utils/page";

export class ComplementaryTaskRepository {

	async create(dto: CreateComplementaryTaskDto): Promise<ComplementaryTaskDto> {
		const taskData: any = {
			categoryId: dto.categoryId,
			responsibleTeam: dto.responsibleTeam,
			startTimestamp: dto.startTimestamp,
			status: dto.status || TaskStatus.ONGOING,
			vesselVisitEventId: dto.vesselVisitEventId,
			impact: dto.impact
		};

		if (dto.endTimestamp !== undefined) {
			taskData.endTimestamp = dto.endTimestamp;
		}

		if (dto.description !== undefined) {
			taskData.description = dto.description;
		}

		const createdDoc = await ComplementaryTaskModel.create(taskData);
		return this.mapToDto(createdDoc);
	}

	async findById(id: string): Promise<ComplementaryTaskDto | null> {
		const doc = await ComplementaryTaskModel.findById(id).populate('categoryId');
		if (!doc) return null;
		return this.mapToDto(doc);
	}

	async findAll(filter: ComplementaryTaskFilter, pageable: Pageable): Promise<Page<ComplementaryTaskDto>> {
		const { pageNumber, pageSize } = pageable;
		const skip = (pageNumber - 1) * pageSize;

		const query = this.buildFilterQuery(filter);

		const data = await ComplementaryTaskModel.find(query)
			.populate('categoryId')
			.sort({ startTimestamp: -1 })
			.skip(skip)
			.limit(pageSize);

		const total = await ComplementaryTaskModel.countDocuments(query);

		return {
			pageNumber,
			pageSize,
			pageCount: Math.ceil(total / pageSize),
			items: data.map(doc => this.mapToDto(doc))
		};
	}

	async update(id: string, dto: UpdateComplementaryTaskDto): Promise<ComplementaryTaskDto | null> {
		const doc = await ComplementaryTaskModel.findByIdAndUpdate(
			id,
			{ $set: dto },
			{ new: true, runValidators: true }
		).populate('categoryId');

		if (!doc) return null;
		return this.mapToDto(doc);
	}

	async delete(id: string): Promise<boolean> {
		const result = await ComplementaryTaskModel.findByIdAndDelete(id);
		return result !== null;
	}

	async findOngoingTasksImpactingOperations(vesselVisitEventId?: string): Promise<ComplementaryTaskDto[]> {
		const query: any = {
			status: TaskStatus.ONGOING,
			impact: 'suspends_operations'
		};

		if (vesselVisitEventId) {
			query.vesselVisitEventId = vesselVisitEventId;
		}

		const docs = await ComplementaryTaskModel.find(query)
			.populate('categoryId')
			.sort({ startTimestamp: -1 });

		return docs.map(doc => this.mapToDto(doc));
	}

	async completeTask(id: string, endTimestamp: Date): Promise<ComplementaryTaskDto | null> {
		const doc = await ComplementaryTaskModel.findByIdAndUpdate(
			id,
			{
				$set: {
					endTimestamp: endTimestamp,
					status: TaskStatus.COMPLETED
				}
			},
			{ new: true, runValidators: true }
		).populate('categoryId');

		if (!doc) return null;
		return this.mapToDto(doc);
	}

	private buildFilterQuery(filter: ComplementaryTaskFilter): any {
		const query: any = {};

		if (filter.vesselVisitEventId) {
			query.vesselVisitEventId = filter.vesselVisitEventId;
		}

		if (filter.status) {
			query.status = filter.status;
		}

		if (filter.impact) {
			query.impact = filter.impact;
		}

		if (filter.categoryId) {
			query.categoryId = filter.categoryId;
		}

		if (filter.responsibleTeam) {
			query.responsibleTeam = { $regex: filter.responsibleTeam, $options: 'i' };
		}

		if (filter.startDate || filter.endDate) {
			query.startTimestamp = {};
			if (filter.startDate) {
				query.startTimestamp.$gte = filter.startDate;
			}
			if (filter.endDate) {
				query.startTimestamp.$lte = filter.endDate;
			}
		}

		return query;
	}

	private mapToDto(doc: any): ComplementaryTaskDto {
		return {
			id: doc._id.toString(),
			categoryId: doc.categoryId?._id?.toString() || doc.categoryId,
			responsibleTeam: doc.responsibleTeam,
			startTimestamp: doc.startTimestamp,
			endTimestamp: doc.endTimestamp,
			status: doc.status,
			vesselVisitEventId: doc.vesselVisitEventId,
			impact: doc.impact,
			description: doc.description
		};
	}
}
