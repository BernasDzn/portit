import { Dock } from "./Dock";
import type { StorageAreaDto, DockRelationDto } from "./dto/StorageAreaDto";

export class DockRelation {
	private _dock: string;
	private _distance?: number;
	private _isServingDock: boolean;

	constructor(params: {
		dock: string;
		distance?: number;
		isServingDock: boolean;
	}) {
		if (!params.dock) throw new Error('Serving dock cannot be null.');

		this._dock = params.dock;
		this._distance = params.distance;
		this._isServingDock = params.isServingDock;
	}

	get dock(): string { return this._dock; }
	get distance(): number | undefined { return this._distance; }
	get isServingDock(): boolean { return this._isServingDock; }

	toDto(): DockRelationDto {
		return {
			dockCode: this._dock,
			distance: this._distance,
			isServingDock: this._isServingDock
		};
	}
}

export class StorageArea {
	static readonly sa_type : Record<number, string> = {
		0: "Yard",
		1: "Warehouse"
	};

	private _nameCode: string;
	private _location: string;
	private _type: number;
	private _capacity: number;
	private _currentOccupancy: number;
	private _dockServices: DockRelation[];

	constructor(params: {
		nameCode: string;
		location: string;
		type: number;
		capacity: number;
		currentOccupancy: number;
		dockServices?: DockRelation[];
	}) {

		if (!params.nameCode) throw new Error('Name code cannot be null or empty.');
		if (!params.location) throw new Error('Location cannot be null or empty.');
		if (params.currentOccupancy > params.capacity) {
			throw new Error(`Current occupancy cannot exceed capacity. Capacity: ${params.capacity}, Attempted Occupancy: ${params.currentOccupancy}`);
		}

		// Warehouse type (1) must serve all docks it is related to
		if (params.type === 1 && params.dockServices && params.dockServices.some(ds => !ds.isServingDock)) {
			throw new Error('A warehouse must serve all docks it is related to.');
		}

		this._nameCode = params.nameCode;
		this._location = params.location;
		this._type = params.type;
		this._capacity = params.capacity;
		this._currentOccupancy = params.currentOccupancy;
		this._dockServices = params.dockServices ?? [];
	}

	get nameCode(): string { return this._nameCode; }
	get location(): string { return this._location; }
	get type(): number { return this._type; }
	get capacity(): number { return this._capacity; }
	get currentOccupancy(): number { return this._currentOccupancy; }
	get dockServices(): DockRelation[] { return this._dockServices; }

	updateNameCode(newCode: string): void {
		if (!newCode) throw new Error('Name code cannot be null or empty.');
		this._nameCode = newCode;
	}

	updateLocation(newLocation: string): void {
		if (!newLocation) throw new Error('Location cannot be null or empty.');
		this._location = newLocation;
	}

	updateCapacity(newCapacity: number): void {
		if (newCapacity < this._currentOccupancy) {
			throw new Error(`New capacity cannot be less than current occupancy. Current Occupancy: ${this._currentOccupancy}, New Capacity: ${newCapacity}`);
		}
		this._capacity = newCapacity;
	}

	updateOccupancy(newOccupancy: number): void {
		if (newOccupancy > this._capacity) {
			throw new Error(`Current occupancy cannot exceed capacity. Capacity: ${this._capacity}, Attempted Occupancy: ${newOccupancy}`);
		}
		this._currentOccupancy = newOccupancy;
	}

	updateAreaType(newType: number): void {
		this._type = newType;
	}

	updateDockServices(newDockServices: DockRelation[]): void {
		if (this._type === 1 && newDockServices && newDockServices.some(ds => !ds.isServingDock)) {
			throw new Error('A warehouse must serve all docks it is related to.');
		}
		this._dockServices = newDockServices;
	}

	canServeDock(dockCode: string): boolean {
		return (
			this._type === 1 || // Warehouse
			this._dockServices.some(ds => ds.dock.code === dockCode && ds.isServingDock)
		);
	}

	toDto(): StorageAreaDto {
		return {
			nameCode: this._nameCode,
			location: this._location,
			type: this._type,
			capacity: this._capacity,
			currentOccupancy: this._currentOccupancy,
			dockServices: this._dockServices.map(d => d.toDto())
		};
	}
}