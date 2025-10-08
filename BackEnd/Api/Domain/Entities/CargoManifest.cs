using Api.Application.DataTransfer;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Utilities;

namespace Api.Domain.Entities;

public class CargoManifest : IDTOAble<CargoManifestDto>
{
    public Guid Id { get; private set; }
    public virtual ICollection<CargoTransport> Items { get; private set; }

    protected CargoManifest() { }

    public CargoManifest(ICollection<CargoTransport> items)
    {
        Id = Guid.NewGuid();
        Items = items;
    }

    public CargoManifestDto ToDTO()
    {
        return new CargoManifestDto
        {
            Items = Items.Select(item => new CargoTransportDto
            {
                ContainerNumber = item.Container.ContainerNumber.ToString(),
                Source = item.Source.ToString(),
                Destination = item.Destination.ToString(),
                CargoType = item.Container.CargoType.Type.ToString(),
                Description = item.Container.Description
            }).ToList()
        };
    }
}