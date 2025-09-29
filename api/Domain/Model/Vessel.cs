using System.Text.RegularExpressions;
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
        if(!validateIMONumber(imoNumber)) throw new ArgumentException("Invalid IMO Number format", nameof(imoNumber));
        if (type == null) throw new ArgumentNullException(nameof(type), "Vessel Type cannot be null");
        if(ownerCitizenshipId <= 0) throw new ArgumentException("Owner Citizenship ID must be a positive integer", nameof(ownerCitizenshipId));
        Id = id;
        Name = name;
        ImoNumber = imoNumber;
        Type = type;
        OwnerCitizenshipId = ownerCitizenshipId;
    }

    private bool validateIMONumber(string imoNumber)
    {
        if (imoNumber == null) return false;

        // Must start with "IMO " and then 7 digits
        var match = Regex.Match(imoNumber, @"^IMO\s?(\d{7})$");
        if (!match.Success)
            return false;

        string digits = match.Groups[1].Value;

        // Extract check digit (last digit)
        int checkDigit = digits[6] - '0';

        // Compute check digit from first 6 digits
        int sum = 0;
        for (int i = 0; i < 6; i++)
        {
            int digit = digits[i] - '0';
            sum += digit * (7 - i);
        }

        int calculated = sum % 10;

        return calculated == checkDigit;
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