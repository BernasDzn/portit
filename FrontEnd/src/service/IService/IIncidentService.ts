import type { Filter, Page } from '@/model/Page';
import type { IncidentDto, CreateIncidentDto, UpdateIncidentDto, IncidentFilter } from '@/model/dto/IncidentDto';

export interface IIncidentService {
    getAllIncidents(filtering?: Filter<IncidentFilter>): Promise<Page<IncidentDto>>;
    getIncidentByBid(bid: string): Promise<IncidentDto | undefined>;
    createIncident(incident: CreateIncidentDto): Promise<IncidentDto>;
    updateIncident(bid: string, incident: UpdateIncidentDto): Promise<IncidentDto>;
    deleteIncident(bid: string): Promise<void>;
    count(): Promise<{count: number}>;
}