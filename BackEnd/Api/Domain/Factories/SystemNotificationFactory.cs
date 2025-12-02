using Api.Domain.ValueObjects;

namespace Api.Domain.Entities;

public class SystemNotificationFactory
{
    public static SystemNotification CreateNotification(
        SystemNotificationUrgency urgency,
        Email targetUser,
        bool shouldSendEmail,
        string title,
        string body
    )
    {
        return new SystemNotification(
            urgency,
            targetUser,
            shouldSendEmail,
            title,
            body,
            DateTime.UtcNow,
            false
        );
    }
    
    public static SystemNotification BroadcastNotification(
        SystemNotificationUrgency urgency,
        bool shouldSendEmail,
        string title,
        string body
    )
    {
        return new SystemNotification(
            urgency,
            null,
            shouldSendEmail,
            title,
            body,
            DateTime.UtcNow,
            false
        );
    }
}