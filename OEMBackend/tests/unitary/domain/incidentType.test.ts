import IncidentType from "../../../src/domain/incidentType";

describe('Incident Type Domain Tests', () => {

	it('should create incident type without children or parent', () => {
		const incidentType = new IncidentType({ 
			name: 'Network Issue',
			description: 'A network issue',
			severity: 'Minor'
		});
		expect(incidentType.bid).toMatch(/^INC-/);
		expect(incidentType.name).toBe('Network Issue');
		expect(incidentType.subtypeOf).toBeUndefined();
		expect(incidentType.subtypes).toBeUndefined();
	});

	it('should add child incident type correctly', () => {
		const parentType = new IncidentType({ 
			name: 'Operational Failures',
			description: 'Operational failures',
			severity: 'Major'
		});
		const childType = new IncidentType({ 
			name: 'Crane Malfunction',
			description: 'Crane malfunction',
			severity: 'Critical'
		});

		parentType.addSubtype(childType);

		expect(parentType.subtypes).toBeDefined();
		expect(parentType.subtypes!.length).toBe(1);
		expect(parentType.subtypes![0]).toBe(childType);
		expect(childType.subtypeOf).toBe(parentType);
	});

});