import {inject, injectable} from 'inversify';
import { TYPES } from '@/inversify/types';

import type { IHttpService } from './IService/IHttpService';
import type { IOperationPlanService } from './IService/IOperationPlanService';

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

    async getAllOperationPlans(): Promise<any> {
        const res = await this.http.get<any>(`/oem/plans`);
        return res.data;
    }
}