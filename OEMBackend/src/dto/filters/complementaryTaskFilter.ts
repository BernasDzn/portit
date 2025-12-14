import { TaskStatus, TaskImpact } from '../../domain/complementaryTask';

export interface ComplementaryTaskFilter {
	vesselVisitEventId?: string | undefined;
	status?: TaskStatus | undefined;
	impact?: TaskImpact | undefined;
	categoryId?: string | undefined;
	startDate?: Date | undefined;
	endDate?: Date | undefined;
	responsibleTeam?: string | undefined;
}
