import { describe, it, expect, vi, beforeEach } from 'vitest';
import { RepresentativeService } from '@/service/RepresentativeService';
import type { IHttpService, Response } from '@/service/IService/IHttpService';
import type { Representative } from '@/model/Representative';
import type { Page } from '@/model/Page';

describe('RepresentativeService', () => {
    let representativeService: RepresentativeService;
    let mockHttpService: IHttpService;

    const mockRepresentative: Representative = {
        name: 'John Doe',
        citizenshipId: '123456789',
        emailAddress: 'john@example.com',
        phone: '+1234567890'
    };

    const mockPage: Page<Representative> = {
        items: [mockRepresentative],
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

        representativeService = new RepresentativeService(mockHttpService);
    });

    describe('getAll', () => {
        it('should return page of representatives', async () => {
            vi.mocked(mockHttpService.get).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: mockPage,
            } as Response<Page<Representative>>);

            const result = await representativeService.getAll();

            expect(mockHttpService.get).toHaveBeenCalledWith('/api/Representative');
            expect(result).toEqual(mockPage);
        });

        it('should throw error when getAll fails', async () => {
            vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Network error'));

            await expect(representativeService.getAll()).rejects.toThrow('Network error');
        });
    });

    describe('getByEmail', () => {
        it('should return representative by email', async () => {
            vi.mocked(mockHttpService.get).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: mockRepresentative,
            } as Response<Representative>);

            const result = await representativeService.getByEmail('john@example.com');

            expect(mockHttpService.get).toHaveBeenCalledWith('/api/Representative/email/john%40example.com');
            expect(result).toEqual(mockRepresentative);
        });

        it('should throw error when getByEmail fails', async () => {
            vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Not found'));

            await expect(representativeService.getByEmail('unknown@example.com')).rejects.toThrow('Not found');
        });
    });

    describe('getByCitizenId', () => {
        it('should return representative by citizen ID', async () => {
            vi.mocked(mockHttpService.get).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: mockRepresentative,
            } as Response<Representative>);

            const result = await representativeService.getByCitizenId('123456789');

            expect(mockHttpService.get).toHaveBeenCalledWith('/api/Representative/citizen/123456789');
            expect(result).toEqual(mockRepresentative);
        });

        it('should throw error when getByCitizenId fails', async () => {
            vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Not found'));

            await expect(representativeService.getByCitizenId('999999999')).rejects.toThrow('Not found');
        });
    });
});
