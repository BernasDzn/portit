import { describe, it, expect, vi, beforeEach } from 'vitest';
import { PrivacyPolicyService } from '@/service/PrivacyPoliceService';
import type { IHttpService, Response } from '@/service/IService/IHttpService';
import type { PrivacyPolicy } from '@/model/PrivacyPolicy';

describe('PrivacyPolicyService', () => {
    let privacyPolicyService: PrivacyPolicyService;
    let mockHttpService: IHttpService;

    const mockPrivacyPolicy: PrivacyPolicy = {
        content: 'This is the privacy policy content',
        updatedOn: new Date('2024-01-01'),
        active: true,
        isActive: () => true,
        updateContent: vi.fn(),
        deactivate: vi.fn(),
        toDto: () => ({ content: 'This is the privacy policy content' })
    } as unknown as PrivacyPolicy;

    beforeEach(() => {
        mockHttpService = {
            get: vi.fn(),
            getWithoutCredentials: vi.fn(),
            post: vi.fn(),
            put: vi.fn(),
            patch: vi.fn(),
            delete: vi.fn(),
        };

        privacyPolicyService = new PrivacyPolicyService(mockHttpService);
    });

    describe('getActivePrivacyPolicy', () => {
        it('should return active privacy policy', async () => {
            vi.mocked(mockHttpService.get).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: mockPrivacyPolicy,
            } as Response<PrivacyPolicy>);

            const result = await privacyPolicyService.getActivePrivacyPolicy();

            expect(mockHttpService.get).toHaveBeenCalledWith('/api/PrivacyPolicy/active');
            expect(result).toEqual(mockPrivacyPolicy);
        });

        it('should throw error when getActivePrivacyPolicy fails', async () => {
            vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Not found'));

            await expect(privacyPolicyService.getActivePrivacyPolicy()).rejects.toThrow('Not found');
        });
    });

    describe('getAllPrivacyPolicies', () => {
        it('should return list of all privacy policies', async () => {
            vi.mocked(mockHttpService.get).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: [mockPrivacyPolicy],
            } as Response<PrivacyPolicy[]>);

            const result = await privacyPolicyService.getAllPrivacyPolicies();

            expect(mockHttpService.get).toHaveBeenCalledWith('/api/PrivacyPolicy');
            expect(result).toEqual([mockPrivacyPolicy]);
        });

        it('should throw error when getAllPrivacyPolicies fails', async () => {
            vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Network error'));

            await expect(privacyPolicyService.getAllPrivacyPolicies()).rejects.toThrow('Network error');
        });
    });

    describe('updatePrivacyPolicy', () => {
        it('should update privacy policy successfully', async () => {
            vi.mocked(mockHttpService.post).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: mockPrivacyPolicy,
            } as Response<PrivacyPolicy>);

            const result = await privacyPolicyService.updatePrivacyPolicy(mockPrivacyPolicy);

            expect(mockHttpService.post).toHaveBeenCalledWith(
                '/api/PrivacyPolicy',
                mockPrivacyPolicy.toDto()
            );
            expect(result).toEqual(mockPrivacyPolicy);
        });

        it('should throw error when updatePrivacyPolicy fails', async () => {
            vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Update failed'));

            await expect(privacyPolicyService.updatePrivacyPolicy(mockPrivacyPolicy)).rejects.toThrow('Update failed');
        });
    });
});
