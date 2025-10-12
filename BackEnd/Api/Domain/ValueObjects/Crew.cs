using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Api.Domain.ValueObjects;

public class Crew
{
    public Designation Captain { get; private set; }
    public uint TotalCrewMembers { get; private set; }
    public ICollection<SafetyOfficer>? SafetyOfficers { get; private set; }

    protected Crew() { }
    public Crew(Designation captain, uint totalCrewMembers, ICollection<SafetyOfficer>? safetyOfficers = null)
    {
        Captain = captain ?? throw new ArgumentNullException(nameof(captain));

        if (totalCrewMembers < 1)
            throw new ArgumentException("Total crew members must be at least 1.");

        if (safetyOfficers != null && safetyOfficers.Count > totalCrewMembers)
            throw new ArgumentException("Number of safety officers cannot exceed total crew members.");

        TotalCrewMembers = totalCrewMembers;
        SafetyOfficers = safetyOfficers;
    }
}