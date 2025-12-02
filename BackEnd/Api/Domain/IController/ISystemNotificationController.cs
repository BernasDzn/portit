using Microsoft.AspNetCore.Mvc;
using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;

public interface ISystemNotificationController
{
    Task<ActionResult<SystemNotificationDto>> NotifyUser(CreateSystemNotificationDto notificationDto);
    Task<ActionResult> BroadcastNotification(BroadcastSystemNotificationDto notificationDto);
    Task<ActionResult<IEnumerable<SystemNotificationDto>>> GetMyNotifications();
    Task<ActionResult> MarkAsRead(string notificationId);
}