import mongoose from "mongoose";
import { Entity } from "../core/domain/entity";

export type Severity = 'Minor' | 'Major' | 'Critical';

export interface IncidentTypeProps {
	bid?: string;
	name: string;
	description: string;
	severity: Severity;
	subtypeOf?: IncidentType;
	subtypes?: IncidentType[];
}
 
export default class IncidentType extends Entity<IncidentTypeProps> {
	get bid(): string { return this.props.bid ? this.props.bid : ""; }
	get name(): string { return this.props.name; }
	get description(): string { return this.props.description; }
	get severity(): Severity { return this.props.severity; }
	get subtypeOf(): IncidentType | undefined { return this.props.subtypeOf; }
	get subtypes(): IncidentType[] | undefined { return this.props.subtypes; }

	constructor(props: IncidentTypeProps, _id?: mongoose.Types.ObjectId) {
		super(
			props,
			_id
		);
		if(!props.bid) props.bid = this.newUniqueIncidentTypeId();
		this.validate();
	}

	private newUniqueIncidentTypeId() : string{
		const timestamp = Date.now().toString(16).substring(4, 7).toUpperCase();
		const random = Math.random().toString(16).substring(2, 8).toUpperCase();
		return "INC-" + timestamp + random;
	}
 
	validate(){
		if(this.props.subtypeOf && this.props.subtypeOf.bid === this.bid){
			throw new Error("An Incident Type cannot be a subtype of itself. Incident Type id: " + this.bid);
		}
		if(this.props.subtypes && this.props.subtypes.some(subtype => subtype.bid === this.bid)){
			throw new Error("An Incident Type cannot have itself as a subtype. Incident Type id: " + this.bid);
		}
		// Additional validations can be added here if needed
		// Since the circular dependency is a critical check, I only did that for now.
	}

	addSubtype(subtype: IncidentType) {
		if (!this.props.subtypes) {
			this.props.subtypes = [];
		}
		this.props.subtypes.push(subtype);
		subtype.props.subtypeOf = this;
		this.validate();
	}

	setSubtypeOf(parentType: IncidentType) {
		this.props.subtypeOf = parentType;
		this.validate();
	}

	hasParent(): boolean {
		return this.props.subtypeOf !== undefined;
	}
}