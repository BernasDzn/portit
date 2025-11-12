import {inject, injectable} from 'inversify';
import { TYPES } from '@/inversify/types';

import type { IHttpService } from './IService/IHttpService';
import type { Filter, Page } from '@/model/Page';
import type { IPhysicalResourceService } from './IService/IPhysicalResourceService';
import type { PhysicalResourceFilter, PhysicalResource, STSCrane, YardCrane, Truck } from '@/model/PhysicalResource';
import type { STSCraneDto, TruckDto, YardCraneDto } from '@/model/dto/PhysicalResourceDto';

@injectable()
export class PhysicalResourceService implements IPhysicalResourceService {
    
	constructor(
		@inject(TYPES.api) 
		private http: IHttpService
	){}

    async getPhysicalResources(filtering?: Filter<PhysicalResourceFilter>): Promise<Page<any>> {
        
        let query: string[] = [];

        if (filtering) {
            query.push(filtering.filter.Code ? `Code=${filtering.filter.Code}&` : '');
            query.push(filtering.filter.Description ? `Description=${filtering.filter.Description}&` : '');
            query.push(filtering.filter.Status !== undefined ? `Status=${filtering.filter.Status}&` : '');
            query.push(filtering.filter.Type !== undefined ? `Type=${filtering.filter.Type}&` : '');
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

    async deactivatePhysicalResource(id: string): Promise<void> {
        
        const res = await this.http.delete<void>(`/PhysicalResource/${id}`);
        return res.data;
    }

    async addSTSCrane(value: STSCraneDto): Promise<STSCrane> {
        // console.log('Adding STS Crane:', JSON.stringify(value));
        const res = await this.http.post<PhysicalResource>(`/PhysicalResource/AddSTSCrane`, value);
        return res.data as STSCrane;
    }

    async updateSTSCrane(value: STSCraneDto): Promise<STSCrane> {
        const res = await this.http.put<PhysicalResource>(`/PhysicalResource/UpdateSTSCrane/${value.code}`, value);
        return res.data as STSCrane;
    }

    async addYardCrane(value: YardCraneDto): Promise<YardCrane> {
        const res = await this.http.post<PhysicalResource>(`/PhysicalResource/AddYardCrane`, value);
        return res.data as YardCrane;
    }

    async updateYardCrane(value: YardCraneDto): Promise<YardCrane> {
        const res = await this.http.put<PhysicalResource>(`/PhysicalResource/UpdateYardCrane/${value.code}`, value);
        return res.data as YardCrane;
    }

    async addTruck(value: TruckDto): Promise<Truck> {
        const res = await this.http.post<PhysicalResource>(`/PhysicalResource/AddTruck`, value);
        return res.data as Truck;
    }

    async updateTruck(value: TruckDto): Promise<Truck> {
        const res = await this.http.put<PhysicalResource>(`/PhysicalResource/UpdateTruck/${value.code}`, value);
        return res.data as Truck;
    }

    async getNumberOfPhysicalResources(): Promise<number> {
        const res = await this.http.get<number>(`/PhysicalResource/count`);
        return res.data;
    }
}