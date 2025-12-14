export interface IncidentTypeDto {
	id: string;
	name: string;
	subtypeOfId?: string | undefined;
	subtypesIds?: string[] | undefined;
}