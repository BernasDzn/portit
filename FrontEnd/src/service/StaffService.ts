import {inject, injectable} from 'inversify';
import { TYPES } from '@/inversify/types';

import type { Staff } from '@/model/Staff';
import type { IStaffService } from './IService/IStaffService';
import type { IHttpService } from './IService/IHttpService';

@injectable()
export class StaffService implements IStaffService {
    
	constructor(
		@inject(TYPES.api) private http: IHttpService
	){}

	async getStaffs(): Promise<Staff[]> {
		const res = await this.http.get<Staff[]>('/Staff');
		return res.data;
	}
    
}