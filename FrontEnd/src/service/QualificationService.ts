import {inject, injectable} from 'inversify';
import { TYPES } from '@/inversify/types';

import type { IHttpService } from './IService/IHttpService';
import type { IQualificationService } from './IService/IQualificationService';
import { Qualification } from '@/model/Qualifications';
import { Page, type Filter } from '@/model/Page';
import type { QualificationDto } from '@/model/dto/QualificationDto';

@injectable()
export class QualificationService implements IQualificationService {
    
	constructor(
		@inject(TYPES.api) 
		private http: IHttpService
	){}

    async updateQualification(value: QualificationDto): Promise<Qualification> {
        const res = await this.http.put<Qualification>(`/Qualification/${value.idCode}`, value);
        return res.data;
    }

    async addQualification(value: QualificationDto): Promise<Qualification> {
        
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

    async getNumberOfQualifications(): Promise<number> {
        const res = await this.http.get<number>(`/Qualification/count`);
        return res.data;
    }
}