import {inject, injectable} from 'inversify';
import { TYPES } from '@/inversify/types';

import type { StorageArea } from '@/model/StorageArea';
import type { IStorageAreaService } from './IService/IStorageAreaService';
import type { IHttpService } from './IService/IHttpService';
import type { Filter, Page } from '@/model/Page';
import type { StorageAreaDto } from '@/model/dto/StorageAreaDto';

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

	async getStorageAreaById(id: string): Promise<StorageArea> {
		const res = await this.http.get<StorageArea>(`/StorageArea/${id}`);
		return res.data;
	}

	async createStorageArea(storageArea: StorageAreaDto): Promise<StorageArea> {
		const res =  await this.http.post<StorageArea>('/StorageArea', storageArea);
		return res.data;
	}

	async updateStorageArea(storageArea: StorageAreaDto): Promise<StorageArea> {
		const res = await this.http.put<StorageArea>(`/StorageArea/${storageArea.nameCode}`, storageArea);
		return res.data;
	}

	async deleteStorageArea(id: string): Promise<void> {
		await this.http.delete<void>(`/StorageArea/${id}`);
	}

	async getNumberOfStorageAreas(): Promise<number> {
		const res = await this.http.get<number>(`/StorageArea/count`);
		return res.data;
	}

}
