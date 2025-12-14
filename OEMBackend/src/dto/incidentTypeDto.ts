import { Severity } from "../domain/incidentType";


export interface IncidentTypeDto {
	id: string;
	name: string;
	description: string;
	severity: Severity;
	subtypeOfId?: string | undefined;
	subtypesIds?: string[] | undefined;
}