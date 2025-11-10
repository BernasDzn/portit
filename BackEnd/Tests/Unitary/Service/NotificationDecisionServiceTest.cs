using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Application.Exceptions;
using Api.Application.Services;
using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Exceptions;
using Api.Infrastructure.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Moq;

namespace Tests.Unitary.Service;

public class NotificationDecisionServiceTest
{
    private readonly Mock<IVesselVisitNotificationRepository> _repositoryMock;
    private readonly Mock<IDockRepository> _dockRepositoryMock;
    private readonly NotificationDecisionService _service;

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
    private static ICollection<VesselVisitNotification> notifications = new List<VesselVisitNotification>
    {
        new VesselVisitNotification(
            new VesselVisitNotificationId(new Designation { Value = "PORTO" }, 1, 2025),
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(5),
            false,
            vessel,
            representative,
            "Requires additional security measures",
            new Crew(new Designation { Value = "Miguel Oliveira" }, 2, new HashSet<SafetyOfficer>()),
            null,
            null
        ),
        new VesselVisitNotification(
            new VesselVisitNotificationId(new Designation { Value = "PORTO" }, 2, 2025),
            DateTime.UtcNow.AddDays(3),
            DateTime.UtcNow.AddDays(7),
            false,
            vessel,
            representative,
            "Handle with care",
            new Crew(new Designation { Value = "Ana Silva" }, 3, new HashSet<SafetyOfficer>()),
            null,
            null
        )
    };

    private static ICollection<NotificationDecision> decisions = new List<NotificationDecision>
    {
        new NotificationDecision(
            NotificationDecisionStatus.Approved,
            DateTime.UtcNow,
            1,
            new Dock(
                Guid.NewGuid(),
                new Code { Value = "DCK001" },
                new Designation { Value = "Dock A" },
                new Designation { Value = "Main docking area" },
                new PhysicalCharacteristics
                {
                    Length = 500,
                    Depth = 20,
                    Draft = 15
                },
                new HashSet<VesselType>{ new VesselType(
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
                    }
                ) }
            )
        ),
        new NotificationDecision(
            NotificationDecisionStatus.Rejected,
            DateTime.UtcNow,
            2,
            null,
            "Insufficient documentation"
        )
    };

    public NotificationDecisionServiceTest()
    {
        _repositoryMock = new Mock<IVesselVisitNotificationRepository>();
        _dockRepositoryMock = new Mock<IDockRepository>();
        _service = new NotificationDecisionService(
            _repositoryMock.Object,
            _dockRepositoryMock.Object,
            new Mock<ILogger<NotificationDecisionService>>().Object
        );


        // Setup common repository mock behavior
        _repositoryMock.Setup(repo => repo.GetVesselVisitNotificationByNotificationIdAsync(It.IsAny<string>()))
            .ReturnsAsync((string id) => (VesselVisitNotification?)notifications.FirstOrDefault(n => n.NotificationId.ToString() == id));

        if (notifications.ElementAt(0).Status == NotificationStatus.InProgress)
            notifications.ElementAt(0).Submit();

        _repositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<VesselVisitNotification>()))
            .ReturnsAsync((VesselVisitNotification n) => n);
    }

    [Fact]
    public async Task GetDecisions_ReturnsListOfNotifications()
    {
        var vvn = notifications.ElementAt(0);
        _repositoryMock.Setup(repo => repo.GetNotificationDecisionsAsync(vvn.NotificationId.ToString()))
            .ReturnsAsync(decisions);

        var result = await _service.GetNotificationDecisions(vvn.NotificationId.ToString());

        Assert.NotNull(result);
        Assert.Equal(decisions.Count, result.Count());
    }

    [Fact]
    public async Task GetDecisions_ThrowsException_WhenNotExists()
    {

        _repositoryMock.Setup(repo => repo.GetVesselVisitNotificationByNotificationIdAsync(It.IsAny<string>()))
            .ReturnsAsync((string id) => notifications.FirstOrDefault(n => n.NotificationId.ToString() == id));

        await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _service.GetNotificationDecisions("NonExistentId"));
    }

    [Fact]
    public async Task Add_ReturnsAddedDecision()
    {
        var newDecisionDto = new CreateNotificationDecisionDto
        {
            Status = 2,
            Reason = "Valid reason",
            DecisionDate = DateTime.UtcNow,
            IsFinal = false
        };

        var result = await _service.Add(newDecisionDto, notifications.ElementAt(0).NotificationId.ToString());

        Assert.NotNull(result);
        Assert.Equal(newDecisionDto.Status, result.Status);
    }

    [Fact]
    public async Task Add_ThrowsException_WhenNotificationNotSubmitted()
    {
        var newDecisionDto = new CreateNotificationDecisionDto
        {
            Status = 2,
            Reason = "Valid reason",
            DecisionDate = DateTime.UtcNow,
            IsFinal = false
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.Add(newDecisionDto, notifications.ElementAt(1).NotificationId.ToString()));
    }

    [Fact]
    public async Task Add_ThrowsException_WhenNotificationNotFound()
    {
        var newDecisionDto = new CreateNotificationDecisionDto
        {
            Status = 2,
            Reason = "Valid reason",
            DecisionDate = DateTime.UtcNow,
            IsFinal = false
        };

        _repositoryMock.Setup(repo => repo.GetVesselVisitNotificationByNotificationIdAsync(It.Is<string>(s => s == notifications.ElementAt(0).NotificationId.ToString())))
            .ReturnsAsync((VesselVisitNotification?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.Add(newDecisionDto, notifications.ElementAt(0).NotificationId.ToString()));
    }

    [Fact]
    public async Task Add_ThrowsException_WhenInvalidStatus()
    {
        var newDecisionDto = new CreateNotificationDecisionDto
        {
            Status = 5,
            Reason = "Invalid status",
            DecisionDate = DateTime.UtcNow,
            IsFinal = false
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.Add(newDecisionDto, notifications.ElementAt(0).NotificationId.ToString()));
    }

    [Fact]
    public async Task Add_ThrowsException_WhenInvalidAssignedDock()
    {
        var newDecisionDto = new CreateNotificationDecisionDto
        {
            Status = 1,
            Reason = "Accepted without dock",
            DecisionDate = DateTime.UtcNow,
            IsFinal = false,
            AssignedDockCode = null
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.Add(newDecisionDto, notifications.ElementAt(0).NotificationId.ToString()));
    }
}