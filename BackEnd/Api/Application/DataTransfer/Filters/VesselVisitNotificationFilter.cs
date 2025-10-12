namespace Api.Application.DataTransfer.Filters;

using Api.Domain.Entities;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Utilities;

public class VesselVisitNotificationFilter : Pageable
{
	public required uint SubmitterCitizeshipId { get; set; }
	public NotificationStatus? Status { get; set; }
	public bool? WithReason { get; set; }
	public bool? WithDockAssigned { get; set; }
	public ImoNumber? Vessel { get; set; }
	public DateTime? ExpectedArrivalFrom { get; set; }
	public DateTime? ExpectedArrivalTo { get; set; }	
}