import type { StorageAreaDto } from "@/model/dto/StorageAreaDto";
import type { Filter, Page } from "@/model/Page";
import type { StorageArea } from "@/model/StorageArea";

export interface IStorageAreaService {
	getStorageAreas(filtering?: Filter<{ nameCode: string }>): Promise<Page<StorageArea>>;
	getStorageAreaById(id: string): Promise<StorageArea | undefined>;
	
	createStorageArea(storageArea: StorageArea): Promise<StorageArea>;
	updateStorageArea(storageArea: StorageArea): Promise<StorageArea>;
	
	getNumberOfStorageAreas(): Promise<number>;
}