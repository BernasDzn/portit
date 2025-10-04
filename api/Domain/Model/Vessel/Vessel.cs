using System.Text.RegularExpressions;
using Api.Controllers;
using Api.Models;
using Domain.Model.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Api.Domain.Model;

public class Vessel : IDTOAble<VesselDto>
{
    public Guid Id { get; private set; }
    public Designation Name { get; private set; }
    public ImoNumber ImoIdentifier { get; private set; }
    public virtual VesselType Type { get; private set; }
    public virtual ShippingAgentOrganization? Owner { get; private set; }
    public PhysicalCharacteristics PhysicalCharacteristics { get; private set; }

    // EF Core
    protected Vessel() { }

    public Vessel(Guid id, Designation name, ImoNumber imoNumber, VesselType type, ShippingAgentOrganization? owner, PhysicalCharacteristics physicalCharacteristics)
    {
        if (type == null) throw new ArgumentNullException(nameof(type), "Vessel Type cannot be null");
        if (physicalCharacteristics == null) throw new ArgumentNullException(nameof(physicalCharacteristics), "Physical Characteristics cannot be null");
        Id = id;
        Name = name;
        ImoIdentifier = imoNumber;
        Type = type;
        Owner = owner;
        PhysicalCharacteristics = physicalCharacteristics;
        validatePhysicalCharacteristics();
    }

    public VesselDto ToDTO()
    {
        return new VesselDto
        {
            Name = this.Name.Value,
            ImoNumber = this.ImoIdentifier.Value,
            Type = this.Type.ToDTO(),
            Owner = Owner?.ToDTO(),
            PhysicalCharacteristics = PhysicalCharacteristics.ToDTO()
        };
    }

    public void UpdateName(string name)
    {
        Name = new Designation { Value = name };
    }
    public void UpdateImoNumber(string imoNumber)
    {
        ImoIdentifier = new ImoNumber { Value = imoNumber };
    }
    public void UpdateVesselType(VesselType vesselType)
    {
        Type = vesselType ?? throw new ArgumentNullException(nameof(vesselType), "Vessel Type cannot be null");
        validatePhysicalCharacteristics();
    }
    public void UpdateOwner(ShippingAgentOrganization? owner)
    {
        Owner = owner;
    }
    public void UpdatePhysicalCharacteristics(PhysicalCharacteristics physicalCharacteristics)
    {
        PhysicalCharacteristics = physicalCharacteristics ?? throw new ArgumentNullException(nameof(physicalCharacteristics));
        validatePhysicalCharacteristics();
    }

    private void validatePhysicalCharacteristics()
    {
        if (PhysicalCharacteristics.Length > Type.PhysicalCharacteristics.Length)
            throw new ArgumentException($"The vessel's length exceeds the maximum length for the vessel type {Type.Name}.");
        if (PhysicalCharacteristics.Depth > Type.PhysicalCharacteristics.Depth)
            throw new ArgumentException($"The vessel's depth exceeds the maximum depth for the vessel type {Type.Name}.");
        if (PhysicalCharacteristics.Draft > Type.PhysicalCharacteristics.Draft)
            throw new ArgumentException($"The vessel's max draft exceeds the maximum draft for the vessel type {Type.Name}.");
    }

}