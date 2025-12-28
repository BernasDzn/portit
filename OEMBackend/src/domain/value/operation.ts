import { OperationDto } from "../../dto/value/operationDto";
import { TaskCategory } from "../taskCategory";
import { Payload } from "./payload";
import { Resource } from "./resource";

export default class Operation {
    id?: string;
	operationType: TaskCategory;
	startTime: Date;
	endTime: Date;
	resources: Resource[];
    payload?: Payload | null;

	constructor(params: {
        id?: string;
		operationType: TaskCategory;
		startTime: Date;
		endTime: Date;
		resources: Resource[];
        payload?: Payload;
	}) {
        if (params.id) {
            this.id = params.id;
        }
		this.operationType = params.operationType;
		this.startTime = params.startTime;
		this.endTime = params.endTime;
		this.resources = params.resources;
        this.payload = params.payload ?? null;
	}

	toDto(): OperationDto {
		return {
			id: this.id,
			type: this.operationType.toDto(),
			startTime: this.startTime.toISOString(),
			endTime: this.endTime.toISOString(),
			resources: this.resources.map(resource => resource.toDto()),
            payload: this.payload ?? null
		};
	}

}