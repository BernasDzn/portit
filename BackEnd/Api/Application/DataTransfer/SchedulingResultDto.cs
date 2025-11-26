using Api.Domain.Entities;
using Api.Domain.ValueObjects;

namespace Api.Application.DataTransfer;

public class VesselTaskFactDto
{
    public VesselDto Vessel { get; set; }
    public string Dock { get; set; }
    public double ETA { get; set; }
    public double ETD { get; set; }
    public double LoadingCount { get; set; }
    public double UnloadingCount { get; set; }
}

public class CraneWorkloadDto
{
    public string Crane { get; set; }
    public uint Speed { get; set; }
    public string Dock { get; set; }
    public OperationalWindow operatingWindow { get; set; }
}

public class SchedulingResultDto
{
    public List<DockDto> Docks { get; set; }
    public List<CraneWorkloadDto> CraneWorkloads { get; set; }
    public List<VesselTaskFactDto> VesselTaskFacts { get; set; }
    public string Comment { get; set; }
}