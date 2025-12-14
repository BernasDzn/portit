import { inject, injectable } from 'inversify';
import { TYPES } from '@/inversify/types';
import type { IHttpService } from './IService/IHttpService';
import type { IIncidentTypeService } from './IService/IIncidentTypeService';
import { IncidentType, type IncidentTypeDto } from '@/model/IncidentType';

@injectable()
export class IncidentTypeService implements IIncidentTypeService {

    constructor(
        @inject(TYPES.api)
        private http: IHttpService
    ) { }

    async getAllIncidentTypes(): Promise<IncidentType[]> {
        const res = await this.http.get<IncidentTypeDto[]>('/oem/incident-types');
        const data = Array.isArray(res.data) ? res.data : [];
        return data.map(dto => IncidentType.fromDto(dto));
    }

    async getIncidentTypeById(id: string): Promise<IncidentType | undefined> {
        const res = await this.http.get<IncidentTypeDto>(`/oem/incident-types/${id}`);
        return res.data ? IncidentType.fromDto(res.data) : undefined;
    }

    async createIncidentType(incidentType: IncidentType): Promise<IncidentType> {
        const res = await this.http.post<IncidentTypeDto>('/oem/incident-types', incidentType.toDto());
        return IncidentType.fromDto(res.data);
    }

    async updateIncidentType(id: string, incidentType: IncidentType): Promise<IncidentType> {
        const res = await this.http.patch<IncidentTypeDto>(`/oem/incident-types/${id}`, incidentType.toDto());
        return IncidentType.fromDto(res.data);
    }

    async deleteIncidentType(id: string): Promise<void> {
        await this.http.delete(`/oem/incident-types/${id}`);
    }
}
