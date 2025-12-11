import IncidentType from "../../src/domain/incidentType";

describe('Incident Type Domain Tests', () => {

	it('should create incident type without children or parent', () => {
		const incidentType = new IncidentType({ name: 'Network Issue' });
		expect(incidentType.id).toMatch(/^INC-/);
		expect(incidentType.name).toBe('Network Issue');
		expect(incidentType.parent).toBeUndefined();
		expect(incidentType.children).toBeUndefined();
	});

	it('should add child incident type correctly', () => {
		const parentType = new IncidentType({ name: 'Operational Failures' });
		const childType = new IncidentType({ name: 'Crane Malfunction' });

		parentType.addChild(childType);

		expect(parentType.children).toBeDefined();
		expect(parentType.children!.length).toBe(1);
		expect(parentType.children![0]).toBe(childType);
		expect(childType.parent).toBe(parentType);
	});

});