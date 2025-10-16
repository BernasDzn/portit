namespace Tests.Unitary.Domain;

using Api.Domain.Entities;
using Api.Domain.ValueObjects;

public class DockTest
{

    VesselType vt1 = new VesselType(Guid.NewGuid(), new Designation { Value = "Panamax" }, new Designation { Value = "Max size for Panama Canal" }, 20, 10, 5, new PhysicalCharacteristics { Length = 300, Depth = 15, Draft = 12 });
    VesselType vt2 = new VesselType(Guid.NewGuid(), new Designation { Value = "Post-Panamax" }, new Designation { Value = "Larger than Panamax" }, 30, 15, 7, new PhysicalCharacteristics { Length = 400, Depth = 18, Draft = 14 });
    HashSet<VesselType> vesselTypes = new HashSet<VesselType>();
    Dock validDock;
    public DockTest()
    {
        vesselTypes.Add(vt1);
        vesselTypes.Add(vt2);

        validDock = new Dock(Guid.NewGuid(), new Code { Value = "DCK001" }, new Designation { Value = "Valid Name" }, new Designation { Value = "Valid Location" },
                new PhysicalCharacteristics { Length = 500, Depth = 20, Draft = 20 },
                vesselTypes
            );
    }

    //---VALID DOCK TEST---
    [Theory]
    [InlineData("DCK001","Valid Name", "Valid Location", 500, 20, 20)]
    [InlineData("DOCK1","D1", "L1", 450, 24, 19)]
    [InlineData("DK1","Dock Alpha", "Location Beta", 600.25, 30.12, 25.05)]
    [InlineData("D001","Dock 123", "Location 456", 700.21, 35, 30.45)]
    public void WhenDockIsValid_ThenIsCreatedSuccessfully(string code, string name, string location, double length, double depth, double draft)
    {
        new Dock(Guid.NewGuid(),
        new Code { Value = code },
        new Designation { Value = name },
        new Designation { Value = location },
        new PhysicalCharacteristics { Length = length, Depth = depth, Draft = draft },
            vesselTypes
        );
    }

    //---NAME TESTS---
    [Theory]
    [InlineData("DCK 0_0_1", "Valid Name", "Valid Location", 500, 20, 20, true)]
    [InlineData("", "Valid Name", "Valid Location", 500, 20, 20, true)]
    [InlineData("DCK001", "", "Valid Location", 500, 20, 20, true)]
    [InlineData("DCK001", "A Dock Name That Is Way Too Long To Be Considered Valid Because It Exceeds The Maximum Length Allowed", "Valid Location", 500, 20, 20, true)]
    //---LOCATION TESTS---
    [InlineData("DCK001", "Valid Name", "", 500, 20, 20, true)]
    [InlineData("DCK001", "Valid Name", "A Location Name That Is Way Too Long To Be Considered Valid Because It Exceeds The Maximum Length Allowed", 500, 20, 20, true)]
    //---PHYSICAL CHARACTERISTICS TESTS---
    [InlineData("DCK001", "Valid Name", "Valid Location", 0, 20, 20, true)]
    [InlineData("DCK001", "Valid Name", "Valid Location", -1, 20, 20, true)]
    [InlineData("DCK001", "Valid Name", "Valid Location", 500, 0, 20, true)]
    [InlineData("DCK001", "Valid Name", "Valid Location", 500, -1, 20, true)]
    [InlineData("DCK001", "Valid Name", "Valid Location", 500, 20, 0, true)]
    [InlineData("DCK001", "Valid Name", "Valid Location", 500, 20, -1, true)]
    //---PHYSICAL CHARACTERISTICS VS VESSEL TYPES TESTS---
    [InlineData("DCK001", "Valid Name", "Valid Location", 100, 20, 20, true)]
    [InlineData("DCK001", "Valid Name", "Valid Location", 500, 10, 20, true)]
    [InlineData("DCK001", "Valid Name", "Valid Location", 500, 20, 10, true)]
    //---VESSEL TYPES TESTS---
    [InlineData("DCK001", "Valid Name", "Valid Location", 500, 20, 20, false)]
    public void WhenPassingInvalidParameters_ThenThrowsException(string code, string dockName, string dockLocation, double length, double depth, double draft, bool useVesselTypes)
    {
        HashSet<VesselType>? vesselTypesParam = useVesselTypes ? new HashSet<VesselType> { vt1, vt2 } : null;

        Assert.Throws<ArgumentException>(() =>
            new Dock(Guid.NewGuid(), new Code { Value = code }, new Designation { Value = dockName }, new Designation { Value = dockLocation },
                new PhysicalCharacteristics { Length = length, Depth = depth, Draft = draft },
                vesselTypesParam!
            )
        );

        if (!useVesselTypes)
            Assert.Throws<ArgumentException>(() => new Dock(Guid.NewGuid(), new Code { Value = code }, new Designation { Value = "Valid Name" }, new Designation { Value = "Valid Location" },
                new PhysicalCharacteristics { Length = 500, Depth = 20, Draft = 20 },
                new HashSet<VesselType> { }
            ));
    }


    //---UPDATE DOCK TEST---
    [Fact]
    public void WhenUpdatingDockWithValidParameters_ThenIsUpdatedSuccessfully()
    {
        Dock dockToUpdate = validDock;

        dockToUpdate.UpdateName("New Valid Name");
        dockToUpdate.UpdateLocation("New Valid Location");
        dockToUpdate.UpdatePhysicalCharacteristics(new PhysicalCharacteristics { Length = 600, Depth = 25, Draft = 22 });

        VesselType vt3 = new VesselType(Guid.NewGuid(), new Designation { Value = "New Type" }, new Designation { Value = "New Description" }, 25, 12, 6, new PhysicalCharacteristics { Length = 350, Depth = 16, Draft = 13 });
        HashSet<VesselType> newVesselTypes = new HashSet<VesselType> { vt1, vt3 };
        dockToUpdate.UpdateVesselTypes(newVesselTypes);

        Assert.Equal("New Valid Name", dockToUpdate.Name.Value);
        Assert.Equal("New Valid Location", dockToUpdate.Location.Value);
        Assert.Equal(600, dockToUpdate.PhysicalCharacteristics.Length);
        Assert.Equal(25, dockToUpdate.PhysicalCharacteristics.Depth);
        Assert.Equal(22, dockToUpdate.PhysicalCharacteristics.Draft);
        Assert.Contains(vt1, dockToUpdate.SupportedVesselTypes);
        Assert.Contains(vt3, dockToUpdate.SupportedVesselTypes);
        Assert.DoesNotContain(vt2, dockToUpdate.SupportedVesselTypes);
    }

    //---UPDATE DOCK TEST WITH INVALID PARAMETERS---
    [Theory]
    [InlineData("", "New Valid Location", 600, 25, 22)]
    [InlineData("A Dock Name That Is Way Too Long To Be Considered Valid Because It Exceeds The Maximum Length Allowed", "New Valid Location", 600, 25, 22)]
    [InlineData("New Valid Name", "", 600, 25, 22)]
    [InlineData("New Valid Name", "A Location Name That Is Way Too Long To Be Considered Valid Because It Exceeds The Maximum Length Allowed", 600, 25, 22)]
    [InlineData("New Valid Name", "New Valid Location", 0, 25, 22)]
    [InlineData("New Valid Name", "New Valid Location", -1, 25, 22)]
    [InlineData("New Valid Name", "New Valid Location", 600, 0, 22)]
    [InlineData("New Valid Name", "New Valid Location", 600, -1, 22)]
    [InlineData("New Valid Name", "New Valid Location", 600, 25, 0)]
    [InlineData("New Valid Name", "New Valid Location", 600, 25, -1)]
    public void WhenUpdatingDockWithInvalidParameters_ThenThrowsException(string dockName, string dockLocation, double length, double depth, double draft)
    {
        Dock dockToUpdate = validDock;

        // Invalid name
        Assert.Throws<ArgumentException>(() =>
        {
            dockToUpdate.UpdateName(dockName);
            dockToUpdate.UpdateLocation(dockLocation);
            dockToUpdate.UpdatePhysicalCharacteristics(new PhysicalCharacteristics { Length = length, Depth = depth, Draft = draft });
        });
    }

    [Fact]
    public void WhenUpdatingDockWithInvalidVesselTypes_ThenThrowsException()
    {
        Dock dockToUpdate = validDock;

        Assert.Throws<ArgumentException>(() => dockToUpdate.UpdateVesselTypes(null!));
        Assert.Throws<ArgumentException>(() => dockToUpdate.UpdateVesselTypes(new HashSet<VesselType> { }));
    }
}