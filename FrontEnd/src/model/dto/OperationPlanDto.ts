import type TaskCategoryDto from "./TaskCategoryDto";

export interface OperationPlanDto {
	id: string;
	relatedVVN: string;
	dock: string;
	operationSchedule: {
		type: TaskCategoryDto;
		startTime: string;
		endTime: string;
		resources: {
			name: string;
			type: string;
		}[];
		payload?: {
			containerId?: string;
			storageLocation?: string;
		};
	}[];
	metadata: {
		createdBy: string;
		createdAt: string;
		algorithmUsed: string;
	};
}