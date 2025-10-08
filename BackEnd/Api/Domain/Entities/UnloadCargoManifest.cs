using Api.Application.DataTransfer;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Utilities;

namespace Api.Domain.Entities;

public class UnloadCargoManifest : IDTOAble<UnloadCargoManifestDto>
{
    public Guid Id { get; private set; }
    public virtual ICollection<CargoTransport> Items { get; private set; }

    protected UnloadCargoManifest() { }

    public UnloadCargoManifest(ICollection<CargoTransport> items)
    {
        Id = Guid.NewGuid();
        Items = items;
    }

    public UnloadCargoManifestDto ToDTO()
    {
        return new UnloadCargoManifestDto
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