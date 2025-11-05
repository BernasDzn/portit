import {inject, injectable} from 'inversify';
import { TYPES } from '@/inversify/types';

import type { StorageArea } from '@/model/StorageArea';
import type { IStorageAreaService } from './IService/IStorageAreaService';
import type { IHttpService } from './IService/IHttpService';
import type { Filter, Page } from '@/model/Page';

@injectable()
export class StorageAreaService implements IStorageAreaService {
	
	constructor(
		@inject(TYPES.api) 
		private http: IHttpService
	){}

	async getStorageAreas(filtering?: Filter<{ nameCode: string }>): Promise<Page<StorageArea>> {
		let query: string[] = [];
		
		if (filtering) {
			query.push(filtering.filter.nameCode ? `NameCode=${filtering.filter.nameCode}&` : '');
		}

		const res = await this.http.get<Page<StorageArea>>(`/StorageArea/filter${query.length ? `?${query.join('')}` : ''}`);

		return res.data;
	}

	async getStorageAreaById(id: number): Promise<StorageArea | undefined> {
		const res = await this.http.get<StorageArea>(`/StorageArea/${id}`);
		return res.data;
	}

	async createStorageArea(storageArea: StorageArea): Promise<StorageArea> {
		const res =  await this.http.post<StorageArea>('/StorageArea', storageArea);
		return res.data;
	}

	async updateStorageArea(id: number, storageArea: StorageArea): Promise<StorageArea> {
		const res = await this.http.put<StorageArea>(`/StorageArea/${id}`, storageArea);
		return res.data;
	}

	async deleteStorageArea(id: number): Promise<void> {
		await this.http.delete<void>(`/StorageArea/${id}`);
	}

}
