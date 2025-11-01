import {inject, injectable} from 'inversify';
import { TYPES } from '@/inversify/types';

import type { IHttpService } from './IService/IHttpService';
import type { IVesselService } from './IService/IVesselService';
import type { Vessel } from '@/model/Vessels';
import type { Filter, Page } from '@/model/Page';

@injectable()
export class VesselService implements IVesselService {
    
	constructor(
		@inject(TYPES.api) 
		private http: IHttpService
	){}

	async getVessels(filtering?: Filter<Vessel>): Promise<Page<Vessel>> {

        let query: string[] = [];

        if (filtering) {
            query.push(filtering.filter.name ? `Name=${filtering.filter.name}&` : '');
            query.push(filtering.filter.imo ? `ImoNumber=${filtering.filter.imo}&` : '');
            query.push(filtering.pageNumber !== undefined ? `PageNumber=${filtering.pageNumber}&` : '');
            query.push(filtering.pageSize !== undefined ? `PageSize=${filtering.pageSize}` : '');
        }

		const res = await this.http.get<Page<Vessel>>(`/Vessel/filter${query.length ? `?${query.join('')}` : ''}`);

		return res.data;
	}
    
}