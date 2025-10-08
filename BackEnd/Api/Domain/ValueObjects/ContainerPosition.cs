using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Api.Domain.ValueObjects;

using System;

[Owned]
public class ContainerPosition : IDTOAble<ContainerPositionDto>
{
    public Guid Id { get; private set; }
    public string Bay { get; private set; }
    public string Row { get; private set; }
    public string Tier { get; private set; }

    public ContainerPosition(string bay, string row, string tier)
    {
        if (string.IsNullOrWhiteSpace(bay) || string.IsNullOrWhiteSpace(row) || string.IsNullOrWhiteSpace(tier))
            throw new ArgumentException("Bay, Row, and Tier must be non-empty strings.");

        Id = Guid.NewGuid();
        Bay = bay;
        Row = row;
        Tier = tier;
    }

    public override string ToString() => $"Bay: {Bay}, Row: {Row}, Tier: {Tier}";

    public ContainerPositionDto ToDTO() => new ContainerPositionDto
    {
        Bay = Bay,
        Row = Row,
        Tier = Tier
    };
}