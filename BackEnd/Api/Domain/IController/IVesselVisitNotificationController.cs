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
    public Task<ActionResult> Update(string id, CreateVesselVisitNotificationDto vesselVisitNotificationDto);
    public Task<ActionResult<Page<VesselVisitNotificationStatusDto>>> Filter([FromQuery] VesselVisitNotificationFilter filter);

    // Draft operations
    public Task<ActionResult> Submit(string id);
    public Task<ActionResult> DeleteDraft(string id);

    // Scheduling operations
    public Task<ActionResult<SchedulingResultDto>> CollectSchedulingData([FromQuery] DateTime day, uint daysAhead);
}