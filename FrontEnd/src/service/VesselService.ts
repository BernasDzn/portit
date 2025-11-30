import {inject, injectable} from 'inversify';
import { TYPES } from '@/inversify/types';

import type { IHttpService } from './IService/IHttpService';
import type { IVesselService } from './IService/IVesselService';
import type { Vessel } from '@/model/Vessel';
import type { Filter, Page } from '@/model/Page';
import type { VesselDto, VesselFilter } from '@/model/dto/VesselDto';

@injectable()
export class VesselService implements IVesselService {
    
	constructor(
		@inject(TYPES.api) 
		private http: IHttpService
	){}
    getVesselByOwner(email: string): Promise<Vessel[]> {
        
        const res = this.http.get<Vessel[]>(`/Vessel/owner/${email}`);
        return res.then(response => response.data);
    }

	async getVessels(filtering?: Filter<VesselFilter>): Promise<Page<Vessel>> {

        let query: string[] = [];

        if (filtering) {
            query.push(filtering.filter.name ? `Name=${filtering.filter.name}&` : '');
            query.push(filtering.filter.imoNumber ? `ImoNumber=${filtering.filter.imoNumber}&` : '');
            query.push(filtering.filter.taxNumber ? `TaxNumber=${filtering.filter.taxNumber}&` : '');
            query.push(filtering.pageNumber !== undefined ? `PageNumber=${filtering.pageNumber}&` : '');
            query.push(filtering.pageSize !== undefined ? `PageSize=${filtering.pageSize}` : '');
        }

		const res = await this.http.get<Page<Vessel>>(`/api/Vessel/filter${query.length ? `?${query.join('')}` : ''}`);

		return res.data;
	}

	async createVessel(vessel: Vessel): Promise<Vessel> {
		const res =  await this.http.post<Vessel>('/api/Vessel', vessel.toDto());
		return res.data;
	}

	async getVesselByIMO(imo: string): Promise<Vessel> {
		const res = await this.http.get<Vessel>(`/api/Vessel/${imo}`);
		return res.data;
	}

	async updateVessel(vessel: Vessel): Promise<Vessel> {
		const res = await this.http.put<Vessel>(`/api/Vessel/${vessel.imoNumber}`, vessel.toDto());
		return res.data;
	}

	async getNumberOfVessels(): Promise<number> {
		const res = await this.http.get<number>(`/api/Vessel/count`);
		return res.data;
	}
    
}