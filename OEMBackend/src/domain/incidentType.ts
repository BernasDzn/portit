import { Entity } from "../core/domain/entity";
import { IncidentTypeID } from "./value/incidentTypeId";

export type Severity = 'Minor' | 'Major' | 'Critical';

export interface IncidentTypeProps {
	name: string;
	description: string;
	severity: Severity;
	subtypeOf: IncidentType | undefined;
	subtypes: IncidentType[] | undefined;
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
		this.validate();
	}
 
	validate(){
		if(this.props.subtypeOf && this.props.subtypeOf.id === this.id){
			throw new Error("An Incident Type cannot be a subtype of itself. Incident Type id: " + this.id);
		}
	}

	addSubtype(subtype: IncidentType) {
		if (!this.props.subtypes) {
			this.props.subtypes = [];
		}
		this.props.subtypes.push(subtype);
		subtype.props.subtypeOf = this;
	}

	hasParent(): boolean {
		return this.props.subtypeOf !== undefined;
	}
}