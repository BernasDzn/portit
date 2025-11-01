import {inject, injectable} from 'inversify';
import { TYPES } from '@/inversify/types';

import type { Dock, DockFilter } from '@/model/Dock';
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

    async createDock(dock: Dock): Promise<Dock> {
        const res = await this.http.post<Dock>('/Dock', dock);
        return res.data;
    }

    async updateDock(code: string, dock: Dock): Promise<Dock> {
        const res = await this.http.put<Dock>(`/Dock/${code}`, dock);
        return res.data;
    }

    async filterDocks(filter: DockFilter): Promise<Dock[]> {
        const res = await this.http.post<Dock[]>('/Dock/filter', filter);
        return res.data;
    }
}