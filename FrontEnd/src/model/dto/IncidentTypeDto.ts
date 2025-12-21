export interface IncidentTypeFilter {
	bid?: string;
	name?: string;
	severity?: string;
	subtypeOfId?: string;
}

export interface IncidentTypeDto {
	bid: string;
	name: string;
	description: string;
	severity: string;
	subtypeOf: string | undefined;
	subtypes: string[] | undefined;
}

// create and update must use this dto to send data to backend
export interface PartialIncidentTypeDto {
	name: string;
	description: string;
	severity: string;
	subtypeOf?: string;
}