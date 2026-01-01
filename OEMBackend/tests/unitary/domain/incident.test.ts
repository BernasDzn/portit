import Incident from "../../../src/domain/incident";
import IncidentType from "../../../src/domain/incidentType";

describe('Incident Domain Tests', () => {

    it('should create incident with required fields and auto-generated bid', () => {
        const type = new IncidentType({ name: 'Fire', description: 'Fire', severity: 'Critical' });
        const incident = new Incident({
            type: type,
            startTime: new Date('2025-01-01T10:00:00Z'),
            severity: 'Major',
            description: 'Test incident',
            createdBy: 'tester@example.com'
        });

        expect(incident.bid).toMatch(/^INC-/);
        expect(incident.type).toBe(type);
        expect(incident.startTime.toISOString()).toBe('2025-01-01T10:00:00.000Z');
        expect(incident.severity).toBe('Major');
        expect(incident.description).toBe('Test incident');
        expect(incident.createdBy).toBe('tester@example.com');
    });

    it('should allow setting endTime and affectedVVECodes', () => {
        const type = new IncidentType({ name: 'Network', description: 'Network', severity: 'Minor' });
        const incident = new Incident({
            type: type,
            startTime: new Date('2025-02-01T08:00:00Z'),
            severity: 'Minor',
            description: 'Network glitch',
            createdBy: 'ops@example.com'
        });

        incident.endTime = new Date('2025-02-01T09:00:00Z');
        expect(incident.endTime).toBeDefined();
        expect(incident.endTime!.toISOString()).toBe('2025-02-01T09:00:00.000Z');

        incident.affectedVVECodes = [{ code: 'VVE1', status: 'Active' } as any];
        expect(incident.affectedVVECodes).toBeDefined();
        expect(incident.affectedVVECodes!.length).toBe(1);
    });

});
