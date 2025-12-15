export interface IncidentTypeDto {
	id: string;
	name: string;
	description: string;
	severity: string;
	subtypeOfId?: string | undefined;
	subtypesIds?: string[] | undefined;
}