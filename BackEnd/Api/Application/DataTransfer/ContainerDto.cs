using Api.Domain.ValueObjects;
namespace Api.Application.DataTransfer;

public class ContainerDto
{
    public required string ContainerNumber { get; set; }
    public required string ContainerRow { get; set; }
    public required string ContainerBay { get; set; }
    public required string ContainerTier { get; set; }
    public required string CargoType { get; set; }
    public required string Description { get; set; }
}