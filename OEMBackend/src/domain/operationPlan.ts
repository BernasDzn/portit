import { LinkedList } from '../utils/linkedList';

import { OperationPlanDto } from '../dto/operationPlanDto';

import { Operation } from './value/operation';
import { OperationPlanMetadata } from './value/operationPlanMetadata';
import mongoose from 'mongoose';

export class OperationPlan {
	id: string;
	relatedVVN: string;
	dock: string;
	operationSchedule: LinkedList<Operation>;
	metadata: OperationPlanMetadata;
	

	constructor(params: {
		id?: string;
		relatedVVN: string;
		dock: string;
		operationSchedule: LinkedList<Operation>;
		metadata: OperationPlanMetadata;

	}) {
		this.id = params.id || new mongoose.Types.ObjectId().toString();
		this.relatedVVN = params.relatedVVN;
		this.dock = params.dock;
		this.operationSchedule = params.operationSchedule;
		this.metadata = params.metadata;
	};

	toDto(): OperationPlanDto {
		return {
			id: this.id,
			relatedVVN: this.relatedVVN,
			dock: this.dock,
			operationSchedule: this.operationSchedule.toArray().map(op => op.toDto()),
			metadata: this.metadata.toDto()
		};
	}
}