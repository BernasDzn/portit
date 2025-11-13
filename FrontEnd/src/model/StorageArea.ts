import { Dock } from "./Dock";
import type { StorageAreaDto, DockRelationDto } from "./dto/StorageAreaDto";

export class DockRelation {
	readonly dock: Dock;
	readonly distance?: number;
	readonly isServingDock: boolean;

	constructor(params: {
		dock: Dock;
		distance?: number;
		isServingDock: boolean;
	}) {
		this.dock = params.dock;
		this.distance = params.distance;
		this.isServingDock = params.isServingDock;
	}

	toDto(): DockRelationDto {
		return {
			dock: this.dock.code,
			distance: this.distance,
			isServingDock: this.isServingDock
		};
	}
}

export class StorageArea {
	readonly nameCode: string;
	readonly location: string;
	readonly type: number;
	readonly capacity: number;
	readonly currentOccupancy: number;
	readonly dockServices: DockRelation[];

	constructor(params: {
		nameCode: string;
		location: string;
		type: number;
		capacity: number;
		currentOccupancy: number;
		dockServices?: DockRelation[];
	}) {
		this.nameCode = params.nameCode;
		this.location = params.location;
		this.type = params.type;
		this.capacity = params.capacity;
		this.currentOccupancy = params.currentOccupancy;
		this.dockServices = params.dockServices ?? [];
	}

	toDto(): StorageAreaDto {
		return {
			nameCode: this.nameCode,
			location: this.location,
			type: this.type,
			capacity: this.capacity,
			currentOccupancy: this.currentOccupancy,
			dockServices: this.dockServices.map(d => d.toDto())
		};
	}
}