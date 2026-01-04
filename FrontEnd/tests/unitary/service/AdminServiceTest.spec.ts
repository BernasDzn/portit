import { describe, it, expect, vi, beforeEach } from 'vitest';
import { AdminService } from '@/service/AdminService';
import type { IHttpService, Response } from '@/service/IService/IHttpService';
import type { Logs } from '@/model/values/Logs';
import type { SystemUser } from '@/model/SystemUser';

describe('AdminService', () => {
    let adminService: AdminService;
    let mockHttpService: IHttpService;

    const mockLogs: Logs = {
        id: '1',
        timestamp: new Date('2024-01-01'),
        userEmail: 'test@example.com',
        action: 'LOGIN',
        details: 'User logged in'
    } as unknown as Logs;

    const mockSystemUser: SystemUser = {
        sub: 'google-oauth2|123456789',
        emailAddress: 'test@example.com',
        isActive: true,
        role: 1
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

        adminService = new AdminService(mockHttpService);
    });

    describe('getLogs', () => {
        it('should return list of audit logs', async () => {
            vi.mocked(mockHttpService.get).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: [mockLogs],
            } as Response<Logs[]>);

            const result = await adminService.getLogs();

            expect(mockHttpService.get).toHaveBeenCalledWith('/api/auditLogs');
            expect(result).toEqual([mockLogs]);
        });

        it('should throw error when getLogs fails', async () => {
            vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Network error'));

            await expect(adminService.getLogs()).rejects.toThrow('Network error');
        });
    });

    describe('changeUserRole', () => {
        it('should change user role successfully', async () => {
            vi.mocked(mockHttpService.put).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: mockSystemUser,
            } as Response<SystemUser>);

            const result = await adminService.changeUserRole('test@example.com', 2);

            expect(mockHttpService.put).toHaveBeenCalledWith(
                '/api/SystemUser/test%40example.com/role?role=2',
                {}
            );
            expect(result).toEqual(mockSystemUser);
        });

        it('should throw error when changeUserRole fails', async () => {
            vi.mocked(mockHttpService.put).mockRejectedValue(new Error('Role change failed'));

            await expect(adminService.changeUserRole('test@example.com', 2)).rejects.toThrow('Role change failed');
        });
    });

    describe('activateUserAccount', () => {
        it('should activate user account successfully', async () => {
            vi.mocked(mockHttpService.put).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: undefined,
            } as Response<void>);

            await adminService.activateUserAccount('test@example.com');

            expect(mockHttpService.put).toHaveBeenCalledWith(
                '/api/SystemUser/test%40example.com/activate',
                {}
            );
        });

        it('should throw error when activateUserAccount fails', async () => {
            vi.mocked(mockHttpService.put).mockRejectedValue(new Error('Activation failed'));

            await expect(adminService.activateUserAccount('test@example.com')).rejects.toThrow('Activation failed');
        });
    });

    describe('deactivateUserAccount', () => {
        it('should deactivate user account successfully', async () => {
            vi.mocked(mockHttpService.put).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: undefined,
            } as Response<void>);

            await adminService.deactivateUserAccount('test@example.com');

            expect(mockHttpService.put).toHaveBeenCalledWith(
                '/api/SystemUser/test%40example.com/deactivate',
                {}
            );
        });

        it('should throw error when deactivateUserAccount fails', async () => {
            vi.mocked(mockHttpService.put).mockRejectedValue(new Error('Deactivation failed'));

            await expect(adminService.deactivateUserAccount('test@example.com')).rejects.toThrow('Deactivation failed');
        });
    });

    describe('createUser', () => {
        it('should create user successfully', async () => {
            vi.mocked(mockHttpService.post).mockResolvedValue({
                status: 201,
                statusText: 'Created',
                data: mockSystemUser,
            } as Response<SystemUser>);

            const result = await adminService.createUser('test@example.com', 1);

            expect(mockHttpService.post).toHaveBeenCalledWith('/api/SystemUser', {
                sub: '',
                isActive: false,
                role: 1,
                email: 'test@example.com'
            });
            expect(result).toEqual(mockSystemUser);
        });

        it('should throw error when createUser fails', async () => {
            vi.mocked(mockHttpService.post).mockRejectedValue(new Error('User creation failed'));

            await expect(adminService.createUser('test@example.com', 1)).rejects.toThrow('User creation failed');
        });
    });

    describe('inviteUser', () => {
        it('should invite user successfully', async () => {
            vi.mocked(mockHttpService.post).mockResolvedValue({
                status: 201,
                statusText: 'Created',
                data: mockSystemUser,
            } as Response<SystemUser>);

            vi.mocked(mockHttpService.put).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: mockSystemUser,
            } as Response<SystemUser>);

            const result = await adminService.inviteUser('test@example.com', 1);

            expect(mockHttpService.post).toHaveBeenCalled();
            expect(mockHttpService.put).toHaveBeenCalled();
            expect(result).toEqual(mockSystemUser);
        });
    });

    describe('getAllUsers', () => {
        it('should return list of all users', async () => {
            vi.mocked(mockHttpService.get).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: [mockSystemUser],
            } as Response<SystemUser[]>);

            const result = await adminService.getAllUsers();

            expect(mockHttpService.get).toHaveBeenCalledWith('/api/SystemUser');
            expect(result).toEqual([mockSystemUser]);
        });
    });
});
