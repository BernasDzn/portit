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

    public VesselVisitNotificationController(VesselVisitNotificationService notificationService, ILogger<VesselVisitNotificationController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    [HttpGet(Name = "GetVesselVisitNotifications")]
    public async Task<ActionResult<IEnumerable<VesselVisitNotificationDto>>> GetAll()
    {
        IEnumerable<VesselVisitNotificationDto> notificationsDto = await _notificationService.GetVesselVisitNotifications();
        return Ok(notificationsDto);
    }

}
