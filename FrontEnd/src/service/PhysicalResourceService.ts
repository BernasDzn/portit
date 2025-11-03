import {inject, injectable} from 'inversify';
import { TYPES } from '@/inversify/types';

import type { IHttpService } from './IService/IHttpService';
import type { Filter, Page } from '@/model/Page';
import type { IPhysicalResourceService } from './IService/IPhysicalResourceService';
import type { PhysicalResourceFilter, PhysicalResource } from '@/model/PhysicalResource';

@injectable()
export class PhysicalResourceService implements IPhysicalResourceService {
    
	constructor(
		@inject(TYPES.api) 
		private http: IHttpService
	){}

    async getPhysicalResources(filtering?: Filter<PhysicalResourceFilter>): Promise<Page<any>> {
        
        let query: string[] = [];

        if (filtering) {
            query.push(filtering.filter.code ? `Code=${filtering.filter.code}&` : '');
            query.push(filtering.filter.description ? `Description=${filtering.filter.description}&` : '');
            query.push(filtering.filter.status !== undefined ? `Status=${filtering.filter.status}&` : '');
            query.push(filtering.filter.type !== undefined ? `Type=${filtering.filter.type}&` : '');
            query.push(filtering.pageNumber !== undefined ? `PageNumber=${filtering.pageNumber}&` : '');
            query.push(filtering.pageSize !== undefined ? `PageSize=${filtering.pageSize}` : '');
        }

        const res = await this.http.get<Page<PhysicalResource>>(`/PhysicalResource/filter${query.length ? `?${query.join('')}` : ''}`);
        return res.data;
    }

    async getPhysicalResourceById(id: string): Promise<PhysicalResource> {
        
        const res = await this.http.get<PhysicalResource>(`/PhysicalResource/${id}`);
        return res.data;
    }
    async addPhysicalResource(value: PhysicalResource): Promise<PhysicalResource> {
        
        const res = await this.http.post<PhysicalResource>('/PhysicalResource', value);
        return res.data;
    }
    async updatePhysicalResource(id: string, value: PhysicalResource): Promise<PhysicalResource> {
        
        const res = await this.http.put<PhysicalResource>(`/PhysicalResource/${id}`, value);
        return res.data;
    }
    async deactivatePhysicalResource(id: string): Promise<void> {
        
        const res = await this.http.delete<void>(`/PhysicalResource/${id}`);
        return res.data;
    }
}