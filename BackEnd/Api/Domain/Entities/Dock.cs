namespace Api.Domain.Entities;

using Api.Domain.ValueObjects;
using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;

public class Dock : IDTOAble<DockDto>
{
    public Guid Id { get; private set; }
    public Designation Name { get; private set; }
    public Designation Location { get; private set; }
    public PhysicalCharacteristics PhysicalCharacteristics { get; private set; }
    public virtual ICollection<VesselType> SupportedVesselTypes { get; private set; }


    //EF Core
    protected Dock() { }
    public Dock(Guid id, Designation name, Designation location, PhysicalCharacteristics physicalCharacteristics, HashSet<VesselType> supportedVesselTypes)
    {
        if (supportedVesselTypes == null || supportedVesselTypes.Count == 0)
            throw new ArgumentException("Invalid arguments!");

        Id = id;
        Name = name;
        Location = location;
        PhysicalCharacteristics = physicalCharacteristics;
        SupportedVesselTypes = supportedVesselTypes;
        validatePhysicalCharacteristics();
    }

    public void UpdateName(Designation newName)
    {
        Name = newName;
    }
    public void UpdateLocation(Designation newLocation)
    {
        Location = newLocation;
    }
    public void UpdatePhysicalCharacteristics(PhysicalCharacteristics physicalCharacteristics)
    {
        PhysicalCharacteristics = physicalCharacteristics ?? throw new ArgumentNullException(nameof(physicalCharacteristics));
        validatePhysicalCharacteristics();
    }

    public void UpdateVesselTypes(ICollection<VesselType> new_vesselTypes)
    {
        if (new_vesselTypes == null || new_vesselTypes.Count == 0)
            throw new ArgumentException("Invalid vessel types", nameof(new_vesselTypes));

        SupportedVesselTypes.Clear();
        SupportedVesselTypes = new_vesselTypes;
        validatePhysicalCharacteristics();
    }

    public DockDto ToDTO()
    {
        return new DockDto
        {
            Name = this.Name.Value,
            Location = this.Location.Value,
            PhysicalCharacteristics = this.PhysicalCharacteristics.ToDTO(),
            SupportedVesselTypes = this.SupportedVesselTypes.Select(vt => vt.ToDTO()).ToList()
        };
    }

    private void validatePhysicalCharacteristics()
    {
        foreach (var vt in SupportedVesselTypes)
        {
            if (PhysicalCharacteristics.Length < vt.PhysicalCharacteristics.Length)
                throw new ArgumentException($"The dock's length is insufficient for the vessel type {vt.Name}.");
            if (PhysicalCharacteristics.Depth < vt.PhysicalCharacteristics.Depth)
                throw new ArgumentException($"The dock's depth is insufficient for the vessel type {vt.Name}.");
            if (PhysicalCharacteristics.Draft < vt.PhysicalCharacteristics.Draft)
                throw new ArgumentException($"The dock's max draft is insufficient for the vessel type {vt.Name}.");
        }
    }
}