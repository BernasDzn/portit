namespace Api.Application.DataTransfer;

public class VesselPositionDto
{
    public string VesselId { get; set; }
    public string DockId { get; set; }
    public DateTime ArrivalTime { get; set; }
    public DateTime DepartureTime { get; set; }
    public float LoadingTime { get; set; }
    public float UnloadingTime { get; set; }
}