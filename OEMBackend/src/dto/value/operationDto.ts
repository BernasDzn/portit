import { TaskCategoryDto } from "../taskCategoryDto";
import { ResourceDto } from "./resourceDto";

export interface OperationDto {
	type: TaskCategoryDto;
	startTime: string;
	endTime: string;
	resources: ResourceDto[];
}