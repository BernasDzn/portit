namespace Api.Application.Controllers;

using Microsoft.AspNetCore.Mvc;
using Api.Application.Services;
using Api.Application.DataTransfer;


[ApiController]
[Route("[controller]")]
public class NotificationDecisionController : ControllerBase
{
    private readonly ILogger<NotificationDecisionController> _logger;
    private readonly NotificationDecisionService _notificationDecisionService;

    public NotificationDecisionController(NotificationDecisionService notificationService, ILogger<NotificationDecisionController> logger)
    {
        _notificationDecisionService = notificationService;
        _logger = logger;
    }

    [HttpGet(Name = "GetNotificationDecisions")]
    public async Task<ActionResult<IEnumerable<NotificationDecisionDto>>> GetAll()
    {
        IEnumerable<NotificationDecisionDto> notificationsDto = await _notificationDecisionService.GetNotificationDecisions();
        return Ok(notificationsDto);
    }

}
