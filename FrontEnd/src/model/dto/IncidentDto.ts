

// export interface CreateIncidentDto {
// 	type: string; 
// 	startTime: string; 
// 	severity: string;
// 	description: string;
// }


export interface IncidentDto {
    bid: string;
    type: string; // incident type bid
    startTime: string; 
    endTime?: string;
    severity: string;
    description: string;
    createdBy: string;
    affectedVVECodes?: string[];
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