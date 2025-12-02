namespace Api.Domain.IRepository;

using Api.Application.DataTransfer.Filters;
using Api.Domain.Entities;
using Api.Infrastructure.Utilities;

public interface ISystemNotificationRepository : IGenericRepository<SystemNotification>
{
    Task<IEnumerable<SystemNotification>> GetMyNotifications(string userEmail);
    Task<SystemNotification> NotifyUser(SystemNotification notification);
    Task BroadcastNotification(SystemNotification notification, IEnumerable<string> systemUsers);
    Task UpdateNotification(SystemNotification notification);
    Task<SystemNotification?> GetById(string id);
}