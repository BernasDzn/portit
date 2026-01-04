import { describe, it, expect, vi, beforeEach } from 'vitest';
import { IncidentTypeService } from '@/service/IncidentTypeService';
import type { IHttpService, Response } from '@/service/IService/IHttpService';
import type { IncidentTypeDto, PartialIncidentTypeDto } from '@/model/dto/IncidentTypeDto';
import type { Page } from '@/model/Page';

describe('IncidentTypeService', () => {
    let incidentTypeService: IncidentTypeService;
    let mockHttpService: IHttpService;

    const mockIncidentTypeDto: IncidentTypeDto = {
        bid: 'IT001',
        name: 'Equipment Failure',
        description: 'Equipment malfunction or failure',
        severity: 'Major',
        subtypeOf: undefined,
        subtypes: []
    };

    const mockPage: Page<IncidentTypeDto> = {
        items: [mockIncidentTypeDto],
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

        incidentTypeService = new IncidentTypeService(mockHttpService);
    });

    describe('getAllIncidentTypes', () => {
        it('should return page of incident types without filter', async () => {
            vi.mocked(mockHttpService.get).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: mockPage,
            } as Response<Page<IncidentTypeDto>>);

            const result = await incidentTypeService.getAllIncidentTypes();

            expect(mockHttpService.get).toHaveBeenCalledWith('/oem/incident-types?pageSize=1000');
            expect(result).toEqual(mockPage);
        });

        it('should return page of incident types with filter', async () => {
            vi.mocked(mockHttpService.get).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: mockPage,
            } as Response<Page<IncidentTypeDto>>);

            const result = await incidentTypeService.getAllIncidentTypes({
                filter: { name: 'Equipment', severity: 'Major' },
                pageNumber: 1,
                pageSize: 10
            });

            expect(mockHttpService.get).toHaveBeenCalledWith(expect.stringContaining('/oem/incident-types?'));
            expect(result).toEqual(mockPage);
        });

        it('should throw error when getAllIncidentTypes fails', async () => {
            vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Network error'));

            await expect(incidentTypeService.getAllIncidentTypes()).rejects.toThrow('Network error');
        });
    });

    describe('getIncidentTypeById', () => {
        it('should return incident type by id', async () => {
            vi.mocked(mockHttpService.get).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: mockIncidentTypeDto,
            } as Response<IncidentTypeDto>);

            const result = await incidentTypeService.getIncidentTypeById('IT001');

            expect(mockHttpService.get).toHaveBeenCalledWith('/oem/incident-types/IT001');
            expect(result).toEqual(mockIncidentTypeDto);
        });

        it('should throw error when getIncidentTypeById fails', async () => {
            vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Not found'));

            await expect(incidentTypeService.getIncidentTypeById('IT999')).rejects.toThrow('Not found');
        });
    });

    describe('createIncidentType', () => {
        it('should create incident type successfully', async () => {
            const createDto: PartialIncidentTypeDto = {
                name: 'New Type',
                description: 'New incident type',
                severity: 'Minor'
            };

            vi.mocked(mockHttpService.post).mockResolvedValue({
                status: 201,
                statusText: 'Created',
                data: mockIncidentTypeDto,
            } as Response<IncidentTypeDto>);

            const result = await incidentTypeService.createIncidentType(createDto);

            expect(mockHttpService.post).toHaveBeenCalledWith('/oem/incident-types', createDto);
            expect(result).toEqual(mockIncidentTypeDto);
        });

        it('should throw error when createIncidentType fails', async () => {
            vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Creation failed'));

            await expect(incidentTypeService.createIncidentType({} as PartialIncidentTypeDto)).rejects.toThrow('Creation failed');
        });
    });

    describe('updateIncidentType', () => {
        it('should update incident type successfully', async () => {
            const updateDto: PartialIncidentTypeDto = {
                description: 'Updated description',
                severity: 'Critical'
            };

            vi.mocked(mockHttpService.put).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: mockIncidentTypeDto,
            } as Response<IncidentTypeDto>);

            const result = await incidentTypeService.updateIncidentType('IT001', updateDto);

            expect(mockHttpService.put).toHaveBeenCalledWith('/oem/incident-types/IT001', updateDto);
            expect(result).toEqual(mockIncidentTypeDto);
        });

        it('should throw error when updateIncidentType fails', async () => {
            vi.mocked(mockHttpService.put).mockRejectedValue(new Error('Update failed'));

            await expect(incidentTypeService.updateIncidentType('IT001', {} as PartialIncidentTypeDto)).rejects.toThrow('Update failed');
        });
    });

    describe('deleteIncidentType', () => {
        it('should delete incident type successfully', async () => {
            vi.mocked(mockHttpService.delete).mockResolvedValue({
                status: 204,
                statusText: 'No Content',
                data: undefined,
            } as Response<void>);

            await incidentTypeService.deleteIncidentType('IT001');

            expect(mockHttpService.delete).toHaveBeenCalledWith('/oem/incident-types/IT001');
        });

        it('should throw error when deleteIncidentType fails', async () => {
            vi.mocked(mockHttpService.delete).mockRejectedValue(new Error('Delete failed'));

            await expect(incidentTypeService.deleteIncidentType('IT001')).rejects.toThrow('Delete failed');
        });
    });

    describe('count', () => {
        it('should return count of incident types', async () => {
            vi.mocked(mockHttpService.get).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: { count: 15 },
            } as Response<{count: number}>);

            const result = await incidentTypeService.count();

            expect(mockHttpService.get).toHaveBeenCalledWith('/oem/incident-types/count');
            expect(result).toEqual({ count: 15 });
        });
    });
});
