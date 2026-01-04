import { describe, it, expect, vi, beforeEach } from 'vitest';
import { OperationPlanService } from '@/service/OperationPlanService';
import type { IHttpService, Response } from '@/service/IService/IHttpService';
import type { OperationPlanDto } from '@/model/dto/OperationPlanDto';
import type { Page } from '@/model/Page';

describe('OperationPlanService', () => {
    let operationPlanService: OperationPlanService;
    let mockHttpService: IHttpService;

    const mockOperationPlanDto: OperationPlanDto = {
        id: 'OP001',
        vesselVisitCode: 'VV001',
        dock: 'DOCK01',
        estimatedStartTime: '2024-01-01T08:00:00Z',
        estimatedEndTime: '2024-01-01T16:00:00Z',
        operations: []
    };

    const mockPage: Page<OperationPlanDto> = {
        items: [mockOperationPlanDto],
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

        operationPlanService = new OperationPlanService(mockHttpService);
    });

    describe('groupOperationPlansByDate', () => {
        it('should return operation plans grouped by date', async () => {
            const mockGroupedPlans = [
                { date: '2024-01-01', plans: [mockOperationPlanDto] },
                { date: '2024-01-02', plans: [] }
            ];

            vi.mocked(mockHttpService.get).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: mockGroupedPlans,
            } as Response<{ date: string; plans: OperationPlanDto[] }[]>);

            const result = await operationPlanService.groupOperationPlansByDate();

            expect(mockHttpService.get).toHaveBeenCalledWith('/oem/operation-plans/by-date');
            expect(result).toEqual(mockGroupedPlans);
        });

        it('should throw error when groupOperationPlansByDate fails', async () => {
            vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Network error'));

            await expect(operationPlanService.groupOperationPlansByDate()).rejects.toThrow('Network error');
        });
    });

    describe('getOperationPlanById', () => {
        it('should return operation plan by id', async () => {
            vi.mocked(mockHttpService.get).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: mockOperationPlanDto,
            } as Response<OperationPlanDto>);

            const result = await operationPlanService.getOperationPlanById('OP001');

            expect(mockHttpService.get).toHaveBeenCalledWith('/oem/operation-plans/OP001');
            expect(result).toEqual(mockOperationPlanDto);
        });

        it('should throw error when getOperationPlanById fails', async () => {
            vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Not found'));

            await expect(operationPlanService.getOperationPlanById('OP999')).rejects.toThrow('Not found');
        });
    });

    describe('getAllOperationPlans', () => {
        it('should return page of operation plans without filter', async () => {
            vi.mocked(mockHttpService.get).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: mockPage,
            } as Response<Page<OperationPlanDto>>);

            const result = await operationPlanService.getAllOperationPlans();

            expect(mockHttpService.get).toHaveBeenCalledWith('/oem/operation-plans');
            expect(result).toEqual(mockPage);
        });

        it('should return page of operation plans with filter', async () => {
            vi.mocked(mockHttpService.get).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: mockPage,
            } as Response<Page<OperationPlanDto>>);

            const result = await operationPlanService.getAllOperationPlans({
                filter: { 
                    startDate: '2024-01-01',
                    endDate: '2024-01-31'
                },
                pageNumber: 1,
                pageSize: 10
            });

            expect(mockHttpService.get).toHaveBeenCalledWith(expect.stringContaining('/oem/operation-plans?'));
            expect(result).toEqual(mockPage);
        });

        it('should throw error when getAllOperationPlans fails', async () => {
            vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Network error'));

            await expect(operationPlanService.getAllOperationPlans()).rejects.toThrow('Network error');
        });
    });

    describe('updateOperationPlan', () => {
        it('should update operation plan successfully', async () => {
            const updates: Partial<OperationPlanDto> = {
                estimatedEndTime: '2024-01-01T18:00:00Z'
            };

            vi.mocked(mockHttpService.patch).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: { ...mockOperationPlanDto, ...updates },
            } as Response<OperationPlanDto>);

            const result = await operationPlanService.updateOperationPlan('OP001', updates);

            expect(mockHttpService.patch).toHaveBeenCalledWith('/oem/operation-plans/OP001', updates);
            expect(result.estimatedEndTime).toEqual(updates.estimatedEndTime);
        });

        it('should throw error when updateOperationPlan fails', async () => {
            vi.mocked(mockHttpService.patch).mockRejectedValue(new Error('Update failed'));

            await expect(operationPlanService.updateOperationPlan('OP001', {})).rejects.toThrow('Update failed');
        });
    });
});
