using Api.Application.Controllers;
using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Application.Services;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Unitary.Controller;

public class PhysicalResourceControllerTest
{
    // Mocked qualification service to ensure isolation of controller tests
    private readonly Mock<IPhysicalResourceService> _physicalResourceServiceMock;
    private readonly PhysicalResourceController _controller;

    public PhysicalResourceControllerTest()
    {
        _physicalResourceServiceMock = new Mock<IPhysicalResourceService>();
        _controller = new PhysicalResourceController(_physicalResourceServiceMock.Object, new Mock<ILogger<PhysicalResourceController>>().Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithListOfResources()
    {
        _physicalResourceServiceMock.Setup(service => service.GetPhysicalResources())
            .ReturnsAsync(new List<PhysicalResourceDto>());

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<QualificationDto>>(okResult.Value);
        Assert.Empty(returnValue);
    }

    [Fact]
    public async Task GetByCode_ReturnsOkResult_WithResource()
    {
        _physicalResourceServiceMock.Setup(service => service.GetResourceByCode(It.IsAny<string>()))
            .ReturnsAsync((string code) => new PhysicalResourceDto
            {
                Code = code,
                Description = "Sample Resource",
                Status = Api.Domain.Entities.ResourceStatus.Available,
                SetupTimeInMinutes = 10,
                Qualifications = new List<QualificationDto>(),
                OperationalWindow = OperationalWindow.FullWeek()
            });

        var testCode = "test-code";

        var result = await _controller.GetByCode(testCode);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<PhysicalResourceDto>(okResult.Value);
        Assert.Equal(testCode, returnValue.Code);
    }

    [Fact]
    public async Task GetByCode_ReturnsNotFound_WhenResourceDoesNotExist()
    {
        _physicalResourceServiceMock.Setup(service => service.GetResourceByCode(It.IsAny<string>()))
            .ReturnsAsync((PhysicalResourceDto?)null!);

        var result = await _controller.GetByCode("non-existent-code");

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("No physical resource found with code: non-existent-code", notFoundResult.Value);
    }

    [Fact]
    public async Task Filter_ReturnsOkResult_WithPagedResources()
    {
        _physicalResourceServiceMock.Setup(service => service.FilterPhysicalResources(It.IsAny<PhysicalResourceFilter>()))
        .ReturnsAsync(new Page<object>
        {
            Items = new List<object>(),
            PageNumber = 1,
            PageSize = 10
        });

        var filter = new PhysicalResourceFilter { PageNumber = 1, PageSize = 10 };

        var result = await _controller.Filter(filter);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<Page<object>>(okResult.Value);
        Assert.Empty(returnValue.Items);
    }

    [Fact]
    public async Task AddSTSCrane_ReturnsCreatedAtActionResult_WithCreatedCrane()
    {
        _physicalResourceServiceMock.Setup(service => service.AddSTSCraneAsync(It.IsAny<STSCraneDto>()))
            .ReturnsAsync((STSCraneDto dto) => dto);

        var newCrane = new STSCraneDto
        {
            Code = "new-crane",
            Description = "New STS Crane",
            Status = Api.Domain.Entities.ResourceStatus.Available,
            SetupTimeInMinutes = 15,
            Qualifications = new List<QualificationDto>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 30,
            LiftingCapacity = 50,
            ServingDock = null!
        };

        var result = await _controller.AddSTSCrane(newCrane);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<STSCraneDto>(createdAtActionResult.Value);
        Assert.Equal("new-crane", returnValue.Code);
    }

    [Fact]
    public async Task AddSTSCrane_ReturnsBadRequest_WhenCreationFails()
    {
        _physicalResourceServiceMock.Setup(service => service.AddSTSCraneAsync(It.IsAny<STSCraneDto>()))
            .ReturnsAsync((STSCraneDto?)null!);

        var newCrane = new STSCraneDto
        {
            Code = "new-crane",
            Description = "New STS Crane",
            Status = Api.Domain.Entities.ResourceStatus.Available,
            SetupTimeInMinutes = 15,
            Qualifications = new List<QualificationDto>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 30,
            LiftingCapacity = 50,
            ServingDock = null!
        };

        var result = await _controller.AddSTSCrane(newCrane);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Could not create STS crane.", badRequestResult.Value);
    }

    [Fact]
    public async Task AddYardCrane_ReturnsCreatedAtActionResult_WithCreatedCrane()
    {
        _physicalResourceServiceMock.Setup(service => service.AddYardCraneAsync(It.IsAny<YardCraneDto>()))
            .ReturnsAsync((YardCraneDto dto) => dto);

        var newCrane = new YardCraneDto
        {
            Code = "new-yard-crane",
            Description = "New Yard Crane",
            Status = Api.Domain.Entities.ResourceStatus.Available,
            SetupTimeInMinutes = 15,
            Qualifications = new List<QualificationDto>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 25,
            LiftingCapacity = 40,
            YardSection = null!
        };

        var result = await _controller.AddYardCrane(newCrane);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<YardCraneDto>(createdAtActionResult.Value);
        Assert.Equal("new-yard-crane", returnValue.Code);
    }

    [Fact]
    public async Task AddYardCrane_ReturnsBadRequest_WhenCreationFails()
    {
        _physicalResourceServiceMock.Setup(service => service.AddYardCraneAsync(It.IsAny<YardCraneDto>()))
            .ReturnsAsync((YardCraneDto?)null!);

        var newCrane = new YardCraneDto
        {
            Code = "new-yard-crane",
            Description = "New Yard Crane",
            Status = Api.Domain.Entities.ResourceStatus.Available,
            SetupTimeInMinutes = 15,
            Qualifications = new List<QualificationDto>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 25,
            LiftingCapacity = 40,
            YardSection = null!
        };

        var result = await _controller.AddYardCrane(newCrane);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Could not create yard crane.", badRequestResult.Value);
    }

    [Fact]
    public async Task AddTruck_ReturnsCreatedAtActionResult_WithCreatedTruck()
    {
        _physicalResourceServiceMock.Setup(service => service.AddTruckAsync(It.IsAny<TruckDto>()))
            .ReturnsAsync((TruckDto dto) => dto);

        var newTruck = new TruckDto
        {
            Code = "new-truck",
            Description = "New Truck",
            Status = Api.Domain.Entities.ResourceStatus.Available,
            SetupTimeInMinutes = 10,
            Qualifications = new List<QualificationDto>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerTrip = 2,
            AverageSpeed = 60,
            MaxLoadCapacity = 2000
        };

        var result = await _controller.AddTruck(newTruck);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<TruckDto>(createdAtActionResult.Value);
        Assert.Equal("new-truck", returnValue.Code);
    }

    [Fact]
    public async Task AddTruck_ReturnsBadRequest_WhenCreationFails()
    {
        _physicalResourceServiceMock.Setup(service => service.AddTruckAsync(It.IsAny<TruckDto>()))
            .ReturnsAsync((TruckDto?)null!);

        var newTruck = new TruckDto
        {
            Code = "new-truck",
            Description = "New Truck",
            Status = Api.Domain.Entities.ResourceStatus.Available,
            SetupTimeInMinutes = 10,
            Qualifications = new List<QualificationDto>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerTrip = 2,
            AverageSpeed = 60,
            MaxLoadCapacity = 2000
        };

        var result = await _controller.AddTruck(newTruck);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Could not create truck.", badRequestResult.Value);
    }

    [Fact]
    public async Task UpdateSTSCrane_ReturnsOkResult_WithUpdatedCrane()
    {
        _physicalResourceServiceMock.Setup(service => service.UpdateSTSCraneAsync(It.IsAny<string>(), It.IsAny<STSCraneDto>()))
            .ReturnsAsync((string code, STSCraneDto dto) => dto);

        var updatedCrane = new STSCraneDto
        {
            Code = "existing-crane",
            Description = "Updated STS Crane",
            Status = Api.Domain.Entities.ResourceStatus.Available,
            SetupTimeInMinutes = 20,
            Qualifications = new List<QualificationDto>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 35,
            LiftingCapacity = 55,
            ServingDock = null!
        };

        var result = await _controller.UpdateSTSCrane("existing-crane", updatedCrane);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<STSCraneDto>(okResult.Value);
        Assert.Equal("existing-crane", returnValue.Code);
    }

    [Fact]
    public async Task UpdateSTSCrane_ReturnsNotFound_WhenUpdateFails()
    {
        _physicalResourceServiceMock.Setup(service => service.UpdateSTSCraneAsync(It.IsAny<string>(), It.IsAny<STSCraneDto>()))
            .ReturnsAsync((STSCraneDto?)null!);

        var updatedCrane = new STSCraneDto
        {
            Code = "existing-crane",
            Description = "Updated STS Crane",
            Status = Api.Domain.Entities.ResourceStatus.Available,
            SetupTimeInMinutes = 20,
            Qualifications = new List<QualificationDto>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 35,
            LiftingCapacity = 55,
            ServingDock = null!
        };

        var result = await _controller.UpdateSTSCrane("existing-crane", updatedCrane);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("No STS crane found with code: existing-crane", notFoundResult.Value);
    }

    [Fact]
    public async Task UpdateYardCrane_ReturnsOkResult_WithUpdatedCrane()
    {
        _physicalResourceServiceMock.Setup(service => service.UpdateYardCraneAsync(It.IsAny<string>(), It.IsAny<YardCraneDto>()))
            .ReturnsAsync((string code, YardCraneDto dto) => dto);

        var updatedCrane = new YardCraneDto
        {
            Code = "existing-yard-crane",
            Description = "Updated Yard Crane",
            Status = Api.Domain.Entities.ResourceStatus.Available,
            SetupTimeInMinutes = 20,
            Qualifications = new List<QualificationDto>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 28,
            LiftingCapacity = 45,
            YardSection = null!
        };

        var result = await _controller.UpdateYardCrane("existing-yard-crane", updatedCrane);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<YardCraneDto>(okResult.Value);
        Assert.Equal("existing-yard-crane", returnValue.Code);
    }

    [Fact]
    public async Task UpdateYardCrane_ReturnsNotFound_WhenUpdateFails()
    {
        _physicalResourceServiceMock.Setup(service => service.UpdateYardCraneAsync(It.IsAny<string>(), It.IsAny<YardCraneDto>()))
            .ReturnsAsync((YardCraneDto?)null!);

        var updatedCrane = new YardCraneDto
        {
            Code = "existing-yard-crane",
            Description = "Updated Yard Crane",
            Status = Api.Domain.Entities.ResourceStatus.Available,
            SetupTimeInMinutes = 20,
            Qualifications = new List<QualificationDto>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerHour = 28,
            LiftingCapacity = 45,
            YardSection = null!
        };

        var result = await _controller.UpdateYardCrane("existing-yard-crane", updatedCrane);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("No yard crane found with code: existing-yard-crane", notFoundResult.Value);
    }

    [Fact]
    public async Task UpdateTruck_ReturnsOkResult_WithUpdatedTruck()
    {
        _physicalResourceServiceMock.Setup(service => service.UpdateTruckAsync(It.IsAny<string>(), It.IsAny<TruckDto>()))
            .ReturnsAsync((string code, TruckDto dto) => dto);

        var updatedTruck = new TruckDto
        {
            Code = "existing-truck",
            Description = "Updated Truck",
            Status = Api.Domain.Entities.ResourceStatus.Available,
            SetupTimeInMinutes = 12,
            Qualifications = new List<QualificationDto>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerTrip = 3,
            AverageSpeed = 65,
            MaxLoadCapacity = 2500
        };

        var result = await _controller.UpdateTruck("existing-truck", updatedTruck);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<TruckDto>(okResult.Value);
        Assert.Equal("existing-truck", returnValue.Code);
    }

    [Fact]
    public async Task UpdateTruck_ReturnsNotFound_WhenUpdateFails()
    {
        _physicalResourceServiceMock.Setup(service => service.UpdateTruckAsync(It.IsAny<string>(), It.IsAny<TruckDto>()))
            .ReturnsAsync((TruckDto?)null!);

        var updatedTruck = new TruckDto
        {
            Code = "existing-truck",
            Description = "Updated Truck",
            Status = Api.Domain.Entities.ResourceStatus.Available,
            SetupTimeInMinutes = 12,
            Qualifications = new List<QualificationDto>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            ContainersPerTrip = 3,
            AverageSpeed = 65,
            MaxLoadCapacity = 2500
        };

        var result = await _controller.UpdateTruck("existing-truck", updatedTruck);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("No truck found with code: existing-truck", notFoundResult.Value);
    }

    [Fact]
    public async Task DeactivateResource_ReturnsOkResult_WhenDeactivationSucceeds()
    {
        _physicalResourceServiceMock.Setup(service => service.DeactivateResource(It.IsAny<string>()))
            .ReturnsAsync(true);

        var result = await _controller.Deactivate("existing-code");

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Resource with code existing-code has been deactivated.", okResult.Value);
    }

    [Fact]
    public async Task DeactivateResource_ReturnsNotFound_WhenDeactivationFails()
    {
        _physicalResourceServiceMock.Setup(service => service.DeactivateResource(It.IsAny<string>()))
            .ReturnsAsync(false);

        var result = await _controller.Deactivate("non-existent-code");

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("No resource found with code: non-existent-code", notFoundResult.Value);
    }
}