namespace Api.Domain.Entities;

using Api.Domain.ValueObjects;
using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;

public class Container : IDTOAble<ContainerDto>
{
    public Guid Id { get; private set; }
    public ContainerNumber ContainerNumber { get; private set; }
    public PhysicalCharacteristics PhysicalCharacteristics { get; private set; }
    public CargoType CargoType { get; private set; }

    public ContainerDto ToDTO()
    {
        return new ContainerDto
        {
            ContainerNumber = ContainerNumber.ToString(),
            PhysicalCharacteristics = PhysicalCharacteristics.ToDTO(),
            CargoType = CargoType.Type.ToString()
        };
    }
}