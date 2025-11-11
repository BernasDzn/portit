namespace Api.Application.DataTransfer;

public class VesselTaskFactDto
{
    public VesselDto Vessel { get; set; }
    public double ETA { get; set; }
    public double ETD { get; set; }
    public double LoadingTime { get; set; }
    public double UnloadingTime { get; set; }
}

public class SchedulingResultDto
{
    public List<VesselTaskFactDto> VesselTaskFacts { get; set; }
    public string Comment { get; set; }
}