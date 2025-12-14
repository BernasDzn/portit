import type { IncidentType } from '@/model/IncidentType';

export interface IIncidentTypeService {
    getAllIncidentTypes(): Promise<IncidentType[]>;
    getIncidentTypeById(id: string): Promise<IncidentType | undefined>;
    createIncidentType(incidentType: IncidentType): Promise<IncidentType>;
    updateIncidentType(id: string, incidentType: IncidentType): Promise<IncidentType>;
    deleteIncidentType(id: string): Promise<void>;
}
