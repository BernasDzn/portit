import { TaskStatus, TaskImpact } from '../domain/complementaryTask';

export interface ComplementaryTaskDto {
	id: string;
	categoryId: string;
	responsibleTeam: string;
	startTimestamp: Date;
	endTimestamp?: Date | undefined;
	status: TaskStatus;
	vesselVisitEventId: string;
	impact: TaskImpact;
	description?: string | undefined;
}

export interface CreateComplementaryTaskDto {
	categoryId: string;
	responsibleTeam: string;
	startTimestamp: Date;
	endTimestamp?: Date | undefined;
	status?: TaskStatus | undefined;
	vesselVisitEventId: string;
	impact: TaskImpact;
	description?: string | undefined;
}

export interface UpdateComplementaryTaskDto {
	categoryId?: string | undefined;
	responsibleTeam?: string | undefined;
	startTimestamp?: Date | undefined;
	endTimestamp?: Date | undefined;
	status?: TaskStatus | undefined;
	vesselVisitEventId?: string | undefined;
	impact?: TaskImpact | undefined;
	description?: string | undefined;
}
