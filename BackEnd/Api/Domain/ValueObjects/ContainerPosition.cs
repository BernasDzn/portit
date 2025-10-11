using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Api.Domain.ValueObjects;

[Owned]
public class ContainerPosition
{
    private string _bay;
    private string _row;
    private string _tier;

    public string Bay
    {
        get => _bay;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Bay must be a two-digit string between '01' and '99'.");
            _bay = value;
        }
    }

    public string Row
    {
        get => _row;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Row must be a two-character string between 'A' and 'Z'.");
            _row = value;
        }
    }

    public string Tier
    {
        get => _tier;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Tier must be a two-digit string between '01' and '99'.");
            _tier = value;
        }
    }
}