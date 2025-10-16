namespace Api.Domain.Entities;

using Api.Domain.ValueObjects;
using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;

public class Dock : IDTOAble<DockDto>
{
    public Guid Id { get; private set; }
    public Code Code { get; private set; }
    public Designation Name { get; private set; }
    public Designation Location { get; private set; }
    public PhysicalCharacteristics PhysicalCharacteristics { get; private set; }
    public virtual ICollection<VesselType> SupportedVesselTypes { get; private set; }


    //EF Core
    protected Dock() { }
    public Dock(Guid id, Code code, Designation name, Designation location, PhysicalCharacteristics physicalCharacteristics, HashSet<VesselType> supportedVesselTypes)
    {
        validatePhysicalCharacteristics(physicalCharacteristics, supportedVesselTypes);

        Id = id;
        Code = code;
        Name = name ?? throw new ArgumentException("Name cannot be null.");
        Location = location ?? throw new ArgumentException("Location cannot be null.");
        PhysicalCharacteristics = physicalCharacteristics;
        SupportedVesselTypes = supportedVesselTypes;
    }

    public void UpdateName(string newName)
    {
        Name = new Designation { Value = newName };
    }
    public void UpdateLocation(string newLocation)
    {
        Location = new Designation { Value = newLocation };
    }
    public void UpdatePhysicalCharacteristics(PhysicalCharacteristics physicalCharacteristics)
    {
        validatePhysicalCharacteristics(physicalCharacteristics, SupportedVesselTypes);
        PhysicalCharacteristics = physicalCharacteristics;
    }

    public void UpdateVesselTypes(ICollection<VesselType> new_vesselTypes)
    {
        validatePhysicalCharacteristics(PhysicalCharacteristics, new_vesselTypes);

        SupportedVesselTypes.Clear();
        SupportedVesselTypes = new_vesselTypes;
    }

    public DockDto ToDTO()
    {
        return new DockDto
        {
            Code = this.Code.Value,
            Name = this.Name.Value,
            Location = this.Location.Value,
            PhysicalCharacteristics = this.PhysicalCharacteristics,
            SupportedVesselTypes = this.SupportedVesselTypes.Select(vt => vt.ToDTO()).ToList()
        };
    }

    private void validatePhysicalCharacteristics(PhysicalCharacteristics physicalCharacteristics, ICollection<VesselType> supportedVesselTypes)
    {
        if (physicalCharacteristics == null)
            throw new ArgumentException("Physical characteristics cannot be null.");
        if (supportedVesselTypes == null || supportedVesselTypes.Count == 0)
            throw new ArgumentException("Invalid vessel types", nameof(supportedVesselTypes));

        foreach (var vt in supportedVesselTypes)
        {
            if (physicalCharacteristics.Length < vt.PhysicalCharacteristics.Length)
                throw new ArgumentException($"The dock's length is insufficient for the vessel type {vt.Name}.");
            if (physicalCharacteristics.Depth < vt.PhysicalCharacteristics.Depth)
                throw new ArgumentException($"The dock's depth is insufficient for the vessel type {vt.Name}.");
            if (physicalCharacteristics.Draft < vt.PhysicalCharacteristics.Draft)
                throw new ArgumentException($"The dock's max draft is insufficient for the vessel type {vt.Name}.");
        }
    }
}