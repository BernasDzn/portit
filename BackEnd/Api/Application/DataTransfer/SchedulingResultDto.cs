namespace Api.Application.DataTransfer;

public class VesselTaskFactDto
{
    public VesselDto Vessel { get; set; }
    public uint ETA { get; set; }
    public uint ETD { get; set; }
    public uint LoadingTime { get; set; }
    public uint UnloadingTime { get; set; }
}

public class SchedulingResultDto
{
    public List<VesselTaskFactDto> VesselTaskFacts { get; set; }
    public string Comment { get; set; }
}