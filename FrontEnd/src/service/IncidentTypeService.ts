import { inject, injectable } from 'inversify';
import { TYPES } from '@/inversify/types';
import type { IHttpService } from './IService/IHttpService';
import type { IIncidentTypeService } from './IService/IIncidentTypeService';
import { IncidentType, type IncidentTypeDto, type IncidentTypeFilter } from '@/model/IncidentType';
import type { Filter, Page } from '@/model/Page';
import type { IncidentTypeCreateDto } from '@/model/dto/IncidentTypeDto';

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
            query.push(filtering.filter.parentId ? `parentId=${filtering.filter.parentId}&` : '');
            query.push(filtering.pageNumber !== undefined ? `pageNumber=${filtering.pageNumber}&` : '');
            query.push(filtering.pageSize !== undefined ? `pageSize=${filtering.pageSize}` : '');
        }else{
            query.push(`pageSize=1000`);
        }
        
        const res = await this.http.get<Page<IncidentTypeDto>>(`/oem/incident-types${query.length ? `?${query.join('')}` : ''}`);
        return res.data;
    }

    async getIncidentTypeById(id: string): Promise<IncidentTypeDto | undefined> {
        const res = await this.http.get<IncidentTypeDto>(`/oem/incident-types/${id}`);
        return res.data ? IncidentType.fromDto(res.data) : undefined;
    }

    async createIncidentType(incidentType: IncidentTypeCreateDto): Promise<IncidentTypeDto> {
        const res = await this.http.post<IncidentTypeDto>('/oem/incident-types', incidentType);
        return res.data;
    }

    async updateIncidentType(id: string, incidentType: IncidentType): Promise<IncidentTypeDto> {
        const res = await this.http.patch<IncidentTypeDto>(`/oem/incident-types/${id}`, incidentType.toDto());
        return res.data;
    }

    async deleteIncidentType(id: string): Promise<void> {
        await this.http.delete(`/oem/incident-types/${id}`);
    }

    async count(): Promise<{count: number}> {
        const res = await this.http.get<{count: number}>('/oem/incident-types/count');
        return res.data;
    }
}
