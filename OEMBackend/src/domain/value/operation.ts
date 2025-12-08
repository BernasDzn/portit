import { OperationDto } from "../../dto/value/operationDto";
import { Resource } from "./resource";

export class Operation {
	operationType: OperationType;
	startTime: Date;
	endTime: Date;
	resources: Resource[];

	constructor(params: {
		operationType: OperationType;
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
			type: OperationType[this.operationType],
			startTime: this.startTime.toISOString(),
			endTime: this.endTime.toISOString(),
			resources: this.resources.map(resource => resource.toDto()),
		};
	}

}

export enum OperationType{
	Unload = "Unload",
	Load = "Load"
}