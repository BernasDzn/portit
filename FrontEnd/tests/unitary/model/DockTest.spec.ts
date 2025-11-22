import { describe, it, expect, beforeEach } from 'vitest';
import { Dock } from '../../../src/model/Dock';
import { VesselType } from '../../../src/model/VesselType';

let vt1: VesselType;
let vt2: VesselType;
let vesselTypes: VesselType[];
let validDock: Dock;

beforeEach(() => {
	vt1 = new VesselType({
		name: 'Panamax',
		description: 'Max size for Panama Canal',
		maxNumberOfRows: 20,
		maxNumberOfBays: 10,
		maxNumberOfTiers: 5,
		physicalCharacteristics: { length: 300, depth: 15, draft: 12 }
	});

	vt2 = new VesselType({
		name: 'Post-Panamax',
		description: 'Larger than Panamax',
		maxNumberOfRows: 30,
		maxNumberOfBays: 15,
		maxNumberOfTiers: 7,
		physicalCharacteristics: { length: 400, depth: 18, draft: 14 }
	});

	vesselTypes = [vt1, vt2];

	validDock = new Dock({
		code: 'DCK001',
		name: 'Valid Name',
		location: 'Valid Location',
		physicalCharacteristics: { length: 500, depth: 20, draft: 20 },
		supportedVesselTypes: vesselTypes
	});
});

describe('Constructor', () => {
	it.each([
		['DCK001', 'Valid Name', 'Valid Location', 500, 20, 20],
		['DOCK1', 'D1', 'L1', 450, 24, 19],
		['DK1', 'Dock Alpha', 'Location Beta', 600.25, 30.12, 25.05],
		['D001', 'Dock 123', 'Location 456', 700.21, 35, 30.45]
	])('should create dock successfully - %s', (code, name, location, length, depth, draft) => {
		const dock = new Dock({
			code,
			name,
			location,
			physicalCharacteristics: { length, depth, draft },
			supportedVesselTypes: vesselTypes
		});

		expect(dock).toBeDefined();
		expect(dock.code).toBe(code);
		expect(dock.name).toBe(name);
		expect(dock.location).toBe(location);
	});

	it.each([
		['', 'Valid Name', 'Valid Location', 500, 20, 20, 'Code cannot be null or empty.'],
		['DCK001', '', 'Valid Location', 500, 20, 20, 'Name cannot be null or empty.'],
		['DCK001', 'Valid Name', '', 500, 20, 20, 'Location cannot be null or empty.']
	])('should throw error with invalid string parameters - %s, %s, %s', (code, name, location, length, depth, draft, expectedError) => {
		expect(() => new Dock({
			code,
			name,
			location,
			physicalCharacteristics: { length, depth, draft },
			supportedVesselTypes: vesselTypes
		})).toThrow(expectedError);
	});

	it('should throw error when vessel types is empty', () => {
		expect(() => new Dock({
			code: 'DCK001',
			name: 'Valid Name',
			location: 'Valid Location',
			physicalCharacteristics: { length: 500, depth: 20, draft: 20 },
			supportedVesselTypes: []
		})).toThrow('Invalid vessel types - at least one vessel type must be supported.');
	});

	it('should throw error when physical characteristics is null', () => {
		expect(() => new Dock({
			code: 'DCK001',
			name: 'Valid Name',
			location: 'Valid Location',
			physicalCharacteristics: null as any,
			supportedVesselTypes: vesselTypes
		})).toThrow('Physical characteristics cannot be null.');
	});
});

describe('Physical characteristics validation', () => {
	it('should throw error when dock length is insufficient for vessel types', () => {
		expect(() => new Dock({
			code: 'DCK001',
			name: 'Valid Name',
			location: 'Valid Location',
			physicalCharacteristics: { length: 100, depth: 20, draft: 20 },
			supportedVesselTypes: vesselTypes
		})).toThrow(`The dock's length is insufficient for the vessel type ${vt1.name}.`);
	});

	it('should throw error when dock depth is insufficient for vessel types', () => {
		expect(() => new Dock({
			code: 'DCK001',
			name: 'Valid Name',
			location: 'Valid Location',
			physicalCharacteristics: { length: 500, depth: 10, draft: 20 },
			supportedVesselTypes: vesselTypes
		})).toThrow(`The dock's depth is insufficient for the vessel type ${vt1.name}.`);
	});

	it('should throw error when dock draft is insufficient for vessel types', () => {
		expect(() => new Dock({
			code: 'DCK001',
			name: 'Valid Name',
			location: 'Valid Location',
			physicalCharacteristics: { length: 500, depth: 20, draft: 10 },
			supportedVesselTypes: vesselTypes
		})).toThrow(`The dock's max draft is insufficient for the vessel type ${vt1.name}.`);
	});
});

describe('Update methods', () => {
	it('should update dock with valid parameters', () => {
		validDock.updateName('New Valid Name');
		validDock.updateLocation('New Valid Location');
		validDock.updatePhysicalCharacteristics({ length: 600, depth: 25, draft: 22 });

		const vt3 = new VesselType({
			name: 'New Type',
			description: 'New Description',
			maxNumberOfRows: 25,
			maxNumberOfBays: 12,
			maxNumberOfTiers: 6,
			physicalCharacteristics: { length: 350, depth: 16, draft: 13 }
		});

		const newVesselTypes = [vt1, vt3];
		validDock.updateVesselTypes(newVesselTypes);

		expect(validDock.name).toBe('New Valid Name');
		expect(validDock.location).toBe('New Valid Location');
		expect(validDock.physicalCharacteristics.length).toBe(600);
		expect(validDock.physicalCharacteristics.depth).toBe(25);
		expect(validDock.physicalCharacteristics.draft).toBe(22);
		expect(validDock.supportedVesselTypes).toContain(vt1);
		expect(validDock.supportedVesselTypes).toContain(vt3);
		expect(validDock.supportedVesselTypes).not.toContain(vt2);
	});

	it('should throw error when updating name with empty string', () => {
		expect(() => validDock.updateName('')).toThrow('Name cannot be null or empty.');
	});

	it('should throw error when updating location with empty string', () => {
		expect(() => validDock.updateLocation('')).toThrow('Location cannot be null or empty.');
	});

	it('should throw error when updating physical characteristics with null', () => {
		expect(() => validDock.updatePhysicalCharacteristics(null as any)).toThrow('Physical characteristics cannot be null.');
	});

	it('should throw error when updating vessel types with empty array', () => {
		expect(() => validDock.updateVesselTypes([])).toThrow('Invalid vessel types - at least one vessel type must be supported.');
	});

	it('should throw error when updating physical characteristics insufficient for vessel types', () => {
		expect(() => validDock.updatePhysicalCharacteristics({ length: 100, depth: 25, draft: 22 }))
			.toThrow(`The dock's length is insufficient for the vessel type ${vt1.name}.`);
	});

});

describe('ToDto', () => {
	it('should return correct DTO', () => {
		const dto = validDock.toDto();

		expect(dto.code).toBe(validDock.code);
		expect(dto.name).toBe(validDock.name);
		expect(dto.location).toBe(validDock.location);
		expect(dto.physicalCharacteristics).toEqual(validDock.physicalCharacteristics);
		expect(dto.supportedVesselTypes).toEqual([vt1.name, vt2.name]);
	});
});
