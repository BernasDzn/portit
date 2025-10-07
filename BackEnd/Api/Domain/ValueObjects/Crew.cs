using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;

namespace Api.Domain.ValueObjects;

public class Crew : IDTOAble<CrewDto>
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Designation Captain { get; private set; }
    public int TotalCrewMembers { get; private set; }
    public ICollection<SafetyOfficer>? SafetyOfficers { get; private set; }

    protected Crew() { }
    public Crew(string captain, int totalCrewMembers, HashSet<SafetyOfficer>? safetyOfficers = null)
    {
        Captain = new Designation { Value = captain };

        if (totalCrewMembers < 1)
            throw new ArgumentException("Total crew members must be at least 1.");

        if (safetyOfficers != null && safetyOfficers.Count > totalCrewMembers)
            throw new ArgumentException("Number of safety officers cannot exceed total crew members.");

        TotalCrewMembers = totalCrewMembers;
        SafetyOfficers = safetyOfficers;
    }

    public CrewDto ToDTO() => new CrewDto
    {
        Captain = Captain.Value,
        TotalCrewMembers = TotalCrewMembers,
        SafetyOfficers = SafetyOfficers?.ToHashSet()
    };
}