using Api.Application.DataTransfer;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Utilities;

namespace Api.Domain.Entities;

public class LoadCargoManifest : IDTOAble<LoadCargoManifestDto>
{
    public Guid Id { get; private set; }
    public virtual ICollection<CargoTransport> Items { get; private set; }

    protected LoadCargoManifest() { }

    public LoadCargoManifest(ICollection<CargoTransport> items)
    {
        Id = Guid.NewGuid();
        Items = items;
    }

    public LoadCargoManifestDto ToDTO()
    {
        return new LoadCargoManifestDto
        {
            Items = Items.Select(item => new CargoTransportDto
            {
                ContainerNumber = item.Container.ContainerNumber.ToString(),
                ContainerPosition = item.Position.ToString(),
                CargoType = item.Container.CargoType.Type.ToString(),
                Description = item.Container.Description
            }).ToList()
        };
    }
}