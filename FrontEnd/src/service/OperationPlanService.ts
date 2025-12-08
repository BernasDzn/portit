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
    async groupOperationPlansByDate(): Promise<{ date: string; plans: any[] }[]> {
        const res = await this.http.get<{ date: string; plans: any[] }[]>(`/oem/operation-plans/by-date`);
        return res.data;
    }

    async getAllOperationPlans(filtering?: Filter<null>): Promise<Page<any>> {
        let query: string[] = [];
        
        if (filtering) {
            if (filtering.pageNumber !== undefined) {
                query.push(`pageNumber=${filtering.pageNumber}`); 
            }
            if (filtering.pageSize !== undefined) {
                query.push(`pageSize=${filtering.pageSize}`);
            }
        }
        
        const queryString = query.length ? `?${query.join('&')}` : '';
        const res = await this.http.get<Page<null>>(`/oem/operation-plans${queryString}`);
        return res.data;
    }
}