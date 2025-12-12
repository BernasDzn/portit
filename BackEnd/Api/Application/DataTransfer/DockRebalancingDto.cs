namespace Api.Application.DataTransfer;

public class DockRebalancingDto
{
    public string VvnId { get; set; }
    public string Imo { get; set; }
    public string CurrentDock { get; set; }
    public string ProposedDock { get; set; }
}

public class RebalancingMetricsDto
{
    public int Reassignments { get; set; }
    public double AvgLoad { get; set; }
    public double LoadRange { get; set; }
    public double MinLoad { get; set; }
    public double MaxLoad { get; set; }
    public double StdDev { get; set; }
    public int VesselCount { get; set; }
    public int DockCount { get; set; }
    public double ComputationTime { get; set; }
}

public class DockRebalancingResponseDto
{
    public DockRebalancingDto[] Assignments { get; set; } = Array.Empty<DockRebalancingDto>();
    public RebalancingMetricsDto Metrics { get; set; } = new();
}
