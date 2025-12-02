using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;

namespace Api.Application.Services;

public interface ISystemNotificationService
{
    public Task<IEnumerable<SystemNotificationDto>> GetMyNotifications(string userEmail);
    public Task<SystemNotificationDto> NotifyUser(CreateSystemNotificationDto notificationDto, string officerEmail);
    public Task BroadcastNotification(BroadcastSystemNotificationDto notificationDto);
    public Task<SystemNotificationDto> MarkAsRead(string notificationId);
}