namespace Api.Application.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Api.Application.Services;
using Api.Application.Exceptions;

[ApiController]
[Route("[controller]")]
public class SystemNotificationController : ControllerBase, ISystemNotificationController
{
	private readonly ILogger<SystemNotificationController> _logger;
    private readonly ISystemNotificationService _systemNotificationService;

	public SystemNotificationController(ISystemNotificationService systemNotificationService, ILogger<SystemNotificationController> logger)
    {
        _systemNotificationService = systemNotificationService;
        _logger = logger;
    }

    [Authorize(Policy = "SystemNotification.Broadcast")]
    [HttpPost("broadcastNotification", Name = "BroadcastNotification")]
    public async Task<ActionResult> BroadcastNotification(BroadcastSystemNotificationDto notificationDto)
    {
        try
        {
            await _systemNotificationService.BroadcastNotification(notificationDto);
            return Ok();
        }
        catch (System.Exception e)
        {
            if (e is ArgumentNullException || e is ArgumentException || e is InvalidOperationException)
            {
                _logger.LogError("Invalid input when broadcasting system notification, {Message}", e.Message);
                return BadRequest(e.Message);
            }

            _logger.LogCritical("Error broadcasting system notification.");
            return StatusCode(500, "An error occurred while broadcasting the notification.");
        }
    }

    [Authorize(Policy = "SystemNotification.View")]
    [HttpGet("myNotifications", Name = "MyNotifications")]
    public async Task<ActionResult<IEnumerable<SystemNotificationDto>>> GetMyNotifications()
    {
        try
        {
            string? userEmail = User.FindFirst("email_address")?.Value;
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                _logger.LogError("User email claim is missing or empty.");
                return BadRequest("User email is required to retrieve notifications.");
            }

            var notifications =  await _systemNotificationService.GetMyNotifications(userEmail);
            return Ok(notifications);
        }
        catch (System.Exception)
        {
            string? userEmail = User.FindFirst("email_address")?.Value;
            _logger.LogCritical("Error retrieving system notifications for user {UserEmail}.", userEmail);
            return StatusCode(500, "An error occurred while retrieving notifications.");
        }
    }

    [Authorize(Policy = "SystemNotification.MarkAsRead")]
    [HttpPut("markAsRead", Name = "MarkAsRead")]
    public async Task<ActionResult> MarkAsRead(string notificationId)
    {
        try
        {
            await _systemNotificationService.MarkAsRead(notificationId);
            return Ok();
        }
        catch (System.Exception)
        {
            _logger.LogCritical("Error marking notification {NotificationId} as read.", notificationId);
            return StatusCode(500, "An error occurred while marking the notification as read.");
        }
    }

    [Authorize(Policy = "SystemNotification.NotifyUser")]
    [HttpPost("notifyUser", Name = "NotifyUser")]
    public async Task<ActionResult<SystemNotificationDto>> NotifyUser(CreateSystemNotificationDto notificationDto)
    {
        try
        {
            var notification = await _systemNotificationService.NotifyUser(notificationDto, notificationDto.TargetUserEmail);
            return Ok(notification);
        }
        catch (EntityNotFoundException e)
        {
            _logger.LogError("Error notifying user {OfficerEmail}, {Message}", notificationDto.TargetUserEmail, e.Message);
            return NotFound(e.Message);
        }
        catch (System.Exception e)
        {
            if (e is ArgumentNullException || e is ArgumentException || e is InvalidOperationException)
			{
                _logger.LogError("Invalid input when notifying user {OfficerEmail}, {Message}", notificationDto.TargetUserEmail, e.Message);
				return BadRequest(e.Message);
			}

            _logger.LogCritical("Error notifying user {OfficerEmail}.", notificationDto.TargetUserEmail);
            return StatusCode(500, "An error occurred while notifying the user.");
        }
    }
}