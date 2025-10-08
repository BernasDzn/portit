namespace Api.Application.Controllers;

using Microsoft.AspNetCore.Mvc;
using Api.Application.Services;
using Api.Application.DataTransfer;


[ApiController]
[Route("[controller]")]
public class VesselVisitNotificationController : ControllerBase
{
    private readonly ILogger<VesselVisitNotificationController> _logger;
    private readonly VesselVisitNotificationService _notificationService;
    private readonly NotificationDecisionService _notificationDecisionService;

    public VesselVisitNotificationController(VesselVisitNotificationService notificationService, NotificationDecisionService notificationDecisionService, ILogger<VesselVisitNotificationController> logger)
    {
        _notificationService = notificationService;
        _notificationDecisionService = notificationDecisionService;
        _logger = logger;
    }

    [HttpGet(Name = "GetVesselVisitNotifications")]
    public async Task<ActionResult<IEnumerable<VesselVisitNotificationDto>>> GetAll()
    {
        IEnumerable<VesselVisitNotificationDto> notificationsDto = await _notificationService.GetVesselVisitNotifications();
        return Ok(notificationsDto);
    }

    [HttpGet("decisions", Name = "GetNotificationDecisions")]
    public async Task<ActionResult<IEnumerable<NotificationDecisionDto>>> GetDecisions([FromQuery] Guid vesselVisitNotificationId)
    {
        IEnumerable<NotificationDecisionDto> notificationsDto = await _notificationDecisionService.GetNotificationDecisions(vesselVisitNotificationId);
        return Ok(notificationsDto);
    }
    
    [HttpPost(Name = "CreateVesselVisitNotification")]
    public async Task<ActionResult<VesselVisitNotificationDto>> Create([FromBody] VesselVisitNotificationDto vesselVisitNotificationDto)
    {
        var createdNotification = await _notificationService.Add(vesselVisitNotificationDto);
        if (createdNotification == null)
        {
            return BadRequest("Invalid data provided.");
        }

        return CreatedAtAction(nameof(GetAll), null, createdNotification);
    }
}
