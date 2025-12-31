import { CreateIncidentDto, UpdateIncidentDto } from "../../../src/dto/incidentDto";
import { IncidentService } from "../../../src/services/incidentService";
import Incident from "../../../src/domain/incident";
import IncidentType from "../../../src/domain/incidentType";
import { Page } from "../../../src/utils/page";
import { NotFoundError } from "../../../src/core/infra/extraErrors";

describe('IncidentService', () => {
    let service: IncidentService;
    let mockRepo: any;
    let mockTypeRepo: any;
    let mockVveRepo: any;

    beforeEach(() => {
        jest.clearAllMocks();

        service = new IncidentService();

        mockRepo = {
            count: jest.fn(),
            getPaged: jest.fn(),
            getByBid: jest.fn(),
            save: jest.fn(),
            update: jest.fn()
        };

        mockTypeRepo = {
            getById: jest.fn()
        };

        mockVveRepo = {
            getByCode: jest.fn()
        };

        (service as any).incidentRepository = mockRepo;
        (service as any).incidentTypeRepository = mockTypeRepo;
        (service as any).vveRepository = mockVveRepo;
    });

    describe('count', () => {
        it('forwards to repository', async () => {
            mockRepo.count.mockResolvedValue(7);
            const result = await service.count();
            expect(mockRepo.count).toHaveBeenCalled();
            expect(result).toBe(7);
        });
    });

    describe('getPaged', () => {
        it('returns mapped page', async () => {
            const incident = new Incident({
                type: new IncidentType({ name: 'T', description: 'd', severity: 'Minor' }),
                startTime: new Date(),
                severity: 'Minor',
                description: 'd',
                createdBy: 'u'
            });

            const mockPage: Page<Incident> = { pageNumber: 1, pageSize: 10, pageCount: 1, items: [incident] };
            mockRepo.getPaged.mockResolvedValue(mockPage);

            const result = await service.getPaged({ pageNumber: 1, pageSize: 10 } as any);
            expect(mockRepo.getPaged).toHaveBeenCalled();
            expect(result.items.length).toBe(1);
        });
    });

    describe('getByBid', () => {
        it('returns incident dto when found', async () => {
            const incident = new Incident({
                type: new IncidentType({ name: 'T', description: 'd', severity: 'Minor' }),
                startTime: new Date('2025-03-01T10:00:00Z'),
                severity: 'Minor',
                description: 'd',
                createdBy: 'u'
            });
            mockRepo.getByBid.mockResolvedValue(incident);

            const result = await service.getByBid('INC-1');
            expect(mockRepo.getByBid).toHaveBeenCalledWith('INC-1');
            expect(result).toBeDefined();
            expect(result.description).toBe('d');
        });
    });

    describe('create', () => {
        it('should save incident when type exists', async () => {
            const dto: CreateIncidentDto = {
                type: 'TYPE-1',
                startTime: '2025-04-01T00:00:00Z',
                severity: 'Minor',
                description: 'new'
            } as any;

            const type = new IncidentType({ name: 'T', description: 'd', severity: 'Minor' });
            const saved = new Incident({ type: type, startTime: new Date(dto.startTime), severity: 'Minor', description: 'new', createdBy: 'me' });

            mockTypeRepo.getById.mockResolvedValue(type);
            mockRepo.save.mockResolvedValue(saved);

            const result = await service.create(dto, 'me');
            expect(mockTypeRepo.getById).toHaveBeenCalledWith('TYPE-1');
            expect(mockRepo.save).toHaveBeenCalled();
            expect(result.description).toBe('new');
        });

        it('throws when type not found', async () => {
            const dto: CreateIncidentDto = { type: 'MISSING', startTime: '2025-04-01T00:00:00Z', severity: 'Minor', description: 'x' } as any;
            mockTypeRepo.getById.mockRejectedValue(new Error('Type not found'));

            await expect(service.create(dto, 'me')).rejects.toThrow();
        });
    });

    describe('update', () => {
        it('throws when new startTime after endTime', async () => {
            const existing = new Incident({
                type: new IncidentType({ name: 'T', description: 'd', severity: 'Minor' }),
                startTime: new Date('2025-05-01T08:00:00Z'),
                endTime: new Date('2025-05-01T10:00:00Z'),
                severity: 'Minor',
                description: 'd',
                createdBy: 'u'
            } as any);

            mockRepo.getByBid.mockResolvedValue(existing);

            await expect(service.update('INC-1', { startTime: '2025-05-01T11:00:00Z' } as any)).rejects.toThrow();
        });

        it('processes affectedVVECodes and throws for invalid or closed', async () => {
            const existing = new Incident({
                type: new IncidentType({ name: 'T', description: 'd', severity: 'Minor' }),
                startTime: new Date('2025-06-01T08:00:00Z'),
                severity: 'Minor',
                description: 'd',
                createdBy: 'u'
            } as any);

            mockRepo.getByBid.mockResolvedValue(existing);

            // invalid code (empty)
            await expect(service.update('INC-1', { affectedVVECodes: [''] } as any)).rejects.toThrow();

            // vve not found
            mockVveRepo.getByCode.mockResolvedValue(null);
            await expect(service.update('INC-1', { affectedVVECodes: ['VVE-X'] } as any)).rejects.toThrow();

            // closed vve
            mockVveRepo.getByCode.mockResolvedValue({ code: 'VVE-Y', status: 'Closed' });
            await expect(service.update('INC-1', { affectedVVECodes: ['VVE-Y'] } as any)).rejects.toThrow();
        });
    });

});
