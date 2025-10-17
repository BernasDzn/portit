using Api.Domain.Entities;
using Api.Domain.ValueObjects;

namespace Tests.Unitary.Domain;

public class VesselTypeTest
{
    VesselType validVesselType;
    Dock validDock;
    Vessel validVessel;

    public VesselTypeTest()
    {
        validVesselType = new VesselType(Guid.NewGuid(), new Designation { Value = "Valid Name" }, new Designation { Value = "Valid Description" },
                10, 10, 10,
                new PhysicalCharacteristics { Length = 70, Depth = 15, Draft = 15 }
            );

        validDock = new Dock(Guid.NewGuid(), new Code { Value = "DCK001" }, new Designation { Value = "Valid Name" }, new Designation { Value = "Valid Location" },
                new PhysicalCharacteristics { Length = 100, Depth = 20, Draft = 20 },
                new HashSet<VesselType> { validVesselType }
            );
        validVessel = new Vessel(Guid.NewGuid(), new Designation { Value = "Valid Name" }, new ImoNumber { Value = "IMO 2467953" },
                validVesselType,
                new ShippingAgentOrganization(Guid.NewGuid(), new Designation { Value= "name" }, new List<Designation>(), new Address("","","",""), new TaxNumber {Value="PT252252252"}, new HashSet<Representative>()
                {
                    new Representative(Guid.NewGuid(), 1234567, new Designation { Value = "Rep Name" }, new Email { Value= "email@email.email"}, new PhoneNumber { Value = "1234567890" })
                }),
                new PhysicalCharacteristics { Length = 50, Depth = 10, Draft = 10 }
            );
    }

    //---VALID VESSEL TYPE TEST---
    [Theory]
    [InlineData("Valid Name", "Valid Description", 10, 10, 10, 70, 15, 15)]
    [InlineData("VT1", "VD1", 1, 1, 1, 1, 1, 1)]
    [InlineData("Vessel Type Alpha", "Vessel Description Beta", 20, 15, 5, 100.5, 20.25, 18.75)]
    [InlineData("VesselType123", "VesselDescription456", 30, 20, 10, 150.75, 25.5, 22.1)]
    public void WhenVesselTypeIsValid_ThenIsCreatedSuccessfully(string vesselTypeName, string vesselTypeDescription, uint maxNumberOfRows, uint maxNumberOfBays, uint maxNumberOfTiers, double length, double depth, double draft)
    {
        new VesselType(Guid.NewGuid(),
         new Designation { Value = vesselTypeName },
         new Designation { Value = vesselTypeDescription },
         maxNumberOfRows, maxNumberOfBays, maxNumberOfTiers,
         new PhysicalCharacteristics { Length = length, Depth = depth, Draft = draft }
             );
    }

    //--NAME TESTS---
    [Theory]
    [InlineData("", "Valid Description", 10, 10, 10, 70, 15, 15)]
    [InlineData("A Vessel Type Name That Is Way Too Long To Be Considered Valid Because It Exceeds The Maximum Length Allowed", "Valid Description", 10, 10, 10, 70, 15, 15)]
    //---DESCRIPTION TESTS---
    [InlineData("Valid Name", "", 10, 10, 10, 70, 15, 15)]
    [InlineData("Valid Name", "A Vessel Type Description That Is Way Too Long To Be Considered Valid Because It Exceeds The Maximum Length Allowed", 10, 10, 10, 70, 15, 15)]
    //---PHYSICAL CHARACTERISTICS TESTS---
    [InlineData("Valid Name", "Valid Description", 10, 10, 10, 0, 15, 15)]
    [InlineData("Valid Name", "Valid Description", 10, 10, 10, -1, 15, 15)]
    [InlineData("Valid Name", "Valid Description", 10, 10, 10, 70, 0, 15)]
    [InlineData("Valid Name", "Valid Description", 10, 10, 10, 70, -1, 15)]
    [InlineData("Valid Name", "Valid Description", 10, 10, 10, 70, 15, 0)]
    [InlineData("Valid Name", "Valid Description", 10, 10, 10, 70, 15, -1)]
    //---MAX NUMBER OF ROWS, BAYS AND TIERS TESTS---
    [InlineData("Valid Name", "Valid Description", 0, 10, 10, 70, 15, 15)]
    [InlineData("Valid Name", "Valid Description", 10, 0, 10, 70, 15, 15)]
    [InlineData("Valid Name", "Valid Description", 10, 10, 0, 70, 15, 15)]
    public void WhenPassingInvalidParameters_ThenThrowsException(string vesselTypeName, string vesselTypeDescription, uint maxNumberOfRows, uint maxNumberOfBays, uint maxNumberOfTiers, double length, double depth, double draft)
    {
        Assert.Throws<ArgumentException>(() =>
            new VesselType(Guid.NewGuid(), new Designation { Value = vesselTypeName }, new Designation { Value = vesselTypeDescription },
                maxNumberOfRows, maxNumberOfBays, maxNumberOfTiers,
                new PhysicalCharacteristics { Length = length, Depth = depth, Draft = draft }
            )
        );
    }

    //---UPDATE TESTS---
    [Fact]
    public void WhenUpdatingVesselTypeWithValidParameters_ThenIsUpdatedSuccessfully()
    {
        validVesselType.UpdateName("New Valid Name");
        validVesselType.UpdateDescription("New Valid Description");
        validVesselType.UpdateMaxNumberOfRows(15);
        validVesselType.UpdateMaxNumberOfBays(12);
        validVesselType.UpdateMaxNumberOfTiers(8);
        validVesselType.UpdatePhysicalCharacteristics(new PhysicalCharacteristics { Length = 80, Depth = 18, Draft = 18 });

        Assert.Equal("New Valid Name", validVesselType.Name.Value);
        Assert.Equal("New Valid Description", validVesselType.Description.Value);
        Assert.Equal((uint)15, validVesselType.MaxNumberOfRows);
        Assert.Equal((uint)12, validVesselType.MaxNumberOfBays);
        Assert.Equal((uint)8, validVesselType.MaxNumberOfTiers);
        Assert.Equal(80, validVesselType.PhysicalCharacteristics.Length);
        Assert.Equal(18, validVesselType.PhysicalCharacteristics.Depth);
        Assert.Equal(18, validVesselType.PhysicalCharacteristics.Draft);
    }

    //---UPDATE TESTS WITH INVALID PARAMETERS---
    //---NAME TESTS---
    [Theory]
    [InlineData("", "Valid Description", 10, 10, 10, 70, 15, 15)]
    [InlineData("A Vessel Type Name That Is Way Too Long To Be Considered Valid Because It Exceeds The Maximum Length Allowed", "Valid Description", 10, 10, 10, 70, 15, 15)]
    //---DESCRIPTION TESTS---
    [InlineData("Valid Name", "", 10, 10, 10, 70, 15, 15)]
    [InlineData("Valid Name", "A Vessel Type Description That Is Way Too Long To Be Considered Valid Because It Exceeds The Maximum Length Allowed", 10, 10, 10, 70, 15, 15)]
    //---PHYSICAL CHARACTERISTICS TESTS---
    [InlineData("Valid Name", "Valid Description", 10, 10, 10, 0, 15, 15)]
    [InlineData("Valid Name", "Valid Description", 10, 10, 10, -1, 15, 15)]
    [InlineData("Valid Name", "Valid Description", 10, 10, 10, 70, 0, 15)]
    [InlineData("Valid Name", "Valid Description", 10, 10, 10, 70, -1, 15)]
    [InlineData("Valid Name", "Valid Description", 10, 10, 10, 70, 15, 0)]
    [InlineData("Valid Name", "Valid Description", 10, 10, 10, 70, 15, -1)]
    //---MAX NUMBER OF ROWS, BAYS AND TIERS TESTS---
    [InlineData("Valid Name", "Valid Description", 0, 10, 10, 70, 15, 15)]
    [InlineData("Valid Name", "Valid Description", 10, 0, 10, 70, 15, 15)]
    [InlineData("Valid Name", "Valid Description", 10, 10, 0, 70, 15, 15)]
    public void WhenUpdatingVesselTypeWithInvalidParameters_ThenThrowsException(string vesselTypeName, string vesselTypeDescription,
     uint maxNumberOfRows, uint maxNumberOfBays, uint maxNumberOfTiers, double length, double depth, double draft)
    {
        Assert.Throws<ArgumentException>(() =>
            {
                validVesselType.UpdateName(vesselTypeName);
                validVesselType.UpdateDescription(vesselTypeDescription);
                validVesselType.UpdateMaxNumberOfRows(maxNumberOfRows);
                validVesselType.UpdateMaxNumberOfBays(maxNumberOfBays);
                validVesselType.UpdateMaxNumberOfTiers(maxNumberOfTiers);
                validVesselType.UpdatePhysicalCharacteristics(new PhysicalCharacteristics { Length = length, Depth = depth, Draft = draft });
            }
        );
    }
}