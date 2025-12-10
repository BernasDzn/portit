import { OperationDto } from "../../dto/value/operationDto";
import { TaskCategory } from "../taskCategory";
import EmptyPayload from "../taskPayloads/emptyPayload";
import LoadPayload from "../taskPayloads/loadPayload";
import { Resource } from "./resource";

export class Operation {
	operationType: TaskCategory;
	startTime: Date;
	endTime: Date;
	resources: Resource[];
    payload: EmptyPayload | LoadPayload;

	constructor(params: {
		operationType: TaskCategory;
		startTime: Date;
		endTime: Date;
		resources: Resource[];
        payload: EmptyPayload | LoadPayload;
	}) {
		this.operationType = params.operationType;
		this.startTime = params.startTime;
		this.endTime = params.endTime;
		this.resources = params.resources;
        this.payload = params.payload;
	}

	toDto(): OperationDto {
		return {
			type: this.operationType.toDto(),
			startTime: this.startTime.toISOString(),
			endTime: this.endTime.toISOString(),
			resources: this.resources.map(resource => resource.toDto()),
            payload: this.payload
		};
	}

}