import { IncidentTypeController } from "../../src/controllers/incidentTypeController";
import { IncidentTypeService } from "../../src/services/incidentTypeService";
import IncidentType from "../../src/domain/incidentType";
import { NotFoundError } from "../../src/core/infra/extraErrors";

describe('IncidentType Controller->Service integration', () => {
    let controller: IncidentTypeController;
    let service: IncidentTypeService;
    let mockRepo: any;

    beforeEach(() => {
        service = new IncidentTypeService();
        controller = new IncidentTypeController();

        mockRepo = { count: jest.fn(), getPaged: jest.fn(), getById: jest.fn(), save: jest.fn(), update: jest.fn(), deleteById: jest.fn() };
        (service as any).incidentTypeRepository = mockRepo;
        (controller as any).incidentTypeService = service;
    });

    describe('count', () => {
        it('forwards to service', async () => {
            mockRepo.count.mockResolvedValue(2);
            const res = await controller.count();
            expect(res).toEqual({ count: 2 });
        });
    });

    describe('getPaged', () => {
        it('returns paged result', async () => {
            const page = { pageNumber: 1, pageSize: 10, pageCount: 1, items: [] };
            mockRepo.getPaged.mockResolvedValue(page);
            const res = await controller.getPaged(1, 10);
            expect((mockRepo.getPaged).mock.calls.length).toBeGreaterThanOrEqual(1);
            expect(res).toEqual(page);
        });
    });

    describe('getById', () => {
        it('returns item when found', async () => {
            const dto = { id: 'IT-1', name: 'n' };
            (service as any).getById = jest.fn().mockResolvedValue(dto);
            const res = await controller.getById('IT-1');
            expect(res).toEqual(dto);
        });

        it('handles not found error', async () => {
            const err = new NotFoundError('not found');
            (service as any).getById = jest.fn().mockRejectedValue(err);
            const res = await controller.getById('NA');
            expect(res).toEqual({ message: err.message });
        });
    });

    describe('createIncidentType', () => {
        it('creates type', async () => {
            const body = { name: 'x', description: 'd', severity: 'Minor' } as any;
            const saved = { id: 'IT-1', ...body };
            mockRepo.save = jest.fn().mockResolvedValue(saved);
            (service as any).create = jest.fn().mockImplementation((b: any) => mockRepo.save(b));

            const res = await controller.createIncidentType(body);
            expect((service as any).create).toHaveBeenCalledWith(body);
            expect(res).toEqual(saved);
        });
    });

    describe('updateIncidentType', () => {
        it('returns not found when update yields null', async () => {
            (service as any).update = jest.fn().mockResolvedValue(null);
            const res = await controller.updateIncidentType('NA', { name: 'u' } as any);
            expect(res).toEqual({ message: 'Incident type not found' });
        });

        it('returns updated type', async () => {
            (service as any).update = jest.fn().mockResolvedValue({ id: 'IT-1', name: 'u' });
            const res = await controller.updateIncidentType('IT-1', { name: 'u' } as any);
            expect(res).toEqual({ id: 'IT-1', name: 'u' });
        });
    });

});
