using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace Api.Domain.ValueObjects;

using System;

[Owned]
public class ContainerNumber
{
    public Guid Id { get; set; }

    public string OwnerCode { get; set; }
    public string Number { get; set; }
    public string CheckDigit { get; set; }

    public ContainerNumber(string number)
    {
        if (!IsValidContainerNumber(number))
            throw new ArgumentException("Invalid container number format.");

        Id = Guid.NewGuid();
        Number = number.ToUpper();
        CheckDigit = CalculateCheckDigit(Number);
    }

    //EF Core
    protected ContainerNumber() { }

    private bool IsValidContainerNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number) || number.Length != 11)
            return false;

        string ownerCode = number.Substring(0, 4);
        string serialNumber = number.Substring(4, 6);
        string checkDigit = number.Substring(10, 1);

        if (!System.Text.RegularExpressions.Regex.IsMatch(ownerCode, @"^[A-Z]{4}$"))
            return false;

        if (!System.Text.RegularExpressions.Regex.IsMatch(serialNumber, @"^\d{6}$"))
            return false;

        return checkDigit == CalculateCheckDigit(number);
    }

    private string CalculateCheckDigit(string number)
    {
        int[] weights = { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512 };
        int sum = 0;

        for (int i = 0; i < 10; i++)
        {
            char c = number[i];
            int value;

            if (char.IsDigit(c))
                value = c - '0';
            else
                value = c - 'A' + 10;

            sum += value * weights[i];
        }

        int remainder = sum % 11;
        return (remainder == 10) ? "0" : remainder.ToString();
    }

    public override string ToString() => OwnerCode + " " + Number + " " + CheckDigit;
}