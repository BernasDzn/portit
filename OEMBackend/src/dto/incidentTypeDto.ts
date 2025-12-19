export interface IncidentTypeDto {
	id: string;
	name: string;
	description: string;
	severity: string;
	subtypeOf: string | undefined;
	subtypes: string[] | undefined;
}

export interface IncidentTypeCreateDto {
	name: string;
	description: string;
	severity: string;
	subtypeOf?: string;
	subtypesIds?: string[];
}