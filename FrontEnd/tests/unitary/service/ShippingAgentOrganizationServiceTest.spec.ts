import { describe, it, expect, vi, beforeEach } from 'vitest';
import { ShippingAgentOrganizationService } from '@/service/ShippingAgentOrganizationService';
import type { IHttpService, Response } from '@/service/IService/IHttpService';
import type { ShippingAgentOrganization } from '@/model/ShippingAgentOrganization';
import type { Page } from '@/model/Page';

describe('ShippingAgentOrganizationService', () => {
    let shippingAgentOrgService: ShippingAgentOrganizationService;
    let mockHttpService: IHttpService;

    const mockShippingAgentOrg: ShippingAgentOrganization = {
        name: 'Global Shipping Co.',
        altNames: ['GSC', 'GlobalShip'],
        taxNumber: '123456789',
        address: {
            id: undefined,
            street: '123 Harbor St',
            city: 'Port City',
            zipCode: '12345',
            country: 'USA'
        },
        representatives: [
            {
                name: 'Jane Smith',
                citizenshipId: '987654321',
                emailAddress: 'jane@globalship.com',
                phone: '+1987654321'
            }
        ]
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

        shippingAgentOrgService = new ShippingAgentOrganizationService(mockHttpService);
    });

    describe('getShippingAgentOrganizations', () => {
        it('should return page of shipping agent organizations', async () => {
            vi.mocked(mockHttpService.get).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: [mockShippingAgentOrg],
            } as Response<ShippingAgentOrganization[]>);

            const result = await shippingAgentOrgService.getShippingAgentOrganizations();

            expect(mockHttpService.get).toHaveBeenCalledWith('/api/ShippingAgentOrganization');
            expect(result).toEqual({
                items: [mockShippingAgentOrg],
                pageNumber: 1,
                pageSize: 1,
                pageCount: 1
            });
        });

        it('should return empty page when no organizations exist', async () => {
            vi.mocked(mockHttpService.get).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: [],
            } as Response<ShippingAgentOrganization[]>);

            const result = await shippingAgentOrgService.getShippingAgentOrganizations();

            expect(result.items).toEqual([]);
            expect(result.pageSize).toBe(0);
        });

        it('should throw error when getShippingAgentOrganizations fails', async () => {
            vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Network error'));

            await expect(shippingAgentOrgService.getShippingAgentOrganizations()).rejects.toThrow('Network error');
        });
    });
});
