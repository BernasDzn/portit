import { inject, injectable } from 'inversify';
import { TYPES } from '@/inversify/types';

import type { Dock, DockFilter } from '@/model/Dock';
import type { IDockService } from './IService/IDockService';
import type { IHttpService } from './IService/IHttpService';
import type { Filter, Page } from '@/model/Page';

@injectable()
export class DockService implements IDockService {

    constructor(
        @inject(TYPES.api)
        private http: IHttpService
    ) { }

    async getDocks(filtering?: Filter<DockFilter>): Promise<Page<Dock>> {

        let query: string[] = [];

        if (filtering) {
            query.push(filtering.filter.name ? `Name=${filtering.filter.name}&` : '');
            query.push(filtering.filter.location ? `Location=${filtering.filter.location}&` : '');
            query.push(filtering.filter.vesselTypeName ? `Type=${filtering.filter.vesselTypeName}&` : '');
            query.push(filtering.pageNumber !== undefined ? `PageNumber=${filtering.pageNumber}&` : '');
            query.push(filtering.pageSize !== undefined ? `PageSize=${filtering.pageSize}` : '');
        }

        const res = await this.http.get<Page<Dock>>(`/Dock/filter${query.length ? `?${query.join('')}` : ''}`);

        return res.data;
    }

    async createDock(dock: Dock): Promise<Dock> {
        const res = await this.http.post<Dock>('/Dock', dock);
        return res.data;
    }

    async updateDock(code: string, dock: Dock): Promise<Dock> {
        const res = await this.http.put<Dock>(`/Dock/${code}`, dock);
        return res.data;
    }
}