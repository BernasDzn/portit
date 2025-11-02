import {inject, injectable} from 'inversify';
import { TYPES } from '@/inversify/types';

import type { IHttpService } from './IService/IHttpService';
import type { IQualificationService } from './IService/IQualificationService';
import type { Qualification } from '@/model/Qualifications';
import type { Filter, Page } from '@/model/Page';

@injectable()
export class QualificationService implements IQualificationService {
    
	constructor(
		@inject(TYPES.api) 
		private http: IHttpService
	){}

    async updateQualification(id: string, value: Qualification): Promise<Qualification> {
        const res = await this.http.put<Qualification>(`/Qualification/${id}`, value);
        return res.data;
    }

    async addQualification(value: Qualification): Promise<Qualification> {
        
        const res = await this.http.post<Qualification>('/Qualification', value);
        return res.data;
    }

	async getQualifications(filtering?: Filter<Qualification>): Promise<Page<Qualification>> {

        let query: string[] = [];

        if (filtering) {
            query.push(filtering.filter.idCode ? `Code=${filtering.filter.idCode}&` : '');
            query.push(filtering.filter.qualificationName ? `QualificationName=${filtering.filter.qualificationName}&` : '');
            query.push(filtering.pageNumber !== undefined ? `PageNumber=${filtering.pageNumber}&` : '');
            query.push(filtering.pageSize !== undefined ? `PageSize=${filtering.pageSize}` : '');
        }

		const res = await this.http.get<Page<Qualification>>(`/Qualification/filter${query.length ? `?${query.join('')}` : ''}`);

		return res.data;
	}
    
    async getQualificationById(id: string): Promise<Qualification> {
        const res = await this.http.get<Qualification>(`/Qualification/${id}`);
        return res.data;
    }
}