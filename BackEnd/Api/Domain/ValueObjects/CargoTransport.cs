using Api.Domain.Entities;
using Api.Domain.ValueObjects;

namespace Api.Domain.ValueObjects;

public class CargoTransport
{
    public Guid Id { get; private set; }
    public virtual ContainerPosition Position { get; private set; }
    public virtual StorageArea Area { get; private set; }
    public virtual Container Container { get; private set; }

    public CargoTransport(ContainerPosition position, StorageArea area, Container container)
    {
        Id = Guid.NewGuid();
        Position = position;
        Area = area;
        Container = container;
    }

    protected CargoTransport() { }
}