import { describe, it, expect } from 'vitest';
import { SystemNotification } from '../../../src/model/SystemNotification';

describe('SystemNotification', () => {
    describe('Constructor', () => {
        it('should create system notification with all fields', () => {
            const createdAt = new Date('2024-01-01T10:00:00Z');
            const notification = new SystemNotification({
                id: 'NOTIF001',
                urgency: 1,
                title: 'Important Update',
                message: 'System maintenance scheduled',
                createdAt,
                isRead: false
            });

            expect(notification.id).toBe('NOTIF001');
            expect(notification.urgency).toBe(1);
            expect(notification.title).toBe('Important Update');
            expect(notification.message).toBe('System maintenance scheduled');
            expect(notification.createdAt).toEqual(createdAt);
            expect(notification.isRead).toBe(false);
        });

        it('should create notification with urgency 0', () => {
            const notification = new SystemNotification({
                id: 'NOTIF002',
                urgency: 0,
                title: 'Regular Update',
                message: 'New features available',
                createdAt: new Date(),
                isRead: true
            });

            expect(notification.urgency).toBe(0);
            expect(notification.isRead).toBe(true);
        });
    });

    describe('notificationId getter', () => {
        it('should return the notification id', () => {
            const notification = new SystemNotification({
                id: 'NOTIF001',
                urgency: 1,
                title: 'Test',
                message: 'Test message',
                createdAt: new Date(),
                isRead: false
            });

            expect(notification.notificationId).toBe('NOTIF001');
        });
    });

    describe('isNotificationRead getter', () => {
        it('should return true when notification is read', () => {
            const notification = new SystemNotification({
                id: 'NOTIF001',
                urgency: 1,
                title: 'Test',
                message: 'Test message',
                createdAt: new Date(),
                isRead: true
            });

            expect(notification.isNotificationRead).toBe(true);
        });

        it('should return false when notification is not read', () => {
            const notification = new SystemNotification({
                id: 'NOTIF001',
                urgency: 1,
                title: 'Test',
                message: 'Test message',
                createdAt: new Date(),
                isRead: false
            });

            expect(notification.isNotificationRead).toBe(false);
        });
    });

    describe('toDto', () => {
        it('should convert notification to DTO correctly', () => {
            const notification = new SystemNotification({
                id: 'NOTIF001',
                urgency: 1,
                title: 'Important Update',
                message: 'System maintenance scheduled',
                createdAt: new Date('2024-01-01'),
                isRead: false
            });

            const dto = notification.toDto();

            expect(dto.urgency).toBe(1);
            expect(dto.shouldSendEmail).toBe(false);
            expect(dto.targetUserEmail).toBe('');
            expect(dto.title).toBe('Important Update');
            expect(dto.message).toBe('System maintenance scheduled');
        });

        it('should always set shouldSendEmail to false in DTO', () => {
            const notification = new SystemNotification({
                id: 'NOTIF001',
                urgency: 0,
                title: 'Test',
                message: 'Test',
                createdAt: new Date(),
                isRead: true
            });

            const dto = notification.toDto();

            expect(dto.shouldSendEmail).toBe(false);
        });

        it('should always set targetUserEmail to empty string in DTO', () => {
            const notification = new SystemNotification({
                id: 'NOTIF001',
                urgency: 0,
                title: 'Test',
                message: 'Test',
                createdAt: new Date(),
                isRead: true
            });

            const dto = notification.toDto();

            expect(dto.targetUserEmail).toBe('');
        });
    });
});
