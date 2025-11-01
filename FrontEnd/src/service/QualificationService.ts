import {inject, injectable} from 'inversify';
import { TYPES } from '@/inversify/types';

import type { IHttpService } from './IService/IHttpService';
import type { IQualificationService } from './IService/IQualificationService';
import type { Qualification } from '@/model/Qualifications';
import type { Page } from '@/model/Page';

@injectable()
export class QualificationService implements IQualificationService {
    
	constructor(
		@inject(TYPES.api) 
		private http: IHttpService
	){}

	async getQualifications(filtering?: Qualification): Promise<Page<Qualification>> {

        let query: string[] = [];

        if (filtering) {
            query.push(filtering.idCode ? `Code=${filtering.idCode}&` : '');
            query.push(filtering.qualificationName ? `QualificationName=${filtering.qualificationName}&` : '');
        }

		const res = await this.http.get<Page<Qualification>>(`/Qualification/filter${query.length ? `?${query.join('')}` : ''}`);
		return res.data;
	}
    
}