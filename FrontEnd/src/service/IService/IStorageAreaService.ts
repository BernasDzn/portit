import type { Page } from "@/model/Page";
import type { StorageArea, StorageAreaCreate } from "@/model/StorageArea";
import type { Filter } from "mongodb";

export interface IStorageAreaService {
	getStorageAreas(filtering?: Filter<{ nameCode: string }>): Promise<Page<StorageArea>>;
	getStorageAreaById(id: string): Promise<StorageArea | undefined>;
	createStorageArea(storageArea: StorageAreaCreate): Promise<StorageAreaCreate>;
	updateStorageArea(id: string, storageArea: StorageArea): Promise<StorageArea>;
	deleteStorageArea(id: string): Promise<void>;
	getNumberOfStorageAreas(): Promise<number>;
}