import { TaskCategoryDto } from "../taskCategoryDto";
import { PayloadDto } from "./payloadDto";
import { ResourceDto } from "./resourceDto";

export interface OperationDto {
	type: TaskCategoryDto;
	startTime: string;
	endTime: string;
	resources: ResourceDto[];
    payload?: PayloadDto | null;
}