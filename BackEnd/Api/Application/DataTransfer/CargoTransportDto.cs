using Api.Infrastructure.Utilities;

namespace Api.Application.DataTransfer;

public class CargoTransportDto
{
    public required ContainerDto Container { get; set; }
    public required Either<StorageAreaDto, ContainerPositionDto> Source { get; set; }
    public required Either<StorageAreaDto, ContainerPositionDto> Destination { get; set; }
}