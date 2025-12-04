import {inject, injectable} from 'inversify';
import { TYPES } from '@/inversify/types';

import type { IHttpService } from './IService/IHttpService';
import type { IOperationPlanService } from './IService/IOperationPlanService';
import type { Filter, Page } from '@/model/Page';

@injectable()
export class OperationPlanService implements IOperationPlanService {
    
	constructor(
		@inject(TYPES.api) 
		private http: IHttpService
	){}
    async groupOperationPlansByDate(): Promise<{ date: string; count: number; }[]> {
        const res = await this.http.get<{ date: string; count: number; }[]>(`/oem/plans/group`);
        return res.data;
    }

    async getAllOperationPlans(filtering?: Filter<null>): Promise<Page<any>> {
        let query: string[] = [];
        
        if (filtering) {
            query.push(filtering.pageNumber !== undefined ? `pageNumber=${filtering.pageNumber}&` : '');
            query.push(filtering.pageSize !== undefined ? `pageSize=${filtering.pageSize}` : '');
        }
        
        const res = await this.http.get<Page<null>>(`/oem/plans${query.length ? `?${query.join('')}` : ''}`);
        return res.data;
    }
}