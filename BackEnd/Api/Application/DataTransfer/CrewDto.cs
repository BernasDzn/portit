using Api.Domain.ValueObjects;
namespace Api.Application.DataTransfer;

public class CrewDto
{
    public required string Captain { get; set; } = string.Empty;
    public required uint TotalCrewMembers { get; set; }
    public required HashSet<SafetyOfficer>? SafetyOfficers { get; set; }
}