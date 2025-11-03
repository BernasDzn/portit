namespace Api.Domain.Entities;

using Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Identity;
using Api.Application.DataTransfer;

public class SystemUser : IdentityUser<Guid>, IDTOAble<SystemUserDto>
{
    // Use properties with PascalCase so EF Core maps them by convention
    public required string Sub { get; set; }
    public required bool Active { get; set; }

    protected SystemUser()
    {
        Id = Guid.NewGuid();
    }

    public SystemUser(string sub, bool active)
    {
        Sub = sub;
        Active = active;
    }

    public override string ToString()
    {
        return $"SystemUser {{ Id: {Id}, Sub: {Sub}, Active: {Active} }}";
    }

    public SystemUserDto ToDTO()
    {
        return new SystemUserDto
        {
            Sub = Sub,
            IsActive = Active
        };
    }
}