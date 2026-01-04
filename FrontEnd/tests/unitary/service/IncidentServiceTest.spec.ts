import { describe, it, expect, vi, beforeEach } from 'vitest';
import { IncidentService } from '@/service/IncidentService';
import type { IHttpService, Response } from '@/service/IService/IHttpService';
import type { IncidentDto, CreateIncidentDto, UpdateIncidentDto } from '@/model/dto/IncidentDto';
import type { Page } from '@/model/Page';

describe('IncidentService', () => {
    let incidentService: IncidentService;
    let mockHttpService: IHttpService;

    const mockIncidentDto: IncidentDto = {
        type: 'INC001',
        startTime: '2024-01-01T10:00:00Z',
        endTime: '2024-01-01T12:00:00Z',
        severity: 'Major',
        description: 'Test incident',
        affectedVVECodes: ['VVE001']
    };

    const mockPage: Page<IncidentDto> = {
        items: [mockIncidentDto],
        pageNumber: 1,
        pageSize: 10,
        pageCount: 1,
    };

    beforeEach(() => {
        mockHttpService = {
            get: vi.fn(),
            getWithoutCredentials: vi.fn(),
            post: vi.fn(),
            put: vi.fn(),
            patch: vi.fn(),
            delete: vi.fn(),
        };

        incidentService = new IncidentService(mockHttpService);
    });

    describe('getAllIncidents', () => {
        it('should return page of incidents without filter', async () => {
            vi.mocked(mockHttpService.get).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: mockPage,
            } as Response<Page<IncidentDto>>);

            const result = await incidentService.getAllIncidents();

            expect(mockHttpService.get).toHaveBeenCalledWith('/oem/incidents?pageSize=1000');
            expect(result).toEqual(mockPage);
        });

        it('should return page of incidents with filter', async () => {
            vi.mocked(mockHttpService.get).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: mockPage,
            } as Response<Page<IncidentDto>>);

            const result = await incidentService.getAllIncidents({
                filter: { vveCode: 'VVE001', severity: 'Major' },
                pageNumber: 1,
                pageSize: 10
            });

            expect(mockHttpService.get).toHaveBeenCalledWith(expect.stringContaining('/oem/incidents?'));
            expect(result).toEqual(mockPage);
        });

        it('should throw error when getAllIncidents fails', async () => {
            vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Network error'));

            await expect(incidentService.getAllIncidents()).rejects.toThrow('Network error');
        });
    });

    describe('getIncidentByBid', () => {
        it('should return incident by bid', async () => {
            vi.mocked(mockHttpService.get).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: mockIncidentDto,
            } as Response<IncidentDto>);

            const result = await incidentService.getIncidentByBid('INC001');

            expect(mockHttpService.get).toHaveBeenCalledWith('/oem/incidents/INC001');
            expect(result).toEqual(mockIncidentDto);
        });

        it('should throw error when getIncidentByBid fails', async () => {
            vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Not found'));

            await expect(incidentService.getIncidentByBid('INC999')).rejects.toThrow('Not found');
        });
    });

    describe('createIncident', () => {
        it('should create incident successfully', async () => {
            const createDto: CreateIncidentDto = {
                type: 'INC001',
                startTime: '2024-01-01T10:00:00Z',
                severity: 'Major',
                description: 'New incident'
            };

            vi.mocked(mockHttpService.post).mockResolvedValue({
                status: 201,
                statusText: 'Created',
                data: mockIncidentDto,
            } as Response<IncidentDto>);

            const result = await incidentService.createIncident(createDto);

            expect(mockHttpService.post).toHaveBeenCalledWith('/oem/incidents', createDto);
            expect(result).toEqual(mockIncidentDto);
        });

        it('should throw error when createIncident fails', async () => {
            vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Creation failed'));

            await expect(incidentService.createIncident({} as CreateIncidentDto)).rejects.toThrow('Creation failed');
        });
    });

    describe('updateIncident', () => {
        it('should update incident successfully', async () => {
            const updateDto: UpdateIncidentDto = {
                endTime: '2024-01-01T13:00:00Z',
                description: 'Updated description'
            };

            vi.mocked(mockHttpService.put).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: mockIncidentDto,
            } as Response<IncidentDto>);

            const result = await incidentService.updateIncident('INC001', updateDto);

            expect(mockHttpService.put).toHaveBeenCalledWith('/oem/incidents/INC001', updateDto);
            expect(result).toEqual(mockIncidentDto);
        });

        it('should throw error when updateIncident fails', async () => {
            vi.mocked(mockHttpService.put).mockRejectedValue(new Error('Update failed'));

            await expect(incidentService.updateIncident('INC001', {} as UpdateIncidentDto)).rejects.toThrow('Update failed');
        });
    });

    describe('count', () => {
        it('should return count of incidents', async () => {
            vi.mocked(mockHttpService.get).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: { count: 42 },
            } as Response<{count: number}>);

            const result = await incidentService.count();

            expect(mockHttpService.get).toHaveBeenCalledWith('/oem/incidents/count');
            expect(result).toEqual({ count: 42 });
        });
    });
});
