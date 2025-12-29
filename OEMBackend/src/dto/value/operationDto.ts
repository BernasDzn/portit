import { TaskCategoryDto } from "../taskCategoryDto";
import { ResourceDto } from "./resourceDto";

export interface OperationDto {
    id?: string;
    type: TaskCategoryDto;
    startTime: string;
    endTime: string;
    resources: ResourceDto[];
    payload?: any;
    impactedOperations?: string[];
}

export interface OperationStartDto {
    id: string;
    type: string;
    startTime: string;
    endTime: string;
    resources: ResourceStartDto[];
    payload?: any;
    impactedOperations?: string[];
};

export interface ResourceStartDto {
    name: string;
    startTime: string;
    endTime: string;
}