namespace Api.Domain.Entities;

using Api.Domain.ValueObjects;
using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;

public enum SystemNotificationUrgency
{
    REGULAR,
    URGENT
}

public class SystemNotification : IDTOAble<SystemNotificationDto>
{
    public Guid Id { get; private set; }
    public SystemNotificationUrgency Urgency { get; private set; }
    public Email? TargetUser { get; private set; } // Null means broadcast
    public bool ShouldSendEmail { get; private set; }

    public string Title { get; private set; }
    public string Body { get; private set; }

    public DateTime ReceiveDate { get; private set; }
    public bool HasBeenRead { get; private set; }

    public SystemNotification(
        SystemNotificationUrgency urgency,
        Email? targetUser,
        bool shouldSendEmail,
        string title,
        string body,
        DateTime receiveDate,
        bool hasBeenRead
    )
    {
        if (title == null) throw new ArgumentNullException(nameof(title));
        if (body == null) throw new ArgumentNullException(nameof(body));

        Id = Guid.NewGuid();
        Urgency = urgency;
        TargetUser = targetUser;
        ShouldSendEmail = shouldSendEmail;
        Title = title;
        Body = body;
        ReceiveDate = receiveDate;
        HasBeenRead = hasBeenRead;
    }

    //EF Core
    protected SystemNotification() { }

    public void MarkAsRead()
    {
        HasBeenRead = true;
    }

    public SystemNotificationDto ToDTO()
    {
        return new SystemNotificationDto
        {
            Id = Id,
            Urgency = Urgency,
            Title = Title,
            Message = Body,
            CreatedAt = ReceiveDate,
            IsRead = HasBeenRead
        };
    }
}