namespace Api.Application.DataTransfer.Filters;

using Api.Domain.Entities;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Utilities;

public enum NotificationStatusFilter
{
	InProgress = 0,
	ApprovalPending = 1,
	Accepted = 2,
	Rejected = 3
}
public class VesselVisitNotificationFilter : Pageable
{
	public required uint SubmitterCitizenshipId { get; set; }
	public NotificationStatusFilter? Status { get; set; }
	public bool? WithReason { get; set; }
	public bool? WithDockAssigned { get; set; }
	public ImoNumber? Vessel { get; set; }
	public DateTime? ExpectedArrivalFrom { get; set; }
	public DateTime? ExpectedArrivalTo { get; set; }
}