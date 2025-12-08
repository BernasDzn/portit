import {inject, injectable} from 'inversify';
import { TYPES } from '@/inversify/types';

import type { IHttpService } from './IService/IHttpService';
import type { IOperationPlanService } from './IService/IOperationPlanService';
import type { Filter, Page } from '@/model/Page';
import type { OperationPlanDto } from '@/model/dto/OperationPlanDto';

@injectable()
export class OperationPlanService implements IOperationPlanService {
    
	constructor(
		@inject(TYPES.api) 
		private http: IHttpService
	){}
    async groupOperationPlansByDate(): Promise<{ date: string; plans: OperationPlanDto[] }[]> {
        const res = await this.http.get<{ date: string; plans: OperationPlanDto[] }[]>(`/oem/operation-plans/by-date`);
        return res.data;
    }

    async getOperationPlanById(id: string): Promise<OperationPlanDto> {
        const res = await this.http.get<OperationPlanDto>(`/oem/operation-plans/${id}`);
        return res.data;
    }

    async getAllOperationPlans(filtering?: Filter<null>): Promise<Page<OperationPlanDto>> {
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
        const res = await this.http.get<Page<OperationPlanDto>>(`/oem/operation-plans${queryString}`);
        return res.data;
    }
}