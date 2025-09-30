using Domain.Model.Generic;
using Microsoft.EntityFrameworkCore;

namespace Api.Domain.Model;

public class VesselType : IDTOAble<VesselTypeDto>
{
    public Guid Id { get; private set; }

    public Designation Name { get; private set; }
    public Designation Description { get; private set; }
    public uint MaxNumberOfRows { get; private set; }
    public uint MaxNumberOfBays { get; private set; }
    public uint MaxNumberOfTiers { get; private set; }

    //EF Core
    protected VesselType() { }
    public VesselType(Guid id, Designation name, Designation description, uint maxNumberOfRows, uint maxNumberOfBays, uint maxNumberOfTiers)
    {
        Id = id;
        Name = name;
        Description = description;
        MaxNumberOfRows = maxNumberOfRows;
        MaxNumberOfBays = maxNumberOfBays;
        MaxNumberOfTiers = maxNumberOfTiers;
    }

    public VesselTypeDto ToDTO()
    {
        return new VesselTypeDto
        {
            Name = this.Name.Value,
            Description = this.Description.Value,
            MaxNumberOfRows = this.MaxNumberOfRows,
            MaxNumberOfBays = this.MaxNumberOfBays,
            MaxNumberOfTiers = this.MaxNumberOfTiers
        };
    }

    public void Update(VesselTypeDto vtype)
    {
        Name = new Designation { Value = vtype.Name };
        Description = new Designation { Value = vtype.Description };
        MaxNumberOfRows = vtype.MaxNumberOfRows;
        MaxNumberOfBays = vtype.MaxNumberOfBays;
        MaxNumberOfTiers = vtype.MaxNumberOfTiers;
    }
}