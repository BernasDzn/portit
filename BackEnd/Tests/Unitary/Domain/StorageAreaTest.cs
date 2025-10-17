namespace Tests.Unitary.Domain;

using System.Security.Policy;
using Api.Application.Exceptions;
using Api.Domain.Entities;
using Api.Domain.ValueObjects;
using Microsoft.Identity.Client.Extensions.Msal;

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
    [InlineData("SA 0_0_1", "Valid Location", StorageAreaType.Yard, 1000, 500, true, typeof(ArgumentException))]
    [InlineData("", "Valid Location", StorageAreaType.Yard, 1000, 500, true, typeof(ArgumentException))]
    //---LOCATION TESTS---
    [InlineData("SA001", "", StorageAreaType.Yard, 1000, 500, true, typeof(ArgumentException))]
    [InlineData("SA001", "A Location Name That Is Way Too Long To Be Considered Valid Because It Exceeds The Maximum Length Allowed", StorageAreaType.Yard, 1000, 500, true, typeof(ArgumentException))]
    //---CAPACITY TESTS---
    [InlineData("SA001", "Valid Location", StorageAreaType.Yard, 10, 20, true, typeof(StorageFullException))]
    //---CURRENT OCCUPANCY TESTS---
    [InlineData("SA001", "Valid Location", StorageAreaType.Yard, 1000, 1001, true, typeof(StorageFullException))]
    //---DOCK SERVICES TESTS---
    [InlineData("SA001", "Valid Location", StorageAreaType.Warehouse, 1000, 500, false, typeof(ArgumentException))] // Warehouse not serving all docks

    public void WhenPassingInvalidParameters_ThenThrowsException(string nameCode, string location, StorageAreaType areaType, uint capacity, uint currentOccupancy, bool isServingDock, Type exceptionType)
    {
        HashSet<StorageArea.DockRelation>? dockServicesParam = isServingDock ? new HashSet<StorageArea.DockRelation> { dr1 } : new HashSet<StorageArea.DockRelation> { new StorageArea.DockRelation(d1, 100, false) };

        Assert.Throws(exceptionType, () =>
            new StorageArea(Guid.NewGuid(), new Code { Value = nameCode }, new Designation { Value = location }, areaType, capacity, currentOccupancy, dockServicesParam)
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

    
    //---UPDATE STORAGE AREA TEST---
    [Fact]
    public void WhenUpdatingStorageAreaWithValidParameters_ThenIsUpdatedSuccessfully()
    {
        Dock d2 = new Dock(
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

        StorageArea.DockRelation dr2 = new StorageArea.DockRelation(d2, 150, true);

        StorageArea storageAreaToUpdate = validStorageArea;

        storageAreaToUpdate.UpdateNameCode("NewCode");
        storageAreaToUpdate.UpdateLocation("New Valid Location");
        storageAreaToUpdate.UpdateCapacity(800);
        storageAreaToUpdate.UpdateOccupancy(400);
        storageAreaToUpdate.UpdateAreaType(StorageAreaType.Warehouse);
        storageAreaToUpdate.UpdateDockServices(new HashSet<StorageArea.DockRelation> { dr2 });

        Assert.Equal("NewCode", storageAreaToUpdate.NameCode.Value);
        Assert.Equal("New Valid Location", storageAreaToUpdate.Location.Value);
        Assert.Equal((uint)800, storageAreaToUpdate.Capacity);
        Assert.Equal((uint)400, storageAreaToUpdate.CurrentOccupancy);
        Assert.Equal(StorageAreaType.Warehouse, storageAreaToUpdate.AreaType);
        Assert.Contains(dr2, storageAreaToUpdate.DockServices!);
    }


    //---UPDATE STORAGE AREA TEST WITH INVALID PARAMETERS---
    [Theory]
    //---NAME CODE TESTS---
    [InlineData("SA 0_0_1", "New Valid Location", StorageAreaType.Warehouse, 800, 400, true, typeof(ArgumentException))]
    [InlineData("", "New Valid Location", StorageAreaType.Warehouse, 800, 400, true, typeof(ArgumentException))]
    //---LOCATION TESTS---
    [InlineData("NewCode", "", StorageAreaType.Warehouse, 800, 400, true, typeof(ArgumentException))]
    [InlineData("NewCode", "A Location Name That Is Way Too Long To Be Considered Valid Because It Exceeds The Maximum Length Allowed", StorageAreaType.Warehouse, 800, 400, true, typeof(ArgumentException))]
    //---CAPACITY TESTS---
    [InlineData("NewCode", "New Valid Location", StorageAreaType.Warehouse, 400, 500, true, typeof(StorageFullException))]
    //---CURRENT OCCUPANCY TESTS---
    [InlineData("NewCode", "New Valid Location", StorageAreaType.Warehouse, 800, 900, true, typeof(StorageFullException))]
    //---DOCK SERVICES TESTS---
    [InlineData("NewCode", "New Valid Location", StorageAreaType.Warehouse, 800, 400, false, typeof(ArgumentException))] // Warehouse not serving all docks
    public void WhenUpdatingStorageAreaWithInvalidParameters_ThenThrowsException(string nameCode, string location, StorageAreaType areaType, uint capacity, uint currentOccupancy, bool isServingDock, Type exceptionType)
    {
        StorageArea storageAreaToUpdate = validStorageArea;

        // Invalid name code
        Assert.Throws(exceptionType,() =>
        {
            storageAreaToUpdate.UpdateNameCode(nameCode);
            storageAreaToUpdate.UpdateLocation(location);
            storageAreaToUpdate.UpdateCapacity(capacity);
            storageAreaToUpdate.UpdateOccupancy(currentOccupancy);
            storageAreaToUpdate.UpdateAreaType(areaType);
            HashSet<StorageArea.DockRelation>? dockServicesParam = isServingDock ? new HashSet<StorageArea.DockRelation> { dr1 } : new HashSet<StorageArea.DockRelation> { new StorageArea.DockRelation(d1, 100, false) };
            storageAreaToUpdate.UpdateDockServices(dockServicesParam);
        });
    }
}