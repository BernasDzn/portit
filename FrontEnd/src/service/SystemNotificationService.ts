import {inject, injectable} from 'inversify';
import { TYPES } from '@/inversify/types';

import type { IHttpService } from './IService/IHttpService';
import type { ISystemNotificationService } from './IService/ISystemNotificationService';
import type { SystemNotificationBroadcastDto, SystemNotificationDto } from '@/model/dto/SystemNotificationDto';
import type { SystemNotification } from '@/model/SystemNotification';

@injectable()
export class SystemNotificationService implements ISystemNotificationService {
    
	constructor(
		@inject(TYPES.api) 
		private http: IHttpService
	){}
    
    async getSystemNotifications(): Promise<SystemNotification[]> {
        
        const res = await this.http.get<SystemNotification[]>('/api/SystemNotification/myNotifications');
        return res.data;
    }
    async notifyUser(notification: SystemNotificationDto): Promise<void> {
        await this.http.post<SystemNotificationDto>('/api/SystemNotification/notifyUser', notification);
    }

    async broadcastNotification(notification: SystemNotificationBroadcastDto): Promise<void> {
        await this.http.post<SystemNotificationDto>('/api/SystemNotification/broadcastNotification', notification);
    }
    async markAsRead(notificationId: string): Promise<void> {
        await this.http.put<void>(`/api/SystemNotification/markAsRead?notificationId=${notificationId}`, { notificationId: notificationId });
    }
    
}