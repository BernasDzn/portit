import { describe, it, expect, beforeEach } from 'vitest';
import { VesselType } from '../../../src/model/VesselType';

let validVesselType: VesselType;

beforeEach(() => {
	validVesselType = new VesselType({
		name: 'Valid Name',
		description: 'Valid Description',
		maxNumberOfRows: 10,
		maxNumberOfBays: 10,
		maxNumberOfTiers: 10,
		physicalCharacteristics: { length: 70, depth: 15, draft: 15 }
	});
});

describe('Constructor', () => {
	it.each([
		['Valid Name', 'Valid Description', 10, 10, 10, 70, 15, 15],
		['VT1', 'VD1', 1, 1, 1, 1, 1, 1],
		['Vessel Type Alpha', 'Vessel Description Beta', 20, 15, 5, 100.5, 20.25, 18.75],
		['VesselType123', 'VesselDescription456', 30, 20, 10, 150.75, 25.5, 22.1]
	])('should create vessel type successfully - %s', (name, description, rows, bays, tiers, length, depth, draft) => {
		const vesselType = new VesselType({
			name,
			description,
			maxNumberOfRows: rows,
			maxNumberOfBays: bays,
			maxNumberOfTiers: tiers,
			physicalCharacteristics: { length, depth, draft }
		});

		expect(vesselType).toBeDefined();
		expect(vesselType.name).toBe(name);
		expect(vesselType.description).toBe(description);
		expect(vesselType.maxNumberOfRows).toBe(rows);
		expect(vesselType.maxNumberOfBays).toBe(bays);
		expect(vesselType.maxNumberOfTiers).toBe(tiers);
	});

	it.each([
		['', 'Valid Description', 10, 10, 10, 70, 15, 15, 'Name cannot be null or empty.'],
		['Valid Name', '', 10, 10, 10, 70, 15, 15, 'Description cannot be null or empty.'],
		['Valid Name', 'Valid Description', 0, 10, 10, 70, 15, 15, 'Max number of rows must be greater than zero.'],
		['Valid Name', 'Valid Description', 10, 0, 10, 70, 15, 15, 'Max number of bays must be greater than zero.'],
		['Valid Name', 'Valid Description', 10, 10, 0, 70, 15, 15, 'Max number of tiers must be greater than zero.']
	])('should throw error with invalid parameters - %s, %s', (name, description, rows, bays, tiers, length, depth, draft, expectedError) => {
		expect(() => new VesselType({
			name,
			description,
			maxNumberOfRows: rows,
			maxNumberOfBays: bays,
			maxNumberOfTiers: tiers,
			physicalCharacteristics: { length, depth, draft }
		})).toThrow(expectedError);
	});
});

describe('Update methods', () => {
	it('should update vessel type with valid parameters', () => {
		validVesselType.updateName('New Valid Name');
		validVesselType.updateDescription('New Valid Description');
		validVesselType.updateMaxNumberOfRows(15);
		validVesselType.updateMaxNumberOfBays(12);
		validVesselType.updateMaxNumberOfTiers(8);
		validVesselType.updatePhysicalCharacteristics({ length: 80, depth: 18, draft: 18 });

		expect(validVesselType.name).toBe('New Valid Name');
		expect(validVesselType.description).toBe('New Valid Description');
		expect(validVesselType.maxNumberOfRows).toBe(15);
		expect(validVesselType.maxNumberOfBays).toBe(12);
		expect(validVesselType.maxNumberOfTiers).toBe(8);
		expect(validVesselType.physicalCharacteristics.length).toBe(80);
		expect(validVesselType.physicalCharacteristics.depth).toBe(18);
		expect(validVesselType.physicalCharacteristics.draft).toBe(18);
	});

	it('should throw error when updating name with empty string', () => {
		expect(() => validVesselType.updateName('')).toThrow('Name cannot be null or empty.');
	});

	it('should throw error when updating description with empty string', () => {
		expect(() => validVesselType.updateDescription('')).toThrow('Description cannot be null or empty.');
	});

	it('should throw error when updating rows to zero', () => {
		expect(() => validVesselType.updateMaxNumberOfRows(0)).toThrow('Max number of rows must be greater than zero.');
	});

	it('should throw error when updating bays to zero', () => {
		expect(() => validVesselType.updateMaxNumberOfBays(0)).toThrow('Max number of bays must be greater than zero.');
	});

	it('should throw error when updating tiers to zero', () => {
		expect(() => validVesselType.updateMaxNumberOfTiers(0)).toThrow('Max number of tiers must be greater than zero.');
	});
});

describe('Capacity calculation', () => {
	it('should calculate capacity correctly', () => {
		const vesselType = new VesselType({
			name: 'Test',
			description: 'Test',
			maxNumberOfRows: 5,
			maxNumberOfBays: 4,
			maxNumberOfTiers: 3,
			physicalCharacteristics: { length: 100, depth: 20, draft: 15 }
		});

		expect(vesselType.capacity).toBe(60); // 5 * 4 * 3
	});
});

describe('ToDto', () => {
	it('should return correct DTO', () => {
		const dto = validVesselType.toDto();

		expect(dto.name).toBe(validVesselType.name);
		expect(dto.description).toBe(validVesselType.description);
		expect(dto.maxNumberOfRows).toBe(validVesselType.maxNumberOfRows);
		expect(dto.maxNumberOfBays).toBe(validVesselType.maxNumberOfBays);
		expect(dto.maxNumberOfTiers).toBe(validVesselType.maxNumberOfTiers);
		expect(dto.capacity).toBe(validVesselType.capacity);
		expect(dto.physicalCharacteristics).toEqual(validVesselType.physicalCharacteristics);
	});
});
