using Api.Application.Controllers;
using Api.Application.Services;
using Api.Application.DataTransfer;
using Moq;
using Api.Domain.IRepository;
using Microsoft.Extensions.Logging;
using Api.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Api.Domain.ValueObjects;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;

namespace Tests.Integration.ControllerToService;

public class VesselVisitNotification_CtS_IntegrationTest
{
    private readonly VesselVisitNotificationController _controller;
    private readonly VesselVisitNotificationService _service;
    private readonly Mock<IVesselVisitNotificationRepository> _repositoryMock;
    private readonly Mock<IVesselRepository> _vesselRepositoryMock = new Mock<IVesselRepository>();
    private readonly Mock<IRepresentativeRepository> _representativeRepositoryMock = new Mock<IRepresentativeRepository>();
    private readonly Mock<IStorageAreaRepository> _storageAreaRepositoryMock = new Mock<IStorageAreaRepository>();
    private readonly Mock<IContainerRepository> _containerRepositoryMock = new Mock<IContainerRepository>();
    private readonly VesselVisitNotificationIdGenerator _idGenerator;
    private readonly Mock<INotificationDecisionService> _notificationDecisionServiceMock = new Mock<INotificationDecisionService>();


    private static Representative representative = new Representative(
        Guid.NewGuid(),
        908029952,
        new Designation { Value = "Patricio Sharply" },
        new Email { Value = "patricio.sharply@globalshipping.com" },
        new PhoneNumber { Value = "6947302134" }
    );

    private static Vessel vessel = new Vessel(
            Guid.NewGuid(),
            new Designation { Value = "Ever Given" },
            new ImoNumber { Value = "IMO 7585229" },
            new VesselType(
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
                }),
                new ShippingAgentOrganization(
                    Guid.NewGuid(),
                    new Designation { Value = "Global Shipping Co." },
                    new List<Designation> {
                        new Designation { Value = "GSC" },
                        new Designation { Value = "Global Ship" }
                    },
                    new Address("123 Ocean Drive", "Maritime City", "90210", "USA"),
                    new TaxNumber { Value = "PT123456789" },
                    new HashSet<Representative> { representative }
                ),
                new PhysicalCharacteristics
                {
                    Length = 270,
                    Depth = 13,
                    Draft = 10
                }
            );

    public VesselVisitNotification_CtS_IntegrationTest()
    {
        _repositoryMock = new Mock<IVesselVisitNotificationRepository>();

        // id generator requires repository
        _idGenerator = new VesselVisitNotificationIdGenerator(_repositoryMock.Object);

        _service = new VesselVisitNotificationService(
            _repositoryMock.Object,
            _vesselRepositoryMock.Object,
            _representativeRepositoryMock.Object,
            _storageAreaRepositoryMock.Object,
            _idGenerator,
            _containerRepositoryMock.Object,
            new Mock<ILogger<VesselVisitNotificationService>>().Object);

        _controller = new VesselVisitNotificationController(
            _service,
            _notificationDecisionServiceMock.Object,
            new Mock<ILogger<VesselVisitNotificationController>>().Object);

    }

    [Fact]
    public async Task GetAllVesselVisitNotifications_ReturnsOkResult_WithListOfNotifications()
    {
        _repositoryMock.Setup(repo => repo.GetVesselVisitNotificationsAsync())
            .ReturnsAsync(new List<VesselVisitNotification>());

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<VesselVisitNotificationDto>>(okResult.Value);

        Assert.Empty(returnValue);
    }

    [Fact]
    public async Task GetVesselVisitNotificationByNotificationId_ReturnsOkResult_WithNotification()
    {

        var testId = "2025-PORTO-000006";

        _repositoryMock.Setup(repo => repo.GetVesselVisitNotificationByNotificationIdAsync(testId))
            .ReturnsAsync(new VesselVisitNotification(
                new VesselVisitNotificationId(new Designation { Value = "PORTO" }, 6, 2025),
                DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow.AddDays(5),
                false,
                vessel,
                representative,
                "Requires additional security measures",
                new Crew(new Designation { Value = "Miguel Oliveira" }, 2, new HashSet<SafetyOfficer>()),
                null,
                null
            ));

        var result = await _controller.GetById(testId);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<VesselVisitNotificationDto>(okResult.Value);
        Assert.Equal(testId, returnValue.NotificationId);
    }

    [Fact]
    public async Task GetVesselVisitNotificationByNotificationId_ReturnsNotFound_WhenNotificationDoesNotExist()
    {
        var testId = "2025_PORTO_999999";

        _repositoryMock.Setup(repo => repo.GetVesselVisitNotificationByNotificationIdAsync(testId))!
            .ReturnsAsync((VesselVisitNotification?)null);

        var result = await _controller.GetById(testId);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetVesselVisitNotificationByNotificationId_ReturnsInternalServerError_WhenExceptionOccurs()
    {
        var testId = "errorid";

        _repositoryMock.Setup(repo => repo.GetVesselVisitNotificationByNotificationIdAsync(testId))
            .ThrowsAsync(new System.Exception("Database error"));

        var result = await _controller.GetById(testId);

        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task CreateVesselVisitNotification_ReturnsCreatedAtActionResult_WithCreatedNotification()
    {
        var newNotificationDto = new CreateVesselVisitNotificationDto
        {
            NotificationId = "2025-PORTO-000001",
            ExpectedArrival = DateTime.UtcNow.AddDays(2),
            ExpectedDeparture = DateTime.UtcNow.AddDays(6),
            IsCargoHazardous = false,
            SpecialRequirements = null,
            CrewDetails = null,
            LoadCargoManifest = null,
            UnloadCargoManifest = null,
            VesselImoNumber = "IMO 7585229",
            SubmitterId = 908029952,
        };


        _vesselRepositoryMock.Setup(repo => repo.GetVesselByIMOAsync(newNotificationDto.VesselImoNumber))
            .ReturnsAsync(vessel);

        _representativeRepositoryMock.Setup(repo => repo.GetByCitizenIdAsync(newNotificationDto.SubmitterId))
            .ReturnsAsync(representative);

        _repositoryMock.Setup(repo => repo.AddAsync(It.IsAny<VesselVisitNotification>()))
            .ReturnsAsync((VesselVisitNotification v) => v);

        var result = await _controller.Create(newNotificationDto);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<VesselVisitNotificationDto>(createdAtActionResult.Value);
        Assert.Equal(newNotificationDto.NotificationId, returnValue.NotificationId);
    }


    [Fact]
    public async Task CreateVesselVisitNotification_ReturnsConflict_OnDuplicateCode()
    {
        var newNotificationDto = new CreateVesselVisitNotificationDto
        {
            NotificationId = "2025_PORTO_000001",
            ExpectedArrival = DateTime.UtcNow.AddDays(2),
            ExpectedDeparture = DateTime.UtcNow.AddDays(6),
            IsCargoHazardous = false,
            SpecialRequirements = null,
            CrewDetails = null,
            LoadCargoManifest = null,
            UnloadCargoManifest = null,
            VesselImoNumber = "IMO 7585229",
            SubmitterId = 908029952,
        };

        _repositoryMock.Setup(repo => repo.GetVesselVisitNotificationByNotificationIdAsync(newNotificationDto.NotificationId))
            .ReturnsAsync(new VesselVisitNotification(
                new VesselVisitNotificationId(new Designation { Value = "PORTO" }, 1, 2025),
                DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow.AddDays(5),
                false,
                vessel,
                representative
            ));


        var result = await _controller.Create(newNotificationDto);

        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateVesselVisitNotification_ReturnsNotFound_WhenVesselDoesNotExist()
    {
        var newNotificationDto = new CreateVesselVisitNotificationDto
        {
            NotificationId = "2025_PORTO_000008",
            ExpectedArrival = DateTime.UtcNow.AddDays(2),
            ExpectedDeparture = DateTime.UtcNow.AddDays(6),
            IsCargoHazardous = false,
            SpecialRequirements = null,
            CrewDetails = null,
            LoadCargoManifest = null,
            UnloadCargoManifest = null,
            VesselImoNumber = "IMO 00000000",
            SubmitterId = 908029952,
        };

        _vesselRepositoryMock.Setup(repo => repo.GetVesselByIMOAsync("IMO 00000000"))
            .ReturnsAsync((Vessel?)null!);

        var result = await _controller.Create(newNotificationDto);

        var badRequestResult = Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateVesselVisitNotification_ReturnsNotFound_WhenRepresentativeDoesNotExist()
    {
        var newNotificationDto = new CreateVesselVisitNotificationDto
        {
            NotificationId = "2025_PORTO_000008",
            ExpectedArrival = DateTime.UtcNow.AddDays(2),
            ExpectedDeparture = DateTime.UtcNow.AddDays(6),
            IsCargoHazardous = false,
            SpecialRequirements = null,
            CrewDetails = null,
            LoadCargoManifest = null,
            UnloadCargoManifest = null,
            VesselImoNumber = "IMO 7585229",
            SubmitterId = 100000000,
        };

        _representativeRepositoryMock.Setup(repo => repo.GetByCitizenIdAsync(100000000))
            .ReturnsAsync((Representative?)null!);

        var result = await _controller.Create(newNotificationDto);

        var badRequestResult = Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateVesselVisitNotification_ReturnsInternalServerError_OnException()
    {
        var newVesselVisitNotificationDto = new CreateVesselVisitNotificationDto
        {
            NotificationId = "2025_PORTO_000009",
            ExpectedArrival = DateTime.UtcNow.AddDays(2),
            ExpectedDeparture = DateTime.UtcNow.AddDays(6),
            IsCargoHazardous = false,
            SpecialRequirements = null,
            CrewDetails = null,
            LoadCargoManifest = null,
            UnloadCargoManifest = null,
            VesselImoNumber = "IMO 7585229",
            SubmitterId = 908029952,
        };

        _repositoryMock.Setup(repo => repo.GetVesselVisitNotificationByNotificationIdAsync(newVesselVisitNotificationDto.NotificationId))
            .ThrowsAsync(new System.Exception("Database error"));

        var result = await _controller.Create(newVesselVisitNotificationDto);

        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task UpdateVesselVisitNotification_ReturnsNoContentResult_WithUpdatedNotification()
    {
        var notificationIdToUpdate = "2025-PORTO-000001";
        var updateVesselVisitNotificationDto = new CreateVesselVisitNotificationDto
        {
            NotificationId = notificationIdToUpdate,
            ExpectedArrival = DateTime.UtcNow.AddDays(2),
            ExpectedDeparture = DateTime.UtcNow.AddDays(6),
            IsCargoHazardous = false,
            SpecialRequirements = null,
            CrewDetails = null,
            LoadCargoManifest = null,
            UnloadCargoManifest = null,
            VesselImoNumber = "IMO 7585229",
            SubmitterId = 908029952,
        };



        _vesselRepositoryMock.Setup(repo => repo.GetVesselByIMOAsync(updateVesselVisitNotificationDto.VesselImoNumber))
            .ReturnsAsync(vessel);

        _representativeRepositoryMock.Setup(repo => repo.GetByCitizenIdAsync(updateVesselVisitNotificationDto.SubmitterId))
            .ReturnsAsync(representative);

        _repositoryMock.Setup(repo => repo.GetVesselVisitNotificationByNotificationIdAsync(notificationIdToUpdate))
            .ReturnsAsync(new VesselVisitNotification
            (
                new VesselVisitNotificationId(new Designation { Value = "PORTO" }, 9, 2025),
                DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow.AddDays(5),
                false,
                vessel,
                representative
            ));

        _repositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<VesselVisitNotification>()))
            .ReturnsAsync((VesselVisitNotification v) => v);

        var result = await _controller.Update(notificationIdToUpdate, updateVesselVisitNotificationDto);

        Assert.IsType<NoContentResult>(result.Result);
    }

    [Fact]
    public async Task UpdateVesselVisitNotification_ReturnsNotFound_WhenNotificationDoesNotExist()
    {

        var dto = new CreateVesselVisitNotificationDto
        {
            ExpectedArrival = DateTime.UtcNow,
            ExpectedDeparture = DateTime.UtcNow.AddDays(1),
            IsCargoHazardous = false,
            VesselImoNumber = "IMO 7585229",
            SubmitterId = 908029952,

        };

        _repositoryMock.Setup(r => r.GetVesselVisitNotificationByNotificationIdAsync(It.IsAny<string>()))
            .ReturnsAsync((VesselVisitNotification)null!);

        var result = await _controller.Update("NON_EXISTENT_ID", dto);
        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateVesselVisitNotification_ReturnsNotFound_WhenVesselDoesNotExist()
    {
        var dto = new CreateVesselVisitNotificationDto
        {
            NotificationId = "2025_PORTO_000005",
            ExpectedArrival = DateTime.UtcNow,
            ExpectedDeparture = DateTime.UtcNow.AddDays(1),
            IsCargoHazardous = false,
            VesselImoNumber = "IMO 0000000",
            SubmitterId = 908029952,

        };

        _vesselRepositoryMock.Setup(r => r.GetVesselByIMOAsync(dto.VesselImoNumber))
            .ReturnsAsync((Vessel)null!);

        var result = await _controller.Update("2025_PORTO_000005", dto);
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateVesselVisitNotification_ReturnsNotFound_WhenRepresentativeDoesNotExist()
    {
        var dto = new CreateVesselVisitNotificationDto
        {
            NotificationId = "2025_PORTO_000005",
            ExpectedArrival = DateTime.UtcNow,
            ExpectedDeparture = DateTime.UtcNow.AddDays(1),
            IsCargoHazardous = false,
            VesselImoNumber = "IMO 7585229",
            SubmitterId = 100000000,

        };

        _representativeRepositoryMock.Setup(r => r.GetByCitizenIdAsync(dto.SubmitterId))
            .ReturnsAsync((Representative)null!);

        var result = await _controller.Update("2025_PORTO_000005", dto);
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateVesselVisitNotification_ReturnsInternalServerError_OnException()
    {
        var existingNotificationId = "2025-PORTO-000005";
        var updateDto = new CreateVesselVisitNotificationDto
        {
            NotificationId = existingNotificationId,
            ExpectedArrival = DateTime.UtcNow,
            ExpectedDeparture = DateTime.UtcNow.AddDays(1),
            IsCargoHazardous = false,
            VesselImoNumber = "IMO 7585229",
            SubmitterId = 908029952,

        };

        _repositoryMock.Setup(repo => repo.GetVesselVisitNotificationByNotificationIdAsync(existingNotificationId))
            .ThrowsAsync(new Exception("Database error"));

        var result = await _controller.Update(existingNotificationId, updateDto);

        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
    }


    [Fact]
    public async Task FilterVesselVisitNotification_ReturnsOk_WhenFound()
    {
        var filter = new VesselVisitNotificationFilter
        {
            SubmitterCitizenshipId = 908029952,
            Status = NotificationStatusFilter.InProgress,
            WithReason = null,
            WithDockAssigned = null,
            Vessel = null,
            ExpectedArrivalFrom = null,
            ExpectedArrivalTo = null,
        };
        _repositoryMock.Setup(repo => repo.FilterVesselVisitNotificationsAsync(filter))
            .ReturnsAsync(new Page<VesselVisitNotification>
            {
                Items = new List<VesselVisitNotification>(),
                PageNumber = 1,
                PageSize = 10,
            });

        var result = await _controller.Filter(filter);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<Page<VesselVisitNotificationStatusDto>>(okResult.Value);
        Assert.Empty(returnValue.Items);
    }


    [Fact]
    public async Task FilterVesselVisitNotification_ReturnsInternalServerError_OnException()
    {

        _repositoryMock.Setup(r => r.FilterVesselVisitNotificationsAsync(It.IsAny<VesselVisitNotificationFilter>()))
            .ThrowsAsync(new Exception("Test Exception"));

        var result = await _controller.Filter(new VesselVisitNotificationFilter { SubmitterCitizenshipId = 1 });
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }
}