import { SchedulingRequestController } from "../../src/controllers/schedulingRequestController";
import { SchedulingRequestService } from "../../src/services/schedulingRequestService";

describe('SchedulingRequest Controller->Service integration', () => {
    let controller: SchedulingRequestController;
    let service: SchedulingRequestService;
    let mockRepo: any;

    beforeEach(() => {
        service = new SchedulingRequestService();
        controller = new SchedulingRequestController();

        mockRepo = { count: jest.fn(), getPaged: jest.fn(), getById: jest.fn(), save: jest.fn(), update: jest.fn() };
        (service as any).schedulingRequestRepository = mockRepo;
        (controller as any).schedulingRequestService = service;
    });

    describe('getQueueState', () => {
        it('forwards to service', async () => {
            mockRepo.getQueueState = jest.fn().mockResolvedValue({ queued: 0 });
            (service as any).getQueueState = jest.fn().mockImplementation(() => mockRepo.getQueueState());
            const res = await controller.getQueueState();
            expect(res).toEqual({ queued: 0 });
        });
    });

    describe('scheduleRequest', () => {
        it('returns 400 when missing params', async () => {
            const req: any = { user: { token: 't', emailAddress: 'me' } };
            const res = await controller.scheduleRequest(undefined as any, undefined as any, 1, req);
            expect(res).toEqual({ message: 'Missing required query parameters: day and alg' });
        });

        it('calls service when params present and token present', async () => {
            const req: any = { user: { token: 't', emailAddress: 'me' } };
            (service as any).scheduleRequest = jest.fn().mockResolvedValue({ scheduled: true });
            const res = await controller.scheduleRequest('2025-01-01', 'alg', 1, req);
            expect(res).toEqual({ scheduled: true });
        });
    });

    describe('acceptRequest / rejectRequest', () => {
        it('acceptRequest returns 400 when id missing', async () => {
            const req: any = { user: { token: 't', emailAddress: 'me' } };
            const res = await controller.acceptRequest(undefined as any, req);
            expect(res).toEqual({ message: 'Missing required query parameter: id' });
        });

        it('acceptRequest calls service with user token', async () => {
            const req: any = { user: { token: 't', emailAddress: 'me' } };
            (service as any).acceptRequest = jest.fn().mockResolvedValue({ accepted: true });
            const res = await controller.acceptRequest('req-1', req);
            expect(res).toEqual({ accepted: true });
        });

        it('rejectRequest returns 400 when id missing', async () => {
            const req: any = { user: { token: 't', emailAddress: 'me' } };
            const res = await controller.rejectRequest(undefined as any, req);
            expect(res).toEqual({ message: 'Missing required query parameter: id' });
        });

        it('rejectRequest calls service with user token', async () => {
            const req: any = { user: { token: 't', emailAddress: 'me' } };
            (service as any).rejectRequest = jest.fn().mockResolvedValue({ rejected: true });
            const res = await controller.rejectRequest('req-2', req);
            expect(res).toEqual({ rejected: true });
        });
    });

});
