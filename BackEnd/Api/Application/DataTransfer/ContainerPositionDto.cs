namespace Api.Application.DataTransfer;

public class ContainerPositionDto
{
    public required string Bay { get; set; }
    public required string Row { get; set; }
    public required string Tier { get; set; }
}