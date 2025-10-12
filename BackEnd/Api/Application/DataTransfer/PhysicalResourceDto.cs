namespace Api.Application.DataTransfer;

using Api.Domain.ValueObjects;
using Api.Domain.Entities;

public class PhysicalResourceDto
{
    public required string Code { get; set; }
    public required string Description { get; set; }
    public required ResourceStatus Status { get; set; }
    public required int SetupTimeInMinutes { get; set; }
    public required List<QualificationDto> Qualifications { get; set; }
    public required OperationalWindow OperationalWindow { get; set; }
}

public class STSCraneDto : PhysicalResourceDto
{
    public required uint ContainersPerHour { get; set; }
    public required uint LiftingCapacity { get; set; }
    public required DockDto ServingDock { get; set; }
}

public class YardCraneDto : PhysicalResourceDto
{
    public required uint ContainersPerHour { get; set; }
    public required uint LiftingCapacity { get; set; }
    public required StorageAreaDto YardSection { get; set; }
}

public class TruckDto : PhysicalResourceDto
{
    public required uint ContainersPerTrip { get; set; }
    public required uint AverageSpeed { get; set; } 
    public required uint MaxLoadCapacity { get; set; }
}