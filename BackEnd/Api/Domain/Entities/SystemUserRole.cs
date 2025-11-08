namespace Api.Domain.Entities;

using Microsoft.AspNetCore.Identity;

// Enum for role types (for convenience and type safety)
public enum SystemUserRoleType
{
	Administrator,
	PortAuthorityOfficer,
	SAORepresentative,
	LogisticsOperator
}

// Identity role class for ASP.NET Core Identity
public class SystemUserRole : IdentityRole<Guid>
{
	public SystemUserRole() : base()
	{
		Id = Guid.NewGuid();
	}

	public SystemUserRole(string roleName) : base(roleName)
	{
		Id = Guid.NewGuid();
	}
}
