using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace Domain.Model.Generic;

[Owned]
public class ImoNumber
{
    private string _value;
    public string Value
    {
        get => _value;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("IMO number cannot be null or empty", nameof(value));
            if (!validateIMONumber(value))
                throw new ArgumentException("Invalid IMO number format", nameof(value));

            _value = value;
        }
    }

    private bool validateIMONumber(string imoNumber)
    {
        if (imoNumber == null) return false;

        // Must start with "IMO " and then 7 digits
        var match = Regex.Match(imoNumber, @"^IMO\s?(\d{7})$");
        if (!match.Success)
            return false;

        string digits = match.Groups[1].Value;

        // Extract check digit (last digit)
        int checkDigit = digits[6] - '0';

        // Compute check digit from first 6 digits
        int sum = 0;
        for (int i = 0; i < 6; i++)
        {
            int digit = digits[i] - '0';
            sum += digit * (7 - i);
        }

        int calculated = sum % 10;

        return calculated == checkDigit;
    }

    public override string ToString() => Value;
}