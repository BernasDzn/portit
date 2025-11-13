using Api.Application.Controllers;
using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Application.Exceptions;
using Api.Application.Services;
using Api.Domain.Entities;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Exceptions;
using Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

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
        
        var claims = new List<Claim>
        {
            new Claim("email_address", "psharply0@yolasite.com"),
            new Claim("id", "test-user-id"),
            new Claim("name", "Test User"),
            new Claim("user_role", "Administrator")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
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
    public async Task GetAll_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        _notificationServiceMock.Setup(service => service.GetVesselVisitNotifications())
            .ThrowsAsync(new System.Exception());

        var result = await _controller.GetAll();

        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
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
    public async Task GetDecisions_ReturnsNotFound_WhenNotificationNotFound()
    {
        _notificationDecisionServiceMock.Setup(r => r.GetNotificationDecisions(It.IsAny<string>()))
            .ThrowsAsync(new EntityNotFoundException());

        var result = await _controller.GetDecisions("NON_EXISTENT_ID");
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetDecisions_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        _notificationDecisionServiceMock.Setup(service => service.GetNotificationDecisions(It.IsAny<string>()))
            .ThrowsAsync(new System.Exception());

        var result = await _controller.GetDecisions("test-id");

        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtActionResult_WithCreatedVesselVisitNotification()
    {
        var newNotification = new CreateVesselVisitNotificationDto
        {
            NotificationId = null!,
            ExpectedArrival = DateTime.UtcNow,
            ExpectedDeparture = DateTime.UtcNow.AddDays(1),
            IsCargoHazardous = false,
            VesselImoNumber = null!
        };
        var expectedNotification = new VesselVisitNotificationDto
        {
            NotificationId = null!,
            Status = NotificationStatus.InProgress,
            ExpectedArrival = DateTime.UtcNow,
            ExpectedDeparture = DateTime.UtcNow.AddDays(1),
            IsCargoHazardous = false,
            Vessel = null!,
            Submitter = null!
        };

        _notificationServiceMock.Setup(service => service.Add(It.IsAny<CreateVesselVisitNotificationDto>(), "psharply0@yolasite.com"))
            .ReturnsAsync(expectedNotification);

        var result = await _controller.Create(newNotification);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<VesselVisitNotificationDto>(createdAtActionResult.Value);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_OnArgumentException()
    {
        var newNotification = new CreateVesselVisitNotificationDto
        {
            NotificationId = null!,
            ExpectedArrival = DateTime.UtcNow,
            ExpectedDeparture = DateTime.UtcNow.AddDays(1),
            IsCargoHazardous = false,
            VesselImoNumber = null!
        };

        _notificationServiceMock.Setup(service => service.Add(It.IsAny<CreateVesselVisitNotificationDto>(), "psharply0@yolasite.com"))
            .ThrowsAsync(new ArgumentException());

        var result = await _controller.Create(newNotification);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsNotFoundResult_OnEntityNotFoundException()
    {
        var newNotification = new CreateVesselVisitNotificationDto
        {
            NotificationId = null!,
            ExpectedArrival = DateTime.UtcNow,
            ExpectedDeparture = DateTime.UtcNow.AddDays(1),
            IsCargoHazardous = false,
            VesselImoNumber = null!
        };

        _notificationServiceMock.Setup(service => service.Add(It.IsAny<CreateVesselVisitNotificationDto>(), "psharply0@yolasite.com"))
            .ThrowsAsync(new EntityNotFoundException());

        var result = await _controller.Create(newNotification);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsConflictResult_OnEntityAlreadyExistsException()
    {
        var newNotification = new CreateVesselVisitNotificationDto
        {
            NotificationId = null!,
            ExpectedArrival = DateTime.UtcNow,
            ExpectedDeparture = DateTime.UtcNow.AddDays(1),
            IsCargoHazardous = false,
            VesselImoNumber = null!
        };

        _notificationServiceMock.Setup(service => service.Add(It.IsAny<CreateVesselVisitNotificationDto>(), "psharply0@yolasite.com"))
            .ThrowsAsync(new EntityAlreadyExistsException());

        var result = await _controller.Create(newNotification);

        var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        var newNotification = new CreateVesselVisitNotificationDto
        {
            NotificationId = null!,
            ExpectedArrival = DateTime.UtcNow,
            ExpectedDeparture = DateTime.UtcNow.AddDays(1),
            IsCargoHazardous = false,
            VesselImoNumber = null!
        };

        _notificationServiceMock.Setup(service => service.Add(It.IsAny<CreateVesselVisitNotificationDto>(), "psharply0@yolasite.com"))
            .ThrowsAsync(new System.Exception());

        var result = await _controller.Create(newNotification);

        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task CreateDecision_ReturnsCreatedAtActionResult_WithCreatedNotificationDecision()
    {
        var newDecision = new CreateNotificationDecisionDto
        {
            Status = 1,
            DecisionDate = DateTime.UtcNow,
            IsFinal = false
        };
        var expectedDecision = new NotificationDecisionDto
        {
            Status = 1,
            DecisionDate = DateTime.UtcNow,
            IsFinal = false
        };

        _notificationDecisionServiceMock.Setup(service => service.Add(It.IsAny<CreateNotificationDecisionDto>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(expectedDecision);

        var result = await _controller.CreateDecision("test-id", newDecision);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<NotificationDecisionDto>(createdAtActionResult.Value);
    }

    [Fact]
    public async Task CreateDecision_ReturnsBadRequest_OnArgumentException()
    {
        var newDecision = new CreateNotificationDecisionDto
        {
            Status = 1,
            DecisionDate = DateTime.UtcNow,
            IsFinal = false
        };

        _notificationDecisionServiceMock.Setup(service => service.Add(It.IsAny<CreateNotificationDecisionDto>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new ArgumentException());

        var result = await _controller.CreateDecision("test-id", newDecision);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateDecision_ReturnsNotFoundResult_OnEntityNotFoundException()
    {
        var newDecision = new CreateNotificationDecisionDto
        {
            Status = 1,
            DecisionDate = DateTime.UtcNow,
            IsFinal = false
        };

        _notificationDecisionServiceMock.Setup(service => service.Add(It.IsAny<CreateNotificationDecisionDto>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new EntityNotFoundException());

        var result = await _controller.CreateDecision("test-id", newDecision);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateDecision_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        var newDecision = new CreateNotificationDecisionDto
        {
            Status = 1,
            DecisionDate = DateTime.UtcNow,
            IsFinal = false
        };

        _notificationDecisionServiceMock.Setup(service => service.Add(It.IsAny<CreateNotificationDecisionDto>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new System.Exception());

        var result = await _controller.CreateDecision("test-id", newDecision);

        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task Update_ReturnsNoContentResult_WithUpdatedVesselVisitNotification()
    {
        var updatedNotification = new CreateVesselVisitNotificationDto
        {
            NotificationId = null!,
            ExpectedArrival = DateTime.UtcNow,
            ExpectedDeparture = DateTime.UtcNow.AddDays(1),
            IsCargoHazardous = false,
            VesselImoNumber = null!
        };
        var expectedNotification = new VesselVisitNotificationDto
        {
            NotificationId = null!,
            Status = NotificationStatus.InProgress,
            ExpectedArrival = DateTime.UtcNow,
            ExpectedDeparture = DateTime.UtcNow.AddDays(1),
            IsCargoHazardous = false,
            Vessel = null!,
            Submitter = null!
        };

        _notificationServiceMock.Setup(service => service.Update(It.IsAny<string>(), It.IsAny<CreateVesselVisitNotificationDto>(), "psharply0@yolasite.com"))
            .ReturnsAsync(expectedNotification);

        var result = await _controller.Update("test-id", updatedNotification);
        var noContentResult = Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_OnArgumentException()
    {
        var updatedNotification = new CreateVesselVisitNotificationDto
        {
            NotificationId = null!,
            ExpectedArrival = DateTime.UtcNow,
            ExpectedDeparture = DateTime.UtcNow.AddDays(1),
            IsCargoHazardous = false,
            VesselImoNumber = null!
        };

        _notificationServiceMock.Setup(service => service.Update(It.IsAny<string>(), It.IsAny<CreateVesselVisitNotificationDto>(), "psharply0@yolasite.com"))
            .ThrowsAsync(new ArgumentException());

        var result = await _controller.Update("test-id", updatedNotification);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsNotFoundResult_OnEntityNotFoundException()
    {
        var updatedNotification = new CreateVesselVisitNotificationDto
        {
            NotificationId = null!,
            ExpectedArrival = DateTime.UtcNow,
            ExpectedDeparture = DateTime.UtcNow.AddDays(1),
            IsCargoHazardous = false,
            VesselImoNumber = null!
        };

        _notificationServiceMock.Setup(service => service.Update(It.IsAny<string>(), It.IsAny<CreateVesselVisitNotificationDto>(), "psharply0@yolasite.com"))
            .ThrowsAsync(new EntityNotFoundException());

        var result = await _controller.Update("test-id", updatedNotification);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        var updatedNotification = new CreateVesselVisitNotificationDto
        {
            NotificationId = null!,
            ExpectedArrival = DateTime.UtcNow,
            ExpectedDeparture = DateTime.UtcNow.AddDays(1),
            IsCargoHazardous = false,
            VesselImoNumber = null!
        };

        _notificationServiceMock.Setup(service => service.Update(It.IsAny<string>(), It.IsAny<CreateVesselVisitNotificationDto>(), "psharply0@yolasite.com"))
            .ThrowsAsync(new System.Exception());

        var result = await _controller.Update("test-id", updatedNotification);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task Filter_ReturnsOkResult_WithPageOfVesselVisitNotificationStatusDto()
    {
        var filter = new VesselVisitNotificationFilter{};

        _notificationServiceMock.Setup(service => service.FilterNotifications(It.IsAny<VesselVisitNotificationFilter>(), "psharply0@yolasite.com"))
            .ReturnsAsync(new Page<VesselVisitNotificationStatusDto>
            {
                Items = new List<VesselVisitNotificationStatusDto>(),
                PageNumber = 1,
                PageSize = 10,
            });

        var result = await _controller.Filter(filter);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<Page<VesselVisitNotificationStatusDto>>(okResult.Value);
        Assert.Empty(returnValue.Items);
    }

    [Fact]
    public async Task Filter_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        var filter = new VesselVisitNotificationFilter{};

        _notificationServiceMock.Setup(service => service.FilterNotifications(It.IsAny<VesselVisitNotificationFilter>(), "psharply0@yolasite.com"))
            .ThrowsAsync(new System.Exception());

        var result = await _controller.Filter(filter);

        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }
}