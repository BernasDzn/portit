export interface OperationPlanDto {
	id: string;
	relatedVVN: string;
	dock: string;
	operationSchedule: {
		type: string;
		startTime: string;
		endTime: string;
		resources: {
			name: string;
			type: string;
		}[];
	}[];
	metadata: {
		createdBy: string;
		createdAt: string;
		algorithmUsed: string;
	};
}