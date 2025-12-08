import { OperationDto } from './value/operationDto';
import { OperationPlanMetadataDto } from './value/operationPlanMetadataDto';

export interface OperationPlanDto {
	id: string;
	relatedVVN: string;
	dock: string;
	operationSchedule: OperationDto[];
	metadata: OperationPlanMetadataDto;
}