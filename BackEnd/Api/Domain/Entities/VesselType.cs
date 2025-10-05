namespace Api.Domain.Entities;

using Api.Domain.ValueObjects;
using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;

public class VesselType : IDTOAble<VesselTypeDto>
{
    public Guid Id { get; private set; }

    public Designation Name { get; private set; }
    public Designation Description { get; private set; }
    public uint MaxNumberOfRows { get; private set; }
    public uint MaxNumberOfBays { get; private set; }
    public uint MaxNumberOfTiers { get; private set; }
    public PhysicalCharacteristics PhysicalCharacteristics { get; private set; }
    public virtual ICollection<Dock> Docks { get; private set; }
    public virtual ICollection<Vessel> Vessels { get; private set; }

    //EF Core
    protected VesselType() { }
    public VesselType(Guid id, Designation name, Designation description, uint maxNumberOfRows, uint maxNumberOfBays, uint maxNumberOfTiers, PhysicalCharacteristics physicalCharacteristics)
    {
        Id = id;
        Name = name;
        Description = description;
        MaxNumberOfRows = maxNumberOfRows;
        MaxNumberOfBays = maxNumberOfBays;
        MaxNumberOfTiers = maxNumberOfTiers;
        PhysicalCharacteristics = physicalCharacteristics;
    }

    public VesselTypeDto ToDTO()
    {
        return new VesselTypeDto
        {
            Name = this.Name.Value,
            Description = this.Description.Value,
            MaxNumberOfRows = this.MaxNumberOfRows,
            MaxNumberOfBays = this.MaxNumberOfBays,
            MaxNumberOfTiers = this.MaxNumberOfTiers,
            PhysicalCharacteristics = this.PhysicalCharacteristics.ToDTO()
        };
    }

    public void UpdateName(string name)
    {
        Name = new Designation { Value = name };
    }
    public void UpdateDescription(string description)
    {
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
    public void UpdatePhysicalCharacteristics(PhysicalCharacteristics physicalCharacteristics)
    {
        foreach (var v in Vessels)
        {
            if (v.PhysicalCharacteristics.Length > physicalCharacteristics.Length ||
               v.PhysicalCharacteristics.Depth > physicalCharacteristics.Depth ||
               v.PhysicalCharacteristics.Draft > physicalCharacteristics.Draft)
            {
                throw new InvalidOperationException("Cannot update physical characteristics of a vessel type assigned to vessels with greater physical characteristics.");
            }
        }

        foreach (var d in Docks)
        {
            if (d.PhysicalCharacteristics.Length < physicalCharacteristics.Length ||
               d.PhysicalCharacteristics.Depth < physicalCharacteristics.Depth ||
               d.PhysicalCharacteristics.Draft < physicalCharacteristics.Draft)
            {
                throw new InvalidOperationException("Cannot update physical characteristics of a vessel type assigned to docks with lesser physical characteristics.");
            }
        }

        PhysicalCharacteristics = physicalCharacteristics ?? throw new ArgumentNullException(nameof(physicalCharacteristics));
    }
}