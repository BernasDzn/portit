import { TaskCategoryDto } from "../taskCategoryDto";
import { ResourceDto } from "./resourceDto";

export interface OperationDto {
	type: TaskCategoryDto;
	startTime: string;
	endTime: string;
	resources: ResourceDto[];
    payload?: any;
}

export interface OperationStartDto {
    id: string;
    type: string;
    startTime: string;
    endTime: string;
    resources: ResourceStartDto[];
    payload?: any;
};

export interface ResourceStartDto {
    name: string;
    startTime: string;
    endTime: string;
}