import { describe, it, expect, beforeEach } from 'vitest';
import { Vessel } from '../../../src/model/Vessel';
import { VesselType } from '../../../src/model/VesselType';
import type { ShippingAgentOrganization } from '../../../src/model/ShippingAgentOrganization';

let validVesselType: VesselType;
let validOwner: ShippingAgentOrganization;

beforeEach(() => {
	validVesselType = new VesselType({
		name: 'Container Ship',
		description: 'A large container ship',
		maxNumberOfRows: 20,
		maxNumberOfBays: 10,
		maxNumberOfTiers: 8,
		physicalCharacteristics: {
			length: 300.0,
			depth: 50.0,
			draft: 15.0
		}
	});

	validOwner = {
		name: 'Maersk',
		altNames: ['A major shipping company'],
		taxNumber: 'PT252252252',
		address: {
			id: '1',
			street: '123 Ocean Drive',
			city: 'Copenhagen',
			country: 'Denmark',
			zipCode: '2100'
		},
		representatives: [{
			name: 'rep',
			citizenshipId: '123456789',
			emailAddress: 'email@email.com',
			phone: '4512345678'
		}]
	};
});

describe('Constructor', () => {
	it('should create vessel with correct data', () => {
		const vessel = new Vessel({
			name: 'Ever Given',
			imoNumber: 'IMO 9811000',
			type: validVesselType,
			owner: validOwner,
			physicalCharacteristics: {
				length: 200.0,
				depth: 40.0,
				draft: 10.0
			}
		});

		expect(vessel).toBeDefined();
		expect(vessel.name).toBe('Ever Given');
		expect(vessel.imoNumber).toBe('IMO 9811000');
		expect(vessel.type).toBe(validVesselType);
		expect(vessel.owner).toBe(validOwner);
	});

	it.each([
		['', 'IMO 9811000', 'Name cannot be null or empty.'],
		['Ever Given', '', 'IMO Number cannot be null or empty.']
	])('should throw error with invalid data - %s, %s', (name, imoNumber, expectedError) => {
		expect(() => new Vessel({
			name,
			imoNumber,
			type: validVesselType,
			owner: validOwner,
			physicalCharacteristics: {
				length: 200.0,
				depth: 30.0,
				draft: 15.0
			}
		})).toThrow(expectedError);
	});

	it('should throw error when vessel type is null', () => {
		expect(() => new Vessel({
			name: 'Ever Given',
			imoNumber: 'IMO 9811000',
			type: null as any,
			owner: validOwner,
			physicalCharacteristics: {
				length: 200.0,
				depth: 30.0,
				draft: 15.0
			}
		})).toThrow('Vessel Type cannot be null.');
	});

	it('should throw error when owner is null', () => {
		expect(() => new Vessel({
			name: 'Ever Given',
			imoNumber: 'IMO 9811000',
			type: validVesselType,
			owner: null as any,
			physicalCharacteristics: {
				length: 200.0,
				depth: 30.0,
				draft: 15.0
			}
		})).toThrow('Owner cannot be null.');
	});

	it('should throw error when physical characteristics is null', () => {
		expect(() => new Vessel({
			name: 'Ever Given',
			imoNumber: 'IMO 9811000',
			type: validVesselType,
			owner: validOwner,
			physicalCharacteristics: null as any
		})).toThrow('Physical Characteristics cannot be null.');
	});
});

describe('Physical characteristics validation', () => {
	it('should throw error when vessel length exceeds type maximum', () => {
		expect(() => new Vessel({
			name: 'Ever Given',
			imoNumber: 'IMO 9811000',
			type: validVesselType,
			owner: validOwner,
			physicalCharacteristics: {
				length: 350.0, // Exceeds 300
				depth: 40.0,
				draft: 10.0
			}
		})).toThrow(`The vessel's length exceeds the maximum length for the vessel type ${validVesselType.name}.`);
	});

	it('should throw error when vessel depth exceeds type maximum', () => {
		expect(() => new Vessel({
			name: 'Ever Given',
			imoNumber: 'IMO 9811000',
			type: validVesselType,
			owner: validOwner,
			physicalCharacteristics: {
				length: 200.0,
				depth: 60.0, // Exceeds 50
				draft: 10.0
			}
		})).toThrow(`The vessel's depth exceeds the maximum depth for the vessel type ${validVesselType.name}.`);
	});

	it('should throw error when vessel draft exceeds type maximum', () => {
		expect(() => new Vessel({
			name: 'Ever Given',
			imoNumber: 'IMO 9811000',
			type: validVesselType,
			owner: validOwner,
			physicalCharacteristics: {
				length: 200.0,
				depth: 40.0,
				draft: 20.0 // Exceeds 15
			}
		})).toThrow(`The vessel's max draft exceeds the maximum draft for the vessel type ${validVesselType.name}.`);
	});
});

describe('Update methods', () => {
	let vessel: Vessel;

	beforeEach(() => {
		vessel = new Vessel({
			name: 'Ever Given',
			imoNumber: 'IMO 9811000',
			type: validVesselType,
			owner: validOwner,
			physicalCharacteristics: {
				length: 200.0,
				depth: 40.0,
				draft: 10.0
			}
		});
	});

	it('should update name successfully', () => {
		vessel.updateName('New Name');
		expect(vessel.name).toBe('New Name');
	});

	it('should throw error when updating name with empty string', () => {
		expect(() => vessel.updateName('')).toThrow('Name cannot be null or empty.');
	});

	it('should update IMO number successfully', () => {
		vessel.updateImoNumber('IMO 1234567');
		expect(vessel.imoNumber).toBe('IMO 1234567');
	});

	it('should throw error when updating IMO number with empty string', () => {
		expect(() => vessel.updateImoNumber('')).toThrow('IMO Number cannot be null or empty.');
	});

	it('should update vessel type successfully', () => {
		const newType = new VesselType({
			name: 'Tanker',
			description: 'Oil tanker',
			maxNumberOfRows: 15,
			maxNumberOfBays: 8,
			maxNumberOfTiers: 6,
			physicalCharacteristics: {
				length: 250.0,
				depth: 45.0,
				draft: 12.0
			}
		});

		vessel.updateVesselType(newType);
		expect(vessel.type).toBe(newType);
	});

	it('should throw error when updating physical characteristics that exceed type limits', () => {
		expect(() => vessel.updatePhysicalCharacteristics({
			length: 350.0,
			depth: 40.0,
			draft: 10.0
		})).toThrow(`The vessel's length exceeds the maximum length for the vessel type ${validVesselType.name}.`);
	});
});

describe('ToDto', () => {
	it('should return correct DTO', () => {
		const vessel = new Vessel({
			name: 'Ever Given',
			imoNumber: 'IMO 9811000',
			type: validVesselType,
			owner: validOwner,
			physicalCharacteristics: {
				length: 200.0,
				depth: 40.0,
				draft: 10.0
			}
		});

		const dto = vessel.toDto();

		expect(dto.name).toBe('Ever Given');
		expect(dto.imoNumber).toBe('IMO 9811000');
		expect(dto.type).toBe(validVesselType.name);
		expect(dto.owner).toBe(validOwner.taxNumber);
		expect(dto.length).toBe(200.0);
		expect(dto.depth).toBe(40.0);
		expect(dto.draft).toBe(10.0);
	});
});
