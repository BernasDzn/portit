using Microsoft.AspNetCore.Identity;

namespace Api.Domain.Entities;

/// <summary>
/// Represents a user in the system. 
/// <br/><br/>
/// <i> - This is still a WIP and MUST be expanded in the future when AuthN/Z is implemented.</i>
/// <br/><br/>
/// <b>Note:</b> Can be replaced with IdentityUser from Microsoft.AspNetCore.Identity
/// </summary>
public class User
{
	public Guid Id { get; set; }

	public string Username { get; private set; } = string.Empty;
	public string PasswordHash { get; private set; } = string.Empty;
	public bool IsActive { get; private set; } = true;
	public DateTime CreatedAt { get; private set; }

	public string? Email { get; private set; } = string.Empty;
	public string? DisplayName { get; private set; } = string.Empty;
	public UserRole? Role { get; private set; }

	public void Deactivate() => IsActive = false;
	public void Activate() => IsActive = true;
	public bool HasPermissions() => Role != null;

}

public enum UserRole
{
	Administrator,
	PortAuthorityOfficer,
	SAORepresentative,
	LogisticsOperator
}