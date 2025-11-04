namespace Api.Domain.Entities;

using Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Identity;
using Api.Application.DataTransfer;
using System;

public class SystemUser : IdentityUser<Guid>, IDTOAble<SystemUserDto>
{
    // Use properties with PascalCase so EF Core maps them by convention
    public string? Sub { get; set; }
    public bool Active { get; set; } = false;
    public virtual SystemUserRole Role { get; set; }
    public string? ActivationToken { get; set; }
    public DateTime? ActivationTokenExpiresAt { get; set; }

    protected SystemUser()
    {
        Id = Guid.NewGuid();
        Active = false;
    }

    public SystemUser(string? sub, bool active, string email)
    {
        Sub = sub;
        Active = active;
        Email = email;
    }

    public override string ToString()
    {
        return $"SystemUser {{ Id: {Id}, Active: {Active}, Email: {Email}, Role: {Role} }}";
    }

    public SystemUserDto ToDTO()
    {
        return new SystemUserDto
        {
            Sub = Sub ?? string.Empty,
            IsActive = Active,
            Role = (int) Role,
            Email = Email!
        };
    }
}