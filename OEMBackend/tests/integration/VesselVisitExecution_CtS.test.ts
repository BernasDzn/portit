import { VesselVisitExecutionController } from "../../src/controllers/vesselVisitExecutionController";
import { VesselVisitExecutionService } from "../../src/services/vesselVisitExecutionService";

describe('VesselVisitExecution Controller->Service integration', () => {
    let controller: VesselVisitExecutionController;
    let service: VesselVisitExecutionService;
    let mockRepo: any;

    beforeEach(() => {
        service = new VesselVisitExecutionService();
        controller = new VesselVisitExecutionController();

        mockRepo = { count: jest.fn(), getPaged: jest.fn(), getByCode: jest.fn(), save: jest.fn(), update: jest.fn() };
        (service as any).vveRepository = mockRepo;
        (controller as any).vesselVisitExecutionService = service;
    });

    describe('countVesselVisitExecutions', () => {
        it('forwards to service', async () => {
            mockRepo.count.mockResolvedValue(6);
            (service as any).count = jest.fn().mockImplementation(() => mockRepo.count());
            const res = await controller.countVesselVisitExecutions();
            expect(res).toEqual({ count: 6 });
        });
    });

    describe('open/close/update/start/complete/getVVE', () => {
        it('open returns 401 when missing token', async () => {
            const req: any = { user: {} };
            const res = await controller.openVesselVisitExecution('VVN1', req);
            expect(res).toEqual({ message: 'Authorization token required' });
        });

        it('open creates execution when token present', async () => {
            const req: any = { user: { token: 't', emailAddress: 'me' } };
            (service as any).createVesselVisitExecution = jest.fn().mockResolvedValue({ vvn: 'VVN1' });
            const res = await controller.openVesselVisitExecution('VVN1', req);
            expect(res).toEqual({ vvn: 'VVN1' });
        });

        it('close returns execution', async () => {
            (service as any).closeVesselVisitExecution = jest.fn().mockResolvedValue({ closed: true });
            const res = await controller.closeVesselVisitExecution('VVN2');
            expect(res).toEqual({ closed: true });
        });

        it('updateBerthDetails returns execution', async () => {
            (service as any).updateBerthDetails = jest.fn().mockResolvedValue({ berthUpdated: true });
            const res = await controller.updateBerthDetails('VVN3', { dock: 'D1', berthTime: new Date().toISOString() } as any);
            expect(res).toEqual({ berthUpdated: true });
        });

        it('startOperation returns execution', async () => {
            (service as any).startOperation = jest.fn().mockResolvedValue({ started: true });
            const res = await controller.startOperation('VVN4', { op: 'x' } as any);
            expect(res).toEqual({ started: true });
        });

        it('completeOperation returns execution', async () => {
            (service as any).completeOperation = jest.fn().mockResolvedValue({ completed: true });
            const res = await controller.completeOperation('VVN5', 'op1', { endTime: new Date().toISOString() } as any);
            expect(res).toEqual({ completed: true });
        });

        it('getVesselVisitExecution returns 404 when not found', async () => {
            (service as any).getVesselVisitExecutionByVVN = jest.fn().mockResolvedValue(null);
            const res = await controller.getVesselVisitExecution('NA');
            expect(res).toEqual({ message: `Vessel Visit Execution with VVN NA not found.` });
        });

        it('getVeselVisitExecution by code returns 404 when not found', async () => {
            (service as any).getVesselVisitExecutionByCode = jest.fn().mockResolvedValue(null);
            const res = await controller.getVeselVisitExecution('CODEX');
            expect(res).toEqual({ message: `Vessel Visit Execution with code CODEX not found.` });
        });
    });

});
