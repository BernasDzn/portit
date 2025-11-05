import type { Page } from "@/model/Page";
import type { StorageArea } from "@/model/StorageArea";

export interface IStorageAreaService {
	getStorageAreas(): Promise<Page<StorageArea>>;
	getStorageAreaById(id: number): Promise<StorageArea | undefined>;
	createStorageArea(storageArea: StorageArea): Promise<StorageArea>;
	updateStorageArea(id: number, storageArea: StorageArea): Promise<StorageArea>;
	deleteStorageArea(id: number): Promise<void>;
}