namespace Api.Domain.Model;

public class Dock : IDTOAble<DockDto>
{
    public Guid Id { get; private set; }
    public string Designation { get; private set; }
    public string Location { get; private set; }
    public int Length { get; private set; }
    public int Depth { get; private set; }
    public int MaxDraft { get; private set; }
    public virtual ICollection<VesselType> SupportedVesselTypes { get; private set; }


    //EF Core
    protected Dock() { }
    public Dock(Guid id, string designation, string location, int length, int depth, int maxDraft, List<VesselType> supportedVesselTypes)
    {
        if (string.IsNullOrEmpty(designation) || string.IsNullOrEmpty(location) || length < 0 || depth < 0 || maxDraft < 0
        || supportedVesselTypes == null || supportedVesselTypes.Count == 0)
            throw new ArgumentException("Invalid arguments!");

        Id = id;
        Designation = designation;
        Location = location;
        Length = length;
        Depth = depth;
        MaxDraft = maxDraft;
        SupportedVesselTypes = supportedVesselTypes;
    }

    public bool UpdateDesignation(string new_designation)
    {
        if (string.IsNullOrWhiteSpace(new_designation))
            return false;

        Designation = new_designation;
        return true;
    }
    public bool UpdateLocation(string new_location)
    {
        if (string.IsNullOrWhiteSpace(new_location))
            return false;

        Location = new_location;
        return true;
    }
    public bool UpdateLength(int new_length)
    {
        if (new_length < 0)
            return false;

        Length = new_length;
        return true;
    }
    public bool UpdateDepth(int new_depth)
    {
        if (new_depth < 0)
            return false;

        Depth = new_depth;
        return true;
    }
    public bool UpdateMaxDraft(int new_maxDraft)
    {
        if (new_maxDraft < 0)
            return false;

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
            Id = this.Id,
            Designation = this.Designation,
            Location = this.Location,
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
            dockDto.Designation,
            dockDto.Location,
            dockDto.Length,
            dockDto.Depth,
            dockDto.MaxDraft,
            dockDto.SupportedVesselTypes.Select(vtDto => new VesselType(
                Guid.NewGuid(),
                vtDto.Name,
                vtDto.Description,
                vtDto.MaxNumberOfRows,
                vtDto.MaxNumberOfBays,
                vtDto.MaxNumberOfTiers)).ToList()
        );
    }
}