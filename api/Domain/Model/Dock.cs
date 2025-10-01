using Domain.Model.Generic;
using Microsoft.EntityFrameworkCore;

namespace Api.Domain.Model;

public class Dock : IDTOAble<DockDto>
{
    public Guid Id { get; private set; }
    public Designation Name { get; private set; }
    public Designation Location { get; private set; }
    public uint Length { get; private set; }
    public uint Depth { get; private set; }
    public uint MaxDraft { get; private set; }
    public virtual ICollection<VesselType> SupportedVesselTypes { get; private set; }


    //EF Core
    protected Dock() { }
    public Dock(Guid id, Designation name, Designation location, uint length, uint depth, uint maxDraft, ICollection<VesselType> supportedVesselTypes)
    {
        if (supportedVesselTypes == null || supportedVesselTypes.Count == 0)
            throw new ArgumentException("Invalid arguments!");

        Id = id;
        Name = name;
        Location = location;
        Length = length;
        Depth = depth;
        MaxDraft = maxDraft;
        SupportedVesselTypes = supportedVesselTypes;
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrEmpty(newName))
            throw new ArgumentException("Name cannot be null or empty", nameof(newName));

        Name = new Designation { Value = newName };
    }
    public void UpdateLocation(string newLocation)
    {
        if (string.IsNullOrEmpty(newLocation))
            throw new ArgumentException("Location cannot be null or empty", nameof(newLocation));
            
        Location = new Designation { Value = newLocation };
    }
    public void UpdateLength(uint new_length)
    {
        Length = new_length;
    }
    public void UpdateDepth(uint new_depth)
    {
        Depth = new_depth;
    }
    public void UpdateMaxDraft(uint new_maxDraft)
    {
        MaxDraft = new_maxDraft;
    }

    public void UpdateVesselTypes(ICollection<VesselType> new_vesselTypes)
    {
        if (new_vesselTypes == null || new_vesselTypes.Count == 0)
            throw new ArgumentException("Invalid vessel types", nameof(new_vesselTypes));

        SupportedVesselTypes.Clear();
        SupportedVesselTypes = new_vesselTypes;
    }

    public DockDto ToDTO()
    {
        return new DockDto
        {
            Name = this.Name.Value,
            Location = this.Location.Value,
            Length = this.Length,
            Depth = this.Depth,
            MaxDraft = this.MaxDraft,
            SupportedVesselTypes = this.SupportedVesselTypes.Select(vt => vt.ToDTO()).ToList()
        };
    }
}