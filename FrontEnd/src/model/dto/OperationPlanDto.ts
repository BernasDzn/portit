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
			type: 'Staff' | 'Crane';
            startTime?: string;
            endTime?: string;
		}[];
		payload?: any;
	}[];
	metadata: {
		createdBy: string;
		createdAt: string;
		algorithmUsed: string;
	};
}

export interface OperationPlanFilter {
    startDate?: string;
    endDate?: string;
};