namespace Domain.Model.Generic;

public class Address
{
    public Guid Id { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }
    public string Country { get; set; }

    //EF Core
    protected Address() { }

    public Address(string street, string city, string zipCode, string country)
    {
        Id = Guid.NewGuid();
        Street = street;
        City = city;
        ZipCode = zipCode;
        Country = country;
    }
    public override string ToString() => $"{Street}, {ZipCode} {City}, {Country}";
}