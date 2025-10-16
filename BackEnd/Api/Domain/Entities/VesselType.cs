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

        if (maxNumberOfBays == 0) throw new ArgumentException("Max number of bays must be greater than zero.");
        if (maxNumberOfRows == 0) throw new ArgumentException("Max number of rows must be greater than zero.");
        if (maxNumberOfTiers == 0) throw new ArgumentException("Max number of tiers must be greater than zero.");

        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        MaxNumberOfRows = maxNumberOfRows;
        MaxNumberOfBays = maxNumberOfBays;
        MaxNumberOfTiers = maxNumberOfTiers;
        PhysicalCharacteristics = physicalCharacteristics ?? throw new ArgumentNullException(nameof(physicalCharacteristics));
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
            PhysicalCharacteristics = this.PhysicalCharacteristics
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
        if (maxNumberOfRows == 0) throw new ArgumentException("Max number of rows must be greater than zero.");
        MaxNumberOfRows = maxNumberOfRows;
    }
    public void UpdateMaxNumberOfBays(uint maxNumberOfBays)
    {
        if (maxNumberOfBays == 0) throw new ArgumentException("Max number of bays must be greater than zero.");
        MaxNumberOfBays = maxNumberOfBays;
    }
    public void UpdateMaxNumberOfTiers(uint maxNumberOfTiers)
    {
        if (maxNumberOfTiers == 0) throw new ArgumentException("Max number of tiers must be greater than zero.");
        MaxNumberOfTiers = maxNumberOfTiers;
    }
    public void UpdatePhysicalCharacteristics(PhysicalCharacteristics physicalCharacteristics)
    {
        if (Vessels != null && Vessels.Count > 0)
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
        }

        if (Docks != null && Docks.Count > 0)
        {
            foreach (var d in Docks)
            {
                if (d.PhysicalCharacteristics.Length < physicalCharacteristics.Length ||
                   d.PhysicalCharacteristics.Depth < physicalCharacteristics.Depth ||
                   d.PhysicalCharacteristics.Draft < physicalCharacteristics.Draft)
                {
                    throw new InvalidOperationException("Cannot update physical characteristics of a vessel type assigned to docks with lesser physical characteristics.");
                }
            }
        }
        PhysicalCharacteristics = physicalCharacteristics ?? throw new ArgumentNullException(nameof(physicalCharacteristics));
    }
}