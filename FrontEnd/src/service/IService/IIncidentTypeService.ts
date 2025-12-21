import type { IncidentTypeDto, IncidentTypeFilter, PartialIncidentTypeDto } from '@/model/dto/IncidentTypeDto';
import type { IncidentType } from '@/model/IncidentType';
import type { Filter, Page } from '@/model/Page';

export interface IIncidentTypeService {
    getAllIncidentTypes(filtering?: Filter<IncidentTypeFilter>): Promise<Page<IncidentTypeDto>>;
    getIncidentTypeById(id: string): Promise<IncidentTypeDto | undefined>;
    createIncidentType(incidentType: PartialIncidentTypeDto): Promise<IncidentTypeDto>;
    updateIncidentType(id: string, incidentType: IncidentType): Promise<IncidentTypeDto>;
    count(): Promise<{count: number}>;
}
