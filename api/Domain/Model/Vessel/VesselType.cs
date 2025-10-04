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
    public virtual ICollection<Dock> Docks { get; private set; }

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
    
    public void UpdateName(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));

        Name = new Designation { Value = name };
    }
    public void UpdateDescription(string description)
    {
        if (string.IsNullOrEmpty(description))
            throw new ArgumentException("Description cannot be null or empty", nameof(description));

        Description = new Designation { Value = description };
    }
    public void UpdateMaxNumberOfRows(uint maxNumberOfRows)
    {
        MaxNumberOfRows = maxNumberOfRows;
    }
    public void UpdateMaxNumberOfBays(uint maxNumberOfBays)
    {
        MaxNumberOfBays = maxNumberOfBays;
    }
    public void UpdateMaxNumberOfTiers(uint maxNumberOfTiers)
    {
        MaxNumberOfTiers = maxNumberOfTiers;
    }
}