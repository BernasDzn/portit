namespace Api.Application.Controllers;

using Microsoft.AspNetCore.Mvc;
using Api.Application.Services;
using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;
using Api.Infrastructure.Exceptions;
using Api.Application.Exceptions;

[ApiController]
[Route("[controller]")]
public class VesselVisitNotificationController : ControllerBase, IVesselVisitNotificationController
{
    private readonly ILogger<VesselVisitNotificationController> _logger;
    private readonly IVesselVisitNotificationService _notificationService;
    private readonly INotificationDecisionService _notificationDecisionService;

    public VesselVisitNotificationController(IVesselVisitNotificationService notificationService, INotificationDecisionService notificationDecisionService, ILogger<VesselVisitNotificationController> logger)
    {
        _notificationService = notificationService;
        _notificationDecisionService = notificationDecisionService;
        _logger = logger;
    }

    [HttpGet(Name = "GetVesselVisitNotifications")]
    public async Task<ActionResult<IEnumerable<VesselVisitNotificationDto>>> GetAll()
    {
        try
        {
            IEnumerable<VesselVisitNotificationDto> notificationsDto = await _notificationService.GetVesselVisitNotifications();
            return Ok(notificationsDto);
        }
        catch (System.Exception e)
        {
            _logger.LogCritical("Error retrieving vessel visit notifications, {Message}", e.Message);
            return StatusCode(500, "An error occurred while retrieving vessel visit notifications.");
        }
    }

    [HttpGet("{id}", Name = "GetVesselVisitNotificationById")]
    public async Task<ActionResult<VesselVisitNotificationDto>> GetById(string id)
    {
        try
        {
            var notification = await _notificationService.GetById(id);
            return Ok(notification);
        }
        catch (EntityNotFoundException e)
        {
            _logger.LogError($"Error retrieving notification with ID {id}: {e.Message}");
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogCritical($"Error retrieving vessel visit notification with ID {id}: {e.Message}");
            return StatusCode(500, "An error occurred while retrieving the vessel visit notification.");
        }
    }

    [HttpGet("decisions", Name = "GetNotificationDecisions")]
    public async Task<ActionResult<IEnumerable<NotificationDecisionDto>>> GetDecisions([FromQuery] string vesselVisitNotificationId)
    {
        try
        {
            IEnumerable<NotificationDecisionDto> notificationsDto = await _notificationDecisionService.GetNotificationDecisions(vesselVisitNotificationId);
            return Ok(notificationsDto);
        }
        catch (EntityNotFoundException e)
        {
            _logger.LogError($"Error retrieving notification decisions: {e.Message}");
            return NotFound(e.Message);
        }
        catch (System.Exception)
        {
            _logger.LogCritical("Error retrieving notification decisions");
            return StatusCode(500, "An error occurred while retrieving notification decisions.");
        }
    }

    [HttpPost(Name = "CreateVesselVisitNotification")]
    public async Task<ActionResult<VesselVisitNotificationDto>> Create([FromBody] CreateVesselVisitNotificationDto vesselVisitNotificationDto)
    {
        try
        {
            var createdNotification = await _notificationService.Add(vesselVisitNotificationDto);
            return CreatedAtAction(nameof(GetAll), new { id = createdNotification.NotificationId }, createdNotification);
        }
        catch (EntityAlreadyExistsException e)
        {
            _logger.LogError($"Entity already exists: {e.Message}");
            return Conflict(e.Message);
        }
        catch (EntityNotFoundException e)
        {
            _logger.LogError($"Entity not found: {e.Message}");
            return NotFound(e.Message);
        }
        catch (System.Exception e)
        {
            if (e is ArgumentException || e is ArgumentNullException || e is InvalidOperationException || e is OutdatedDecisionException)
            {
                _logger.LogError($"Invalid arguments provided for creating vessel visit notification: {e.Message}");
                return BadRequest(e.Message);
            }

            _logger.LogCritical($"Error creating vessel visit notification: {e.Message}");
            return StatusCode(500, "An error occurred while creating the vessel visit notification.");
        }
    }

    [HttpPost("decisions", Name = "CreateNotificationDecision")]
    public async Task<ActionResult<NotificationDecisionDto>> CreateDecision([FromQuery] string vesselVisitNotificationId, [FromBody] CreateNotificationDecisionDto notificationDecisionDto)
    {
        try
        {
            var createdDecision = await _notificationDecisionService.Add(notificationDecisionDto, vesselVisitNotificationId);
            return CreatedAtAction(nameof(GetDecisions), new { vesselVisitNotificationId }, createdDecision);
        }
        catch (EntityNotFoundException e)
        {
            _logger.LogError($"Decision creation failed, entity not found: {e.Message}");
            return NotFound(e.Message);
        }
        catch (System.Exception ex)
        {
            if (ex is ArgumentException || ex is ArgumentNullException || ex is InvalidOperationException || ex is OutdatedDecisionException)
            {
                _logger.LogError($"Invalid arguments provided for creating notification decision: {ex.Message}");
                return BadRequest(ex.Message);
            }

            _logger.LogCritical($"Error creating notification decision: {ex.Message}");
            return StatusCode(500, "An error occurred while creating the notification decision.");
        }
    }

    [HttpPut("{id?}", Name = "UpdateVesselVisitNotification")]
    public async Task<ActionResult<VesselVisitNotificationDto>> Update(string id, CreateVesselVisitNotificationDto vesselVisitNotificationDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogError("Update called without an id");
                return BadRequest("Notification id is required.");
            }
            var updatedNotification = await _notificationService.Update(id, vesselVisitNotificationDto);
            return NoContent();
        }
        catch (EntityNotFoundException e)
        {
            _logger.LogError($"Error updating notification with ID {id}: {e.Message}");
            return NotFound(e.Message);
        }
        catch (Exception ex)
        {
            if (ex is ArgumentException || ex is ArgumentNullException || ex is InvalidOperationException)
            {
                _logger.LogError($"Invalid arguments provided for updating vessel visit notification: {ex.Message}");
                return BadRequest(ex.Message);
            }

            _logger.LogCritical($"Error updating vessel visit notification with ID {id}: {ex.Message}");
            return StatusCode(500, "An error occurred while updating the vessel visit notification.");
        }
    }


    [HttpGet("filter", Name = "FilterVesselVisitNotifications")]
    public async Task<ActionResult<Page<VesselVisitNotificationStatusDto>>> Filter([FromQuery] VesselVisitNotificationFilter filter)
    {
        try
        {
            var filteredNotifications = await _notificationService.FilterNotifications(filter);
            return Ok(filteredNotifications);
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Error filtering notifications: {ex.Message}");
            return StatusCode(500, "An error occurred while filtering vessel visit notifications.");
        }
    }
}
