namespace Api.Domain.Entities;

using Api.Domain.ValueObjects;
using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;

public class Container : IDTOAble<ContainerDto>
{
    public Guid Id { get; private set; }
    public virtual ContainerNumber ContainerNumber { get; private set; }
    public CargoType Type { get; private set; }
    public Designation Description { get; private set; }

    public Container(Guid id, ContainerNumber containerNumber, CargoType cargoType, Designation description)
    {
        Id = id;
        ContainerNumber = containerNumber;
        Type = cargoType;
        Description = description;
    }

    //EF Core
    protected Container() { }

    public ContainerDto ToDTO()
    {
        return new ContainerDto
        {
            ContainerNumber = ContainerNumber.Value,
            CargoType = Type,
            Description = Description.Value
        };
    }
}