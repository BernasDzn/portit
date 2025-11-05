import type { Page } from "@/model/Page";
import type { StorageArea } from "@/model/StorageArea";

export interface IStorageAreaService {
	getStorageAreas(): Promise<Page<StorageArea>>;
	getStorageAreaById(id: string): Promise<StorageArea | undefined>;
	createStorageArea(storageArea: StorageArea): Promise<StorageArea>;
	updateStorageArea(id: string, storageArea: StorageArea): Promise<StorageArea>;
	deleteStorageArea(id: string): Promise<void>;
}