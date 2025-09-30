using Microsoft.EntityFrameworkCore;

namespace Domain.Model.Generic;

[Owned]
public class Email
{
    private static readonly string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

    private string _value;
    public string Value
    {
        get => _value;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email cannot be null or empty", nameof(value));
            if (!System.Text.RegularExpressions.Regex.IsMatch(value, EmailPattern))
                throw new ArgumentException("Invalid email format", nameof(value));

            _value = value;
        }
    }
}