using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace Domain.Model.Generic;

[Owned]
public class PhoneNumber
{
    private static readonly string PhoneNumberPattern = @"^\+?[1-9]\d{1,14}$";

    private string _value;
    public string Value
    {
        get => _value;
        set
        {
            if (string.IsNullOrWhiteSpace(value) || !Regex.IsMatch(value, PhoneNumberPattern))
                throw new ArgumentException("Invalid phone number format", nameof(value));

            _value = value;
        }
    }
    
    public override string ToString() => Value;
}