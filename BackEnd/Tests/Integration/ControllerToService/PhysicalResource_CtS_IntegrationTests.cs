using Xunit;
using Api.Application.Controllers;
using Api.Application.Services;
using Api.Application.DataTransfer;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;
using Api.Infrastructure.Persistence.Repositories;
using Api.Infrastructure.Persistence;
using Moq;
using Api.Domain.IRepository;
using Microsoft.Extensions.Logging;
using Api.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Api.Domain.ValueObjects;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Tests.Integration.ControllerToService;

public class PhysicalResource_CtS_IntegrationTest
{
    private readonly PhysicalResourceController _controller;
    private readonly PhysicalResourceService _service;
    private readonly Mock<IPhysicalResourceRepository> _repositoryMock;
    private readonly Mock<IDockRepository> _dockRepositoryMock;
    private readonly Mock<IQualificationRepository> _qualificationRepositoryMock;
    private readonly Mock<IStorageAreaRepository> _storageAreaRepositoryMock;
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
            new StorageArea(
                Guid.NewGuid(),
                new Code { Value = "STORAGE1" },
                new Designation { Value = "Storage Area 1" },
                StorageAreaType.Yard,
                100,
                10,
                new HashSet<StorageArea.DockRelation>()
            ),
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

    public PhysicalResource_CtS_IntegrationTest()
    {
        _repositoryMock = new Mock<IPhysicalResourceRepository>();
        _dockRepositoryMock = new Mock<IDockRepository>();
        _qualificationRepositoryMock = new Mock<IQualificationRepository>();
        _storageAreaRepositoryMock = new Mock<IStorageAreaRepository>();

        _service = new PhysicalResourceService(
            _repositoryMock.Object,
            _dockRepositoryMock.Object,
            _qualificationRepositoryMock.Object,
            _storageAreaRepositoryMock.Object,
            new Mock<ILogger<PhysicalResourceService>>().Object
        );
        _controller = new PhysicalResourceController(_service, new Mock<ILogger<PhysicalResourceController>>().Object);
    }

    [Fact]
    public async Task GetAllPhysicalResources_ReturnsOkResult_WithListOfPhysicalResources()
    {
        _repositoryMock.Setup(repo => repo.GetPhysicalResourcesAsync())
            .ReturnsAsync(new List<PhysicalResource>());

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<object>>(okResult.Value);
        Assert.Empty(returnValue);
    }

    [Fact]
    public async Task GetPhysicalResourceByCodeTruck_ReturnsOkResult_WithPhysicalResource()
    {
        var testCode = resources.OfType<Truck>().First().Code.Value;
        _repositoryMock.Setup(repo => repo.GetResourceByCodeAsync(testCode))
            .ReturnsAsync(resources.OfType<Truck>().First());

        var result = await _controller.GetByCode(testCode);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<TruckDto>(okResult.Value);
        Assert.Equal(testCode, returnValue.Code);
    }

    [Fact]
    public async Task GetPhysicalResourceByCodeYardCrane_ReturnsOkResult_WithPhysicalResource()
    {
        var testCode = resources.OfType<YardCrane>().First().Code.Value;
        _repositoryMock.Setup(repo => repo.GetResourceByCodeAsync(testCode))
            .ReturnsAsync(resources.OfType<YardCrane>().First());

        var result = await _controller.GetByCode(testCode);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<YardCraneDto>(okResult.Value);
        Assert.Equal(testCode, returnValue.Code);
    }

    [Fact]
    public async Task GetPhysicalResourceByCodeSTSCrane_ReturnsOkResult_WithPhysicalResource()
    {
        var testCode = resources.OfType<STSCrane>().First().Code.Value;
        _repositoryMock.Setup(repo => repo.GetResourceByCodeAsync(testCode))
            .ReturnsAsync(resources.OfType<STSCrane>().First());

        var result = await _controller.GetByCode(testCode);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<STSCraneDto>(okResult.Value);
        Assert.Equal(testCode, returnValue.Code);
    }

    [Fact]
    public async Task GetPhysicalResourceByCode_ReturnsNotFound_WhenPhysicalResourceDoesNotExist()
    {
        var testCode = "NONEXISTENT";
        _repositoryMock.Setup(repo => repo.GetResourceByCodeAsync(testCode))
            .ReturnsAsync((PhysicalResource?)null);

        var result = await _controller.GetByCode(testCode);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task FilterPhysicalResources_ReturnsOkResult_WithFilteredPhysicalResources()
    {
        var filter = new PhysicalResourceFilter { };
        _repositoryMock.Setup(repo => repo.FilterPhysicalResourcesAsync(filter))
            .ReturnsAsync(new Page<PhysicalResource>
            {
                Items = new List<PhysicalResource>(),
                PageNumber = 1,
                PageSize = 10,
            });

        var result = await _controller.Filter(filter);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<Page<object>>(okResult.Value);
        Assert.Empty(returnValue.Items);
    }

    [Fact]
    public async Task CreateSTSCrane_ReturnsCreatedAtActionResult_WithCreatedPhysicalResource()
    {
        var newResourceDto = new CreateSTSCraneDto
        {
            Code = "PR456",
            Description = "New Physical Resource",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 20,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 30,
            LiftingCapacity = 5000,
            ServingDockCode = "DOCK1"
        };

        _repositoryMock.Setup(repo => repo.AddSTSCrane(It.IsAny<STSCrane>()))
            .ReturnsAsync((STSCrane resource) => resource);

        _dockRepositoryMock.Setup(repo => repo.GetDockByCodeAsync(newResourceDto.ServingDockCode))
            .ReturnsAsync(new Dock(
                Guid.NewGuid(),
                new Code { Value = newResourceDto.ServingDockCode },
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
            ));

        var result = await _controller.AddSTSCrane(newResourceDto);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<STSCraneDto>(createdAtActionResult.Value);
        Assert.Equal(newResourceDto.Code, returnValue.Code);
    }

    [Fact]
    public async Task CreateSTSCrane_ReturnsNotFound_WhenDockDoesNotExist()
    {
        var newResourceDto = new CreateSTSCraneDto
        {
            Code = "PR456",
            Description = "New Physical Resource",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 20,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 30,
            LiftingCapacity = 5000,
            ServingDockCode = "NONEXISTENT"
        };

        _dockRepositoryMock.Setup(repo => repo.GetDockByCodeAsync(newResourceDto.ServingDockCode))
            .ReturnsAsync((Dock)null!);

        var result = await _controller.AddSTSCrane(newResourceDto);

        var badRequestResult = Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateSTSCrane_ReturnsConflict_OnDuplicateCode()
    {
        var newResourceDto = new CreateSTSCraneDto
        {
            Code = "CRANE1",
            Description = "New Physical Resource",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 20,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 30,
            LiftingCapacity = 5000,
            ServingDockCode = "DOCK1"
        };

        _repositoryMock.Setup(repo => repo.GetResourceByCodeAsync(newResourceDto.Code))
            .ReturnsAsync(resources.OfType<STSCrane>().First());

        var result = await _controller.AddSTSCrane(newResourceDto);

        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateYardCrane_ReturnsCreatedAtActionResult_WithCreatedPhysicalResource()
    {
        var newResourceDto = new CreateYardCraneDto
        {
            Code = "PR789",
            Description = "New Yard Crane",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 15,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 25,
            YardSectionCode = "STORAGE1",
            LiftingCapacity = 4000
        };

        _repositoryMock.Setup(repo => repo.AddYardCrane(It.IsAny<YardCrane>()))
            .ReturnsAsync((YardCrane resource) => resource);

        _storageAreaRepositoryMock.Setup(repo => repo.GetStorageAreaByCodeAsync(newResourceDto.YardSectionCode))
            .ReturnsAsync(new StorageArea(
                Guid.NewGuid(),
                new Code { Value = newResourceDto.YardSectionCode },
                new Designation { Value = "Storage Area 1" },
                StorageAreaType.Yard,
                100,
                10,
                new HashSet<StorageArea.DockRelation>()
            ));

        var result = await _controller.AddYardCrane(newResourceDto);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<YardCraneDto>(createdAtActionResult.Value);
        Assert.Equal(newResourceDto.Code, returnValue.Code);
    }

    [Fact]
    public async Task CreateYardCrane_ReturnsNotFound_WhenStorageAreaDoesNotExist()
    {
        var newResourceDto = new CreateYardCraneDto
        {
            Code = "PR789",
            Description = "New Yard Crane",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 15,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 25,
            YardSectionCode = "NONEXISTENT",
            LiftingCapacity = 4000
        };

        _storageAreaRepositoryMock.Setup(repo => repo.GetStorageAreaByCodeAsync(newResourceDto.YardSectionCode))
            .ReturnsAsync((StorageArea?)null);

        var result = await _controller.AddYardCrane(newResourceDto);

        var badRequestResult = Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateYardCrane_ReturnsConflict_OnDuplicateCode()
    {
        var newResourceDto = new CreateYardCraneDto
        {
            Code = "YCRANE1",
            Description = "New Yard Crane",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 15,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 25,
            YardSectionCode = "STORAGE1",
            LiftingCapacity = 4000
        };

        _repositoryMock.Setup(repo => repo.GetResourceByCodeAsync(newResourceDto.Code))
            .ReturnsAsync(resources.OfType<YardCrane>().First());

        var result = await _controller.AddYardCrane(newResourceDto);

        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateTruck_ReturnsCreatedAtActionResult_WithCreatedPhysicalResource()
    {
        var newResourceDto = new CreateTruckDto
        {
            Code = "TRUCK2",
            Description = "New Truck",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 10,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            MaxLoadCapacity = 15,
            AverageSpeed = 60,
            ContainersPerTrip = 2
        };

        _repositoryMock.Setup(repo => repo.AddTruck(It.IsAny<Truck>()))
            .ReturnsAsync((Truck resource) => resource);

        var result = await _controller.AddTruck(newResourceDto);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<TruckDto>(createdAtActionResult.Value);
        Assert.Equal(newResourceDto.Code, returnValue.Code);
    }

    [Fact]
    public async Task CreateTruck_ReturnsConflict_OnDuplicateCode()
    {
        var newResourceDto = new CreateTruckDto
        {
            Code = "TRUCK1",
            Description = "New Truck",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 10,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            MaxLoadCapacity = 15,
            AverageSpeed = 60,
            ContainersPerTrip = 2
        };

        _repositoryMock.Setup(repo => repo.GetResourceByCodeAsync(newResourceDto.Code))
            .ReturnsAsync(resources.OfType<Truck>().First());

        var result = await _controller.AddTruck(newResourceDto);

        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    [Fact]
    public async Task DeactivatePhysicalResource_ReturnsNoContent_WhenSuccessful()
    {
        var testCode = resources.First().Code.Value;

        _repositoryMock.Setup(repo => repo.GetResourceByCodeAsync(testCode))
            .ReturnsAsync(resources.First());
        _repositoryMock.Setup(repo => repo.Update(It.IsAny<PhysicalResource>()))
            .ReturnsAsync((PhysicalResource resource) => resource);

        var result = await _controller.Deactivate(testCode);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeactivatePhysicalResource_ReturnsNotFound_WhenPhysicalResourceDoesNotExist()
    {
        var testCode = "NONEXISTENT";

        _repositoryMock.Setup(repo => repo.GetResourceByCodeAsync(testCode))
            .ReturnsAsync((PhysicalResource?)null);

        var result = await _controller.Deactivate(testCode);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task DeactivatePhysicalResource_ReturnsInternalServerError_OnException()
    {
        var testCode = resources.First().Code.Value;

        _repositoryMock.Setup(repo => repo.GetResourceByCodeAsync(testCode))
            .ThrowsAsync(new Exception("Database error"));

        var result = await _controller.Deactivate(testCode);

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    [Fact]
    public async Task UpdateSTSCrane_ReturnsNoContent_WhenSuccessful()
    {
        var existingCode = resources.OfType<STSCrane>().First().Code.Value;
        var updateDto = new CreateSTSCraneDto
        {
            Code = existingCode,
            Description = "Updated STS Crane",
            Status = ResourceStatus.Maintenance,
            SetupTimeInMinutes = 25,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 35,
            LiftingCapacity = 5500,
            ServingDockCode = "DOCK1"
        };

        _repositoryMock.Setup(repo => repo.GetResourceByCodeAsync(existingCode))
            .ReturnsAsync(resources.OfType<STSCrane>().First());
        _dockRepositoryMock.Setup(repo => repo.GetDockByCodeAsync(updateDto.ServingDockCode))
            .ReturnsAsync(new Dock(
                Guid.NewGuid(),
                new Code { Value = updateDto.ServingDockCode },
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
            ));
        _repositoryMock.Setup(repo => repo.UpdateSTSCrane(It.IsAny<STSCrane>()))
            .ReturnsAsync((STSCrane resource) => resource);

        var result = await _controller.UpdateSTSCrane(existingCode, updateDto);

        Assert.IsType<NoContentResult>(result.Result);
    }

    [Fact]
    public async Task UpdateSTSCrane_ReturnsNotFound_WhenPhysicalResourceDoesNotExist()
    {
        var nonExistentCode = "NONEXISTENT";
        var updateDto = new CreateSTSCraneDto
        {
            Code = nonExistentCode,
            Description = "Updated STS Crane",
            Status = ResourceStatus.Maintenance,
            SetupTimeInMinutes = 25,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 35,
            LiftingCapacity = 5500,
            ServingDockCode = "DOCK1"
        };

        _repositoryMock.Setup(repo => repo.GetResourceByCodeAsync(nonExistentCode))
            .ReturnsAsync((PhysicalResource?)null);

        var result = await _controller.UpdateSTSCrane(nonExistentCode, updateDto);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateSTSCrane_ReturnsNotFound_WhenDockDoesNotExist()
    {
        var existingCode = resources.OfType<STSCrane>().First().Code.Value;
        var updateDto = new CreateSTSCraneDto
        {
            Code = existingCode,
            Description = "Updated STS Crane",
            Status = ResourceStatus.Maintenance,
            SetupTimeInMinutes = 25,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 35,
            LiftingCapacity = 5500,
            ServingDockCode = "NONEXISTENT"
        };

        _repositoryMock.Setup(repo => repo.GetResourceByCodeAsync(existingCode))
            .ReturnsAsync(resources.OfType<STSCrane>().First());
        _dockRepositoryMock.Setup(repo => repo.GetDockByCodeAsync(updateDto.ServingDockCode))
            .ReturnsAsync((Dock?)null);

        var result = await _controller.UpdateSTSCrane(existingCode, updateDto);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateSTSCrane_ReturnsInternalServerError_OnException()
    {
        var existingCode = resources.OfType<STSCrane>().First().Code.Value;
        var updateDto = new CreateSTSCraneDto
        {
            Code = existingCode,
            Description = "Updated STS Crane",
            Status = ResourceStatus.Maintenance,
            SetupTimeInMinutes = 25,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 35,
            LiftingCapacity = 5500,
            ServingDockCode = "DOCK1"
        };

        _repositoryMock.Setup(repo => repo.GetResourceByCodeAsync(existingCode))
            .ThrowsAsync(new Exception("Database error"));

        var result = await _controller.UpdateSTSCrane(existingCode, updateDto);

        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    [Fact]
    public async Task UpdateYardCrane_ReturnsNoContent_WhenSuccessful()
    {
        var existingCode = resources.OfType<YardCrane>().First().Code.Value;
        var updateDto = new CreateYardCraneDto
        {
            Code = existingCode,
            Description = "Updated Yard Crane",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 12,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 28,
            YardSectionCode = "STORAGE1",
            LiftingCapacity = 4500
        };

        _repositoryMock.Setup(repo => repo.GetResourceByCodeAsync(existingCode))
            .ReturnsAsync(resources.OfType<YardCrane>().First());
        _storageAreaRepositoryMock.Setup(repo => repo.GetStorageAreaByCodeAsync(updateDto.YardSectionCode))
            .ReturnsAsync(new StorageArea(
                Guid.NewGuid(),
                new Code { Value = updateDto.YardSectionCode },
                new Designation { Value = "Storage Area 1" },
                StorageAreaType.Yard,
                100,
                10,
                new HashSet<StorageArea.DockRelation>()
            ));
        _repositoryMock.Setup(repo => repo.UpdateYardCrane(It.IsAny<YardCrane>()))
            .ReturnsAsync((YardCrane resource) => resource);

        var result = await _controller.UpdateYardCrane(existingCode, updateDto);

        Assert.IsType<NoContentResult>(result.Result);
    }

    [Fact]
    public async Task UpdateYardCrane_ReturnsNotFound_WhenPhysicalResourceDoesNotExist()
    {
        var nonExistentCode = "NONEXISTENT";
        var updateDto = new CreateYardCraneDto
        {
            Code = nonExistentCode,
            Description = "Updated Yard Crane",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 12,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 28,
            YardSectionCode = "STORAGE1",
            LiftingCapacity = 4500
        };

        _repositoryMock.Setup(repo => repo.GetResourceByCodeAsync(nonExistentCode))
            .ReturnsAsync((PhysicalResource?)null);

        var result = await _controller.UpdateYardCrane(nonExistentCode, updateDto);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateYardCrane_ReturnsNotFound_WhenStorageAreaDoesNotExist()
    {
        var existingCode = resources.OfType<YardCrane>().First().Code.Value;
        var updateDto = new CreateYardCraneDto
        {
            Code = existingCode,
            Description = "Updated Yard Crane",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 12,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 28,
            YardSectionCode = "NONEXISTENT",
            LiftingCapacity = 4500
        };

        _repositoryMock.Setup(repo => repo.GetResourceByCodeAsync(existingCode))
            .ReturnsAsync(resources.OfType<YardCrane>().First());
        _storageAreaRepositoryMock.Setup(repo => repo.GetStorageAreaByCodeAsync(updateDto.YardSectionCode))
            .ReturnsAsync((StorageArea?)null);

        var result = await _controller.UpdateYardCrane(existingCode, updateDto);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateYardCrane_ReturnsInternalServerError_OnException()
    {
        var existingCode = resources.OfType<YardCrane>().First().Code.Value;
        var updateDto = new CreateYardCraneDto
        {
            Code = existingCode,
            Description = "Updated Yard Crane",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 12,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 28,
            YardSectionCode = "STORAGE1",
            LiftingCapacity = 4500
        };

        _repositoryMock.Setup(repo => repo.GetResourceByCodeAsync(existingCode))
            .ThrowsAsync(new Exception("Database error"));

        var result = await _controller.UpdateYardCrane(existingCode, updateDto);

        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    [Fact]
    public async Task UpdateTruck_ReturnsNoContent_WhenSuccessful()
    {
        var existingCode = resources.OfType<Truck>().First().Code.Value;
        var updateDto = new CreateTruckDto
        {
            Code = existingCode,
            Description = "Updated Truck",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 8,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            MaxLoadCapacity = 25,
            AverageSpeed = 70,
            ContainersPerTrip = 3
        };

        _repositoryMock.Setup(repo => repo.GetResourceByCodeAsync(existingCode))
            .ReturnsAsync(resources.OfType<Truck>().First());
        _repositoryMock.Setup(repo => repo.UpdateTruck(It.IsAny<Truck>()))
            .ReturnsAsync((Truck resource) => resource);

        var result = await _controller.UpdateTruck(existingCode, updateDto);

        Assert.IsType<NoContentResult>(result.Result);
    }

    [Fact]
    public async Task UpdateTruck_ReturnsNotFound_WhenPhysicalResourceDoesNotExist()
    {
        var nonExistentCode = "NONEXISTENT";
        var updateDto = new CreateTruckDto
        {
            Code = nonExistentCode,
            Description = "Updated Truck",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 8,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            MaxLoadCapacity = 25,
            AverageSpeed = 70,
            ContainersPerTrip = 3
        };

        _repositoryMock.Setup(repo => repo.GetResourceByCodeAsync(nonExistentCode))
            .ReturnsAsync((PhysicalResource?)null);

        var result = await _controller.UpdateTruck(nonExistentCode, updateDto);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateTruck_ReturnsInternalServerError_OnException()
    {
        var existingCode = resources.OfType<Truck>().First().Code.Value;
        var updateDto = new CreateTruckDto
        {
            Code = existingCode,
            Description = "Updated Truck",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 8,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            MaxLoadCapacity = 25,
            AverageSpeed = 70,
            ContainersPerTrip = 3
        };

        _repositoryMock.Setup(repo => repo.GetResourceByCodeAsync(existingCode))
            .ThrowsAsync(new Exception("Database error"));

        var result = await _controller.UpdateTruck(existingCode, updateDto);

        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    [Fact]
    public async Task GetAllPhysicalResources_ReturnsInternalServerError_OnException()
    {
        _repositoryMock.Setup(repo => repo.GetPhysicalResourcesAsync())
            .ThrowsAsync(new System.Exception("Database error"));

        var result = await _controller.GetAll();

        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task GetPhysicalResourceByCode_ReturnsInternalServerError_OnException()
    {
        var testCode = "testcode";
        _repositoryMock.Setup(repo => repo.GetResourceByCodeAsync(testCode))
            .ThrowsAsync(new System.Exception("Database error"));

        var result = await _controller.GetByCode(testCode);

        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task FilterPhysicalResources_ReturnsInternalServerError_OnException()
    {
        var filter = new PhysicalResourceFilter { };
        _repositoryMock.Setup(repo => repo.FilterPhysicalResourcesAsync(filter))
            .ThrowsAsync(new System.Exception("Database error"));

        var result = await _controller.Filter(filter);

        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task CreateSTSCrane_ReturnsInternalServerError_OnException()
    {
        var newResourceDto = new CreateSTSCraneDto
        {
            Code = "PR456",
            Description = "New Physical Resource",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 20,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 30,
            LiftingCapacity = 5000,
            ServingDockCode = "DOCK1"
        };

        _dockRepositoryMock.Setup(repo => repo.GetDockByCodeAsync(newResourceDto.ServingDockCode))
            .ThrowsAsync(new System.Exception("Database error"));

        var result = await _controller.AddSTSCrane(newResourceDto);

        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task CreateYardCrane_ReturnsInternalServerError_OnException()
    {
        var newResourceDto = new CreateYardCraneDto
        {
            Code = "PR789",
            Description = "New Yard Crane",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 15,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 25,
            YardSectionCode = "STORAGE1",
            LiftingCapacity = 4000
        };

        _storageAreaRepositoryMock.Setup(repo => repo.GetStorageAreaByCodeAsync(newResourceDto.YardSectionCode))
            .ThrowsAsync(new System.Exception("Database error"));

        var result = await _controller.AddYardCrane(newResourceDto);

        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task CreateTruck_ReturnsInternalServerError_OnException()
    {
        var newResourceDto = new CreateTruckDto
        {
            Code = "TRUCK2",
            Description = "New Truck",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 10,
            QualificationsCodes = new List<string>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            MaxLoadCapacity = 15,
            AverageSpeed = 60,
            ContainersPerTrip = 2
        };

        _repositoryMock.Setup(repo => repo.AddTruck(It.IsAny<Truck>()))
            .ThrowsAsync(new System.Exception("Database error"));

        var result = await _controller.AddTruck(newResourceDto);

        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }
}
