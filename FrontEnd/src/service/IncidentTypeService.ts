import { inject, injectable } from 'inversify';
import { TYPES } from '@/inversify/types';
import type { IHttpService } from './IService/IHttpService';
import type { IIncidentTypeService } from './IService/IIncidentTypeService';
import { IncidentType } from '@/model/IncidentType';
import type { Filter, Page } from '@/model/Page';
import type { IncidentTypeDto, IncidentTypeFilter, PartialIncidentTypeDto } from '@/model/dto/IncidentTypeDto';

@injectable()
export class IncidentTypeService implements IIncidentTypeService {

    constructor(
        @inject(TYPES.api)
        private http: IHttpService
    ) { }

    async getAllIncidentTypes(filtering?: Filter<IncidentTypeFilter>): Promise<Page<IncidentTypeDto>> {
        let query: string[] = [];
        if (filtering) {
            query.push(filtering.filter.name ? `name=${filtering.filter.name}&` : '');
            query.push(filtering.filter.severity ? `severity=${filtering.filter.severity}&` : '');
            query.push(filtering.filter.subtypeOfId ? `subtypeOfId=${filtering.filter.subtypeOfId}&` : '');
            query.push(filtering.pageNumber !== undefined ? `pageNumber=${filtering.pageNumber}&` : '');
            query.push(filtering.pageSize !== undefined ? `pageSize=${filtering.pageSize}` : '');
        }else{
            query.push(`pageSize=1000`);
        }
        
        const res = await this.http.get<Page<IncidentTypeDto>>(`/oem/incident-types${query.length ? `?${query.join('')}` : ''}`);
        return res.data;
    }

    async getIncidentTypeById(bid: string): Promise<IncidentTypeDto | undefined> {
        const res = await this.http.get<IncidentTypeDto>(`/oem/incident-types/${bid}`);
        return res.data;
    }

    async createIncidentType(incidentType: PartialIncidentTypeDto): Promise<IncidentTypeDto> {
        const res = await this.http.post<IncidentTypeDto>('/oem/incident-types', incidentType);
        return res.data;
    }

    async updateIncidentType(bid: string, incidentType: IncidentType): Promise<IncidentTypeDto> {
        const res = await this.http.patch<IncidentTypeDto>(`/oem/incident-types/${bid}`, incidentType.toDto());
        return res.data;
    }

    async deleteIncidentType(bid: string): Promise<void> {
        await this.http.delete(`/oem/incident-types/${bid}`);
    }

    async count(): Promise<{count: number}> {
        const res = await this.http.get<{count: number}>('/oem/incident-types/count');
        return res.data;
    }
}
