using Domain.Model.Generic;

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
    public Dock(Guid id, Designation designation, Designation location, uint length, uint depth, uint maxDraft, List<VesselType> supportedVesselTypes)
    {
        Id = id;
        Name = designation;
        Location = location;
        Length = length;
        Depth = depth;
        MaxDraft = maxDraft;
        SupportedVesselTypes = supportedVesselTypes;
    }

    public bool UpdateDesignation(Designation newDesignation) {
        Name = newDesignation ?? throw new ArgumentNullException(nameof(newDesignation));
        return true;
    }
    public bool UpdateLocation(Designation newLocation) {
        
        Location = newLocation ?? throw new ArgumentNullException(nameof(newLocation));
        return true;
    }
    public bool UpdateLength(uint newLength)
    {
        Length = newLength;
        return true;
    }
    public bool UpdateDepth(uint newDepth)
    {
        Depth = newDepth;
        return true;
    }
    public bool UpdateMaxDraft(uint newMaxDraft)
    {
        MaxDraft = newMaxDraft;
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
            Id = this.Id,
            Designation = this.Name.Value,
            Location = this.Location.Value,
            Length = this.Length,
            Depth = this.Depth,
            MaxDraft = this.MaxDraft,
            SupportedVesselTypes = this.SupportedVesselTypes.Select(vt => vt.ToDTO()).ToList()
        };
    }

    internal static Dock FromDTO(DockDto dockDto)
    {
        return new Dock(
            dockDto.Id,
            new Designation { Value = dockDto.Designation },
            new Designation { Value = dockDto.Location },
            dockDto.Length,
            dockDto.Depth,
            dockDto.MaxDraft,
            dockDto.SupportedVesselTypes.Select(vtDto => new VesselType(
                Guid.NewGuid(),
                new Designation { Value = vtDto.Name },
                vtDto.Description,
                vtDto.MaxNumberOfRows,
                vtDto.MaxNumberOfBays,
                vtDto.MaxNumberOfTiers)).ToList()
        );
    }
}