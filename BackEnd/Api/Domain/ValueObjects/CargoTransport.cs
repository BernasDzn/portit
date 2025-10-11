using Api.Domain.Entities;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Utilities;
using Api.Application.DataTransfer;
using Microsoft.EntityFrameworkCore;


namespace Api.Domain.ValueObjects;

[Owned]
public class CargoTransport : IDTOAble<CargoTransportDto>
{
    public ContainerPosition Position { get; private set; }
    public virtual StorageArea Area { get; private set; }
    public virtual Container Container { get; private set; }

    public CargoTransport(ContainerPosition position, StorageArea area, Container container)
    {
        Position = position ?? throw new ArgumentNullException("Invalid position in CargoTransport: " + position);
        Area = area ?? throw new ArgumentNullException("Invalid area in CargoTransport: " + area);
        Container = container ?? throw new ArgumentNullException("Invalid container in CargoTransport: " + container);
    }

    // EF Core
    protected CargoTransport() { }

    public CargoTransportDto ToDTO()
    {
        return new CargoTransportDto
        {
            Position = Position,
            Area = Area.ToDTO(),
            Container = Container.ToDTO()
        };
    }
}