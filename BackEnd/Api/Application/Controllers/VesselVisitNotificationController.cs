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
    public async Task<ActionResult<IEnumerable<NotificationDecisionDto>>> GetDecisions([FromQuery] string vesselVisitNotificationId)
    {
        try
        {
            IEnumerable<NotificationDecisionDto> notificationsDto = await _notificationDecisionService.GetNotificationDecisions(vesselVisitNotificationId);
            return Ok(notificationsDto);
        }
        catch (System.Exception)
        {
            return BadRequest("An error occurred while retrieving the notification decisions");
        }
    }

    [HttpPost(Name = "CreateVesselVisitNotification")]
    public async Task<ActionResult<VesselVisitNotificationDto>> Create([FromBody] VesselVisitNotificationDto vesselVisitNotificationDto)
    {
        try
        {
            var createdNotification = await _notificationService.Add(vesselVisitNotificationDto);
            if (createdNotification == null)
            {
                return BadRequest("Invalid data provided.");
            }

            return CreatedAtAction(nameof(GetAll), null, createdNotification);
        }
        catch (System.Exception)
        {
            return BadRequest("An error occurred while creating the notification.");
        }
    }

    [HttpPost("decisions", Name = "CreateNotificationDecision")]
    public async Task<ActionResult<NotificationDecisionDto>> CreateDecision([FromQuery] string vesselVisitNotificationId, [FromBody] NotificationDecisionDto notificationDecisionDto)
    {
        try
        {
            var createdDecision = await _notificationDecisionService.Add(notificationDecisionDto, vesselVisitNotificationId);
            if (createdDecision == null)
            {
                return BadRequest("Invalid data provided.");
            }

            return CreatedAtAction(nameof(GetDecisions), null, createdDecision);
        }
        catch (System.Exception ex)
        {
            return BadRequest("An error occurred while creating the notification decision. " + ex.Message);
        }
    }

    [HttpPut("{id}", Name = "UpdateVesselVisitNotification")]
    public async Task<ActionResult<VesselVisitNotificationDto>> Update(string id, VesselVisitNotificationDto vesselVisitNotificationDto)
	{
        try
        {
            var updatedNotification = await _notificationService.Update(id, vesselVisitNotificationDto);
            if (updatedNotification == null)
            {
                return BadRequest("Could not update the notification. It may not exist.");
            }
            return Ok(updatedNotification);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating notification with ID {id}: {ex.Message}");
            return BadRequest($"An error occurred while updating the notification: {ex.Message}");
        }
	}

}
