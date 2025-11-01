import {inject, injectable} from 'inversify';
import { TYPES } from '@/inversify/types';

import type { IHttpService } from './IService/IHttpService';
import type { IQualificationService } from './IService/IQualificationService';
import type { Qualification } from '@/model/Qualifications';

@injectable()
export class QualificationService implements IQualificationService {
    
	constructor(
		@inject(TYPES.api) 
		private http: IHttpService
	){}

	async getQualifications(): Promise<Qualification[]> {
		const res = await this.http.get<Qualification[]>('/Qualification');
		return res.data;
	}
    
}