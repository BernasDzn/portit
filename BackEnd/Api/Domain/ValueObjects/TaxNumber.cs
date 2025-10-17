using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace Api.Domain.ValueObjects;

[Owned]
public class TaxNumber
{
    private static readonly Dictionary<string, string> Patterns = new()
    {
        { "AT", @"^ATU\d{8}$" },                  // Austria
        { "BE", @"^BE0?\d{9}$" },                // Belgium
        { "BG", @"^BG\d{9,10}$" },               // Bulgaria
        { "CY", @"^CY\d{8}[A-Z]$" },             // Cyprus
        { "CZ", @"^CZ\d{8,10}$" },               // Czech Republic
        { "DE", @"^DE\d{9}$" },                  // Germany
        { "DK", @"^DK\d{8}$" },                  // Denmark
        { "EE", @"^EE\d{9}$" },                  // Estonia
        { "EL", @"^EL\d{9}$" },                  // Greece
        { "ES", @"^ES[A-Z0-9]\d{7}[A-Z0-9]$" },  // Spain
        { "FI", @"^FI\d{8}$" },                  // Finland
        { "FR", @"^FR[A-Z0-9]{2}\d{9}$" },       // France
        { "HR", @"^HR\d{11}$" },                 // Croatia
        { "HU", @"^HU\d{8}$" },                  // Hungary
        { "IE", @"^IE\d{7}[A-Z]{1,2}$" },        // Ireland
        { "IT", @"^IT\d{11}$" },                 // Italy
        { "LT", @"^LT(\d{9}|\d{12})$" },         // Lithuania
        { "LU", @"^LU\d{8}$" },                  // Luxembourg
        { "LV", @"^LV\d{11}$" },                 // Latvia
        { "MT", @"^MT\d{8}$" },                  // Malta
        { "NL", @"^NL\d{9}B\d{2}$" },            // Netherlands
        { "PL", @"^PL\d{10}$" },                 // Poland
        { "PT", @"^PT\d{9}$" },                  // Portugal
        { "RO", @"^RO\d{2,10}$" },               // Romania
        { "SE", @"^SE\d{10}01$" },               // Sweden
        { "SI", @"^SI\d{8}$" },                  // Slovenia
        { "SK", @"^SK\d{10}$" },                 // Slovakia
        { "GB", @"^GB(\d{9}|\d{12}|GD\d{3}|HA\d{3})$" }, // United Kingdom
        { "CH", @"^CHE\d{9}(MWST|TVA|IVA)?$" }   // Switzerland
    };

    private string _value = null!;
    public string Value
    {
        get => _value;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Tax number cannot be null or empty.", nameof(value));

            value = value.Trim().ToUpperInvariant();

            string countryCode = value.Length >= 2 ? value.Substring(0, 2) : string.Empty;
            if (!Patterns.TryGetValue(countryCode, out string? pattern))
                throw new ArgumentException($"Unsupported or invalid country prefix '{countryCode}'.", nameof(value));

            if (!Regex.IsMatch(value, pattern))
                throw new ArgumentException($"Invalid tax number format for country '{countryCode}'.", nameof(value));

            _value = value;
        }
    }

    public override string ToString() => Value;
}


