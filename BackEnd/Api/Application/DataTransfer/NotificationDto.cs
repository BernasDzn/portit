using Api.Domain.Entities;

public class SystemNotificationDto
{
    public Guid Id { get; set; }
    public SystemNotificationUrgency Urgency { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}

public class BroadcastSystemNotificationDto
{
    public SystemNotificationUrgency Urgency { get; set; }
    public bool ShouldSendEmail { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
}

public class CreateSystemNotificationDto
{
    public SystemNotificationUrgency Urgency { get; set; }
    public bool ShouldSendEmail { get; set; }
    public string TargetUserEmail { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
}