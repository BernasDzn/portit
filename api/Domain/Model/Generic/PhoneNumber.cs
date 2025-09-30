using Microsoft.EntityFrameworkCore;

namespace Domain.Model.Generic;

[Owned]
public class PhoneNumber
{
    private static readonly string PhonePattern = @"^\+?[1-9]\d{1,14}$";

    private string _value;
    public string Value
    {
        get => _value;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Phone number cannot be null or empty", nameof(value));
            if (!System.Text.RegularExpressions.Regex.IsMatch(value, PhonePattern))
                throw new ArgumentException("Invalid phone number format", nameof(value));

            _value = value;
        }
    }
}