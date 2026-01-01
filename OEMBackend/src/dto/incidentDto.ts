import { IncidentTypeDto } from "./incidentTypeDto";
import { VesselVisitExecutionDto } from "./vesselVisitExecutionDto";

export interface IncidentDto {
	bid: string;
	type: IncidentTypeDto; // incident type bid
	startTime: string; 
	endTime?: string;
	severity: string;
	description: string;
	createdBy: string;
	affectedVVECodes?: VesselVisitExecutionDto[];
}

export interface CreateIncidentDto {
	type: string; 
	startTime: string; 
	severity: string;
	description: string;
}

export interface UpdateIncidentDto {
	type: string; 
	startTime: string; 
	endTime?: string;
	severity: string;
	description: string;
	affectedVVECodes?: string[];
}