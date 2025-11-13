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
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

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
    private readonly Mock<IDockRepository> _dockRepositoryMock = new Mock<IDockRepository>();
    private readonly Mock<IPhysicalResourceRepository> _physicalResourceRepositoryMock = new Mock<IPhysicalResourceRepository>();
    private readonly VesselVisitNotificationIdGenerator _idGenerator;


    private static Representative representative = new Representative(
        Guid.NewGuid(),
        908029952,
        new Designation { Value = "Patricio Sharply" },
        new Email { Value = "psharply0@yolasite.com" },
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

        _idGenerator = new VesselVisitNotificationIdGenerator(_repositoryMock.Object);

        _service = new VesselVisitNotificationService(
            _repositoryMock.Object,
            _vesselRepositoryMock.Object,
            _representativeRepositoryMock.Object,
            _storageAreaRepositoryMock.Object,
            _idGenerator,
            _containerRepositoryMock.Object,
            _dockRepositoryMock.Object,
            _physicalResourceRepositoryMock.Object,
            new Mock<ILogger<VesselVisitNotificationService>>().Object);


        var notificationDecisionService = new NotificationDecisionService(
            _repositoryMock.Object,
            _dockRepositoryMock.Object,
            new Mock<ILogger<NotificationDecisionService>>().Object);

        _controller = new VesselVisitNotificationController(
            _service,
            notificationDecisionService,
            new Mock<ILogger<VesselVisitNotificationController>>().Object);

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

        // Mock representative lookup for authentication
        _representativeRepositoryMock.Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(representative);
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
        var testId = "2025-PORTO-999999";

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
            VesselImoNumber = "IMO 7585229"
        };


        _vesselRepositoryMock.Setup(repo => repo.GetVesselByIMOAsync(newNotificationDto.VesselImoNumber))
            .ReturnsAsync(vessel);

        _representativeRepositoryMock.Setup(repo => repo.GetByCitizenIdAsync(908029952))
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
            NotificationId = "2025-PORTO-000001",
            ExpectedArrival = DateTime.UtcNow.AddDays(2),
            ExpectedDeparture = DateTime.UtcNow.AddDays(6),
            IsCargoHazardous = false,
            SpecialRequirements = null,
            CrewDetails = null,
            LoadCargoManifest = null,
            UnloadCargoManifest = null,
            VesselImoNumber = "IMO 7585229"
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
            NotificationId = "2025-PORTO-000008",
            ExpectedArrival = DateTime.UtcNow.AddDays(2),
            ExpectedDeparture = DateTime.UtcNow.AddDays(6),
            IsCargoHazardous = false,
            SpecialRequirements = null,
            CrewDetails = null,
            LoadCargoManifest = null,
            UnloadCargoManifest = null,
            VesselImoNumber = "IMO 00000000"
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
            NotificationId = "2025-PORTO-000008",
            ExpectedArrival = DateTime.UtcNow.AddDays(2),
            ExpectedDeparture = DateTime.UtcNow.AddDays(6),
            IsCargoHazardous = false,
            SpecialRequirements = null,
            CrewDetails = null,
            LoadCargoManifest = null,
            UnloadCargoManifest = null,
            VesselImoNumber = "IMO 7585229"
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
            NotificationId = "2025-PORTO-000009",
            ExpectedArrival = DateTime.UtcNow.AddDays(2),
            ExpectedDeparture = DateTime.UtcNow.AddDays(6),
            IsCargoHazardous = false,
            SpecialRequirements = null,
            CrewDetails = null,
            LoadCargoManifest = null,
            UnloadCargoManifest = null,
            VesselImoNumber = "IMO 7585229"
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
        var notificationIdToUpdate = "2025-PORTO-000009";
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
            VesselImoNumber = "IMO 7585229"
        };



        _vesselRepositoryMock.Setup(repo => repo.GetVesselByIMOAsync(updateVesselVisitNotificationDto.VesselImoNumber))
            .ReturnsAsync(vessel);

        _representativeRepositoryMock.Setup(repo => repo.GetByCitizenIdAsync(908029952))
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

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateVesselVisitNotification_ReturnsNotFound_WhenNotificationDoesNotExist()
    {

        var dto = new CreateVesselVisitNotificationDto
        {
            NotificationId = "NON_EXISTENT_ID",
            ExpectedArrival = DateTime.UtcNow,
            ExpectedDeparture = DateTime.UtcNow.AddDays(1),
            IsCargoHazardous = false,
            VesselImoNumber = "IMO 7585229",
        };

        _repositoryMock.Setup(r => r.GetVesselVisitNotificationByNotificationIdAsync(It.IsAny<string>()))
            .ReturnsAsync((VesselVisitNotification)null!);

        var result = await _controller.Update("NON_EXISTENT_ID", dto);
        Assert.IsType<NotFoundObjectResult>(result);
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
            VesselImoNumber = "IMO 7585229"
        };

        _repositoryMock.Setup(repo => repo.GetVesselVisitNotificationByNotificationIdAsync(existingNotificationId))
            .ThrowsAsync(new Exception("Database error"));

        var result = await _controller.Update(existingNotificationId, updateDto);

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }


    [Fact]
    public async Task FilterVesselVisitNotification_ReturnsOk_WhenFound()
    {
        var filter = new VesselVisitNotificationFilter
        {
            Status = NotificationStatusFilter.InProgress,
            WithReason = null,
            WithDockAssigned = null,
            Vessel = null,
            ExpectedArrivalFrom = null,
            ExpectedArrivalTo = null,
        };
        _repositoryMock.Setup(repo => repo.FilterVesselVisitNotificationsAsync(filter, 908029952))
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

        _repositoryMock.Setup(r => r.FilterVesselVisitNotificationsAsync(It.IsAny<VesselVisitNotificationFilter>(), 1))
            .ThrowsAsync(new Exception("Test Exception"));

        var result = await _controller.Filter(new VesselVisitNotificationFilter { });
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }


    [Fact]
    public async Task GetDecisions_ReturnsOkWithList()
    {
        var vvn = new VesselVisitNotification(
            new VesselVisitNotificationId(new Designation { Value = "PORTO" }, 10, 2025),
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(5),
            false,
            vessel,
            representative
        );

        vvn.Submit();
        var decision = NotificationDecisionFactory.CreateRejected("officer@email.com", "No reason", true, DateTime.UtcNow);

        _repositoryMock.Setup(r => r.GetNotificationDecisionsAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<NotificationDecision> { decision });

        _repositoryMock.Setup(r => r.GetVesselVisitNotificationByNotificationIdAsync(It.IsAny<string>()))
            .ReturnsAsync(vvn);

        var result = await _controller.GetDecisions(vvn.NotificationId.ToString());
        var response = Assert.IsType<OkObjectResult>(result.Result);
        Assert.IsAssignableFrom<IEnumerable<NotificationDecisionDto>>(response.Value);
    }

    [Fact]
    public async Task GetDecisions_ReturnsNotFound_WhenNotificationNotFound()
    {
        _repositoryMock.Setup(r => r.GetVesselVisitNotificationByNotificationIdAsync(It.IsAny<string>()))
            .ReturnsAsync((VesselVisitNotification)null!);

        var result = await _controller.GetDecisions("NON_EXISTENT_ID");
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetDecisions_ReturnsInternalServerError_OnException()
    {
        _repositoryMock.Setup(r => r.GetNotificationDecisionsAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Test Exception"));

        _repositoryMock.Setup(r => r.GetVesselVisitNotificationByNotificationIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new VesselVisitNotification(
                new VesselVisitNotificationId(new Designation { Value = "PORTO" }, 99, 2025),
                DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow.AddDays(5),
                false,
                vessel,
                representative
            ));

        var result = await _controller.GetDecisions("NON_EXISTENT_ID");
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task CreateDecision_ReturnsCreatedAtAction_WhenAccepted()
    {
        var vvn = new VesselVisitNotification(
            new VesselVisitNotificationId(new Designation { Value = "PORTO" }, 10, 2025),
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(5),
            false,
            vessel,
            representative
        );

        vvn.Submit();

        var vt = new VesselType(
            Guid.NewGuid(),
            new Designation { Value = "Large Vessel Type" },
            new Designation { Value = "Vessel Type Description" },
            15,
            10,
            5,
            new PhysicalCharacteristics
            {
                Length = 200,
                Depth = 25,
                Draft = 10
            }
            );

        var dock = new Dock(
            Guid.NewGuid(),
            new Code { Value = "DCK001" },
            new Designation { Value = "Dock 1" },
            new Designation { Value = "Location 1" },
            new PhysicalCharacteristics
            {
                Length = 250,
                Depth = 40,
                Draft = 15
            },
            new HashSet<VesselType> { vt }
        );

        _dockRepositoryMock.Setup(r => r.GetDockByCodeAsync(It.IsAny<string>()))
            .ReturnsAsync(dock);

        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<VesselVisitNotification>()))
            .ReturnsAsync((VesselVisitNotification v) => v);

        _repositoryMock.Setup(r => r.GetVesselVisitNotificationByNotificationIdAsync(It.IsAny<string>()))
        .ReturnsAsync(vvn);

        var vvnDto = new CreateNotificationDecisionDto
        {
            Status = 1,
            AssignedDockCode = "DCK001",
            DecisionDate = DateTime.UtcNow,
            IsFinal = true
        };

        var result = await _controller.CreateDecision(vvn.NotificationId.ToString(), vvnDto);
        var response = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.IsType<NotificationDecisionDto>(response.Value);
    }

    [Fact]
    public async Task CreateDecision_ReturnsNotFound_WhenNotificationNotFound()
    {

        var vvnDto = new CreateNotificationDecisionDto
        {
            Status = 2,
            DecisionDate = DateTime.UtcNow,
            IsFinal = false
        };

        _repositoryMock.Setup(r => r.GetVesselVisitNotificationByNotificationIdAsync(It.IsAny<string>()))
                   .ReturnsAsync((VesselVisitNotification)null!);

        var result = await _controller.CreateDecision("NON_EXISTENT_ID", vvnDto);
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateDecision_ReturnsBadRequest_WhenMissingAssignedDock()
    {

        var vvn = new VesselVisitNotification(
            new VesselVisitNotificationId(new Designation { Value = "PORTO" }, 10, 2025),
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(5),
            false,
            vessel,
            representative
        );

        vvn.Submit();

        _repositoryMock.Setup(r => r.GetVesselVisitNotificationByNotificationIdAsync(It.IsAny<string>()))
            .ReturnsAsync(vvn);

        var vvnDto = new CreateNotificationDecisionDto
        {
            Status = 1,
            AssignedDockCode = null,
            DecisionDate = DateTime.UtcNow,
            IsFinal = true
        };

        var result = await _controller.CreateDecision(vvn.NotificationId.ToString(), vvnDto);
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateDecision_ReturnsInternalServerError_OnException()
    {
        var vvnDto = new CreateNotificationDecisionDto
        {
            Status = 2,
            DecisionDate = DateTime.UtcNow,
            IsFinal = false
        };

        _repositoryMock.Setup(r => r.GetVesselVisitNotificationByNotificationIdAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Test Exception"));

        var result = await _controller.CreateDecision("NON_EXISTENT_ID", vvnDto);
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

}