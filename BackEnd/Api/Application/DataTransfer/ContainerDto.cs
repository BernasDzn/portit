using Api.Domain.ValueObjects;
namespace Api.Application.DataTransfer;

public class ContainerDto
{
    public required string ContainerNumber { get; set; }
    public required CargoType CargoType { get; set; }
    public required string Description { get; set; }
}

public class CargoTransportDto
{
    public required ContainerPosition Position { get; set; }
    public required StorageAreaDto Area { get; set; }
    public required ContainerDto Container { get; set; }
}

public class CreateCargoTransportDto
{
    public required ContainerPosition Position { get; set; }
    public required string StorageAreaCode { get; set; }
    public required ContainerDto Container { get; set; }
}