

// export interface CreateIncidentDto {
// 	type: string; 
// 	startTime: string; 
// 	severity: string;
// 	description: string;
// }

import type { IncidentTypeDto } from "./IncidentTypeDto";
import type { VesselVisitExecutionDto } from "./VesselVisitExecutionDto";


export interface IncidentDto {
    bid: string;
    type: IncidentTypeDto;
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

export interface IncidentFilter {
    vveCode?: string;
    severity?: string;
    isResolved?: boolean;
    filterStartTime?: string;
    filterEndTime?: string;
}