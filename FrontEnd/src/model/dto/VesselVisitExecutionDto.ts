import type { OperationStatus, VesselVisitExecutionStatus } from '../VesselVisitExecution';
import type TaskCategoryDto from './TaskCategoryDto';

export interface ResourceDto {
    name: string;
    type: string;
    startTime?: string;
    endTime?: string;
}

export interface OperationDto {
    id: string;
    type: TaskCategoryDto;
    startTime: string;
    endTime: string;
    resources: ResourceDto[];
    payload?: Object;
}

export interface OperationWithStatusDto {
    id: string;
    operation: OperationDto;
    status: OperationStatus;
    impactedOperations: string[];
}

export interface VesselVisitExecutionDto {
    id: string;
    code: string;
    relatedVVN: string;
    operationsExecuted: OperationWithStatusDto[];
    dateOpen?: string;
    dateClosed?: string;
    status: VesselVisitExecutionStatus;
    dock?: string;
    berthTime?: string;
    createdBy: string;
}

export interface VesselVisitExecutionFilter {
    startDate?: string;
    endDate?: string;
    relatedVVN?: string;
    status?: string;
}

export interface ComplementaryTaskDto {
    vveCode: string;
    vveRelatedVVN: string;
    operationType: string;
    taskId: string;
    operation: string;
    status: string;
    impactedOperations: string[];
}