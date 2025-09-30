using Domain.Model.Generic;
using Microsoft.EntityFrameworkCore;

namespace Api.Domain.Model;

public class VesselType : IDTOAble<VesselTypeDto>
{
    public Guid Id { get; private set; }

    public Designation Name { get; private set; }
    public string Description { get; private set; }
    public uint MaxNumberOfRows { get; private set; }
    public uint MaxNumberOfBays { get; private set; }
    public uint MaxNumberOfTiers { get; private set; }

    //EF Core
    protected VesselType() { }
    public VesselType(Guid id, Designation name, string description, uint maxNumberOfRows, uint maxNumberOfBays, uint maxNumberOfTiers)
    {
        if (name == null) throw new ArgumentNullException(nameof(name), "Name cannot be null");
        if (string.IsNullOrEmpty(description)) throw new ArgumentException("Description cannot be null or empty", nameof(description));

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
            Description = this.Description,
            MaxNumberOfRows = this.MaxNumberOfRows,
            MaxNumberOfBays = this.MaxNumberOfBays,
            MaxNumberOfTiers = this.MaxNumberOfTiers
        };
    }

    public void Update(VesselTypeDto vtype)
    {
        if (string.IsNullOrEmpty(vtype.Name)) throw new ArgumentException("Name cannot be null or empty", nameof(vtype.Name));
        if (string.IsNullOrEmpty(vtype.Description)) throw new ArgumentException("Description cannot be null or empty", nameof(vtype.Description));
        if (vtype.MaxNumberOfRows < 0) throw new ArgumentException("MaxNumberOfRows cannot be negative", nameof(vtype.MaxNumberOfRows));
        if (vtype.MaxNumberOfBays < 0) throw new ArgumentException("MaxNumberOfBays cannot be negative", nameof(vtype.MaxNumberOfBays));
        if (vtype.MaxNumberOfTiers < 0) throw new ArgumentException("MaxNumberOfTiers cannot be negative", nameof(vtype.MaxNumberOfTiers));

        Name = new Designation { Value = vtype.Name };
        Description = vtype.Description;
        MaxNumberOfRows = vtype.MaxNumberOfRows;
        MaxNumberOfBays = vtype.MaxNumberOfBays;
        MaxNumberOfTiers = vtype.MaxNumberOfTiers;
    }
}