namespace Tests.Unitary.Domain;

using Api.Domain.Entities;
using Api.Domain.ValueObjects;

public class PhysicalResourcesTest
{
    [Theory]
    [InlineData("CR001", "Crane 1", 30, 100, 50)]
    [InlineData("CR002", "Crane 2", 45, 150, 75)]
    [InlineData("CR003", "Crane 3", 60, 200, 100)]
    public void STSCraneWhenPassingCorrectData_ThenPhysicalResourceIsCreated(string code, string designation, int time, uint capacity, uint cph)
    {
        new STSCrane(
            Guid.NewGuid(),
            new Code { Value = code },
            new Designation { Value = designation },
            ResourceStatus.Available,
            TimeSpan.FromMinutes(time),
            new HashSet<Qualification> { },
            OperationalWindow.FullWeek(),
            capacity,
            new Dock(
                Guid.NewGuid(),
                new Designation { Value = "Dock A" },
                new Designation { Value = "Main Dock" },
                new PhysicalCharacteristics { Length = 300, Depth = 50, Draft = 15 },
                new HashSet<VesselType>
                {
                    new VesselType(
                        Guid.NewGuid(),
                        new Designation { Value = "Container Ship" },
                        new Designation { Value = "A ship designed to carry containerized cargo" },
                        10, 10, 10,
                        new PhysicalCharacteristics { Length = 200, Depth = 30, Draft = 10 }
                    )
                }
            ),
            cph
        );
    }

    [Theory]
    [InlineData("", "Crane 1", 30, 100, 50)]
    [InlineData("CR002", "", 45, 150, 75)]
    public void STSCraneWhenPassingInvalidData_ThenThrowsException(string code, string designation, int time, uint capacity, uint cph)
    {
        Assert.Throws<ArgumentException>(() =>
            new STSCrane(
                Guid.NewGuid(),
                new Code { Value = code },
                new Designation { Value = designation },
                ResourceStatus.Available,
                TimeSpan.FromMinutes(time),
                new HashSet<Qualification> { },
                OperationalWindow.FullWeek(),
                capacity,
                new Dock(
                    Guid.NewGuid(),
                    new Designation { Value = "Dock A" },
                    new Designation { Value = "Main Dock" },
                    new PhysicalCharacteristics { Length = 300, Depth = 50, Draft = 15 },
                    new HashSet<VesselType>
                    {
                        new VesselType(
                            Guid.NewGuid(),
                            new Designation { Value = "Container Ship" },
                            new Designation { Value = "A ship designed to carry containerized cargo" },
                            10, 10, 10,
                            new PhysicalCharacteristics { Length = 200, Depth = 30, Draft = 10 }
                        )
                    }
                ),
                cph
            )
        );
    }

    [Theory]
    [InlineData("YC001", "Yard Crane 1", 20, 80, 40)]
    [InlineData("YC002", "Yard Crane 2", 25, 120, 60)]
    [InlineData("YC003", "Yard Crane 3", 35, 160, 80)]
    public void YardCraneWhenPassingCorrectData_ThenPhysicalResourceIsCreated(string code, string designation, int time, uint capacity, uint cph)
    {

        new YardCrane(
            Guid.NewGuid(),
            new Code { Value = code },
            new Designation { Value = designation },
            ResourceStatus.Available,
            TimeSpan.FromMinutes(time),
            new HashSet<Qualification> { },
            OperationalWindow.FullWeek(),
            capacity,
            new StorageArea(
                Guid.NewGuid(),
                new Code { Value = "area1" },
                new Designation { Value = "Main Storage Area" },
                StorageAreaType.Yard,
                200,
                100,
                []
            ),
            cph
        );
    }

    [Theory]
    [InlineData("", "Yard Crane 1", 20, 80, 40, StorageAreaType.Yard)]
    [InlineData("YC002", "", 25, 120, 60, StorageAreaType.Yard)]
    [InlineData("YC003", "Yard Crane 3", 35, 160, 80, StorageAreaType.Warehouse)]
    public void YardCraneWhenPassingInvalidData_ThenThrowsException(string code, string designation, int time, uint capacity, uint cph, StorageAreaType type)
    {
        Assert.Throws<ArgumentException>(() =>
            new YardCrane(
                Guid.NewGuid(),
                new Code { Value = code },
                new Designation { Value = designation },
                ResourceStatus.Available,
                TimeSpan.FromMinutes(time),
                new HashSet<Qualification> { },
                OperationalWindow.FullWeek(),
                capacity,
                new StorageArea(
                    Guid.NewGuid(),
                    new Code { Value = "area1" },
                    new Designation { Value = "Main Storage Area" },
                    type,
                    200,
                    100,
                    new HashSet<StorageArea.DockRelation> { }
                ),
                cph
            )
        );
    }

    [Theory]
    [InlineData("TR001", "Truck 1", 15, 60, 30, 80)]
    [InlineData("TR002", "Truck 2", 20, 90, 45, 70)]
    [InlineData("TR003", "Truck 3", 25, 120, 60, 60)]
    public void TruckWhenPassingCorrectData_ThenPhysicalResourceIsCreated(string code, string designation, int time, uint capacity, uint cpt, uint speed)
    {
        new Truck(
            Guid.NewGuid(),
            new Code { Value = code },
            new Designation { Value = designation },
            ResourceStatus.Available,
            TimeSpan.FromMinutes(time),
            new HashSet<Qualification> { },
            OperationalWindow.FullWeek(),
            capacity,
            cpt,
            speed
        );
    }

    [Theory]
    [InlineData("", "Truck 1", 15, 60, 30, 80)]
    [InlineData("TR002", "", 20, 90, 45, 70)]
    public void TruckWhenPassingInvalidData_ThenThrowsException(string code, string designation, int time, uint capacity, uint cpt, uint speed)
    {
        Assert.Throws<ArgumentException>(() =>
            new Truck(
                Guid.NewGuid(),
                new Code { Value = code },
                new Designation { Value = designation },
                ResourceStatus.Available,
                TimeSpan.FromMinutes(time),
                new HashSet<Qualification> { },
                OperationalWindow.FullWeek(),
                capacity,
                cpt,
                speed
            )
        );
    }
}