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

public class VesselVisitNotificationControllerTest
{
    private readonly Mock<IVesselVisitNotificationService> _notificationServiceMock;
    private readonly Mock<INotificationDecisionService> _notificationDecisionServiceMock;
    private readonly VesselVisitNotificationController _controller;

    public VesselVisitNotificationControllerTest()
    {
        _notificationServiceMock = new Mock<IVesselVisitNotificationService>();
        _notificationDecisionServiceMock = new Mock<INotificationDecisionService>();
        _controller = new VesselVisitNotificationController(_notificationServiceMock.Object, _notificationDecisionServiceMock.Object, new Mock<ILogger<VesselVisitNotificationController>>().Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithListOfVesselVisitNotifications()
    {
        _notificationServiceMock.Setup(service => service.GetVesselVisitNotifications())
            .ReturnsAsync(new List<VesselVisitNotificationDto>());

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<VesselVisitNotificationDto>>(okResult.Value);
        Assert.Empty(returnValue);
    }

    [Fact]
    public async Task GetDecisions_ReturnsOkResult_WithListOfNotificationDecisions()
    {
        _notificationDecisionServiceMock.Setup(service => service.GetNotificationDecisions(It.IsAny<string>()))
            .ReturnsAsync(new List<NotificationDecisionDto>());

        var result = await _controller.GetDecisions("test-id");

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<NotificationDecisionDto>>(okResult.Value);
        Assert.Empty(returnValue);
    }

    [Fact]
    public async Task GetDecisions_ReturnsBadRequest_WhenExceptionIsThrown()
    {
        _notificationDecisionServiceMock.Setup(service => service.GetNotificationDecisions(It.IsAny<string>()))
            .ThrowsAsync(new System.Exception());

        var result = await _controller.GetDecisions("test-id");

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtActionResult_WithCreatedVesselVisitNotification()
    {
        var newNotification = new VesselVisitNotificationDto
        {
            NotificationId = null!,
            ExpectedArrival = DateTime.UtcNow,
            ExpectedDeparture = DateTime.UtcNow.AddDays(1),
            IsCargoHazardous = false,
            Vessel = null!,
            Submitter = null!
        };

        _notificationServiceMock.Setup(service => service.Add(It.IsAny<VesselVisitNotificationDto>()))
            .ReturnsAsync(newNotification);

        var result = await _controller.Create(newNotification);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<VesselVisitNotificationDto>(createdAtActionResult.Value);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenCreationFails()
    {
        var newNotification = new VesselVisitNotificationDto
        {
            NotificationId = null!,
            ExpectedArrival = DateTime.UtcNow,
            ExpectedDeparture = DateTime.UtcNow.AddDays(1),
            IsCargoHazardous = false,
            Vessel = null!,
            Submitter = null!
        };

        _notificationServiceMock.Setup(service => service.Add(It.IsAny<VesselVisitNotificationDto>()))
            .ReturnsAsync((VesselVisitNotificationDto?)null);

        var result = await _controller.Create(newNotification);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenExceptionIsThrown()
    {
        var newNotification = new VesselVisitNotificationDto
        {
            NotificationId = null!,
            ExpectedArrival = DateTime.UtcNow,
            ExpectedDeparture = DateTime.UtcNow.AddDays(1),
            IsCargoHazardous = false,
            Vessel = null!,
            Submitter = null!
        };

        _notificationServiceMock.Setup(service => service.Add(It.IsAny<VesselVisitNotificationDto>()))
            .ThrowsAsync(new System.Exception());

        var result = await _controller.Create(newNotification);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateDecision_ReturnsCreatedAtActionResult_WithCreatedNotificationDecision()
    {
        var newDecision = new NotificationDecisionDto
        {
            Status = 1,
            DecisionDate = DateTime.UtcNow,
            IsFinal = false
        };

        _notificationDecisionServiceMock.Setup(service => service.Add(It.IsAny<NotificationDecisionDto>(), It.IsAny<string>()))
            .ReturnsAsync(newDecision);

        var result = await _controller.CreateDecision("test-id", newDecision);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<NotificationDecisionDto>(createdAtActionResult.Value);
    }

    [Fact]
    public async Task CreateDecision_ReturnsBadRequest_WhenCreationFails()
    {
        var newDecision = new NotificationDecisionDto
        {
            Status = 1,
            DecisionDate = DateTime.UtcNow,
            IsFinal = false
        };

        _notificationDecisionServiceMock.Setup(service => service.Add(It.IsAny<NotificationDecisionDto>(), It.IsAny<string>()))
            .ReturnsAsync((NotificationDecisionDto)null!);

        var result = await _controller.CreateDecision("test-id", newDecision);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateDecision_ReturnsBadRequest_WhenExceptionIsThrown()
    {
        var newDecision = new NotificationDecisionDto
        {
            Status = 1,
            DecisionDate = DateTime.UtcNow,
            IsFinal = false
        };

        _notificationDecisionServiceMock.Setup(service => service.Add(It.IsAny<NotificationDecisionDto>(), It.IsAny<string>()))
            .ThrowsAsync(new System.Exception());

        var result = await _controller.CreateDecision("test-id", newDecision);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Update_ReturnsOkResult_WithUpdatedVesselVisitNotification()
    {
        var updatedNotification = new VesselVisitNotificationDto
        {
            NotificationId = "test-id",
            ExpectedArrival = DateTime.UtcNow,
            ExpectedDeparture = DateTime.UtcNow.AddDays(1),
            IsCargoHazardous = false,
            Vessel = null!,
            Submitter = null!
        };

        _notificationServiceMock.Setup(service => service.Update(It.IsAny<string>(), It.IsAny<VesselVisitNotificationDto>()))
            .ReturnsAsync(updatedNotification);

        var result = await _controller.Update("test-id", updatedNotification);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<VesselVisitNotificationDto>(okResult.Value);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenUpdateFails()
    {
        var updatedNotification = new VesselVisitNotificationDto
        {
            NotificationId = "test-id",
            ExpectedArrival = DateTime.UtcNow,
            ExpectedDeparture = DateTime.UtcNow.AddDays(1),
            IsCargoHazardous = false,
            Vessel = null!,
            Submitter = null!
        };

        _notificationServiceMock.Setup(service => service.Update(It.IsAny<string>(), It.IsAny<VesselVisitNotificationDto>()))
            .ReturnsAsync((VesselVisitNotificationDto?)null);

        var result = await _controller.Update("test-id", updatedNotification);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenExceptionIsThrown()
    {
        var updatedNotification = new VesselVisitNotificationDto
        {
            NotificationId = "test-id",
            ExpectedArrival = DateTime.UtcNow,
            ExpectedDeparture = DateTime.UtcNow.AddDays(1),
            IsCargoHazardous = false,
            Vessel = null!,
            Submitter = null!
        };

        _notificationServiceMock.Setup(service => service.Update(It.IsAny<string>(), It.IsAny<VesselVisitNotificationDto>()))
            .ThrowsAsync(new System.Exception());

        var result = await _controller.Update("test-id", updatedNotification);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}