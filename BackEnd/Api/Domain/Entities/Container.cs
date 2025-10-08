namespace Api.Domain.Entities;

using Api.Domain.ValueObjects;
using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;

public class Container : IDTOAble<ContainerDto>
{
    public Guid Id { get; private set; }
    public virtual ContainerNumber ContainerNumber { get; private set; }
    public virtual ContainerPosition ContainerPosition { get; private set; }
    public virtual CargoType CargoType { get; private set; }
    public string Description { get; private set; }

    public ContainerDto ToDTO()
    {
        return new ContainerDto
        {
            ContainerNumber = ContainerNumber.ToString(),
            ContainerPosition = ContainerPosition.ToString(),
            CargoType = CargoType.Type.ToString(),
            Description = Description
        };
    }
}