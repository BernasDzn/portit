import { OperationPlanController } from "../../src/controllers/operationPlanController";
import { OperationPlanService } from "../../src/services/operationPlanService";

describe('OperationPlan Controller->Service integration', () => {
    let controller: OperationPlanController;
    let service: OperationPlanService;
    let mockRepo: any;

    beforeEach(() => {
        service = new OperationPlanService();
        controller = new OperationPlanController();

        mockRepo = { count: jest.fn(), getPaged: jest.fn(), getById: jest.fn(), save: jest.fn(), update: jest.fn() };
        (service as any).operationPlanRepository = mockRepo;
        (controller as any).operationPlanService = service;
    });

    describe('getPlans', () => {
        it('forwards to service and returns plans', async () => {
            const page = { pageNumber: 1, pageSize: 10, pageCount: 1, items: [] };
            mockRepo.getAll = jest.fn().mockResolvedValue(page);
            (service as any).getAll = jest.fn().mockImplementation((f: any) => mockRepo.getAll(f));
            const res = await controller.getPlans(1, 10);
            expect(res).toEqual(page);
        });
    });

    describe('getPlansByDate', () => {
        it('returns grouped plans from service', async () => {
            const grouped = [{ date: '2025-01-01', plans: [] }];
            (service as any).getByDateGrouped = jest.fn().mockResolvedValue(grouped);
            const res = await controller.getPlansByDate();
            expect(res).toEqual(grouped);
        });
    });

    describe('notifications and regeneration', () => {
        it('returns 401 when no token for notifications', async () => {
            const req: any = { headers: {} };
            const res = await controller.getNotificationWithoutPlan(req);
            expect(res).toEqual({ message: 'Authorization token required' });
        });

        it('calls service when token present for notifications', async () => {
            const req: any = { headers: { authorization: 'Bearer tok' } };
            (service as any).getNotificationsWithoutPlan = jest.fn().mockResolvedValue([]);
            const res = await controller.getNotificationWithoutPlan(req);
            expect(res).toEqual([]);
        });

        it('regeneratePlansForDay validates body and token', async () => {
            const badReq: any = { headers: {} };
            const badBody = { day: '', algorithm: '' };
            const resBad = await controller.regeneratePlansForDay(badBody as any, badReq);
            expect(resBad).toEqual({ message: 'Missing required parameters: day and algorithm' });

            const req: any = { headers: { authorization: 'Bearer tok' } };
            const body = { day: '2025-01-01', algorithm: 'alg', daysAhead: 2 };
            (service as any).regeneratePlansForDay = jest.fn().mockResolvedValue({ ok: true });
            const res = await controller.regeneratePlansForDay(body as any, req);
            expect(res).toEqual({ ok: true });
        });
    });

    describe('getPlanById and updateOperationPlan', () => {
        it('returns 404 when plan not found', async () => {
            (service as any).getById = jest.fn().mockResolvedValue(null);
            const res = await controller.getPlanById('NOTFOUND');
            expect(res).toEqual({ message: 'Operation plan not found' });
        });

        it('returns plan when found', async () => {
            const plan = { id: 'P1' };
            (service as any).getById = jest.fn().mockResolvedValue(plan);
            const res = await controller.getPlanById('P1');
            expect(res).toEqual(plan);
        });

        it('updateOperationPlan returns 404 when not found', async () => {
            (service as any).updateOperationPlan = jest.fn().mockResolvedValue(null);
            const res = await controller.updateOperationPlan('NA', {} as any);
            expect(res).toEqual({ message: 'Operation plan not found for update' });
        });

        it('updateOperationPlan returns updated plan', async () => {
            (service as any).updateOperationPlan = jest.fn().mockResolvedValue({ id: 'P2', updated: true });
            const res = await controller.updateOperationPlan('P2', { some: 'data' } as any);
            expect(res).toEqual({ id: 'P2', updated: true });
        });
    });

});
