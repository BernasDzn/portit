namespace Api.Application.Services;

using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Application.Exceptions;
using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Exceptions;
using Api.Infrastructure.Utilities;
using Api.Infrastructure.Utilities.Email;

public class SystemNotificationService : ISystemNotificationService
{
    private readonly ISystemUserService _systemUserService;
    private readonly ISystemNotificationRepository _systemNotificationRepository;
    private readonly ILogger<SystemNotificationService> _logger;
    private readonly IEmailService _emailService;

    public SystemNotificationService(ISystemUserService systemUserService, IEmailService emailService, ISystemNotificationRepository systemNotificationRepository, ILogger<SystemNotificationService> logger)
    {
        _systemUserService = systemUserService;
        _emailService = emailService;
        _systemNotificationRepository = systemNotificationRepository;
        _logger = logger;
    }

    public async Task<SystemNotificationDto> NotifyUser(CreateSystemNotificationDto notificationDto, string officerEmail)
    {
        var user = await _systemUserService.GetByEmailAddress(officerEmail);
        if (user == null)
            throw new EntityNotFoundException("The specified user is not active.");

        SystemNotification notification = SystemNotificationFactory.CreateNotification(
            notificationDto.Urgency,
            new Email { Value = officerEmail },
            notificationDto.ShouldSendEmail,
            notificationDto.Title,
            notificationDto.Message
        );

        if (notificationDto.ShouldSendEmail)
            await _emailService.SendEmailAsync(officerEmail, notificationDto.Title, notificationDto.Message);


        return (await _systemNotificationRepository.NotifyUser(notification)).ToDTO();
    }

    public async Task BroadcastNotification(BroadcastSystemNotificationDto notificationDto)
    {
        SystemNotification notification = SystemNotificationFactory.BroadcastNotification(
            notificationDto.Urgency,
            notificationDto.ShouldSendEmail,
            notificationDto.Title,
            notificationDto.Message
        );

        IEnumerable<SystemUserDto> systemUsers = await _systemUserService.GetAll();
        var targetUsers = systemUsers
            .Where(u => u.Email != null)
            .Select(u => u.Email)!
            .ToList();

        if (notificationDto.ShouldSendEmail)
        {
            foreach (var email in targetUsers)
                await _emailService.SendEmailAsync(email, notificationDto.Title, notificationDto.Message);
        }

        await _systemNotificationRepository.BroadcastNotification(notification, targetUsers);
    }

    public async Task<IEnumerable<SystemNotificationDto>> GetMyNotifications(string userEmail)
    {
        IEnumerable<SystemNotification> notifications = await _systemNotificationRepository
            .GetMyNotifications(userEmail);

        return notifications.Select(n => n.ToDTO());
    }

    public async Task<SystemNotificationDto> MarkAsRead(string notificationId)
    {
        var notification = await _systemNotificationRepository.GetById(notificationId);
        if (notification == null)
            throw new EntityNotFoundException("The specified notification does not exist.");

        notification.MarkAsRead();
        await _systemNotificationRepository.UpdateNotification(notification);

        return notification.ToDTO();
    }
}