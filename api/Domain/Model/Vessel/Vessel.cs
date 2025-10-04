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

    // EF Core
    protected Vessel() { }

    public Vessel(Guid id, Designation name, ImoNumber imoNumber, VesselType type, ShippingAgentOrganization? owner)
    {
        if (type == null) throw new ArgumentNullException(nameof(type), "Vessel Type cannot be null");
        Id = id;
        Name = name;
        ImoIdentifier = imoNumber;
        Type = type;
        Owner = owner;
    }

    public VesselDto ToDTO()
    {
        return new VesselDto
        {
            Name = this.Name.Value,
            ImoNumber = this.ImoIdentifier.Value,
            Type = this.Type.ToDTO(),
            Owner = Owner?.ToDTO()
        };
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));
            
        Name = new Designation { Value = name };
    }
    public void UpdateImoNumber(string imoNumber)
    {
        if (string.IsNullOrEmpty(imoNumber))
            throw new ArgumentException("IMO Number cannot be null or empty", nameof(imoNumber));

        ImoIdentifier = new ImoNumber { Value = imoNumber };
    }
    public void UpdateVesselType(VesselType vesselType)
    {
        if (vesselType == null)
            throw new ArgumentNullException(nameof(vesselType), "Vessel Type cannot be null");

        Type = vesselType;
    }
    public void UpdateOwner(ShippingAgentOrganization? owner)
    {
        Owner = owner;
    }

}