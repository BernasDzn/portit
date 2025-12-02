import type { StaffDto } from './dto/StaffDto';
import type { SystemNotificationDto } from './dto/SystemNotificationDto';
import type { Qualification } from './Qualifications';
import type { OperationalWindow } from './values/OperationalWindow';

export class SystemNotification {

    public id: string;        
    public urgency: 0 | 1;
    public title: string;
    public message: string;
    public createdAt: Date;
    public isRead: boolean;

    constructor( params: {
        id: string;
        urgency: 0 | 1;
        title: string;
        message: string;
        createdAt: Date;
        isRead: boolean;
    } ) {
        this.id = params.id;
        this.urgency = params.urgency;
        this.title = params.title;
        this.message = params.message;
        this.createdAt = params.createdAt;
        this.isRead = params.isRead;
    }

    get notificationId(): string {
        return this.id;
    }
    
    get isNotificationRead(): boolean {
        return this.isRead;
    }

    toDto(): SystemNotificationDto {
        return {
            urgency: this.urgency,
            shouldSendEmail: false,
            targetUserEmail: '',
            title: this.title,
            message: this.message
        };
    }
}