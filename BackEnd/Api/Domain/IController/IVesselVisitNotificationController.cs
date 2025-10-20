using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Application.Controllers;

public interface IVesselVisitNotificationController
{
    public Task<ActionResult<IEnumerable<VesselVisitNotificationDto>>> GetAll();
    public Task<ActionResult<IEnumerable<NotificationDecisionDto>>> GetDecisions([FromQuery] string vesselVisitNotificationId);
    public Task<ActionResult<VesselVisitNotificationDto>> Create([FromBody] CreateVesselVisitNotificationDto vesselVisitNotificationDto);
    public Task<ActionResult<NotificationDecisionDto>> CreateDecision([FromQuery] string vesselVisitNotificationId, [FromBody] CreateNotificationDecisionDto notificationDecisionDto);
    public Task<ActionResult<VesselVisitNotificationDto>> Update(string id, CreateVesselVisitNotificationDto vesselVisitNotificationDto);
    public Task<ActionResult<Page<VesselVisitNotificationStatusDto>>> Filter([FromQuery] VesselVisitNotificationFilter filter);
}