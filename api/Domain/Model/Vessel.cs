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
    public virtual ShippingAgentOrganization Owner { get; private set; }

    // EF Core
    protected Vessel() { }

    public Vessel(Guid id, Designation name, ImoNumber imoNumber, VesselType type, ShippingAgentOrganization owner)
    {
        if (type == null) throw new ArgumentNullException(nameof(type), "Vessel Type cannot be null");
        if (owner == null) throw new ArgumentException("Owner cannot be null", nameof(owner));

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
            Type = this.Type,
            Owner = this.Owner.ToDTO()
        };
    }

    internal void Update(VesselDto vessel)
    {
        if (vessel.Name.IsNullOrEmpty()) throw new ArgumentException("Name cannot be null or empty", nameof(vessel.Name));
        if (vessel.Type == null) throw new ArgumentNullException(nameof(vessel.Type), "Vessel Type cannot be null");

        Name = new Designation { Value = vessel.Name };
        ImoIdentifier = new ImoNumber { Value = vessel.ImoNumber };
        Type.Update(vessel.Type.ToDTO());
    }
}