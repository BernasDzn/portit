import { IncidentController } from "../../src/controllers/incidentController";
import { IncidentService } from "../../src/services/incidentService";
import Incident from "../../src/domain/incident";
import IncidentType from "../../src/domain/incidentType";

describe('Incident Controller->Service integration', () => {
    let controller: IncidentController;
    let service: IncidentService;
    let mockRepo: any;
    let mockTypeRepo: any;
    let mockVveRepo: any;

    beforeEach(() => {
        service = new IncidentService();
        controller = new IncidentController();

        mockRepo = { count: jest.fn(), getPaged: jest.fn(), getByBid: jest.fn(), save: jest.fn(), update: jest.fn() };
        mockTypeRepo = { getById: jest.fn() };
        mockVveRepo = { getByCode: jest.fn() };

        (service as any).incidentRepository = mockRepo;
        (service as any).incidentTypeRepository = mockTypeRepo;
        (service as any).vveRepository = mockVveRepo;

        (controller as any).incidentService = service;
    });

    describe('count', () => {
        it('returns count from service', async () => {
            mockRepo.count.mockResolvedValue(5);
            const res = await controller.count();
            expect(res).toEqual({ count: 5 });
        });
    });

    describe('getByBid', () => {
        it('returns incident when found', async () => {
            const incident = new Incident({ type: new IncidentType({ name: 'T', description: 'd', severity: 'Minor' }), startTime: new Date(), severity: 'Minor', description: 'd', createdBy: 'u' });
            mockRepo.getByBid.mockResolvedValue(incident);
            const res = await controller.getByBid('INC-1');
            expect(mockRepo.getByBid).toHaveBeenCalledWith('INC-1');
            expect((res as any).description).toBe('d');
        });

        it('handles service errors gracefully', async () => {
            mockRepo.getByBid.mockRejectedValue(new Error('fail'));
            const res = await controller.getByBid('INC-ERR');
            expect(res).toEqual({ message: 'Internal server error: fail' });
        });
    });

    describe('createIncident', () => {
        it('returns 401 when request missing token', async () => {
            const body = { type: 'T1', startTime: new Date().toISOString(), severity: 'Minor', description: 'x' } as any;
            const req: any = { user: {} };
            const res = await controller.createIncident(body, req);
            expect(res).toEqual({ message: 'Authorization token required' });
        });

        it('creates incident when token present', async () => {
            const body = { type: 'T1', startTime: new Date().toISOString(), severity: 'Minor', description: 'x' } as any;
            const req: any = { user: { token: 't', emailAddress: 'me' } };
            mockTypeRepo.getById.mockResolvedValue(new IncidentType({ name: 'T1', description: 'd', severity: 'Minor' }));
            mockRepo.save.mockResolvedValue(new Incident({ type: new IncidentType({ name: 'T1', description: 'd', severity: 'Minor' }), startTime: new Date(body.startTime), severity: 'Minor', description: 'x', createdBy: 'me' } as any));

            const res = await controller.createIncident(body, req);
            expect(mockTypeRepo.getById).toHaveBeenCalledWith('T1');
            expect((res as any).description).toBe('x');
        });
    });

    describe('updateIncident', () => {
        it('returns 404 when service returns null', async () => {
            (service as any).update = jest.fn().mockResolvedValue(null);
            const res = await controller.updateIncident('INC-NA', { description: 'u' } as any);
            expect(res).toEqual({ message: 'Incident not found' });
        });

        it('returns updated incident', async () => {
            (service as any).update = jest.fn().mockResolvedValue({ description: 'updated' } as any);
            const res = await controller.updateIncident('INC-1', { description: 'updated' } as any);
            expect((res as any).description).toBe('updated');
        });
    });

});
