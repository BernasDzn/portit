namespace Api.Domain.Entities;

using Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Identity;
using Api.Application.DataTransfer;
using System;

public class SystemUser : IdentityUser<Guid>, IDTOAble<SystemUserDto>
{
    // Google OAuth sub identifier
    public string? Sub { get; set; }
    // Whether the user account is active
    public bool Active { get; set; } = false;
    // Token used for email-based activation
    public string? ActivationToken { get; set; }
    // Expiration date for the activation token
    public DateTime? ActivationTokenExpiresAt { get; set; }

    public SystemUser() : base()
    {
        Id = Guid.NewGuid();
        Active = false;
    }

    public SystemUser(string email) : this()
    {
        Email = email;
        UserName = email; // Identity requires UserName to be set
    }

    public override string ToString()
    {
        return $"SystemUser {{ Id: {Id}, Active: {Active}, Email: {Email}, UserName: {UserName} }}";
    }

    public SystemUserDto ToDTO()
    {
        return new SystemUserDto
        {
            Sub = Sub ?? string.Empty,
            IsActive = Active,
            Role = 0, // Will be set by service layer from roles
            Email = Email!
        };
    }
}