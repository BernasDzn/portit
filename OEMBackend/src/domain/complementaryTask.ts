import { Entity } from '../core/domain/entity';
import { ComplementaryTaskDto } from '../dto/complementaryTaskDto';
import mongoose from 'mongoose';

export enum TaskStatus {
	ONGOING = 'ongoing',
	COMPLETED = 'completed',
	CANCELLED = 'cancelled'
}

export enum TaskImpact {
	PARALLEL = 'parallel',
	SUSPENDS_OPERATIONS = 'suspends_operations'
}

export interface ComplementaryTaskProps {
	categoryId: string;
	responsibleTeam: string;
	startTimestamp: Date;
	endTimestamp?: Date;
	status: TaskStatus;
	vesselVisitEventId: string;
	impact: TaskImpact;
	description?: string;
}

export default class ComplementaryTask extends Entity<ComplementaryTaskProps> {
	get id(): string { return this._id; }
	get categoryId(): string { return this.props.categoryId; }
	get responsibleTeam(): string { return this.props.responsibleTeam; }
	get startTimestamp(): Date { return this.props.startTimestamp; }
	get endTimestamp(): Date | undefined { return this.props.endTimestamp; }
	get status(): TaskStatus { return this.props.status; }
	get vesselVisitEventId(): string { return this.props.vesselVisitEventId; }
	get impact(): TaskImpact { return this.props.impact; }
	get description(): string | undefined { return this.props.description; }

	constructor(props: ComplementaryTaskProps, id?: any) {
		super(
			id || new mongoose.Types.ObjectId().toString(), 
			props
		);
	}

	public toDto(): ComplementaryTaskDto {
		return {
			id: this.id,
			categoryId: this.categoryId,
			responsibleTeam: this.responsibleTeam,
			startTimestamp: this.startTimestamp,
			endTimestamp: this.endTimestamp,
			status: this.status,
			vesselVisitEventId: this.vesselVisitEventId,
			impact: this.impact,
			description: this.description
		};
	}

	public complete(endTimestamp: Date): void {
		this.props.endTimestamp = endTimestamp;
		this.props.status = TaskStatus.COMPLETED;
	}

	public cancel(): void {
		this.props.status = TaskStatus.CANCELLED;
		if (!this.props.endTimestamp) {
			this.props.endTimestamp = new Date();
		}
	}

	public isOngoing(): boolean {
		return this.props.status === TaskStatus.ONGOING;
	}

	public isSuspendingOperations(): boolean {
		return this.props.impact === TaskImpact.SUSPENDS_OPERATIONS;
	}
}
