import type { SystemNotificationDto } from "@/model/dto/SystemNotificationDto";
import type { SystemNotification } from "@/model/SystemNotification";

export interface ISystemNotificationService {
    getSystemNotifications(): Promise<SystemNotification[]>;
    
    notifyUser(notification: SystemNotificationDto): Promise<void>;
    broadcastNotification(notification: SystemNotificationDto): Promise<void>;

    markAsRead(notificationId: string): Promise<void>;
}
