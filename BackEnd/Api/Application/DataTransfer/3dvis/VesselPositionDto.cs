namespace Api.Application.DataTransfer;

public class VesselPositionDto
{
    public string VesselId { get; set; }
    public string DockId { get; set; }
    public DateTime ArrivalTime { get; set; }
    public DateTime DepartureTime { get; set; }

    public VesselPositionDto(string vesselId, string dockId, DateTime arrivalTime, DateTime departureTime)
    {
        VesselId = vesselId;
        DockId = dockId;
        ArrivalTime = arrivalTime;
        DepartureTime = departureTime;
    }
}