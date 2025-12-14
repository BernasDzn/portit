import { Entity } from "../core/domain/entity";
import { IncidentTypeDto } from "../dto/incidentTypeDto";

export type Severity = 'Minor' | 'Major' | 'Critical';

export interface IncidentTypeProps {
	name: string;
	description: string;
	severity: Severity;
	subtypeOf?: IncidentType | undefined;
	subtypes?: IncidentType[] | undefined;
}

export class IncidentTypeID{
	value: string;
	
	constructor() {
		const timestamp = Date.now().toString(16).substring(4, 7).toUpperCase();
		const random = Math.random().toString(16).substring(2, 8).toUpperCase();
		this.value = "INC-" + timestamp + random;
	}
}

export default class IncidentType extends Entity<IncidentTypeProps> {
	get id(): string { return this._id; }
	get name(): string { return this.props.name; }
	get description(): string { return this.props.description; }
	get severity(): Severity { return this.props.severity; }
	get subtypeOf(): IncidentType | undefined { return this.props.subtypeOf; }
	get subtypes(): IncidentType[] | undefined { return this.props.subtypes; }

	constructor(props: IncidentTypeProps, id?: string) {
		super(
			id || new IncidentTypeID().value, 
			props
		);
	}

	addSubtype(subtype: IncidentType) {
		if (!this.props.subtypes) {
			this.props.subtypes = [];
		}
		this.props.subtypes.push(subtype);
		subtype.props.subtypeOf = this;
	}

	toDto() : IncidentTypeDto {
		return {
			id: this.id,
			name: this.name,
			description: this.description,
			severity: this.severity
		};
	}

}