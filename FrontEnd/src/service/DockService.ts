import {inject, injectable} from 'inversify';
import { TYPES } from '@/inversify/types';

import type { Dock } from '@/model/Dock';
import type { IDockService } from './IService/IDockService';
import type { IHttpService } from './IService/IHttpService';

@injectable()
export class DockService implements IDockService {
    
	constructor(
		@inject(TYPES.api) private http: IHttpService
	){}

	async getDocks(): Promise<Dock[]> {
		const res = await this.http.get<Dock[]>('/Dock');
		return res.data;
	}

}