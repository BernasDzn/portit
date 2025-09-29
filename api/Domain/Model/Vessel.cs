using Microsoft.IdentityModel.Tokens;

namespace Api.Domain.Model;

public class Vessel : IDTOAble<VesselDto>
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string ImoNumber { get; private set; }
    public virtual VesselType Type { get; private set; }
    public uint OwnerCitizenshipId { get; private set; }

    // EF Core
    protected Vessel() { }

    public Vessel(Guid id, string name, string imoNumber, VesselType type, uint ownerCitizenshipId)
    {
        if(name.IsNullOrEmpty()) throw new ArgumentException("Name cannot be null or empty", nameof(name));
        if(imoNumber.IsNullOrEmpty()) throw new ArgumentException("IMO Number cannot be null or empty", nameof(imoNumber));
        if(type == null) throw new ArgumentNullException(nameof(type), "Vessel Type cannot be null");
        if(ownerCitizenshipId <= 0) throw new ArgumentException("Owner Citizenship ID must be a positive integer", nameof(ownerCitizenshipId));
        Id = id;
        Name = name;
        ImoNumber = imoNumber;
        Type = type;
        OwnerCitizenshipId = ownerCitizenshipId;
    }

    public VesselDto ToDTO()
    {
        return new VesselDto
        {
            Id = this.Id,
            Name = this.Name,
            ImoNumber = this.ImoNumber,
            Type = this.Type,
            OwnerCitizenshipId = this.OwnerCitizenshipId
        };
    }

    public static Vessel FromDTO(VesselDto dto)
    {
        return new Vessel(
            dto.Id,
            dto.Name,
            dto.ImoNumber,
            dto.Type,
            dto.OwnerCitizenshipId
        );
    }
}