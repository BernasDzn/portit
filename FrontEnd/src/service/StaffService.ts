import {inject, injectable} from 'inversify';
import { TYPES } from '@/inversify/types';

import type { Staff } from '@/model/Staff';
import type { IStaffService } from './IService/IStaffService';
import type { IHttpService } from './IService/IHttpService';
import type { Filter, Page } from '@/model/Page';

@injectable()
export class StaffService implements IStaffService {
    
	constructor(
		@inject(TYPES.api) 
		private http: IHttpService
	){}

	async getStaffs(filtering?: Filter<Staff>): Promise<Page<Staff>> {
		
		let query: string[] = [];

		if (filtering) {
			query.push(filtering.filter.mechanographicNumber ? `MechanographicNumber=${filtering.filter.mechanographicNumber}&` : '');
			query.push(filtering.filter.name ? `Name=${filtering.filter.name}&` : '');
			query.push(filtering.pageNumber !== undefined ? `PageNumber=${filtering.pageNumber}&` : '');
			query.push(filtering.pageSize !== undefined ? `PageSize=${filtering.pageSize}` : '');
		}

		const res = await this.http.get<Page<Staff>>(`/Staff/filter${query.length ? `?${query.join('')}` : ''}`);
		
		return res.data;
	}

	async getStaffByMechanographicNumber(mechanographicNumber: string): Promise<Staff | undefined> {
		const res = await this.http.get<Page<Staff>>(`/Staff/filter?MechanographicNumber=${mechanographicNumber}`);
		let staff = res.data.items[0];
		return staff;
	}

	async createStaff(staff: Staff): Promise<Staff> {
		const res =  await this.http.post<Staff>('/Staff', staff);
		return res.data;
	}

	async deactivateStaff(mechanographicNumber: string): Promise<void> {
		await this.http.delete<void>(`/Staff/${mechanographicNumber}`);
	}

	async updateStaff(mechanographicNumber: string, staff: Staff): Promise<Staff> {
		const res = await this.http.put<Staff>(`/Staff/${mechanographicNumber}`, staff);
		return res.data;
	}
    
}