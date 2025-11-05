import type { Dock }  from "./Dock";

export interface StorageArea {
	nameCode: string;
	location: string;
	type : number;
	capacity: number;
	currentOccupancy: number;
	dockServices: DockRelation[];
}

export interface DockRelation {
	dock: Dock;
	distance?: number;
	isServingDock: boolean;
}