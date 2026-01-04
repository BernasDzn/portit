import { describe, it, expect, vi, beforeEach } from 'vitest';
import { SystemNotificationService } from '@/service/SystemNotificationService';
import type { IHttpService, Response } from '@/service/IService/IHttpService';
import type { SystemNotification } from '@/model/SystemNotification';
import type { SystemNotificationDto, SystemNotificationBroadcastDto } from '@/model/dto/SystemNotificationDto';

describe('SystemNotificationService', () => {
    let systemNotificationService: SystemNotificationService;
    let mockHttpService: IHttpService;

    const mockSystemNotification: SystemNotification = {
        id: 'NOTIF001',
        urgency: 1,
        title: 'Important Update',
        message: 'System maintenance scheduled',
        createdAt: new Date('2024-01-01'),
        isRead: false,
        notificationId: 'NOTIF001',
        isNotificationRead: false,
        toDto: () => ({
            urgency: 1,
            shouldSendEmail: false,
            targetUserEmail: '',
            title: 'Important Update',
            message: 'System maintenance scheduled'
        })
    } as SystemNotification;

    const mockNotificationDto: SystemNotificationDto = {
        urgency: 1,
        shouldSendEmail: true,
        targetUserEmail: 'user@example.com',
        title: 'Test Notification',
        message: 'This is a test'
    };

    const mockBroadcastDto: SystemNotificationBroadcastDto = {
        urgency: 0,
        shouldSendEmail: false,
        title: 'Broadcast',
        message: 'Broadcast message'
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

        systemNotificationService = new SystemNotificationService(mockHttpService);
    });

    describe('getSystemNotifications', () => {
        it('should return list of system notifications', async () => {
            vi.mocked(mockHttpService.get).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: [mockSystemNotification],
            } as Response<SystemNotification[]>);

            const result = await systemNotificationService.getSystemNotifications();

            expect(mockHttpService.get).toHaveBeenCalledWith('/api/SystemNotification/myNotifications');
            expect(result).toEqual([mockSystemNotification]);
        });

        it('should throw error when getSystemNotifications fails', async () => {
            vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Network error'));

            await expect(systemNotificationService.getSystemNotifications()).rejects.toThrow('Network error');
        });
    });

    describe('notifyUser', () => {
        it('should notify user successfully', async () => {
            vi.mocked(mockHttpService.post).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: undefined,
            } as Response<void>);

            await systemNotificationService.notifyUser(mockNotificationDto);

            expect(mockHttpService.post).toHaveBeenCalledWith(
                '/api/SystemNotification/notifyUser',
                mockNotificationDto
            );
        });

        it('should throw error when notifyUser fails', async () => {
            vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Notification failed'));

            await expect(systemNotificationService.notifyUser(mockNotificationDto)).rejects.toThrow('Notification failed');
        });
    });

    describe('broadcastNotification', () => {
        it('should broadcast notification successfully', async () => {
            vi.mocked(mockHttpService.post).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: undefined,
            } as Response<void>);

            await systemNotificationService.broadcastNotification(mockBroadcastDto);

            expect(mockHttpService.post).toHaveBeenCalledWith(
                '/api/SystemNotification/broadcastNotification',
                mockBroadcastDto
            );
        });

        it('should throw error when broadcastNotification fails', async () => {
            vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Broadcast failed'));

            await expect(systemNotificationService.broadcastNotification(mockBroadcastDto)).rejects.toThrow('Broadcast failed');
        });
    });

    describe('markAsRead', () => {
        it('should mark notification as read successfully', async () => {
            vi.mocked(mockHttpService.put).mockResolvedValue({
                status: 200,
                statusText: 'OK',
                data: undefined,
            } as Response<void>);

            await systemNotificationService.markAsRead('NOTIF001');

            expect(mockHttpService.put).toHaveBeenCalledWith(
                '/api/SystemNotification/markAsRead?notificationId=NOTIF001',
                { notificationId: 'NOTIF001' }
            );
        });

        it('should throw error when markAsRead fails', async () => {
            vi.mocked(mockHttpService.put).mockRejectedValue(new Error('Mark as read failed'));

            await expect(systemNotificationService.markAsRead('NOTIF001')).rejects.toThrow('Mark as read failed');
        });
    });
});
