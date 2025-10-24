using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Application.Exceptions;
using Api.Application.Services;
using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Exceptions;
using Api.Infrastructure.Utilities;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Moq;

namespace Tests.Unitary.Service;

public class PhysicalResourceServiceTest
{
    private readonly Mock<IPhysicalResourceRepository> _physicalResourceRepositoryMock;
    private readonly Mock<IDockRepository> _dockRepositoryMock;
    private readonly Mock<IQualificationRepository> _qualificationRepositoryMock;
    private readonly Mock<IStorageAreaRepository> _storageAreaRepositoryMock;

    private readonly PhysicalResourceService _service;

    private static ICollection<PhysicalResource> resources = new List<PhysicalResource>
    {
        new STSCrane(
            Guid.NewGuid(),
            new Code { Value = "CRANE1" },
            new Designation { Value = "STS Crane 1" },
            ResourceStatus.Available,
            TimeSpan.FromMinutes(15),
            new HashSet<Qualification>(),
            OperationalWindow.FullWeek(),
            50,
            new Dock(
                Guid.NewGuid(),
                new Code { Value = "DOCK1" },
                new Designation { Value = "Dock 1" },
                new Designation { Value = "Location 1" },
                new PhysicalCharacteristics { Depth = 10, Length = 300, Draft = 15 },
                new HashSet<VesselType>() {
                    new VesselType(
                        Guid.NewGuid(),
                        new Designation { Value = "Container Ship" },
                        new Designation { Value = "A ship designed to carry containerized cargo." },
                        10, 10, 10,
                        new PhysicalCharacteristics { Depth = 8, Length = 200, Draft = 10 }
                    )
                }
            ),
            10
        ),
        new YardCrane(
            Guid.NewGuid(),
            new Code { Value = "YCRANE1" },
            new Designation { Value = "Yard Crane 1" },
            ResourceStatus.Maintenance,
            TimeSpan.FromMinutes(10),
            new HashSet<Qualification>(),
            OperationalWindow.FullWeek(),
            30,
            10
        ),
        new Truck(
            Guid.NewGuid(),
            new Code { Value = "TRUCK1" },
            new Designation { Value = "Truck 1" },
            ResourceStatus.OutOfService,
            TimeSpan.FromMinutes(5),
            new HashSet<Qualification>(),
            OperationalWindow.FullWeek(),
            20,
            5,
            10
        )
    };

    public PhysicalResourceServiceTest()
    {
        _physicalResourceRepositoryMock = new Mock<IPhysicalResourceRepository>();
        _dockRepositoryMock = new Mock<IDockRepository>();
        _qualificationRepositoryMock = new Mock<IQualificationRepository>();
        _storageAreaRepositoryMock = new Mock<IStorageAreaRepository>();

        _service = new PhysicalResourceService(
            _physicalResourceRepositoryMock.Object,
            _dockRepositoryMock.Object,
            _qualificationRepositoryMock.Object,
            _storageAreaRepositoryMock.Object,
            new Mock<Microsoft.Extensions.Logging.ILogger<PhysicalResourceService>>().Object
        );
    }

    [Fact]
    public async Task GetPhysicalResources_ReturnsListOfResources()
    {
        _physicalResourceRepositoryMock.Setup(repo => repo.GetPhysicalResourcesAsync())
            .ReturnsAsync(resources);

        var result = await _service.GetPhysicalResources();

        var r = (PhysicalResourceDto)result.ElementAt(0);

        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.Equal(resources.ElementAt(0).ToDTO().Code, r.Code);
    }

    [Fact]
    public async Task GetResourceByCode_ReturnsResource_WhenExists()
    {
        _physicalResourceRepositoryMock.Setup(repo => repo.GetResourceByCodeAsync(It.IsAny<string>()))
            .ReturnsAsync((string code) => resources.FirstOrDefault(r => r.Code.Value == code));

        var result = await _service.GetResourceByCode(resources.ElementAt(0).Code.Value);

        var r = (PhysicalResourceDto)result!;

        Assert.NotNull(result);
        Assert.Equal(resources.ElementAt(0).ToDTO().Code, r.Code);
    }

    [Fact]
    public async Task GetResourceByCode_ReturnsNull_WhenNotExists()
    {
        _physicalResourceRepositoryMock.Setup(repo => repo.GetResourceByCodeAsync(It.IsAny<string>()))
            .ReturnsAsync((PhysicalResource?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _service.GetResourceByCode("NonExistentCode"));
    }

    [Fact]
    public async Task AddSTSCraneAsync_ReturnsAddedCrane()
    {
        Dock d = new Dock(
                Guid.NewGuid(),
                new Code { Value = "DOCK1" },
                new Designation { Value = "Dock 1" },
                new Designation { Value = "Location 1" },
                new PhysicalCharacteristics { Depth = 10, Length = 300, Draft = 15 },
                new HashSet<VesselType>()
                {
                    new VesselType(
                        Guid.NewGuid(),
                        new Designation { Value = "Container Ship" },
                        new Designation { Value = "A ship designed to carry containerized cargo." },
                        10, 10, 10,
                        new PhysicalCharacteristics { Depth = 8, Length = 200, Draft = 10 }
                    )
                }
            );

        var newCraneDto = new CreateSTSCraneDto
        {
            Code = "CRANE2",
            Description = "STS Crane 2",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 20,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            LiftingCapacity = 60,
            ServingDockCode = d.Code.Value,
            ContainersPerHour = 12,
        };

        _physicalResourceRepositoryMock.Setup(repo => repo.GetResourceByCodeAsync(newCraneDto.Code))
            .ReturnsAsync(resources.FirstOrDefault(r => r.Code.Value == newCraneDto.Code));

        _dockRepositoryMock.Setup(repo => repo.GetDockByCodeAsync(It.IsAny<string>()))
            .ReturnsAsync(d);

        _qualificationRepositoryMock.Setup(repo => repo.GetQualificationByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new Qualification(Guid.NewGuid(), new Code { Value = "Q1" }, new Designation { Value = "Qualification 1" }));

        _physicalResourceRepositoryMock.Setup(repo => repo.AddSTSCrane(It.IsAny<STSCrane>()))
            .ReturnsAsync((STSCrane crane) => crane);

        var result = await _service.AddSTSCraneAsync(newCraneDto);

        Assert.NotNull(result);
        Assert.Equal(newCraneDto.Code, result.Code);
    }

    [Fact]
    public async Task AddSTSCraneAsync_ThrowsException_WhenDockDoesNotExist()
    {
        var newCraneDto = new CreateSTSCraneDto
        {
            Code = "CRANE2",
            Description = "STS Crane 2",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 20,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            LiftingCapacity = 60,
            ServingDockCode = "NON_EXISTENT_DOCK",
            ContainersPerHour = 12,
        };

        _physicalResourceRepositoryMock.Setup(repo => repo.GetResourceByCodeAsync(newCraneDto.Code))
            .ReturnsAsync(resources.FirstOrDefault(r => r.Code.Value == newCraneDto.Code));

        _dockRepositoryMock.Setup(repo => repo.GetDockByCodeAsync(It.IsAny<string>()))
            .ReturnsAsync((Dock)null!);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.AddSTSCraneAsync(newCraneDto));
    }

    [Fact]
    public async Task AddSTSCraneAsync_ThrowsException_WhenQualificationDoesNotExist()
    {
        Dock d = new Dock(
                Guid.NewGuid(),
                new Code { Value = "DOCK1" },
                new Designation { Value = "Dock 1" },
                new Designation { Value = "Location 1" },
                new PhysicalCharacteristics { Depth = 10, Length = 300, Draft = 15 },
                new HashSet<VesselType>()
                {
                    new VesselType(
                        Guid.NewGuid(),
                        new Designation { Value = "Container Ship" },
                        new Designation { Value = "A ship designed to carry containerized cargo." },
                        10, 10, 10,
                        new PhysicalCharacteristics { Depth = 8, Length = 200, Draft = 10 }
                    )
                }
            );

        var newCraneDto = new CreateSTSCraneDto
        {
            Code = "CRANE2",
            Description = "STS Crane 2",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 20,
            QualificationsCodes = new List<string> { "NON_EXISTENT_QUAL" },
            OperationalWindow = OperationalWindow.FullWeek(),
            LiftingCapacity = 60,
            ServingDockCode = d.Code.Value,
            ContainersPerHour = 12,
        };

        _physicalResourceRepositoryMock.Setup(repo => repo.GetResourceByCodeAsync(newCraneDto.Code))
            .ReturnsAsync(resources.FirstOrDefault(r => r.Code.Value == newCraneDto.Code));

        _dockRepositoryMock.Setup(repo => repo.GetDockByCodeAsync(It.IsAny<string>()))
            .ReturnsAsync(d);

        _qualificationRepositoryMock.Setup(repo => repo.GetQualificationByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((Qualification?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.AddSTSCraneAsync(newCraneDto));
    }

    [Fact]
    public async Task AddSTSCraneAsync_ThrowsException_WhenCodeAlreadyExists()
    {
        var existingCrane = resources.OfType<STSCrane>().First();
        var newCraneDto = new CreateSTSCraneDto
        {
            Code = existingCrane.Code.Value,
            Description = "STS Crane Duplicate",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 20,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            LiftingCapacity = 60,
            ServingDockCode = existingCrane.ServingDock.Code.Value,
            ContainersPerHour = 12,
        };

        _physicalResourceRepositoryMock.Setup(repo => repo.GetResourceByCodeAsync(newCraneDto.Code))
            .ReturnsAsync(existingCrane);

        await Assert.ThrowsAsync<EntityAlreadyExistsException>(() => _service.AddSTSCraneAsync(newCraneDto));
    }

    [Fact]
    public async Task AddYardCraneAsync_ReturnsAddedCrane()
    {

        var newCraneDto = new CreateYardCraneDto
        {
            Code = "YCRANE2",
            Description = "Yard Crane 2",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 15,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            LiftingCapacity = 40,
            ContainersPerHour = 15,
        };

        _physicalResourceRepositoryMock.Setup(repo => repo.GetResourceByCodeAsync(newCraneDto.Code))
            .ReturnsAsync(resources.FirstOrDefault(r => r.Code.Value == newCraneDto.Code));

        _qualificationRepositoryMock.Setup(repo => repo.GetQualificationByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new Qualification(Guid.NewGuid(), new Code { Value = "Q1" }, new Designation { Value = "Qualification 1" }));

        _physicalResourceRepositoryMock.Setup(repo => repo.AddYardCrane(It.IsAny<YardCrane>()))
            .ReturnsAsync((YardCrane crane) => crane);

        var result = await _service.AddYardCraneAsync(newCraneDto);

        Assert.NotNull(result);
        Assert.Equal(newCraneDto.Code, result.Code);
    }

    [Fact]
    public async Task AddYardCraneAsync_ThrowsException_WhenCodeAlreadyExists()
    {
        var existingCrane = resources.OfType<YardCrane>().First();
        var newCraneDto = new CreateYardCraneDto
        {
            Code = existingCrane.Code.Value,
            Description = "Yard Crane Duplicate",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 15,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            LiftingCapacity = 40,
            ContainersPerHour = 15,
        };

        _physicalResourceRepositoryMock.Setup(repo => repo.GetResourceByCodeAsync(newCraneDto.Code))
            .ReturnsAsync(existingCrane);

        await Assert.ThrowsAsync<EntityAlreadyExistsException>(() => _service.AddYardCraneAsync(newCraneDto));
    }

    [Fact]
    public async Task AddYardCraneAsync_ThrowsException_WhenQualificationDoesNotExist()
    {

        var newCraneDto = new CreateYardCraneDto
        {
            Code = "YCRANE2",
            Description = "Yard Crane 2",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 15,
            QualificationsCodes = new List<string> { "NON_EXISTENT_QUAL" },
            OperationalWindow = OperationalWindow.FullWeek(),
            LiftingCapacity = 40,
            ContainersPerHour = 15,
        };

        _physicalResourceRepositoryMock.Setup(repo => repo.GetResourceByCodeAsync(newCraneDto.Code))
            .ReturnsAsync(resources.FirstOrDefault(r => r.Code.Value == newCraneDto.Code));

        _qualificationRepositoryMock.Setup(repo => repo.GetQualificationByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((Qualification?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.AddYardCraneAsync(newCraneDto));
    }

    [Fact]
    public async Task AddTruckAsync_ReturnsAddedTruck()
    {
        var newTruckDto = new CreateTruckDto
        {
            Code = "TRUCK2",
            Description = "Truck 2",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 10,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerTrip = 25,
            AverageSpeed = 60,
            MaxLoadCapacity = 15
        };

        _physicalResourceRepositoryMock.Setup(repo => repo.GetResourceByCodeAsync(newTruckDto.Code))
            .ReturnsAsync(resources.FirstOrDefault(r => r.Code.Value == newTruckDto.Code));

        _qualificationRepositoryMock.Setup(repo => repo.GetQualificationByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new Qualification(Guid.NewGuid(), new Code { Value = "Q1" }, new Designation { Value = "Qualification 1" }));

        _physicalResourceRepositoryMock.Setup(repo => repo.AddTruck(It.IsAny<Truck>()))
            .ReturnsAsync((Truck truck) => truck);

        var result = await _service.AddTruckAsync(newTruckDto);

        Assert.NotNull(result);
        Assert.Equal(newTruckDto.Code, result.Code);
    }

    [Fact]
    public async Task AddTruckAsync_ThrowsException_WhenCodeAlreadyExists()
    {
        var existingTruck = resources.OfType<Truck>().First();
        var newTruckDto = new CreateTruckDto
        {
            Code = existingTruck.Code.Value,
            Description = "Truck Duplicate",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 10,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerTrip = 25,
            AverageSpeed = 60,
            MaxLoadCapacity = 15
        };

        _physicalResourceRepositoryMock.Setup(repo => repo.GetResourceByCodeAsync(newTruckDto.Code))
            .ReturnsAsync(existingTruck);

        await Assert.ThrowsAsync<EntityAlreadyExistsException>(() => _service.AddTruckAsync(newTruckDto));
    }

    [Fact]
    public async Task AddTruckAsync_ThrowsException_WhenQualificationDoesNotExist()
    {
        var newTruckDto = new CreateTruckDto
        {
            Code = "TRUCK2",
            Description = "Truck 2",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 10,
            QualificationsCodes = new List<string> { "NON_EXISTENT_QUAL" },
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerTrip = 25,
            AverageSpeed = 60,
            MaxLoadCapacity = 15
        };

        _physicalResourceRepositoryMock.Setup(repo => repo.GetResourceByCodeAsync(newTruckDto.Code))
            .ReturnsAsync(resources.FirstOrDefault(r => r.Code.Value == newTruckDto.Code));

        _qualificationRepositoryMock.Setup(repo => repo.GetQualificationByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((Qualification?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.AddTruckAsync(newTruckDto));
    }

    [Fact]
    public async Task UpdateSTSCraneAsync_ReturnsUpdatedCrane()
    {
        var existingCrane = resources.OfType<STSCrane>().First();

        var updateCraneDto = new CreateSTSCraneDto
        {
            Code = existingCrane.Code.Value,
            Description = "Updated STS Crane",
            Status = ResourceStatus.Maintenance,
            SetupTimeInMinutes = 25,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            LiftingCapacity = 70,
            ServingDockCode = existingCrane.ServingDock.Code.Value,
            ContainersPerHour = 14,
        };

        _physicalResourceRepositoryMock.Setup(repo => repo.GetResourceByCodeAsync(updateCraneDto.Code))
            .ReturnsAsync(existingCrane);

        _dockRepositoryMock.Setup(repo => repo.GetDockByCodeAsync(It.IsAny<string>()))
            .ReturnsAsync(existingCrane.ServingDock);

        _qualificationRepositoryMock.Setup(repo => repo.GetQualificationByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new Qualification(Guid.NewGuid(), new Code { Value = "Q1" }, new Designation { Value = "Qualification 1" }));

        _physicalResourceRepositoryMock.Setup(repo => repo.UpdateSTSCrane(It.IsAny<STSCrane>()))
            .ReturnsAsync((STSCrane crane) => crane);

        var result = await _service.UpdateSTSCraneAsync(updateCraneDto.Code, updateCraneDto);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateSTSCraneAsync_ThrowsException_WhenCraneDoesNotExist()
    {
        var updateCraneDto = new CreateSTSCraneDto
        {
            Code = "NON_EXISTENT_CRANE",
            Description = "Updated STS Crane",
            Status = ResourceStatus.Maintenance,
            SetupTimeInMinutes = 25,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            LiftingCapacity = 70,
            ServingDockCode = "DOCK1",
            ContainersPerHour = 14,
        };

        _physicalResourceRepositoryMock.Setup(repo => repo.GetResourceByCodeAsync(updateCraneDto.Code))
            .ReturnsAsync((PhysicalResource?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.UpdateSTSCraneAsync(updateCraneDto.Code, updateCraneDto));
    }

    [Fact]
    public async Task UpdateSTSCraneAsync_ThrowsException_WhenDockDoesNotExist()
    {
        var existingCrane = resources.OfType<STSCrane>().First();

        var updateCraneDto = new CreateSTSCraneDto
        {
            Code = existingCrane.Code.Value,
            Description = "Updated STS Crane",
            Status = ResourceStatus.Maintenance,
            SetupTimeInMinutes = 25,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            LiftingCapacity = 70,
            ServingDockCode = "NON_EXISTENT_DOCK",
            ContainersPerHour = 14,
        };

        _physicalResourceRepositoryMock.Setup(repo => repo.GetResourceByCodeAsync(updateCraneDto.Code))
            .ReturnsAsync(existingCrane);

        _dockRepositoryMock.Setup(repo => repo.GetDockByCodeAsync(It.IsAny<string>()))
            .ReturnsAsync((Dock)null!);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.UpdateSTSCraneAsync(updateCraneDto.Code, updateCraneDto));
    }

    [Fact]
    public async Task UpdateSTSCraneAsync_ThrowsException_WhenQualificationDoesNotExist()
    {
        var existingCrane = resources.OfType<STSCrane>().First();

        var updateCraneDto = new CreateSTSCraneDto
        {
            Code = existingCrane.Code.Value,
            Description = "Updated STS Crane",
            Status = ResourceStatus.Maintenance,
            SetupTimeInMinutes = 25,
            QualificationsCodes = new List<string> { "NON_EXISTENT_QUAL" },
            OperationalWindow = OperationalWindow.FullWeek(),
            LiftingCapacity = 70,
            ServingDockCode = existingCrane.ServingDock.Code.Value,
            ContainersPerHour = 14,
        };

        _physicalResourceRepositoryMock.Setup(repo => repo.GetResourceByCodeAsync(updateCraneDto.Code))
            .ReturnsAsync(existingCrane);

        _dockRepositoryMock.Setup(repo => repo.GetDockByCodeAsync(It.IsAny<string>()))
            .ReturnsAsync(existingCrane.ServingDock);

        _qualificationRepositoryMock.Setup(repo => repo.GetQualificationByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((Qualification?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.UpdateSTSCraneAsync(updateCraneDto.Code, updateCraneDto));
    }

    [Fact]
    public async Task UpdateSTSCraneAsync_ThrowsException_WhenNotACrane()
    {
        var existingTruck = resources.OfType<Truck>().First();

        var updateCraneDto = new CreateSTSCraneDto
        {
            Code = existingTruck.Code.Value,
            Description = "Updated STS Crane",
            Status = ResourceStatus.Maintenance,
            SetupTimeInMinutes = 25,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            LiftingCapacity = 70,
            ServingDockCode = "DOCK1",
            ContainersPerHour = 14,
        };

        _physicalResourceRepositoryMock.Setup(repo => repo.GetResourceByCodeAsync(updateCraneDto.Code))
            .ReturnsAsync(existingTruck);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateSTSCraneAsync(updateCraneDto.Code, updateCraneDto));
    }

    [Fact]
    public async Task UpdateYardCraneAsync_ReturnsUpdatedCrane()
    {
        var existingCrane = resources.OfType<YardCrane>().First();

        var updateCraneDto = new CreateYardCraneDto
        {
            Code = existingCrane.Code.Value,
            Description = "Updated Yard Crane",
            Status = ResourceStatus.Maintenance,
            SetupTimeInMinutes = 20,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            LiftingCapacity = 45,
            ContainersPerHour = 18,
        };

        _physicalResourceRepositoryMock.Setup(repo => repo.GetResourceByCodeAsync(updateCraneDto.Code))
            .ReturnsAsync(existingCrane);

        _qualificationRepositoryMock.Setup(repo => repo.GetQualificationByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new Qualification(Guid.NewGuid(), new Code { Value = "Q1" }, new Designation { Value = "Qualification 1" }));

        _physicalResourceRepositoryMock.Setup(repo => repo.UpdateYardCrane(It.IsAny<YardCrane>()))
            .ReturnsAsync((YardCrane crane) => crane);

        var result = await _service.UpdateYardCraneAsync(updateCraneDto.Code, updateCraneDto);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateYardCraneAsync_ThrowsException_WhenCraneDoesNotExist()
    {
        var updateCraneDto = new CreateYardCraneDto
        {
            Code = "NON_EXISTENT_CRANE",
            Description = "Updated Yard Crane",
            Status = ResourceStatus.Maintenance,
            SetupTimeInMinutes = 20,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            LiftingCapacity = 45,
            ContainersPerHour = 18,
        };

        _physicalResourceRepositoryMock.Setup(repo => repo.GetResourceByCodeAsync(updateCraneDto.Code))
            .ReturnsAsync((PhysicalResource?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.UpdateYardCraneAsync(updateCraneDto.Code, updateCraneDto));
    }

    [Fact]
    public async Task UpdateYardCraneAsync_ThrowsException_WhenQualificationDoesNotExist()
    {
        var existingCrane = resources.OfType<YardCrane>().First();

        var updateCraneDto = new CreateYardCraneDto
        {
            Code = existingCrane.Code.Value,
            Description = "Updated Yard Crane",
            Status = ResourceStatus.Maintenance,
            SetupTimeInMinutes = 20,
            QualificationsCodes = new List<string> { "NON_EXISTENT_QUAL" },
            OperationalWindow = OperationalWindow.FullWeek(),
            LiftingCapacity = 45,
            ContainersPerHour = 18,
        };

        _physicalResourceRepositoryMock.Setup(repo => repo.GetResourceByCodeAsync(updateCraneDto.Code))
            .ReturnsAsync(existingCrane);

        _qualificationRepositoryMock.Setup(repo => repo.GetQualificationByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((Qualification?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.UpdateYardCraneAsync(updateCraneDto.Code, updateCraneDto));
    }

    [Fact]
    public async Task UpdateYardCraneAsync_ThrowsException_WhenNotACrane()
    {
        var existingTruck = resources.OfType<Truck>().First();

        var updateCraneDto = new CreateYardCraneDto
        {
            Code = existingTruck.Code.Value,
            Description = "Updated Yard Crane",
            Status = ResourceStatus.Maintenance,
            SetupTimeInMinutes = 20,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            LiftingCapacity = 45,
            ContainersPerHour = 18,
        };

        _physicalResourceRepositoryMock.Setup(repo => repo.GetResourceByCodeAsync(updateCraneDto.Code))
            .ReturnsAsync(existingTruck);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateYardCraneAsync(updateCraneDto.Code, updateCraneDto));
    }

    [Fact]
    public async Task UpdateTruckAsync_ReturnsUpdatedTruck()
    {
        var existingTruck = resources.OfType<Truck>().First();

        var updateTruckDto = new CreateTruckDto
        {
            Code = existingTruck.Code.Value,
            Description = "Updated Truck",
            Status = ResourceStatus.Maintenance,
            SetupTimeInMinutes = 8,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerTrip = 30,
            AverageSpeed = 65,
            MaxLoadCapacity = 18
        };

        _physicalResourceRepositoryMock.Setup(repo => repo.GetResourceByCodeAsync(updateTruckDto.Code))
            .ReturnsAsync(existingTruck);

        _qualificationRepositoryMock.Setup(repo => repo.GetQualificationByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new Qualification(Guid.NewGuid(), new Code { Value = "Q1" }, new Designation { Value = "Qualification 1" }));

        _physicalResourceRepositoryMock.Setup(repo => repo.UpdateTruck(It.IsAny<Truck>()))
            .ReturnsAsync((Truck truck) => truck);

        var result = await _service.UpdateTruckAsync(updateTruckDto.Code, updateTruckDto);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateTruckAsync_ThrowsException_WhenTruckDoesNotExist()
    {
        var updateTruckDto = new CreateTruckDto
        {
            Code = "NON_EXISTENT_TRUCK",
            Description = "Updated Truck",
            Status = ResourceStatus.Maintenance,
            SetupTimeInMinutes = 8,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerTrip = 30,
            AverageSpeed = 65,
            MaxLoadCapacity = 18
        };

        _physicalResourceRepositoryMock.Setup(repo => repo.GetResourceByCodeAsync(updateTruckDto.Code))
            .ReturnsAsync((PhysicalResource?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.UpdateTruckAsync(updateTruckDto.Code, updateTruckDto));
    }

    [Fact]
    public async Task UpdateTruckAsync_ThrowsException_WhenQualificationDoesNotExist()
    {
        var existingTruck = resources.OfType<Truck>().First();

        var updateTruckDto = new CreateTruckDto
        {
            Code = existingTruck.Code.Value,
            Description = "Updated Truck",
            Status = ResourceStatus.Maintenance,
            SetupTimeInMinutes = 8,
            QualificationsCodes = new List<string> { "NON_EXISTENT_QUAL" },
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerTrip = 30,
            AverageSpeed = 65,
            MaxLoadCapacity = 18
        };

        _physicalResourceRepositoryMock.Setup(repo => repo.GetResourceByCodeAsync(updateTruckDto.Code))
            .ReturnsAsync(existingTruck);

        _qualificationRepositoryMock.Setup(repo => repo.GetQualificationByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((Qualification?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.UpdateTruckAsync(updateTruckDto.Code, updateTruckDto));
    }

    [Fact]
    public async Task UpdateTruckAsync_ThrowsException_WhenNotATruck()
    {
        var existingCrane = resources.OfType<STSCrane>().First();

        var updateTruckDto = new CreateTruckDto
        {
            Code = existingCrane.Code.Value,
            Description = "Updated Truck",
            Status = ResourceStatus.Maintenance,
            SetupTimeInMinutes = 8,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerTrip = 30,
            AverageSpeed = 65,
            MaxLoadCapacity = 18
        };

        _physicalResourceRepositoryMock.Setup(repo => repo.GetResourceByCodeAsync(updateTruckDto.Code))
            .ReturnsAsync(existingCrane);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateTruckAsync(updateTruckDto.Code, updateTruckDto));
    }

    [Fact]
    public async Task FilterPhysicalResources_ReturnsFilteredResources()
    {
        _physicalResourceRepositoryMock.Setup(repo => repo.FilterPhysicalResourcesAsync(It.IsAny<PhysicalResourceFilter>()))
            .ReturnsAsync((PhysicalResourceFilter filter) =>
            {
                return new Page<PhysicalResource>
                {
                    Items = resources.ToList(),
                    PageNumber = 10,
                    PageSize = 20
                };
            });

        var result = await _service.FilterPhysicalResources(new PhysicalResourceFilter { });

        Assert.NotNull(result);
        Assert.IsType<Page<object>>(result);
        Assert.Equal(resources.Count(), result.Items.Count());
    }

    [Fact]
    public async Task DeactivateResource_Succeeds_WhenResourceExists()
    {
        var existingTruck = resources.OfType<Truck>().First();

        _physicalResourceRepositoryMock.Setup(repo => repo.GetResourceByCodeAsync(existingTruck.Code.Value))
            .ReturnsAsync(existingTruck);

        _physicalResourceRepositoryMock.Setup(repo => repo.UpdateTruck(It.IsAny<Truck>()))
            .ReturnsAsync((Truck truck) => truck);

        var result = await _service.DeactivateResource(existingTruck.Code.Value);
        Assert.True(result);
    }

    [Fact]
    public async Task DeactivateResource_ThrowsException_WhenResourceDoesNotExist()
    {
        _physicalResourceRepositoryMock.Setup(repo => repo.GetResourceByCodeAsync("NON_EXISTENT_RESOURCE"))
            .ReturnsAsync((PhysicalResource)null!);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.DeactivateResource("NON_EXISTENT_RESOURCE"));
    }
}