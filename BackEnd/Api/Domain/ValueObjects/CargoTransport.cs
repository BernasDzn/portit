using Api.Domain.Entities;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Utilities;
using Api.Application.DataTransfer;


namespace Api.Domain.ValueObjects;

public class CargoTransport : IDTOAble<CargoTransportDto>
{
    public Guid Id { get; private set; }
    public virtual ContainerPosition Position { get; private set; }
    public virtual StorageArea Area { get; private set; }
    public virtual Container Container { get; private set; }

    public CargoTransport(ContainerPosition position, StorageArea area, Container container)
    {
        Id = Guid.NewGuid();
        Position = position ?? throw new ArgumentNullException("Invalid position in CargoTransport: " + position);
        Area = area ?? throw new ArgumentNullException("Invalid area in CargoTransport: " + area);
        Container = container ?? throw new ArgumentNullException("Invalid container in CargoTransport: " + container);
    }

    protected CargoTransport() { }

    public CargoTransportDto ToDTO()
    {
        return new CargoTransportDto
        {
            Container = Container.ToDTO(),
            Area = Area.ToDTO(),
            Position = Position.ToDTO()
        };
    }
}