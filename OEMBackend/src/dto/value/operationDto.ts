import { ResourceDto } from "./resourceDto";

export interface OperationDto {
	type: string;
	startTime: string;
	endTime: string;
	resources: ResourceDto[];
}