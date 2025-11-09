using Api.Domain.Entities;
using Api.Domain.ValueObjects;

namespace Api.Application.DataTransfer;

public class VesselVisitNotificationDto
{
    public string NotificationId { get; set; }
    public required NotificationStatus Status { get; set; }
    public required DateTime ExpectedArrival { get; set; }
    public required DateTime ExpectedDeparture { get; set; }
    public required bool IsCargoHazardous { get; set; }
    public string? SpecialRequirements { get; set; }
    public Crew? CrewDetails { get; set; }
    public ICollection<CargoTransportDto>? LoadCargoManifest { get; set; }
    public ICollection<CargoTransportDto>? UnloadCargoManifest { get; set; }
    public required VesselDto Vessel { get; set; }
    public required RepresentativeDto Submitter { get; set; }
}

public class CreateVesselVisitNotificationDto
{
    public string NotificationId { get; set; }
    public required DateTime ExpectedArrival { get; set; }
    public required DateTime ExpectedDeparture { get; set; }
    public required bool IsCargoHazardous { get; set; }
    public string? SpecialRequirements { get; set; }
    public Crew? CrewDetails { get; set; }
    public ICollection<CreateCargoTransportDto>? LoadCargoManifest { get; set; }
    public ICollection<CreateCargoTransportDto>? UnloadCargoManifest { get; set; }
    public required string VesselImoNumber { get; set; }
}

public class VesselVisitNotificationStatusDto
{
    public string NotificationId { get; set; }
    public required NotificationStatus Status { get; set; }
    public required DateTime ExpectedArrival { get; set; }
    public required DateTime ExpectedDeparture { get; set; }
    public required VesselDto Vessel { get; set; }
    public required RepresentativeDto Submitter { get; set; }
    public NotificationDecisionDto[] Decisions { get; set; }
}