using System.Text.RegularExpressions;
using Api.Controllers;
using Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Api.Domain.Model;

[Index(nameof(ImoNumber), IsUnique = true)]
public class Vessel : IDTOAble<VesselDto>
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string ImoNumber { get; private set; }
    public virtual VesselType Type { get; private set; }
    public virtual ShippingAgentOrganization Owner { get; private set; }

    // EF Core
    protected Vessel() { }

    public Vessel(Guid id, string name, string imoNumber, VesselType type, ShippingAgentOrganization owner)
    {
        if (name.IsNullOrEmpty()) throw new ArgumentException("Name cannot be null or empty", nameof(name));
        if (!validateIMONumber(imoNumber)) throw new ArgumentException("Invalid IMO Number format", nameof(imoNumber));
        if (type == null) throw new ArgumentNullException(nameof(type), "Vessel Type cannot be null");
        if (owner == null) throw new ArgumentException("Owner cannot be null", nameof(owner));
        Id = id;
        Name = name;
        ImoNumber = imoNumber;
        Type = type;
        Owner = owner;
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
            Name = this.Name,
            ImoNumber = this.ImoNumber,
            Type = this.Type,
            Owner = this.Owner.ToDTO()
        };
    }

    internal void Update(VesselDto vessel)
    {
        if (vessel.Name.IsNullOrEmpty()) throw new ArgumentException("Name cannot be null or empty", nameof(vessel.Name));
        if (!validateIMONumber(vessel.ImoNumber)) throw new ArgumentException("Invalid IMO Number format", nameof(vessel.ImoNumber));
        if (vessel.Type == null) throw new ArgumentNullException(nameof(vessel.Type), "Vessel Type cannot be null");

        Name = vessel.Name;
        ImoNumber = vessel.ImoNumber;
        Type.Update(vessel.Type.ToDTO());
    }
}