import type { IncidentTypeCreateDto } from '@/model/dto/IncidentTypeDto';
import type { IncidentType, IncidentTypeDto, IncidentTypeFilter } from '@/model/IncidentType';
import type { Filter, Page } from '@/model/Page';

export interface IIncidentTypeService {
    getAllIncidentTypes(filtering?: Filter<IncidentTypeFilter>): Promise<Page<IncidentTypeDto>>;
    getIncidentTypeById(id: string): Promise<IncidentTypeDto | undefined>;
    createIncidentType(incidentType: IncidentTypeCreateDto): Promise<IncidentTypeDto>;
    updateIncidentType(id: string, incidentType: IncidentType): Promise<IncidentTypeDto>;
    deleteIncidentType(id: string): Promise<void>;
    count(): Promise<number>;
}
