
using Api.Application.Exceptions;
using Api.Application.Services;
using Api.Domain.Entities;
using Api.Domain.ValueObjects;

namespace Tests.Unitary.Domain;

public class VesselVisitNotificationTest
{
    Representative representative;
    Representative invalidRepresentative;
    Vessel vessel;
    Crew crew;
    ICollection<CargoTransport> unloadCargoManifest;
    ICollection<CargoTransport> loadCargoManifest;

    public VesselVisitNotificationTest()
    {
        VesselType vt1 = new VesselType(
            Guid.NewGuid(),
            new Designation { Value = "Panamax" },
            new Designation { Value = "Max size for Panama Canal" },
            20,
            10,
            5,
            new PhysicalCharacteristics
            {
                Length = 300,
                Depth = 15,
                Draft = 12
            }
        );
        VesselType vt2 = new VesselType(
            Guid.NewGuid(),
            new Designation { Value = "Post-Panamax" },
            new Designation { Value = "Larger than Panamax" },
            30,
            15,
            7,
            new PhysicalCharacteristics
            {
                Length = 400,
                Depth = 18,
                Draft = 14
            }
        );

        Dock dock1 = new Dock(
            Guid.NewGuid(),
            new Code { Value = "DCK001" },
            new Designation { Value = "Main Dock" },
            new Designation { Value = "Harbor Area 1" },
            new PhysicalCharacteristics { Length = 500, Depth = 20, Draft = 20 },
            new HashSet<VesselType> { vt1, vt2 }
        );

        Dock dock2 = new Dock(
            Guid.NewGuid(),
            new Code { Value = "DCK002" },
            new Designation { Value = "Secondary Dock" },
            new Designation { Value = "Harbor Area 2" },
            new PhysicalCharacteristics { Length = 600, Depth = 25, Draft = 22 },
            new HashSet<VesselType> { vt1, vt2 }
        );


        HashSet<StorageArea.DockRelation> ds1 = new()
        {
            new StorageArea.DockRelation(dock1, null, false),
            new StorageArea.DockRelation(dock2, 100, false)
        };

        HashSet<StorageArea.DockRelation> ds2 = new()
        {
            new StorageArea.DockRelation(dock1, 200, false),
            new StorageArea.DockRelation(dock2, null, false)
        };

        StorageArea sa1 = new StorageArea(
            Guid.NewGuid(),
            new Code { Value = "YARD1" },
            new Designation { Value = "North Yard" },
            StorageAreaType.Yard, 1000, 200, ds1);
        StorageArea sa2 = new StorageArea(
            Guid.NewGuid(),
            new Code { Value = "YARD2" },
            new Designation { Value = "South Yard" },
            StorageAreaType.Yard, 1500, 300, ds2);

        representative = new Representative(
                            Guid.NewGuid(),
                            123456789,
                            new Designation { Value = "rep" },
                            new Email { Value = "email@email.com" },
                            new PhoneNumber { Value = "4512345678" }
                        );

        invalidRepresentative = new Representative(
                            Guid.NewGuid(),
                            987654321,
                            new Designation { Value = "invalid rep" },
                            new Email { Value = "invalid@email.com" },
                            new PhoneNumber { Value = "9876543210" }
                        );

        vessel = new Vessel(
            Guid.NewGuid(),
            new Designation { Value = "Test Vessel" },
            new ImoNumber { Value = "IMO1234567" },
            new VesselType(
                Guid.NewGuid(),
                new Designation { Value = "Panamax" },
                new Designation { Value = "Max size for Panama Canal" },
                20,
                10,
                5,
                new PhysicalCharacteristics
                {
                    Length = 300,
                    Depth = 15,
                    Draft = 12
                }
            ),
            new ShippingAgentOrganization(
                Guid.NewGuid(),
                new Designation { Value = "Maersk" },
                new List<Designation> { new Designation { Value = "A major shipping company" } },
                new Address("123 Ocean Drive", "Copenhagen", "Denmark", "2100"),
                new TaxNumber { Value = "PT252252252" },
                new HashSet<Representative>()
                {
                                    representative
                }
            ),
            new PhysicalCharacteristics { Length = 200, Depth = 10, Draft = 8 }
        );

        unloadCargoManifest = new List<CargoTransport>
        {
            new CargoTransport(
                new ContainerPosition { Row = "10", Bay = "5", Tier = "10"},
                sa1,
                new Container(
                    Guid.NewGuid(),
                    new ContainerNumber { Value = "CMAU2468103" },
                    CargoType.GENERAL_CONSUMER_PRODUCTS,
                    new Designation { Value = "chilly yummy food" }
                )
            ),
            new CargoTransport(
                new ContainerPosition { Row = "12", Bay = "6", Tier = "8" },
                sa2,
                new Container(
                    Guid.NewGuid(),
                    new ContainerNumber { Value = "ABCD1234560" },
                    CargoType.ELECTRONICS,
                    new Designation { Value = "various electronic items" }
                )
            )
        };

        loadCargoManifest = new List<CargoTransport>
        {
            new CargoTransport(
                new ContainerPosition { Row = "14", Bay = "7", Tier = "9"},
                sa1,
                new Container(
                    Guid.NewGuid(),
                    new ContainerNumber { Value = "MSKU1234565" },
                    CargoType.ELECTRONICS,
                    new Designation { Value = "assorted electronics" }
                )
            ),
            new CargoTransport(
                new ContainerPosition { Row = "16", Bay = "8", Tier = "6" },
                sa2,
                new Container(
                    Guid.NewGuid(),
                    new ContainerNumber { Value = "TGHU7654320" },
                    CargoType.OTHER,
                    new Designation { Value = "industrial machinery parts" }
                )
            )
        };

        HashSet<SafetyOfficer> safetyOfficers = new HashSet<SafetyOfficer>
        {
            new SafetyOfficer{CitizenID = "CITIZEN001", Name = "John Doe", Nationality = "US" },
            new SafetyOfficer{CitizenID = "CITIZEN002", Name = "Jane Smith", Nationality = "GB" }
        };

        crew = new Crew(new Designation { Value = "Ana Costa" }, 3, safetyOfficers);

    }

    [Theory]
    [InlineData("2024-07-01T10:00:00Z", "2024-07-01T12:00:00Z", false, null)]
    [InlineData("2024-08-15T14:30:00Z", "2024-08-15T16:30:00Z", true, "Requires special handling")]
    [InlineData("2024-09-20T08:00:00Z", "2024-09-20T10:00:00Z", false, "Fragile cargo")]
    [InlineData("2024-10-05T09:15:00Z", "2024-10-05T11:45:00Z", true, null)]
    public void WhenPassingCorrectData_ThenVesselVisitNotificationIsCreated(string arrivalStr, string departureStr, bool isCargoHazardous, string specialRequirements)
    {
        DateTime arrival = DateTime.Parse(arrivalStr);
        DateTime departure = DateTime.Parse(departureStr);

        new VesselVisitNotification(
            new VesselVisitNotificationId(
                new Designation { Value = "PORTO" },
                1,
                (uint)DateTime.UtcNow.Year
            ),
            arrival,
            departure,
            isCargoHazardous,
            vessel,
            representative,
            specialRequirements,
            crew,
            loadCargoManifest,
            unloadCargoManifest
        );
    }

    [Theory]
    //INVALID ID
    [InlineData("", 1, "2024-07-01T12:00:00Z", "2024-07-01T10:00:00Z", false, null, true, true, typeof(ArgumentException))]
    [InlineData("P ort o", 1, "2024-07-01T12:00:00Z", "2024-07-01T10:00:00Z", false, null, true, true, typeof(ArgumentException))]
    [InlineData("P", 1, "2024-07-01T12:00:00Z", "2024-07-01T10:00:00Z", false, null, true, true, typeof(ArgumentException))]
    [InlineData("PORTOOOOOOOOO", 1, "2024-07-01T12:00:00Z", "2024-07-01T10:00:00Z", false, null, true, true, typeof(ArgumentException))]
    //INVALID DATES
    [InlineData("PORTO", 1, "", "2024-07-01T10:00:00Z", false, null, true, true, typeof(FormatException))]
    [InlineData("PORTO", 1, "2024-08-15T14:30:00Z", "", true, "Requires special handling", true, true, typeof(FormatException))]
    //INVALID REPRESENTATIVE
    [InlineData("PORTO", 1, "2024-09-20T08:00:00Z", "2024-09-20T10:00:00Z", true, "Fragile cargo", false, true, typeof(InvalidRepresentativeException))]
    //INVALID VESSEL
    [InlineData("PORTO", 1, "2024-09-20T08:00:00Z", "2024-09-20T10:00:00Z", true, "Fragile cargo", false, false, typeof(ArgumentNullException))]

    public void WhenPassingInvalidData_ThenThrowsException(
        string id_portCode, uint id_number,
        string arrivalStr, string departureStr, bool isCargoHazardous,
        string specialRequirements, bool isRepresentativeValid, bool useVessel, Type exceptionType)
    {
        Assert.Throws(exceptionType, () =>
        {
            DateTime arrival = DateTime.Parse(arrivalStr);
            DateTime departure = DateTime.Parse(departureStr);

            new VesselVisitNotification(
                new VesselVisitNotificationId(
                    new Designation { Value = id_portCode },
                    id_number,
                    (uint)DateTime.UtcNow.Year
                ),
                arrival,
                departure,
                isCargoHazardous,
                useVessel ? vessel : null,
                isRepresentativeValid ? representative : invalidRepresentative,
                specialRequirements,
                crew,
                loadCargoManifest,
                unloadCargoManifest
            );
        });
    }

    [Fact]
    public void WhenSubmittingValidVVN_ThenSubmits()
    {
        var vvn = new VesselVisitNotification(
            new VesselVisitNotificationId(
                new Designation { Value = "PORTO" },
                1,
                (uint)DateTime.UtcNow.Year
            ),
            DateTime.Parse("2024-07-01T12:00:00Z"),
            DateTime.Parse("2024-07-01T10:00:00Z"),
            false,
            vessel,
            representative,
            null,
            crew,
            loadCargoManifest,
            unloadCargoManifest
        );

        vvn.Submit();
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void WhenSubmittingInvalidVVN_ThenThrowsException(bool isCargoHazardous, bool tryDoubleSubmit)
    {

        var vvn = new VesselVisitNotification(
            new VesselVisitNotificationId(
                new Designation { Value = "PORTO" },
                1,
                (uint)DateTime.UtcNow.Year
            ),
            DateTime.Parse("2024-07-01T12:00:00Z"),
            DateTime.Parse("2024-07-01T10:00:00Z"),
            isCargoHazardous,
            vessel,
            representative,
            null,
            isCargoHazardous ? null : crew,
            loadCargoManifest,
            unloadCargoManifest
        );

        if (tryDoubleSubmit)
            vvn.Submit();

        Assert.Throws<InvalidOperationException>(() => vvn.Submit());

    }

    [Fact]
    public void WhenUpdatingValidData_ThenUpdates()
    {
        var vvn = new VesselVisitNotification(
            new VesselVisitNotificationId(
                new Designation { Value = "PORTO" },
                1,
                (uint)DateTime.UtcNow.Year
            ),
            DateTime.Parse("2024-07-01T12:00:00Z"),
            DateTime.Parse("2024-07-01T10:00:00Z"),
            true,
            vessel,
            representative,
            null,
            crew,
            loadCargoManifest,
            unloadCargoManifest
        );

        vvn.Update(
            DateTime.Parse("2024-07-05T12:00:00Z"),
            DateTime.Parse("2024-07-10T10:00:00Z"),
            false
        );
    }

    [Fact]
    public void WhenUpdatingApprovalPendingVVN_ThenThrowsException()
    {
        var vvn = new VesselVisitNotification(
            new VesselVisitNotificationId(
                new Designation { Value = "PORTO" },
                1,
                (uint)DateTime.UtcNow.Year
            ),
            DateTime.Parse("2024-07-01T12:00:00Z"),
            DateTime.Parse("2024-07-01T10:00:00Z"),
            true,
            vessel,
            representative,
            null,
            crew,
            loadCargoManifest,
            unloadCargoManifest
        );

        vvn.Submit();

        Assert.Throws<InvalidOperationException>(() => vvn.Update(
            DateTime.Parse("2024-07-05T12:00:00Z"),
            DateTime.Parse("2024-07-10T10:00:00Z"),
            false
        ));
    }

}