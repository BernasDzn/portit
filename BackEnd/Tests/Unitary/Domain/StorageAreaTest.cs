namespace Tests.Unitary.Domain;

using System.Security.Policy;
using Api.Domain.Entities;
using Api.Domain.ValueObjects;

public class StorageAreaTest
{

    VesselType vt1;
    Dock d1;
    StorageArea.DockRelation dr1;
    StorageArea validStorageArea;
    public StorageAreaTest()
    {
        vt1 = new VesselType(
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
            });
        d1 = new Dock(
            Guid.NewGuid(),
            new Code { Value = "DCK001" },
            new Designation { Value = "Dock 1" },
            new Designation { Value = "Location 1" },
            new PhysicalCharacteristics
            {
                Length = 500,
                Depth = 20,
                Draft = 20
            },
            new HashSet<VesselType> { vt1 }
        );
        dr1 = new StorageArea.DockRelation(d1, 100, true);
        validStorageArea = new StorageArea(
            Guid.NewGuid(),
            new Code { Value = "SA001" },
            new Designation { Value = "Valid Location" },
            StorageAreaType.Yard,
            1000,
            500,
            new HashSet<StorageArea.DockRelation> {
                dr1
            }
            );
    }

    //---VALID STORAGE AREA TEST---
    [Theory]
    [InlineData("SA001", "Valid Location", StorageAreaType.Yard, 1000, 500)]
    [InlineData("STORAGE1", "Location Alpha", StorageAreaType.Warehouse, 2000, 1500)]
    [InlineData("SA123", "Location 123", StorageAreaType.Yard, 500, 0)]
    public void WhenStorageAreaIsValid_ThenIsCreatedSuccessfully(string nameCode, string location, StorageAreaType areaType, uint capacity, uint currentOccupancy)
    {
        new StorageArea(Guid.NewGuid(),
        new Code { Value = nameCode },
        new Designation { Value = location },
        areaType,
        capacity,
        currentOccupancy,
        new HashSet<StorageArea.DockRelation> { dr1 }
        );
    }

    //---NAME CODE TESTS---
    [Theory]
    [InlineData("SA 0_0_1", "Valid Location", StorageAreaType.Yard, 1000, 500)]
    [InlineData("", "Valid Location", StorageAreaType.Yard, 1000, 500)]
    //---LOCATION TESTS---
    [InlineData("SA001", "", StorageAreaType.Yard, 1000, 500)]
    [InlineData("SA001", "A Location Name That Is Way Too Long To Be Considered Valid Because It Exceeds The Maximum Length Allowed", StorageAreaType.Yard, 1000, 500)]
    //---CAPACITY TESTS---
    [InlineData("SA001", "Valid Location", StorageAreaType.Yard, 10, 20)]
    //---CURRENT OCCUPANCY TESTS---
    [InlineData("SA001", "Valid Location", StorageAreaType.Yard, 1000, 1001)]
    public void WhenPassingInvalidParameters_ThenThrowsException(string nameCode, string location, StorageAreaType areaType, uint capacity, uint currentOccupancy)
    {

        Assert.Throws<ArgumentException>(() =>
            new StorageArea(Guid.NewGuid(), new Code { Value = nameCode }, new Designation { Value = location }, areaType, capacity, currentOccupancy,
                new HashSet<StorageArea.DockRelation> { dr1 }
            )
        );
        
    }

    //---CAN SERVE DOCK TEST---
    [Fact]
    public void WhenStorageAreaCanServeDock_ThenIsValid()
    {
        var storageArea = validStorageArea;
        var result = storageArea.CanServeDock(dr1.ServingDock);
        Assert.True(result);
    }

    //---CANNOT SERVE DOCK TEST---
    [Fact]
    public void WhenStorageAreaCannotServeDock_ThenIsInvalid()
    {
        var storageArea = validStorageArea;
        var newDock = new Dock(
            Guid.NewGuid(),
            new Code { Value = "DCK002" },
            new Designation { Value = "Dock 2" },
            new Designation { Value = "Location 2" },
            new PhysicalCharacteristics
            {
                Length = 400,
                Depth = 45,
                Draft = 40
            },
            new HashSet<VesselType> { vt1 }
        );
        var result = storageArea.CanServeDock(newDock);
        Assert.False(result);
    }
}