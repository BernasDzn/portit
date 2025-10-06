using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Api.Domain.ValueObjects;

[Owned]
public class Crew
{
    private Designation _captain;
    public string Captain
    {
        get => _captain.Value;
        set => _captain = new Designation { Value = value };
    }

    private int _totalCrewMembers;
    public int TotalCrewMembers
    {
        get => _totalCrewMembers;
        set
        {
            if (value < 1)
                throw new ArgumentException("Total crew members must be at least 1");

            _totalCrewMembers = value;
        }
    }

    //TODO: EF Core does not support collections in owned types, so we need to research how to handle this manually later
    [NotMapped]
    private HashSet<SafetyOfficer>? _safetyOfficers;
    [NotMapped] 
    public HashSet<SafetyOfficer>? SafetyOfficers
    {
        get => _safetyOfficers;
        set
        {
            if (value != null && value.Count > TotalCrewMembers)
                throw new ArgumentException("Number of safety officers cannot exceed total crew members");

            _safetyOfficers = value;
        }
    }
    

    public override string ToString() => $"{Captain}, {TotalCrewMembers} crew members, Safety Officers: [{string.Join("; ", SafetyOfficers ?? new HashSet<SafetyOfficer>())}]";
}