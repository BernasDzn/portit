export interface SystemNotificationDto {
    urgency: 0 | 1;
    shouldSendEmail: boolean;   
    targetUserEmail: string;
    title: string;
    message: string;
}

export interface SystemNotificationBroadcastDto {
    urgency: 0 | 1;
    shouldSendEmail: boolean;   
    title: string;
    message: string;
}