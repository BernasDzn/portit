import type { IncidentTypeDto } from "./dto/IncidentTypeDto";

export type Severity = 'Minor' | 'Major' | 'Critical';

export class IncidentType{
	bid : string;
    name : string;
    description : string;
    severity : Severity;
    subtypeOf : IncidentType | undefined;
    subtypes : IncidentType[] | undefined;

    constructor(
        id: string,
        name: string,
        description: string,
        severity: Severity,
        subtypeOf?: IncidentType,
        subtypes?: IncidentType[]
    ){
        this.bid = id;
        this.name = name;
        this.description = description;
        this.severity = severity;
        this.subtypeOf = subtypeOf;
        this.subtypes = subtypes;
    }
 
	validate(){
		if(this.subtypeOf && this.subtypeOf.bid === this.bid){
			throw new Error("An Incident Type cannot be a subtype of itself. Incident Type id: " + this.bid);
		}
	}

	addSubtype(subtype: IncidentType) {
		if (!this.subtypes) {
			this.subtypes = [];
		}
		this.subtypes.push(subtype);
		subtype.subtypeOf = this;
	}

	hasParent(): boolean {
		return this.subtypeOf !== undefined;
	}

    toDto(): IncidentTypeDto {
        return {
            bid: this.bid,
            name: this.name,
            description: this.description,
            severity: this.severity,
            subtypeOf: this.subtypeOf ? this.subtypeOf.bid : undefined,
            subtypes: this.subtypes ? this.subtypes.map(subtype => subtype.bid) : undefined
        };
    }

}