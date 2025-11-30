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

    async getAllOperationPlans(): Promise<any> {
        const res = await this.http.get<any>(`/oem/plans`);
        return res.data;
    }
}