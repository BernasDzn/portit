namespace Api.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

using System;

[Owned]
public class CargoType
{
    public Guid Id { get; set; }

    public CargoTypes Type { get; set; }

    public CargoType(CargoTypes type)
    {
        Id = Guid.NewGuid();
        Type = type;
    }

    //EF Core
    protected CargoType() { }

    public override string ToString() => Type.ToString();

    public static CargoTypes FromString(string type)
    {
        return type.ToUpper() switch
        {
            "REGRIGERATED_GOODS" => CargoTypes.REGRIGERATED_GOODS,
            "GENERAL_CONSUMER_PRODUCTS" => CargoTypes.GENERAL_CONSUMER_PRODUCTS,
            "ELECTRONICS" => CargoTypes.ELECTRONICS,
            "HAZMAT" => CargoTypes.HAZMAT,
            "OVERSIZED_INDUSTRIAL_EQUIPMENT" => CargoTypes.OVERSIZED_INDUSTRIAL_EQUIPMENT,
            "OTHER" => CargoTypes.OTHER,
            _ => throw new ArgumentException($"Invalid cargo type: {type}")
        };
    }
}

public enum CargoTypes
{
    REGRIGERATED_GOODS = 0,
    GENERAL_CONSUMER_PRODUCTS = 1 ,
    ELECTRONICS = 2,
    HAZMAT = 3 ,
    OVERSIZED_INDUSTRIAL_EQUIPMENT = 4,
    OTHER = 5
}