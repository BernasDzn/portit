namespace Tests.Unitary.Domain;

using Api.Domain.Entities;
using Api.Domain.ValueObjects;

public class VesselTest
{
    [Fact]
    public void WhenPassingCorrectData_ThenVesselIsCreated()
    {
        var vesselType = new VesselType(
            Guid.NewGuid(),
            new Designation { Value = "Container Ship" },
            new Designation { Value = "A large container ship" },
            20,
            10,
            8,
            new PhysicalCharacteristics
            {
                Length = 300.0,
                Depth = 50.0,
                Draft = 15.0
            }
        );

        var vessel = new Vessel(
            Guid.NewGuid(),
            new Designation { Value = "Ever Given" },
            new ImoNumber { Value = "IMO 9811000" },
            vesselType,
            new ShippingAgentOrganization(
                Guid.NewGuid(), new Designation { Value = "Maersk" }, new List<Designation> { new Designation { Value = "A major shipping company" } },
                new Address("123 Ocean Drive", "Copenhagen", "Denmark", "2100"), new TaxNumber { Value = "123456789" }, new HashSet<Representative>()
                {
                    new Representative(Guid.NewGuid(), 123456789, new Designation {Value = "rep"}, new Email{ Value = "email@email.com" }, new PhoneNumber{ Value = "4512345678" } )
                }
            ),
            new PhysicalCharacteristics
            {
                Length = 200.0,
                Depth = 40.0,
                Draft = 10.0
            }
        );
    }

    [Theory]
    [InlineData("", "9811000")]
    [InlineData("Ever Given", "")]
    [InlineData("Ever Given", "123")]
    [InlineData("Ever Given", "IMO1234568")]
    [InlineData("Ever#Given", "12345678X")]
    public void WhenPassingInvalidData_ThenThrowsException(string name, string imoNumber)
    {
        var vesselType = new VesselType(
            Guid.NewGuid(),
            new Designation { Value = "Container Ship" },
            new Designation { Value = "A large container ship" },
            20,
            10,
            8,
            new PhysicalCharacteristics
            {
                Length = 300.0,
                Depth = 50.0,
                Draft = 15.0
            }
        );
        Assert.Throws<ArgumentException>(() =>
            new Vessel(
                Guid.NewGuid(),
                new Designation { Value = name },
                new ImoNumber { Value = imoNumber },
                vesselType,
                null,
                new PhysicalCharacteristics
                {
                    Length = 200.0,
                    Depth = 30.0,
                    Draft = 15.0
                }
            )
        );
    }
}