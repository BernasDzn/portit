using Api.Application.Controllers;
using Api.Application.DataTransfer;
using Api.Application.Services;
using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;
using Api.Infrastructure.Persistence.Repositories;

namespace Tests.Integration.ControllerToService;

public class VesselVisitNotification_CtS_IntegrationTests
{
    private readonly Mock<IVesselVisitNotificationRepository> _notificationRepositoryMock;
    private readonly Mock<IVesselRepository> _vesselRepositoryMock;
    private readonly Mock<IRepresentativeRepository> _representativeRepositoryMock;
    private readonly Mock<IStorageAreaRepository> _storageAreaRepositoryMock;
    private readonly Mock<IContainerRepository> _containerRepositoryMock;
    private readonly Mock<IDockRepository> _dockRepositoryMock;
    private readonly VesselVisitNotificationController _controller;

    public VesselVisitNotification_CtS_IntegrationTests()
    {
        _notificationRepositoryMock = new Mock<IVesselVisitNotificationRepository>();
        _vesselRepositoryMock = new Mock<IVesselRepository>();
        _representativeRepositoryMock = new Mock<IRepresentativeRepository>();
        _storageAreaRepositoryMock = new Mock<IStorageAreaRepository>();
        _containerRepositoryMock = new Mock<IContainerRepository>();

        var loggerService = new Mock<ILogger<VesselVisitNotificationService>>();
        var loggerController = new Mock<ILogger<VesselVisitNotificationController>>();
        var loggerDecisionService = new Mock<ILogger<NotificationDecisionService>>();

        var idGenerator = new Api.Application.Services.VesselVisitNotificationIdGenerator(_notificationRepositoryMock.Object);
        var notificationService = new VesselVisitNotificationService(
            _notificationRepositoryMock.Object,
            _vesselRepositoryMock.Object,
            _representativeRepositoryMock.Object,
            _storageAreaRepositoryMock.Object,
            idGenerator,
            _containerRepositoryMock.Object,
            loggerService.Object
        );

    _dockRepositoryMock = new Mock<IDockRepository>();
    var decisionService = new NotificationDecisionService(_notificationRepositoryMock.Object, _dockRepositoryMock.Object, loggerDecisionService.Object);

        _controller = new VesselVisitNotificationController(notificationService, decisionService, loggerController.Object);
    }

    private (Vessel vessel, Representative representative) BuildVesselAndRepresentative()
    {
        var rep = new Representative(Guid.NewGuid(), 123456789, new Designation { Value = "Rep Name" }, new Email { Value = "rep@example.com" }, new PhoneNumber { Value = "4512345678" });
        var owner = new ShippingAgentOrganization(
            Guid.NewGuid(),
            new Designation { Value = "Owner Org" },
            new List<Designation> { new Designation { Value = "Alt" } },
            new Address("Street 1", "City", "0000", "Country"),
            new TaxNumber { Value = "PT252252252" },
            new HashSet<Representative> { rep }
        );

        var vesselType = new VesselType(Guid.NewGuid(), new Designation { Value = "TypeA" }, new Designation { Value = "Type A Description" }, 10u, 5u, 3u, new PhysicalCharacteristics { Length = 100, Depth = 20, Draft = 10 });
        var vessel = new Vessel(Guid.NewGuid(), new Designation { Value = "Vessel 1" }, new ImoNumber { Value = "IMO1234567" }, vesselType, owner, new PhysicalCharacteristics { Length = 50, Depth = 10, Draft = 5 });

        return (vessel, rep);
    }

    private VesselVisitNotification BuildSampleNotification(Vessel vessel, Representative rep, int seq = 1)
    {
        return new VesselVisitNotification(
            new VesselVisitNotificationId(new Designation { Value = "PORTO" }, (uint)seq, (uint)DateTime.UtcNow.Year),
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(1),
            false,
            vessel,
            rep
        );
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithListOfNotifications()
    {
        var (vessel, rep) = BuildVesselAndRepresentative();
    var sample = BuildSampleNotification(vessel, rep);
    // mark as submitted so decisions can be added (domain requires ApprovalPending)
    sample.Submit();

        _notificationRepositoryMock.Setup(r => r.GetVesselVisitNotificationsAsync())
            .ReturnsAsync(new List<VesselVisitNotification> { sample });

        var result = await _controller.GetAll();
    var response = Assert.IsType<OkObjectResult>(result.Result);
    var value = Assert.IsAssignableFrom<IEnumerable<VesselVisitNotificationDto>>(response.Value);
    Assert.Single(value);
    }

    [Fact]
    public async Task GetAll_ShouldReturnInternalServerError_OnException()
    {
        _notificationRepositoryMock.Setup(r => r.GetVesselVisitNotificationsAsync())
            .ThrowsAsync(new Exception("Test Exception"));

        var result = await _controller.GetAll();

        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task GetDecisions_ReturnsOkWithList()
    {
        var (vessel, rep) = BuildVesselAndRepresentative();
    var sample = BuildSampleNotification(vessel, rep);
    // mark as submitted so decisions can be added (domain requires ApprovalPending)
    sample.Submit();
        var decision = NotificationDecisionFactory.CreateRejected("no", true, DateTime.UtcNow);

        _notificationRepositoryMock.Setup(r => r.GetNotificationDecisionsAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<NotificationDecision> { decision });

        var result = await _controller.GetDecisions(sample.NotificationId.ToString());
        var response = Assert.IsType<OkObjectResult>(result.Result);
        var value = Assert.IsType<List<NotificationDecisionDto>>(response.Value);
    }

    [Fact]
    public async Task GetDecisions_ShouldReturnInternalServerError_OnException()
    {
        _notificationRepositoryMock.Setup(r => r.GetNotificationDecisionsAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Test Exception"));

        var result = await _controller.GetDecisions("ANYID");

        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsCreated_WhenValid()
    {
        var (vessel, rep) = BuildVesselAndRepresentative();

        _notificationRepositoryMock.Setup(r => r.GetVesselVisitNotificationByNotificationIdAsync(It.IsAny<string>()))
            .ReturnsAsync((VesselVisitNotification)null!);

        _notificationRepositoryMock.Setup(r => r.GetVesselVisitNotificationsAsync())
            .ReturnsAsync(new List<VesselVisitNotification>());

        _vesselRepositoryMock.Setup(r => r.GetVesselByIMOAsync(It.IsAny<string>()))
            .ReturnsAsync(vessel);

        _representativeRepositoryMock.Setup(r => r.GetByCitizenIdAsync(It.IsAny<uint>()))
            .ReturnsAsync(rep);

        _notificationRepositoryMock.Setup(r => r.AddAsync(It.IsAny<VesselVisitNotification>()))
            .ReturnsAsync((VesselVisitNotification v) => v);

        var dto = new CreateVesselVisitNotificationDto
        {
            ExpectedArrival = DateTime.UtcNow,
            ExpectedDeparture = DateTime.UtcNow.AddDays(1),
            IsCargoHazardous = false,
            VesselImoNumber = vessel.ImoIdentifier.Value,
            SubmitterId = rep.CitizenshipId
        };

        var result = await _controller.Create(dto);
        var response = Assert.IsType<CreatedAtActionResult>(result.Result);
        var value = Assert.IsType<VesselVisitNotificationDto>(response.Value);
        Assert.Equal(dto.VesselImoNumber, value.Vessel.ImoNumber);
    }

    [Fact]
    public async Task Create_WhenVesselNotFound_ReturnsNotFound()
    {
        var (vessel, rep) = BuildVesselAndRepresentative();

        _notificationRepositoryMock.Setup(r => r.GetVesselVisitNotificationByNotificationIdAsync(It.IsAny<string>()))
            .ReturnsAsync((VesselVisitNotification)null!);

        _vesselRepositoryMock.Setup(r => r.GetVesselByIMOAsync(It.IsAny<string>()))
            .ReturnsAsync((Vessel)null!);

        var dto = new CreateVesselVisitNotificationDto
        {
            ExpectedArrival = DateTime.UtcNow,
            ExpectedDeparture = DateTime.UtcNow.AddDays(1),
            IsCargoHazardous = false,
            VesselImoNumber = "IMO0000000",
            SubmitterId = rep.CitizenshipId
        };

        var result = await _controller.Create(dto);
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_WhenRepresentativeNotFound_ReturnsNotFound()
    {
        var (vessel, rep) = BuildVesselAndRepresentative();

        _notificationRepositoryMock.Setup(r => r.GetVesselVisitNotificationByNotificationIdAsync(It.IsAny<string>()))
            .ReturnsAsync((VesselVisitNotification)null!);

        _vesselRepositoryMock.Setup(r => r.GetVesselByIMOAsync(It.IsAny<string>()))
            .ReturnsAsync(vessel);

        _representativeRepositoryMock.Setup(r => r.GetByCitizenIdAsync(It.IsAny<uint>()))
            .ReturnsAsync((Representative)null!);

        var dto = new CreateVesselVisitNotificationDto
        {
            ExpectedArrival = DateTime.UtcNow,
            ExpectedDeparture = DateTime.UtcNow.AddDays(1),
            IsCargoHazardous = false,
            VesselImoNumber = vessel.ImoIdentifier.Value,
            SubmitterId = rep.CitizenshipId
        };

        var result = await _controller.Create(dto);
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_ShouldReturnInternalServerError_OnRepoException()
    {
        var (vessel, rep) = BuildVesselAndRepresentative();

        _notificationRepositoryMock.Setup(r => r.GetVesselVisitNotificationByNotificationIdAsync(It.IsAny<string>()))
            .ReturnsAsync((VesselVisitNotification)null!);

        _vesselRepositoryMock.Setup(r => r.GetVesselByIMOAsync(It.IsAny<string>()))
            .ReturnsAsync(vessel);

        _representativeRepositoryMock.Setup(r => r.GetByCitizenIdAsync(It.IsAny<uint>()))
            .ReturnsAsync(rep);

        _notificationRepositoryMock.Setup(r => r.AddAsync(It.IsAny<VesselVisitNotification>()))
            .ThrowsAsync(new Exception("Test Exception"));

        var dto = new CreateVesselVisitNotificationDto
        {
            ExpectedArrival = DateTime.UtcNow,
            ExpectedDeparture = DateTime.UtcNow.AddDays(1),
            IsCargoHazardous = false,
            VesselImoNumber = vessel.ImoIdentifier.Value,
            SubmitterId = rep.CitizenshipId
        };

        var result = await _controller.Create(dto);
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task CreateDecision_ReturnsCreated_WhenAccepted()
    {
        var (vessel, rep) = BuildVesselAndRepresentative();
    var sample = BuildSampleNotification(vessel, rep);
    // mark as submitted so decisions can be added (domain requires ApprovalPending)
    sample.Submit();

        _notificationRepositoryMock.Setup(r => r.GetVesselVisitNotificationByNotificationIdAsync(It.IsAny<string>()))
            .ReturnsAsync(sample);

        var dockVt = new VesselType(Guid.NewGuid(), new Designation { Value = "TypeD" }, new Designation { Value = "desc" }, 10u, 5u, 3u, new PhysicalCharacteristics { Length = 100, Depth = 20, Draft = 10 });
        var dock = new Dock(Guid.NewGuid(), new Code { Value = "D1" }, new Designation { Value = "Dock 1" }, new Designation { Value = "Loc 1" }, new PhysicalCharacteristics { Length = 150, Depth = 30, Draft = 15 }, new HashSet<VesselType> { dockVt });
        _dockRepositoryMock.Setup(r => r.GetDockByCodeAsync(It.IsAny<string>()))
            .ReturnsAsync(dock);

        _notificationRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<VesselVisitNotification>()))
            .ReturnsAsync((VesselVisitNotification v) => v);

        var dto = new CreateNotificationDecisionDto
        {
            Status = 1,
            AssignedDockCode = "D1",
            DecisionDate = DateTime.UtcNow,
            IsFinal = true
        };

        var result = await _controller.CreateDecision(sample.NotificationId.ToString(), dto);
    var response = Assert.IsType<CreatedAtActionResult>(result.Result);
    var value = Assert.IsType<NotificationDecisionDto>(response.Value);
    }

    [Fact]
    public async Task CreateDecision_WhenNotificationNotFound_ReturnsInternalServerError()
    {
        _notificationRepositoryMock.Setup(r => r.GetVesselVisitNotificationByNotificationIdAsync(It.IsAny<string>()))
            .ReturnsAsync((VesselVisitNotification)null!);

        var dto = new CreateNotificationDecisionDto
        {
            Status = 2,
            DecisionDate = DateTime.UtcNow,
            IsFinal = false
        };

        var result = await _controller.CreateDecision("ANYID", dto);
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task CreateDecision_WhenMissingAssignedDock_ReturnsBadRequest()
    {
        var (vessel, rep) = BuildVesselAndRepresentative();
        var sample = BuildSampleNotification(vessel, rep);

        _notificationRepositoryMock.Setup(r => r.GetVesselVisitNotificationByNotificationIdAsync(It.IsAny<string>()))
            .ReturnsAsync(sample);

        var dto = new CreateNotificationDecisionDto
        {
            Status = 1,
            AssignedDockCode = null,
            DecisionDate = DateTime.UtcNow,
            IsFinal = true
        };

        var result = await _controller.CreateDecision(sample.NotificationId.ToString(), dto);
        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Update_ReturnsNoContent_WithValidData()
    {
        var (vessel, rep) = BuildVesselAndRepresentative();
        var sample = BuildSampleNotification(vessel, rep);

        _notificationRepositoryMock.Setup(r => r.GetVesselVisitNotificationByNotificationIdAsync(It.IsAny<string>()))
            .ReturnsAsync(sample);

        _notificationRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<VesselVisitNotification>()))
            .ReturnsAsync((VesselVisitNotification v) => v);

        var dto = new CreateVesselVisitNotificationDto
        {
            ExpectedArrival = DateTime.UtcNow,
            ExpectedDeparture = DateTime.UtcNow.AddDays(1),
            IsCargoHazardous = false,
            VesselImoNumber = vessel.ImoIdentifier.Value,
            SubmitterId = rep.CitizenshipId
        };

        var result = await _controller.Update(sample.NotificationId.ToString(), dto);
        Assert.IsType<NoContentResult>(result.Result);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenNotFound()
    {
        _notificationRepositoryMock.Setup(r => r.GetVesselVisitNotificationByNotificationIdAsync(It.IsAny<string>()))
            .ReturnsAsync((VesselVisitNotification)null!);

        var dto = new CreateVesselVisitNotificationDto
        {
            ExpectedArrival = DateTime.UtcNow,
            ExpectedDeparture = DateTime.UtcNow.AddDays(1),
            IsCargoHazardous = false,
            VesselImoNumber = "IMO0000000",
            SubmitterId = 1
        };

    var result = await _controller.Update("ANYID", dto);
    var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_OnArgumentException()
    {
        var (vessel, rep) = BuildVesselAndRepresentative();
        var sample = BuildSampleNotification(vessel, rep);

        _notificationRepositoryMock.Setup(r => r.GetVesselVisitNotificationByNotificationIdAsync(It.IsAny<string>()))
            .ReturnsAsync(sample);

        _notificationRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<VesselVisitNotification>()))
            .ThrowsAsync(new ArgumentException("Bad args"));

        var dto = new CreateVesselVisitNotificationDto
        {
            ExpectedArrival = DateTime.UtcNow,
            ExpectedDeparture = DateTime.UtcNow.AddDays(1),
            IsCargoHazardous = false,
            VesselImoNumber = vessel.ImoIdentifier.Value,
            SubmitterId = rep.CitizenshipId
        };

    var result = await _controller.Update(sample.NotificationId.ToString(), dto);
    var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Filter_ShouldReturnOk_WhenFound()
    {
        var (vessel, rep) = BuildVesselAndRepresentative();
        var sample = BuildSampleNotification(vessel, rep);
        var page = Page<VesselVisitNotification>.Of(new List<VesselVisitNotification> { sample }, new Pageable { PageNumber = 1, PageSize = 10 });

        _notificationRepositoryMock.Setup(r => r.FilterVesselVisitNotificationsAsync(It.IsAny<VesselVisitNotificationFilter>()))
            .ReturnsAsync(page);

    var result = await _controller.Filter(new VesselVisitNotificationFilter { SubmitterCitizeshipId = 0u });
        var response = Assert.IsType<OkObjectResult>(result.Result);
        var value = Assert.IsType<Page<VesselVisitNotificationStatusDto>>(response.Value);
    }

    [Fact]
    public async Task Filter_ShouldReturnInternalServerError_WhenException()
    {
        _notificationRepositoryMock.Setup(r => r.FilterVesselVisitNotificationsAsync(It.IsAny<VesselVisitNotificationFilter>()))
            .ThrowsAsync(new Exception("Test Exception"));

        var result = await _controller.Filter(new VesselVisitNotificationFilter { SubmitterCitizeshipId = 0u });
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task FilterVesselVisitNotifications_ReturnsOkResult_WithFilteredNotifications()
    {
        var filter = new VesselVisitNotificationFilter
        {
            SubmitterCitizeshipId = 908029952
        };

        _notificationRepositoryMock.Setup(repo => repo.FilterVesselVisitNotificationsAsync(filter))
            .ReturnsAsync(new Page<VesselVisitNotification>
            {
                Items = new List<VesselVisitNotification>(),
                PageNumber = 1,
                PageSize = 10
            });

        var result = await _controller.Filter(filter);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<Page<VesselVisitNotificationStatusDto>>(okResult.Value);
        Assert.Empty(returnValue.Items);
    }

    [Fact]
    public async Task FilterVesselVisitNotifications_ReturnsInternalServerError_WhenExceptionThrown()
    {
        var filter = new VesselVisitNotificationFilter
        {
            SubmitterCitizeshipId = 12345
        };

        _notificationRepositoryMock.Setup(repo => repo.FilterVesselVisitNotificationsAsync(filter))
            .ThrowsAsync(new Exception("Database error"));

        var result = await _controller.Filter(filter);

        var objectResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, objectResult.StatusCode);
    }
}