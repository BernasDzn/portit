using Api.Domain.ValueObjects;
namespace Api.Application.DataTransfer;

public class ContainerDto
{
    public required string ContainerNumber { get; set; }
    public required string ContainerPosition { get; set; }
    public required string CargoType { get; set; }
}