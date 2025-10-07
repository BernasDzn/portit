namespace Api.Application.DataTransfer;

public class VesselVisitNotificationDto
{
    public required DateTime ExpectedArrival { get; set; }
    public required DateTime ExpectedDeparture { get; set; }
    public required bool IsCargoHazardous { get; set; }
    public string? SpecialRequirements { get; set; }
    public CrewDto? CrewDetails { get; set; }
    public required VesselDto Vessel { get; set; }
    public required RepresentativeDto Representative { get; set; }
    
}