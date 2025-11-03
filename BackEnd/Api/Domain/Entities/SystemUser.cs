namespace Api.Domain.Entities;

using Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Identity;
using Api.Application.DataTransfer;
using System;

public class SystemUser : IdentityUser<Guid>, IDTOAble<SystemUserDto>
{
    // Use properties with PascalCase so EF Core maps them by convention
    public required string Sub { get; set; }
    public required bool Active { get; set; }
    public virtual SystemUserRole Role { get; set; }
    // Activation token and expiry used when a user is authorized for the first time
    public string? ActivationToken { get; set; }
    public DateTime? ActivationTokenExpiresAt { get; set; }

    protected SystemUser()
    {
        Id = Guid.NewGuid();
    }

    public SystemUser(string sub, bool active, string email)
    {
        Sub = sub;
        Active = active;
        Email = email;
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
            IsActive = Active,
            Role = (int)Role,
            Email = Email
        };
    }
}