using Api.Domain.Entities;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Utilities;


namespace Api.Domain.ValueObjects;

public class CargoTransport
{
    public Guid Id { get; private set; }
    public virtual Either<StorageArea, ContainerPosition> Source { get; private set; }
    public virtual Either<StorageArea, ContainerPosition> Destination { get; private set; }
    public virtual Container Container { get; private set; }

    public CargoTransport(Either<StorageArea, ContainerPosition> source,
                          Either<StorageArea, ContainerPosition> destination, Container container)
    {
        Id = Guid.NewGuid();
        Source = source;
        Destination = destination;
        Container = container;
    }

    protected CargoTransport() { }
}