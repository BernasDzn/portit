import { inject, injectable } from 'inversify';
import { TYPES } from '@/inversify/types';
import type { IHttpService } from './IService/IHttpService';
import type { IIncidentService } from './IService/IIncidentService';
import type { Filter, Page } from '@/model/Page';
import type { IncidentDto, CreateIncidentDto, UpdateIncidentDto, IncidentFilter } from '@/model/dto/IncidentDto';

@injectable()
export class IncidentService implements IIncidentService {
    constructor(
        @inject(TYPES.api)
        private http: IHttpService
    ) { }

    async getAllIncidents(filtering?: Filter<IncidentFilter>): Promise<Page<IncidentDto>> {
        let query: string[] = [];
        if (filtering) {
            query.push(filtering.filter.vveCode ? `vveCode=${filtering.filter.vveCode}&` : '');
            query.push(filtering.filter.severity ? `severity=${filtering.filter.severity}&` : '');
            query.push(filtering.filter.isResolved !== undefined ? `isResolved=${filtering.filter.isResolved}&` : '');
            query.push(filtering.filter.filterStartTime ? `filterStartTime=${filtering.filter.filterStartTime}&` : '');
            query.push(filtering.filter.filterEndTime ? `filterEndTime=${filtering.filter.filterEndTime}&` : '');
            query.push(filtering.pageNumber !== undefined ? `pageNumber=${filtering.pageNumber}&` : '');
            query.push(filtering.pageSize !== undefined ? `pageSize=${filtering.pageSize}` : '');
        } else {
            query.push('pageSize=1000');
        }
        
        const res = await this.http.get<Page<IncidentDto>>(`/oem/incidents${query.length ? `?${query.join('')}` : ''}`);
        return res.data;
    }

    async getIncidentByBid(bid: string): Promise<IncidentDto | undefined> {
        const res = await this.http.get<IncidentDto>(`/oem/incidents/${bid}`);
        return res.data;
    }

    async createIncident(incident: CreateIncidentDto): Promise<IncidentDto> {
        const res = await this.http.post<IncidentDto>('/oem/incidents', incident);
        return res.data;
    }

    async updateIncident(bid: string, incident: UpdateIncidentDto): Promise<IncidentDto> {
        const res = await this.http.put<IncidentDto>(`/oem/incidents/${bid}`, incident);
        return res.data;
    }

    async deleteIncident(bid: string): Promise<void> {
        await this.http.delete(`/oem/incidents/${bid}`);
    }

    async count(): Promise<{count: number}> {
        const res = await this.http.get<{count: number}>('/oem/incidents/count');
        return res.data;
    }
}