using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Api.Domain.ValueObjects;

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
    private Designation _name;
    public string Name
    {
        get => _name.Value;
        set => _name = new Designation { Value = value };
    }

    private RegionInfo _nationality;
    public string Nationality {
        get => _nationality.TwoLetterISORegionName;
        set {
            _nationality = new RegionInfo(value);
        }
    }

    public override string ToString() => $"ID: {CitizenID}, Name: {Name}, Nationality: {Nationality}";
}