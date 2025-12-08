import { ResourceDto } from "../../dto/value/resourceDto";

export enum ResourceType {
	Crane = "Crane",
	Staff = "Staff",
} // Keeping it future proof for potential clarifications in the forum

export class Resource {
	name: string;
	type: ResourceType;

	constructor(params: {
		name: string;
		type: ResourceType;
	}) {
		this.name = params.name;
		this.type = params.type;
	}

	toDto(): ResourceDto {
		return {
			name: this.name,
			type: ResourceType[this.type],
		};
	}

}