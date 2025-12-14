import ComplementaryTask from '../domain/complementaryTask';
import { ComplementaryTaskDto } from '../dto/complementaryTaskDto';

export class ComplementaryTaskMapper {

	static toDto(task: ComplementaryTask): ComplementaryTaskDto {
		return task.toDto();
	}

	static toDomain(dto: ComplementaryTaskDto): ComplementaryTask {
		return new ComplementaryTask({
			categoryId: dto.categoryId,
			responsibleTeam: dto.responsibleTeam,
			startTimestamp: dto.startTimestamp,
			...(dto.endTimestamp !== undefined && { endTimestamp: dto.endTimestamp }),
			status: dto.status,
			vesselVisitEventId: dto.vesselVisitEventId,
			impact: dto.impact,
			...(dto.description !== undefined && { description: dto.description })
		}, dto.id);
	}
}
