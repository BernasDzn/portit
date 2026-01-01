import { IncidentController } from "../../../src/controllers/incidentController";
import { IncidentService } from "../../../src/services/incidentService";
import { CreateIncidentDto, UpdateIncidentDto } from "../../../src/dto/incidentDto";
import Incident from "../../../src/domain/incident";
import IncidentType from "../../../src/domain/incidentType";

jest.mock('../../../src/services/incidentService');

describe('IncidentController', () => {
    let controller: IncidentController;
    let mockService: jest.Mocked<IncidentService>;

    beforeEach(() => {
        jest.clearAllMocks();
        mockService = new IncidentService() as jest.Mocked<IncidentService>;
        controller = new IncidentController();
        (controller as any).incidentService = mockService;
    });
    describe('count', () => {
        it('returns repository count', async () => {
            mockService.count = jest.fn().mockResolvedValue(3);
            const result = await controller.count();
            expect(mockService.count).toHaveBeenCalled();
            expect(result).toEqual({ count: 3 });
        });
    });

    describe('getByBid', () => {
        it('returns incident when found', async () => {
            const incident = new Incident({ type: new IncidentType({ name: 'T', description: 'd', severity: 'Minor' }), startTime: new Date(), severity: 'Minor', description: 'd', createdBy: 'u' });
            mockService.getByBid = jest.fn().mockResolvedValue({ description: incident.description } as any);
            const result = await controller.getByBid('INC-1');
            expect(mockService.getByBid).toHaveBeenCalledWith('INC-1');
            expect(result).toBeDefined();
        });

        it('handles service errors with 500', async () => {
            mockService.getByBid = jest.fn().mockRejectedValue(new Error('DB'));
            const result = await controller.getByBid('INC-ERR');
            expect(result).toEqual({ message: 'Internal server error: DB' });
        });
    });

    describe('getPaged', () => {
        it('calls service with defaults', async () => {
            const mockPage = { pageNumber: 1, pageSize: 10, pageCount: 1, items: [] };
            mockService.getPaged = jest.fn().mockResolvedValue(mockPage as any);
            await controller.getPaged(undefined, undefined, undefined, undefined, undefined, 1, 10);
            const callArg = (mockService.getPaged).mock.calls[0][0];
            expect(callArg.pageNumber).toBe(1);
            expect(callArg.pageSize).toBe(10);
        });
    });

    describe('createIncident', () => {
        it('returns 401 when no token', async () => {
            const body: CreateIncidentDto = { type: 'TYPE', startTime: new Date().toISOString(), severity: 'Minor', description: 'x' } as any;
            const req: any = { user: {} };
            const result = await controller.createIncident(body, req);
            expect(result).toEqual({ message: 'Authorization token required' });
        });

        it('creates when token present', async () => {
            const body: CreateIncidentDto = { type: 'TYPE', startTime: new Date().toISOString(), severity: 'Minor', description: 'x' } as any;
            const req: any = { user: { token: 't', emailAddress: 'me@x' } };
            mockService.create = jest.fn().mockResolvedValue({ description: 'x' } as any);
            const result = await controller.createIncident(body, req);
            expect(mockService.create).toHaveBeenCalledWith(body, 'me@x');
            expect(result).toEqual({ description: 'x' });
        });
    });

    describe('updateIncident', () => {
        it('returns 404 when service returns null', async () => {
            mockService.update = jest.fn().mockResolvedValue(null as any);
            const result = await controller.updateIncident('INC-NA', { description: 'u' } as any);
            expect(result).toEqual({ message: 'Incident not found' });
        });

        it('returns updated incident', async () => {
            mockService.update = jest.fn().mockResolvedValue({ description: 'updated' } as any);
            const result = await controller.updateIncident('INC-1', { description: 'updated' } as any);
            expect(mockService.update).toHaveBeenCalledWith('INC-1', { description: 'updated' });
            expect(result).toEqual({ description: 'updated' });
        });
    });

});
