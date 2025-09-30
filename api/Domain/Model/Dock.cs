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
    public Dock(Guid id, Designation name, Designation location, uint length, uint depth, uint maxDraft, List<VesselType> supportedVesselTypes)
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

    public bool UpdateName(string newName)
    {
        Name = new Designation { Value = newName };
        return true;
    }
    public bool UpdateLocation(string newLocation)
    {
        Location = new Designation { Value = newLocation };
        return true;
    }
    public bool UpdateLength(uint new_length)
    {
        Length = new_length;
        return true;
    }
    public bool UpdateDepth(uint new_depth)
    {
        Depth = new_depth;
        return true;
    }
    public bool UpdateMaxDraft(uint new_maxDraft)
    {
        MaxDraft = new_maxDraft;
        return true;
    }

    public bool UpdateVesselTypes(List<VesselType> new_vesselTypes)
    {
        if (new_vesselTypes == null || new_vesselTypes.Count == 0)
            return false;

        SupportedVesselTypes = new_vesselTypes;
        return true;
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
            //SupportedVesselTypes = this.SupportedVesselTypes.Select(vt => vt.ToDTO()).ToList()
            SupportedVesselTypes = this.SupportedVesselTypes.ToList()
        };
    }
}