using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace Api.Domain.ValueObjects;

[Owned]
public class ContainerNumber
{
    private string _value;
    public string Value
    {
        get => _value;
        set
        {
            if (!IsValidContainerNumber(value))
                throw new ArgumentException("Invalid container code format.", nameof(value));
            _value = value;
        }
    }

    private bool IsValidContainerNumber(string code)
    {
        if (string.IsNullOrWhiteSpace(code) || code.Length != 11)
            return false;

        string owner = code.Substring(0, 4).ToUpperInvariant();
        string serial = code.Substring(4, 6);
        string check = code.Substring(10, 1);

        if (!Regex.IsMatch(owner, @"^[A-Z]{4}$")) return false;
        if (!Regex.IsMatch(serial, @"^\d{6}$")) return false;

        string expected = CalculateCheckDigit(owner + serial);
        return expected == check;
    }
    private string CalculateCheckDigit(string first10)
    {
        int[] weights = { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512 };
        int sum = 0;

        for (int i = 0; i < 10; i++)
        {
            char c = first10[i];
            int value;

            if (char.IsDigit(c))
            {
                value = c - '0';
            }
            else
            {
                // A=10, B=12, C=13, D=14, E=15, F=16, G=17, H=18, I=19, J=20, K=21, L=23, M=24, N=25, O=26, P=27,
                // Q=28, R=29, S=30, T=31, U=32, V=34, W=35, X=36, Y=37, Z=38
                int[] isoValues = {
                    10, 12, 13, 14, 15, 16, 17, 18, 19, 20,
                    21, 23, 24, 25, 26, 27, 28, 29, 30, 31,
                    32, 34, 35, 36, 37, 38
                };
                value = isoValues[c - 'A'];
            }

            sum += value * weights[i];
        }

        int remainder = sum % 11;
        return (remainder == 10 ? 0 : remainder).ToString();
    }
}
