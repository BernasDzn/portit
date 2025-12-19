export type Severity = 'Minor' | 'Major' | 'Critical';

export class IncidentType{
	id : string;
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
        this.id = id;
        this.name = name;
        this.description = description;
        this.severity = severity;
        this.subtypeOf = subtypeOf;
        this.subtypes = subtypes;
    }
 
	validate(){
		if(this.subtypeOf && this.subtypeOf.id === this.id){
			throw new Error("An Incident Type cannot be a subtype of itself. Incident Type id: " + this.id);
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
}