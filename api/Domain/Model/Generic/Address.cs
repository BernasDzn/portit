namespace Domain.Model.Generic;

public class Address : IDTOAble<AddressDto>
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
    
    public AddressDto ToDTO()
    {
        return new AddressDto
        {
            Street = this.Street,
            City = this.City,
            PostalCode = this.ZipCode,
            Country = this.Country
        };
    }
}