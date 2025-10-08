namespace Api.Domain.ValueObjects;

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