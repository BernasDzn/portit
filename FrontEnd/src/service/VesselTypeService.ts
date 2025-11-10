import { inject, injectable } from 'inversify';
import { TYPES } from '@/inversify/types';

import type { VesselType } from '@/model/VesselType';
import type { IVesselTypeService } from './IService/IVesselTypeService';
import type { IHttpService } from './IService/IHttpService';
import type { Filter, Page } from '@/model/Page';

@injectable()
export class VesselTypeService implements IVesselTypeService {

    constructor(
        @inject(TYPES.api)
        private http: IHttpService
    ) { }

    async getVesselTypes(filtering?: Filter<VesselType>): Promise<Page<VesselType>> {

        let query: string[] = [];

        if (filtering) {
            query.push(filtering.filter.name ? `Name=${filtering.filter.name}&` : '');
            query.push(filtering.filter.description ? `Description=${filtering.filter.description}&` : '');
            query.push(filtering.pageNumber !== undefined ? `PageNumber=${filtering.pageNumber}&` : '');
            query.push(filtering.pageSize !== undefined ? `PageSize=${filtering.pageSize}` : '');
        }

        const res = await this.http.get<Page<VesselType>>(`/VesselType/filter${query.length ? `?${query.join('')}` : ''}`);

        return res.data;
    }

    async getVesselTypeByName(name: string): Promise<VesselType | undefined> {
        const res = await this.http.get<VesselType>(`/VesselType/${name}`);
        return res.data;
    }

    async createVesselType(vesselType: VesselType): Promise<VesselType> {
        const res = await this.http.post<VesselType>('/VesselType', vesselType);
        return res.data;
    }

    async updateVesselType(name: string, vesselType: VesselType): Promise<VesselType> {
        const res = await this.http.put<VesselType>(`/VesselType/${name}`, vesselType);
        return res.data;
    }

    async getNumberOfVesselTypes(): Promise<number> {
        const res = await this.http.get<number>(`/VesselType/count`);
        return res.data;
    }
}