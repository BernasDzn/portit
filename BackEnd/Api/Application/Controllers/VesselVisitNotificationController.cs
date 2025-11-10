namespace Api.Application.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Api.Application.Services;
using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;
using Api.Infrastructure.Exceptions;
using Api.Application.Exceptions;
using Api.Domain.ValueObjects;

[ApiController]
[Route("[controller]")]
[Authorize(Policy = "VesselVisitNotification.View")]    
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

    [HttpGet("collectScheduleData", Name = "GetVesselVisitNotificationsOnDay")]
    [AllowAnonymous]
    public async Task<ActionResult<SchedulingResultDto>> CollectSchedulingData([FromQuery] Code dockCode, DateTime day, uint daysAhead = 1)
    {
        try
        {
            SchedulingResultDto resultDto = await _notificationService.CollectSchedulingData(day, daysAhead, dockCode);
            return Ok(resultDto);
        }
                catch (EntityNotFoundException e)
        {
            _logger.LogError($"Error retrieving scheduling data for vessel visit notifications: {e.Message}");
            return NotFound(e.Message);
        }
        catch (System.Exception e)
        {
            _logger.LogCritical("Error collecting scheduling data for vessel visit notifications, {Message}", e.Message);
            return StatusCode(500, "An error occurred while collecting scheduling data for vessel visit notifications.");
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
    [Authorize(Policy = "VesselVisitNotification.Edit")]
    public async Task<ActionResult<VesselVisitNotificationDto>> Create([FromBody] CreateVesselVisitNotificationDto vesselVisitNotificationDto)
    {
        try
        {
            string? userEmail = User.FindFirst("email_address")?.Value;
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                _logger.LogError("Create notification called without a user email");
                return BadRequest("You require a valid user email to create a notification.");
            }

            var createdNotification = await _notificationService.Add(vesselVisitNotificationDto, userEmail);
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
    [Authorize(Policy = "VesselVisitNotification.Approve")]
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
    [Authorize(Policy = "VesselVisitNotification.Edit")]
    public async Task<ActionResult> Update(string id, CreateVesselVisitNotificationDto vesselVisitNotificationDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogError("Update notification called without an id");
                return BadRequest("Notification id is required.");
            }

            string? userEmail = User.FindFirst("email_address")?.Value;
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                _logger.LogError("Update notification called without a user email");
                return BadRequest("You require a valid user email to update a notification.");
            }

            var updatedNotification = await _notificationService.Update(id, vesselVisitNotificationDto, userEmail);
            return NoContent();
        }
        catch (UnauthorizedAccessException e)
        {
            _logger.LogError($"Unauthorized attempt to update notification with ID {id}: {e.Message}");
            return Forbid();
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


    [HttpPut("submit/{id}", Name = "SubmitVesselVisitNotification")]
    [Authorize(Policy = "VesselVisitNotification.Submit")]
    public async Task<ActionResult> Submit(string id)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                {
                    _logger.LogError("Submit cpdate called without an id");
                    return BadRequest("Notification id is required.");
                }
            }
            string? userEmail = User.FindFirst("email_address")?.Value;
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                _logger.LogError("Submit notification called without a user email");
                return BadRequest("You require a valid user email to submit a notification.");
            }

            await _notificationService.SubmitNotification(id, userEmail);
            return NoContent();
        }
        catch (EntityNotFoundException e)
        {
            _logger.LogError($"Error submitting notification with ID {id}: {e.Message}");
            return NotFound(e.Message);
        }
        catch (UnauthorizedAccessException e)
        {
            _logger.LogError($"Unauthorized attempt to submit notification with ID {id}: {e.Message}");
            return Forbid();
        }
        catch (Exception ex)
        {
            if (ex is ArgumentException || ex is ArgumentNullException || ex is InvalidOperationException || ex is InvalidOperationException)
            {
                _logger.LogError($"Invalid arguments provided for submitting vessel visit notification: {ex.Message}");
                return BadRequest(ex.Message);
            }

            _logger.LogCritical($"Error updating vessel visit notification with ID {id}: {ex.Message}");
            return StatusCode(500, "An error occurred while updating the vessel visit notification.");
        }
    }


    [HttpGet("filter", Name = "FilterVesselVisitNotifications")]
    [Authorize(Policy = "VesselVisitNotification.View")]
    public async Task<ActionResult<Page<VesselVisitNotificationStatusDto>>> Filter([FromQuery] VesselVisitNotificationFilter filter)
    {
        try
        {
            string? userEmail = User.FindFirst("email_address")?.Value;
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                _logger.LogError("Filter notifications called without a user email");
                return BadRequest("You require a valid user email to filter notifications.");
            }

            var filteredNotifications = await _notificationService.FilterNotifications(filter, userEmail);
            return Ok(filteredNotifications);
        }
        catch (UnauthorizedAccessException e)
        {
            _logger.LogError($"Unauthorized attempt to filter notifications: {e.Message}");
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Error filtering notifications: {ex.Message}");
            return StatusCode(500, "An error occurred while filtering vessel visit notifications.");
        }
    }

    [HttpDelete(Name = "DeleteDraft")]
    [Authorize(Policy = "VesselVisitNotification.Edit")]
    public async Task<ActionResult> DeleteDraft(string id)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogError("Delete draft notification called without an id");
                return BadRequest("Notification id is required.");
            }

            string? userEmail = User.FindFirst("email_address")?.Value;
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                _logger.LogError("Delete draft notification called without a user email");
                return BadRequest("You require a valid user email to delete a draft notification.");
            }

            await _notificationService.DeleteNotificationDraft(id, userEmail);
            return NoContent();
        }
        catch (EntityNotFoundException e)
        {
            _logger.LogError($"Error deleting draft notification with ID {id}: {e.Message}");
            return NotFound(e.Message);
        }
        catch (UnauthorizedAccessException e)
        {
            _logger.LogError($"Unauthorized attempt to delete draft notification with ID {id}: {e.Message}");
            return Forbid();
        }
        catch (System.Exception e)
        {
            if (e is ArgumentException || e is ArgumentNullException || e is InvalidOperationException)
            {
                _logger.LogError($"Invalid arguments provided for deleting vessel visit notification draft: {e.Message}");
                return BadRequest(e.Message);
            }

            _logger.LogCritical($"Error deleting draft notification with ID {id}");
            return StatusCode(500, "An error occurred while deleting the vessel visit notification draft.");
        }
    }
}
