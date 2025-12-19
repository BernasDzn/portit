import { Entity } from '../core/domain/entity';
import { OperationPlanDto } from '../dto/operationPlanDto';
import LinkedList from '../utils/linkedList';
import Operation from './value/operation';
import OperationPlanMetadata from './value/operationPlanMetadata';
import mongoose from 'mongoose';

export interface OperationPlanProps {
	date: string;
	relatedVVN: string;
	dock: string;
	operationSchedule: LinkedList<Operation>;
	metadata: OperationPlanMetadata;
}

export default class OperationPlan extends Entity<OperationPlanProps> {
	get id(): string { return this._id.toString(); }
	get date(): string { return this.props.date; }
	get relatedVVN(): string { return this.props.relatedVVN; }
	get dock(): string { return this.props.dock; }
	get operationSchedule(): LinkedList<Operation> { return this.props.operationSchedule; }
	get metadata(): OperationPlanMetadata { return this.props.metadata; }

	constructor(props: OperationPlanProps, id?: any) {
		super(
			props,
			id || new mongoose.Types.ObjectId()
		);
	}

	public toDto() : OperationPlanDto {
		return {
			id: this.id,
			date: this.date,
			relatedVVN: this.relatedVVN,
			dock: this.dock,
			operationSchedule: this.operationSchedule.toArray().map(op => op.toDto()),
			metadata: this.metadata.toDto()
		};
	}

}