import { OperationDto } from "../../dto/value/operationDto";
import { TaskCategory } from "../taskCategory";
import { Resource } from "./resource";

export class Operation {
	operationType: TaskCategory;
	startTime: Date;
	endTime: Date;
	resources: Resource[];

	constructor(params: {
		operationType: TaskCategory;
		startTime: Date;
		endTime: Date;
		resources: Resource[];
	}) {
		this.operationType = params.operationType;
		this.startTime = params.startTime;
		this.endTime = params.endTime;
		this.resources = params.resources;
	}

	toDto(): OperationDto {
		return {
			type: this.operationType.toDto(),
			startTime: this.startTime.toISOString(),
			endTime: this.endTime.toISOString(),
			resources: this.resources.map(resource => resource.toDto()),
		};
	}

}