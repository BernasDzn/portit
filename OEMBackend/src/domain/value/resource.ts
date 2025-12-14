import { ResourceDto } from "../../dto/value/resourceDto";

export enum ResourceType {
	Crane = "Crane",
	Staff = "Staff",
} // Keeping it future proof for potential clarifications in the forum

export class Resource {
	name: string;
	type: ResourceType;
    startTime?: Date ;
    endTime?: Date;

	constructor(params: {
		name: string;
		type: ResourceType;
        startTime?: Date;
        endTime?: Date;
	}) {
		this.name = params.name;
		this.type = params.type;
        this.startTime = params.startTime;
        this.endTime = params.endTime;
	}

	toDto(): ResourceDto {
		return {
			name: this.name,
			type: ResourceType[this.type],
            startTime: this.startTime?.toISOString(),
            endTime: this.endTime?.toISOString(),
		};
	}

}