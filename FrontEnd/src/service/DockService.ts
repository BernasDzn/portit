import { inject, injectable } from 'inversify';
import { TYPES } from '@/inversify/types';

import type { Dock } from '@/model/Dock';
import type { IDockService } from './IService/IDockService';
import type { IHttpService } from './IService/IHttpService';
import type { Filter, Page } from '@/model/Page';
import type { DockDto, DockFilter } from '@/model/dto/DockDto';

@injectable()
export class DockService implements IDockService {

    constructor(
        @inject(TYPES.api)
        private http: IHttpService
    ) { }

    async getDocks(filtering?: Filter<DockFilter>): Promise<Page<Dock>> {

        let query: string[] = [];

        if (filtering) {
            query.push(filtering.filter.dockName ? `DockName=${filtering.filter.dockName}&` : '');
            query.push(filtering.filter.location ? `Location=${filtering.filter.location}&` : '');
            query.push(filtering.filter.vesselTypeName ? `VesselTypeName=${filtering.filter.vesselTypeName}&` : '');
            query.push(filtering.pageNumber !== undefined ? `PageNumber=${filtering.pageNumber}&` : '');
            query.push(filtering.pageSize !== undefined ? `PageSize=${filtering.pageSize}` : '');
        }

        const res = await this.http.get<Page<Dock>>(`/api/Dock/filter${query.length ? `?${query.join('')}` : ''}`);

        return res.data;
    }

    async getDockByCode(code: string): Promise<Dock | undefined> {
        const res = await this.http.get<Dock>(`/api/Dock/${code}`);
        return res.data;
    }

    async createDock(dock: Dock): Promise<Dock> {
        const res = await this.http.post<Dock>('/api/Dock', dock.toDto());
        return res.data;
    }

    async updateDock(dock: Dock): Promise<Dock> {
        const res = await this.http.put<Dock>(`/api/Dock/${dock.code}`, dock.toDto());
        return res.data;
    }

    async getNumberOfDocks(): Promise<number> {
        const res = await this.http.get<number>(`/api/Dock/count`);
        return res.data;
    }
}