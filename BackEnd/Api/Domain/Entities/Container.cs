namespace Api.Domain.Entities;

using Api.Domain.ValueObjects;
using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;

public class Container : IDTOAble<ContainerDto>
{
    public Guid Id { get; private set; }
    public virtual ContainerNumber ContainerNumber { get; private set; }
    public virtual ContainerPosition? ContainerPosition { get; private set; }
    public virtual CargoType CargoType { get; private set; }
    public string Description { get; private set; }

    public Container(ContainerNumber containerNumber, ContainerPosition? containerPosition, CargoType cargoType, string description)
    {
        Id = Guid.NewGuid();
        ContainerNumber = containerNumber;
        ContainerPosition = containerPosition;
        CargoType = cargoType;
        Description = description;
    }

    //EF Core
    protected Container() { }

    public ContainerDto ToDTO()
    {
        return new ContainerDto
        {
            ContainerNumber = ContainerNumber.ToString(),
            ContainerRow = ContainerPosition?.Row,
            ContainerBay = ContainerPosition?.Bay,
            ContainerTier = ContainerPosition?.Tier,
            CargoType = CargoType.Type.ToString(),
            Description = Description
        };
    }

    public override string ToString()
    {
        return $"Container Number: {ContainerNumber}, Position: {ContainerPosition}, Cargo Type: {CargoType}, Description: {Description}";
    }
}