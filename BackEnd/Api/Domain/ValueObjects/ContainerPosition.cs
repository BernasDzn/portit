using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;

namespace Api.Domain.ValueObjects;

public class ContainerPosition
{
    public string Bay { get; private set; }
    public string Row { get; private set; }
    public string Tier { get; private set; }

    public ContainerPosition(string bay, string row, string tier)
    {
        if (string.IsNullOrWhiteSpace(bay) || string.IsNullOrWhiteSpace(row) || string.IsNullOrWhiteSpace(tier))
            throw new ArgumentException("Bay, Row, and Tier must be non-empty strings.");

        Bay = bay;
        Row = row;
        Tier = tier;
    }

    public override string ToString() => $"Bay: {Bay}, Row: {Row}, Tier: {Tier}";
}