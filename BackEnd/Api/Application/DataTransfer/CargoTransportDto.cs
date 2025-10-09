using Api.Infrastructure.Utilities;

namespace Api.Application.DataTransfer;

public class CargoTransportDto
{
    public required ContainerDto Container { get; set; }
    public required ContainerPositionDto Position { get; set; }
    public required StorageAreaDto Area { get; set; }
}