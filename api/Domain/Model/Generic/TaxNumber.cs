using Microsoft.EntityFrameworkCore;

namespace Domain.Model.Generic;

[Owned]
public class TaxNumber
{
    private static readonly string TaxNumberPattern = @"^[A-Z0-9]{8,15}$";

    private string _value;
    public string Value
    {
        get => _value;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Tax number cannot be null or empty", nameof(value));
            if (!System.Text.RegularExpressions.Regex.IsMatch(value, TaxNumberPattern))
                throw new ArgumentException("Invalid tax number format", nameof(value));

            _value = value;
        }
    }
}