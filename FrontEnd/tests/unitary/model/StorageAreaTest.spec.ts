import { describe, it, expect, beforeEach } from 'vitest';
import { StorageArea, DockRelation } from '../../../src/model/StorageArea';
import { Dock } from '../../../src/model/Dock';
import { VesselType } from '../../../src/model/VesselType';

let vt1: VesselType;
let d1: Dock;
let dr1: DockRelation;
let validStorageArea: StorageArea;

beforeEach(() => {
	vt1 = new VesselType({
		name: 'Panamax',
		description: 'Max size for Panama Canal',
		maxNumberOfRows: 20,
		maxNumberOfBays: 10,
		maxNumberOfTiers: 5,
		physicalCharacteristics: { length: 300, depth: 15, draft: 12 }
	});

	d1 = new Dock({
		code: 'DCK001',
		name: 'Dock 1',
		location: 'Location 1',
		physicalCharacteristics: { length: 500, depth: 20, draft: 20 },
		supportedVesselTypes: [vt1]
	});

	dr1 = new DockRelation({
		dock: d1,
		distance: 100,
		isServingDock: true
	});

	validStorageArea = new StorageArea({
		nameCode: 'SA001',
		location: 'Valid Location',
		type: 0, // Yard
		capacity: 1000,
		currentOccupancy: 500,
		dockServices: [dr1]
	});
});

describe('DockRelation', () => {
	it('should create dock relation successfully', () => {
		const relation = new DockRelation({
			dock: d1,
			distance: 100,
			isServingDock: true
		});

		expect(relation.dock).toBe(d1);
		expect(relation.distance).toBe(100);
		expect(relation.isServingDock).toBe(true);
	});

	it('should throw error when dock is null', () => {
		expect(() => new DockRelation({
			dock: null as any,
			distance: 100,
			isServingDock: true
		})).toThrow('Serving dock cannot be null.');
	});

	it('should return correct DTO', () => {
		const dto = dr1.toDto();

		expect(dto.dockCode).toBe(d1.code);
		expect(dto.distance).toBe(100);
		expect(dto.isServingDock).toBe(true);
	});
});

describe('StorageArea Constructor', () => {
	it.each([
		['SA001', 'Valid Location', 0, 1000, 500], // Yard
		['STORAGE1', 'Location Alpha', 1, 2000, 1500], // Warehouse
		['SA123', 'Location 123', 0, 500, 0] // Empty Yard
	])('should create storage area successfully - %s', (nameCode, location, areaType, capacity, currentOccupancy) => {
		const storageArea = new StorageArea({
			nameCode,
			location,
			type: areaType,
			capacity,
			currentOccupancy,
			dockServices: [dr1]
		});

		expect(storageArea).toBeDefined();
		expect(storageArea.nameCode).toBe(nameCode);
		expect(storageArea.location).toBe(location);
		expect(storageArea.type).toBe(areaType);
		expect(storageArea.capacity).toBe(capacity);
		expect(storageArea.currentOccupancy).toBe(currentOccupancy);
	});

	it.each([
		['', 'Valid Location', 0, 1000, 500, 'Name code cannot be null or empty.'],
		['SA001', '', 0, 1000, 500, 'Location cannot be null or empty.']
	])('should throw error with invalid string parameters - %s, %s', (nameCode, location, areaType, capacity, currentOccupancy, expectedError) => {
		expect(() => new StorageArea({
			nameCode,
			location,
			type: areaType,
			capacity,
			currentOccupancy,
			dockServices: [dr1]
		})).toThrow(expectedError);
	});

	it('should throw error when occupancy exceeds capacity', () => {
		expect(() => new StorageArea({
			nameCode: 'SA001',
			location: 'Valid Location',
			type: 0,
			capacity: 10,
			currentOccupancy: 20,
			dockServices: [dr1]
		})).toThrow('Current occupancy cannot exceed capacity. Capacity: 10, Attempted Occupancy: 20');
	});

	it('should throw error when warehouse does not serve all docks', () => {
		const nonServingRelation = new DockRelation({
			dock: d1,
			distance: 100,
			isServingDock: false
		});

		expect(() => new StorageArea({
			nameCode: 'SA001',
			location: 'Valid Location',
			type: 1, // Warehouse
			capacity: 1000,
			currentOccupancy: 500,
			dockServices: [nonServingRelation]
		})).toThrow('A warehouse must serve all docks it is related to.');
	});
});

describe('CanServeDock', () => {
	it('should return true when storage area can serve dock', () => {
		const result = validStorageArea.canServeDock(d1.code);
		expect(result).toBe(true);
	});

	it('should return false when storage area cannot serve dock', () => {
		const newDock = new Dock({
			code: 'DCK002',
			name: 'Dock 2',
			location: 'Location 2',
			physicalCharacteristics: { length: 400, depth: 45, draft: 40 },
			supportedVesselTypes: [vt1]
		});

		const result = validStorageArea.canServeDock(newDock.code);
		expect(result).toBe(false);
	});

	it('should return true for warehouse regardless of dock services', () => {
		const warehouse = new StorageArea({
			nameCode: 'WH001',
			location: 'Warehouse Location',
			type: 1, // Warehouse
			capacity: 2000,
			currentOccupancy: 1000,
			dockServices: [dr1]
		});

		const newDock = new Dock({
			code: 'DCK999',
			name: 'Random Dock',
			location: 'Random Location',
			physicalCharacteristics: { length: 400, depth: 45, draft: 40 },
			supportedVesselTypes: [vt1]
		});

		expect(warehouse.canServeDock(newDock.code)).toBe(true);
	});
});

describe('Update methods', () => {
	it('should update storage area with valid parameters', () => {
		const d2 = new Dock({
			code: 'DCK002',
			name: 'Dock 2',
			location: 'Location 2',
			physicalCharacteristics: { length: 400, depth: 45, draft: 40 },
			supportedVesselTypes: [vt1]
		});

		const dr2 = new DockRelation({
			dock: d2,
			distance: 150,
			isServingDock: true
		});

		validStorageArea.updateNameCode('NewCode');
		validStorageArea.updateLocation('New Valid Location');
		validStorageArea.updateCapacity(800);
		validStorageArea.updateOccupancy(400);
		validStorageArea.updateAreaType(1); // Warehouse
		validStorageArea.updateDockServices([dr2]);

		expect(validStorageArea.nameCode).toBe('NewCode');
		expect(validStorageArea.location).toBe('New Valid Location');
		expect(validStorageArea.capacity).toBe(800);
		expect(validStorageArea.currentOccupancy).toBe(400);
		expect(validStorageArea.type).toBe(1);
		expect(validStorageArea.dockServices).toContain(dr2);
	});

	it('should throw error when updating name code with empty string', () => {
		expect(() => validStorageArea.updateNameCode('')).toThrow('Name code cannot be null or empty.');
	});

	it('should throw error when updating location with empty string', () => {
		expect(() => validStorageArea.updateLocation('')).toThrow('Location cannot be null or empty.');
	});

	it('should throw error when updating capacity below current occupancy', () => {
		expect(() => validStorageArea.updateCapacity(400)).toThrow('New capacity cannot be less than current occupancy. Current Occupancy: 500, New Capacity: 400');
	});

	it('should throw error when updating occupancy above capacity', () => {
		expect(() => validStorageArea.updateOccupancy(1100)).toThrow('Current occupancy cannot exceed capacity. Capacity: 1000, Attempted Occupancy: 1100');
	});

	it('should throw error when updating warehouse dock services with non-serving relation', () => {
		validStorageArea.updateAreaType(1); // Make it a warehouse

		const nonServingRelation = new DockRelation({
			dock: d1,
			distance: 100,
			isServingDock: false
		});

		expect(() => validStorageArea.updateDockServices([nonServingRelation]))
			.toThrow('A warehouse must serve all docks it is related to.');
	});
});

describe('ToDto', () => {
	it('should return correct DTO', () => {
		const dto = validStorageArea.toDto();

		expect(dto.nameCode).toBe(validStorageArea.nameCode);
		expect(dto.location).toBe(validStorageArea.location);
		expect(dto.type).toBe(validStorageArea.type);
		expect(dto.capacity).toBe(validStorageArea.capacity);
		expect(dto.currentOccupancy).toBe(validStorageArea.currentOccupancy);
		expect(dto.dockServices).toHaveLength(1);
		expect(dto.dockServices[0].dockCode).toBe(d1.code);
	});
});
