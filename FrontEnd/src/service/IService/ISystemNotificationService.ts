import type { SystemNotificationBroadcastDto, SystemNotificationDto } from "@/model/dto/SystemNotificationDto";
import type { SystemNotification } from "@/model/SystemNotification";

export interface ISystemNotificationService {
    getSystemNotifications(): Promise<SystemNotification[]>;
    
    notifyUser(notification: SystemNotificationDto): Promise<void>;
    broadcastNotification(notification: SystemNotificationBroadcastDto): Promise<void>;

    markAsRead(notificationId: string): Promise<void>;
}
