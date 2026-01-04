import { describe, it, expect, vi, beforeEach } from 'vitest';
import { VesselVisitExecutionService } from '@/service/VesselExecutionService';
import type { IHttpService, Response } from '@/service/IService/IHttpService';
import type { VesselVisitExecution } from '@/model/VesselVisitExecution';
import type { Page } from '@/model/Page';

describe('VesselVisitExecutionService', () => {
    let vesselExecutionService: VesselVisitExecutionService;
    let mockHttpService: IHttpService;

    const mockVesselVisitExecution: VesselVisitExecution = {
        id: 'VVE001',
        code: 'VVE-2024-001',
        relatedVVN: 'VVN001',
        operationsExecuted: [],
        dateOpen: new Date('2024-01-01'),
        status: 'Open',
        createdBy: 'user@example.com'
    } as VesselVisitExecution;

    const mockPage: Page<VesselVisitExecution> = {
        items: [mockVesselVisitExecution],
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

        vesselExecutionService = new VesselVisitExecutionService(mockHttpService);
    });

    describe('count', () => {
        it('should return count of vessel visit executions', async () => {
            vi.mocked(mockHttpService.get).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: { count: 42 },
            } as Response<{count: number}>);

            const result = await vesselExecutionService.count();

            expect(mockHttpService.get).toHaveBeenCalledWith('/oem/vessel-visit-executions/count');
            expect(result).toBe(42);
        });

        it('should throw error when count fails', async () => {
            vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Network error'));

            await expect(vesselExecutionService.count()).rejects.toThrow('Network error');
        });
    });

    describe('openVesselVisitExecution', () => {
        it('should open vessel visit execution successfully', async () => {
            vi.mocked(mockHttpService.post).mockResolvedValue({
                status: 201,
                statusText: 'Created',
                data: mockVesselVisitExecution,
            } as Response<VesselVisitExecution>);

            const result = await vesselExecutionService.openVesselVisitExecution('VVN001');

            expect(mockHttpService.post).toHaveBeenCalledWith(
                '/oem/vessel-visit-executions/VVN001/open',
                {}
            );
            expect(result).toEqual(mockVesselVisitExecution);
        });

        it('should throw error when openVesselVisitExecution fails', async () => {
            vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Open failed'));

            await expect(vesselExecutionService.openVesselVisitExecution('VVN001')).rejects.toThrow('Open failed');
        });
    });

    describe('closeVesselVisitExecution', () => {
        it('should close vessel visit execution successfully', async () => {
            const closedExecution = { ...mockVesselVisitExecution, status: 'Closed', dateClosed: new Date() };

            vi.mocked(mockHttpService.put).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: closedExecution,
            } as Response<VesselVisitExecution>);

            const result = await vesselExecutionService.closeVesselVisitExecution('VVN001');

            expect(mockHttpService.put).toHaveBeenCalledWith(
                '/oem/vessel-visit-executions/VVN001/close',
                {}
            );
            expect(result).toEqual(closedExecution);
        });

        it('should throw error when closeVesselVisitExecution fails', async () => {
            vi.mocked(mockHttpService.put).mockRejectedValue(new Error('Close failed'));

            await expect(vesselExecutionService.closeVesselVisitExecution('VVN001')).rejects.toThrow('Close failed');
        });
    });

    describe('getVesselVisitExecutionByVVN', () => {
        it('should return vessel visit execution by VVN', async () => {
            vi.mocked(mockHttpService.get).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: mockVesselVisitExecution,
            } as Response<VesselVisitExecution>);

            const result = await vesselExecutionService.getVesselVisitExecutionByVVN('VVN001');

            expect(mockHttpService.get).toHaveBeenCalledWith('/oem/vessel-visit-executions/VVN001');
            expect(result).toEqual(mockVesselVisitExecution);
        });

        it('should throw error when getVesselVisitExecutionByVVN fails', async () => {
            vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Not found'));

            await expect(vesselExecutionService.getVesselVisitExecutionByVVN('VVN999')).rejects.toThrow('Not found');
        });
    });

    describe('getAllVesselVisitExecutions', () => {
        it('should return page of vessel visit executions without filter', async () => {
            vi.mocked(mockHttpService.get).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: mockPage,
            } as Response<Page<VesselVisitExecution>>);

            const result = await vesselExecutionService.getAllVesselVisitExecutions();

            expect(mockHttpService.get).toHaveBeenCalledWith('/oem/vessel-visit-executions');
            expect(result).toEqual(mockPage);
        });

        it('should return page of vessel visit executions with filter', async () => {
            vi.mocked(mockHttpService.get).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: mockPage,
            } as Response<Page<VesselVisitExecution>>);

            const result = await vesselExecutionService.getAllVesselVisitExecutions({
                filter: {
                    startDate: '2024-01-01',
                    endDate: '2024-01-31',
                    status: 'Open'
                },
                pageNumber: 1,
                pageSize: 10
            });

            expect(mockHttpService.get).toHaveBeenCalledWith(expect.stringContaining('/oem/vessel-visit-executions?'));
            expect(result).toEqual(mockPage);
        });

        it('should throw error when getAllVesselVisitExecutions fails', async () => {
            vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Network error'));

            await expect(vesselExecutionService.getAllVesselVisitExecutions()).rejects.toThrow('Network error');
        });
    });
});
