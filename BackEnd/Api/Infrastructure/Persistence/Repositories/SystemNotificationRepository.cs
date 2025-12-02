namespace Api.Infrastructure.Persistence.Repositories;

using Api.Domain.Entities;
using Api.Domain.IRepository;
using Microsoft.EntityFrameworkCore;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;
using Api.Application.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System;
using Api.Domain.ValueObjects;

public class SystemNotificationRepository : GenericRepository<SystemNotification>, ISystemNotificationRepository
{
    private new readonly ApiContext _context;
    public SystemNotificationRepository(ApiContext context) : base(context)
    {
        _context = context;
    }

    public async Task BroadcastNotification(SystemNotification notification, IEnumerable<string> systemUsers)
    {
        try
        {
            var notifications = systemUsers
                .Select(user => new SystemNotification(
                    notification.Urgency,
                    new Email {Value = user},
                    notification.ShouldSendEmail,
                    notification.Title,
                    notification.Body,
                    notification.ReceiveDate,
                    notification.HasBeenRead
                ));

            await _context.AddRangeAsync(notifications);
            await _context.SaveChangesAsync();
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public async Task<IEnumerable<SystemNotification>> GetMyNotifications(string userEmail)
    {
        try
        {
            var notifications = await _context.SystemNotifications
                .Where(n => n.TargetUser != null && n.TargetUser.Value == userEmail)
                .ToListAsync();

            return notifications;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public async Task<SystemNotification?> GetById(string id)
    {
        try
        {
            var notification = await _context.SystemNotifications
                .FirstOrDefaultAsync(n => n.Id == new Guid(id));

            return notification;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public async Task<SystemNotification> NotifyUser(SystemNotification notification)
    {
        try
        {
            await _context.AddAsync(notification);
            await _context.SaveChangesAsync();
            return notification;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public async Task UpdateNotification(SystemNotification notification)
    {
        try
        {
            _context.SystemNotifications.Update(notification);
            await _context.SaveChangesAsync();
        }
        catch (System.Exception)
        {   
            throw;
        }
    }
}
