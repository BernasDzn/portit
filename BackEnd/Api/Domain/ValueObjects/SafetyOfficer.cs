using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace Api.Domain.ValueObjects;

[Owned]
public class SafetyOfficer
{
    private string _citizenID;
    public string CitizenID
    {
        get => _citizenID;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Citizen ID cannot be empty");

            _citizenID = value;
        }
    }
    private string _name;
    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Name cannot be empty");

            _name = value;
        }
    }

    private RegionInfo _nationality;
    public string Nationality
    {
        get => _nationality.TwoLetterISORegionName;
        set
        {
            _nationality = new RegionInfo(value);
        }
    }

    public override string ToString() => $"ID: {CitizenID}, Name: {Name}, Nationality: {Nationality}";
}