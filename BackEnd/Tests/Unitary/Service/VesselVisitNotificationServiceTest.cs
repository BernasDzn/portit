using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Application.Exceptions;
using Api.Application.Services;
using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Exceptions;
using Api.Infrastructure.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Unitary.Service;

public class VesselVisitNotificationServiceTest
{
    private readonly Mock<IVesselVisitNotificationRepository> _repositoryMock;
    private readonly Mock<IVesselRepository> _vesselRepositoryMock;
    private readonly Mock<IRepresentativeRepository> _representativeRepositoryMock;
    private readonly Mock<IStorageAreaRepository> _storageAreaRepositoryMock;
    private readonly Mock<IContainerRepository> _containerRepositoryMock;
    private readonly Mock<IDockRepository> _dockRepositoryMock;
    private readonly Mock<IPhysicalResourceRepository> _physicalResourceRepositoryMock;
    private readonly Mock<IStaffRepository> _staffRepositoryMock;
    private readonly VesselVisitNotificationService _service;

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

    public VesselVisitNotificationServiceTest()
    {
        _repositoryMock = new Mock<IVesselVisitNotificationRepository>();
        _vesselRepositoryMock = new Mock<IVesselRepository>();
        _representativeRepositoryMock = new Mock<IRepresentativeRepository>();
        _storageAreaRepositoryMock = new Mock<IStorageAreaRepository>();
        _containerRepositoryMock = new Mock<IContainerRepository>();
        _dockRepositoryMock = new Mock<IDockRepository>();
        _physicalResourceRepositoryMock = new Mock<IPhysicalResourceRepository>();
        _staffRepositoryMock = new Mock<IStaffRepository>();

        var idGenerator = new VesselVisitNotificationIdGenerator(_repositoryMock.Object);

        _representativeRepositoryMock.Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(representative);

        _service = new VesselVisitNotificationService(
            _repositoryMock.Object,
            _vesselRepositoryMock.Object,
            _representativeRepositoryMock.Object,
            _storageAreaRepositoryMock.Object,
            idGenerator,
            _containerRepositoryMock.Object,
            _dockRepositoryMock.Object,
            _physicalResourceRepositoryMock.Object,
            _staffRepositoryMock.Object,
            new Mock<ILogger<VesselVisitNotificationService>>().Object,
            null!
        );
    }

    [Fact]
    public async Task GetVesselVisitNotifications_ReturnsListOfNotifications()
    {
        _repositoryMock.Setup(repo => repo.GetVesselVisitNotificationsAsync())
            .ReturnsAsync(notifications);

        var result = await _service.GetVesselVisitNotifications();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal(notifications.ElementAt(0).NotificationId.ToString(), result.ElementAt(0).NotificationId.ToString());
    }

    [Fact]
    public async Task GetVesselVisitNotificationById_ReturnsNotification_WhenExists()
    {
        _repositoryMock.Setup(repo => repo.GetVesselVisitNotificationByNotificationIdAsync(It.IsAny<string>()))
            .ReturnsAsync((string id) => notifications.FirstOrDefault(n => n.NotificationId.ToString() == id));

        var result = await _service.GetById(notifications.ElementAt(0).NotificationId.ToString());

        Assert.NotNull(result);
        Assert.Equal(notifications.ElementAt(0).NotificationId.ToString(), result.NotificationId.ToString());
    }

    [Fact]
    public async Task GetVesselVisitNotificationById_ThrowsException_WhenNotExists()
    {
        _repositoryMock.Setup(repo => repo.GetVesselVisitNotificationByNotificationIdAsync(It.IsAny<string>()))
            .ReturnsAsync((VesselVisitNotification?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _service.GetById("NonExistentId"));
    }

    [Fact]
    public async Task Add_ReturnsAddedVesselVisitNotification()
    {
        var newNotificationDto = new CreateVesselVisitNotificationDto
        {
            NotificationId = "2025-PORTO-000001",
            ExpectedArrival = DateTime.UtcNow.AddDays(1),
            ExpectedDeparture = DateTime.UtcNow.AddDays(5),
            IsCargoHazardous = true,
            VesselImoNumber = "IMO 7585229"
        };

        _vesselRepositoryMock.Setup(repo => repo.GetVesselByIMOAsync(newNotificationDto.VesselImoNumber))
            .ReturnsAsync(vessel);

        _representativeRepositoryMock.Setup(repo => repo.GetByCitizenIdAsync(908029952))
            .ReturnsAsync(representative);

        _repositoryMock.Setup(repo => repo.AddAsync(It.IsAny<VesselVisitNotification>()))
            .ReturnsAsync((VesselVisitNotification n) => n);

        var result = await _service.Add(newNotificationDto, "psharply0@yolasite.com");

        Assert.NotNull(result);

        // the service generates the notification id via the id generator and repository
        // we assert the generated id has the expected prefix rather than a fixed value
        Assert.StartsWith("2026-PORTO-", result.NotificationId.ToString());
    }

    [Fact]
    public async Task Add_ThrowsException_WhenNotificationIdAlreadyExists()
    {
        var existingNotificationDto = new CreateVesselVisitNotificationDto
        {
            NotificationId = notifications.ElementAt(0).NotificationId.ToString(),
            ExpectedArrival = DateTime.UtcNow.AddDays(1),
            ExpectedDeparture = DateTime.UtcNow.AddDays(5),
            IsCargoHazardous = true,
            VesselImoNumber = "IMO 7585229"
        };

        _repositoryMock.Setup(repo => repo.GetVesselVisitNotificationByNotificationIdAsync(existingNotificationDto.NotificationId))
            .ReturnsAsync(notifications.FirstOrDefault(n => n.NotificationId.ToString() == existingNotificationDto.NotificationId));

        await Assert.ThrowsAsync<EntityAlreadyExistsException>(() => _service.Add(existingNotificationDto, "psharply0@yolasite.com"));
    }

    [Fact]
    public async Task Add_ThrowsException_WhenVesselNotFound()
    {
        var newNotificationDto = new CreateVesselVisitNotificationDto
        {
            NotificationId = "2025-PORTO-3",
            ExpectedArrival = DateTime.UtcNow.AddDays(1),
            ExpectedDeparture = DateTime.UtcNow.AddDays(5),
            IsCargoHazardous = true,
            VesselImoNumber = "IMO 0000000"
        };

        _repositoryMock.Setup(repo => repo.GetVesselVisitNotificationByNotificationIdAsync(It.Is<string>(s => s == newNotificationDto.NotificationId)))
            .ReturnsAsync(notifications.FirstOrDefault(n => n.NotificationId.ToString() == newNotificationDto.NotificationId));

        _vesselRepositoryMock.Setup(repo => repo.GetVesselByIMOAsync(It.IsAny<string>()))
            .ReturnsAsync((Vessel?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.Add(newNotificationDto, "psharply0@yolasite.com"));
    }

    [Fact]
    public async Task Add_ThrowsException_WhenRepresentativeNotFound()
    {
        var newNotificationDto = new CreateVesselVisitNotificationDto
        {
            NotificationId = "2025-PORTO-3",
            ExpectedArrival = DateTime.UtcNow.AddDays(1),
            ExpectedDeparture = DateTime.UtcNow.AddDays(5),
            IsCargoHazardous = true,
            VesselImoNumber = "IMO 0000000"
        };

        _repositoryMock.Setup(repo => repo.GetVesselVisitNotificationByNotificationIdAsync(It.Is<string>(s => s == newNotificationDto.NotificationId)))
            .ReturnsAsync(notifications.FirstOrDefault(n => n.NotificationId.ToString() == newNotificationDto.NotificationId));

        _vesselRepositoryMock.Setup(repo => repo.GetVesselByIMOAsync(It.IsAny<string>()))
            .ReturnsAsync((Vessel?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.Add(newNotificationDto, "psharply0@yolasite.com"));
    }

    [Fact]
    public async Task UpdateVesselVisitNotification_ReturnsUpdatedNotification_WhenExists()
    {
        var existingNotification = notifications.ElementAt(0);
        var updatedNotificationDto = new CreateVesselVisitNotificationDto
        {
            NotificationId = existingNotification.NotificationId.ToString(),
            ExpectedArrival = DateTime.UtcNow.AddDays(2),
            ExpectedDeparture = DateTime.UtcNow.AddDays(6),
            IsCargoHazardous = existingNotification.IsCargoHazardous,
            VesselImoNumber = existingNotification.Vessel.ImoIdentifier.Value,
            SpecialRequirements = "Updated special requirements"
        };

        _repositoryMock.Setup(repo => repo.GetVesselVisitNotificationByNotificationIdAsync(existingNotification.NotificationId.ToString()))
            .ReturnsAsync(existingNotification);

        _repositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<VesselVisitNotification>()))
            .ReturnsAsync((VesselVisitNotification n) => n);

        var result = await _service.Update(existingNotification.NotificationId.ToString(), updatedNotificationDto, "psharply0@yolasite.com");

        Assert.NotNull(result);
        Assert.Equal(updatedNotificationDto.NotificationId, result.NotificationId.ToString());
        Assert.Equal(updatedNotificationDto.ExpectedArrival, result.ExpectedArrival);
        Assert.Equal(updatedNotificationDto.ExpectedDeparture, result.ExpectedDeparture);
        Assert.Equal(updatedNotificationDto.SpecialRequirements, result.SpecialRequirements);
    }

    [Fact]
    public async Task UpdateVesselVisitNotification_ThrowsException_WhenNotificationIdNotExists()
    {
        var nonExistentNotificationDto = new CreateVesselVisitNotificationDto
        {
            NotificationId = "NonExistentId",
            ExpectedArrival = DateTime.UtcNow.AddDays(1),
            ExpectedDeparture = DateTime.UtcNow.AddDays(5),
            IsCargoHazardous = true,
            VesselImoNumber = "IMO 7585229"
        };

        _repositoryMock.Setup(repo => repo.GetVesselVisitNotificationByNotificationIdAsync(nonExistentNotificationDto.NotificationId))
            .ReturnsAsync((VesselVisitNotification?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.Update(nonExistentNotificationDto.NotificationId, nonExistentNotificationDto, "psharply0@yolasite.com"));
    }

    [Fact]
    public async Task FilteredVesselVisitNotification_ReturnsPage()
    {
        var filter = new VesselVisitNotificationFilter { };

        _repositoryMock.Setup(repo => repo.FilterVesselVisitNotificationsAsync(filter, 908029952))
            .ReturnsAsync(new Page<VesselVisitNotification>
            {
                Items = notifications.ToList(),
                PageNumber = 1,
                PageSize = notifications.Count
            });

        var result = await _service.FilterNotifications(filter,  "psharply0@yolasite.com");

        Assert.NotNull(result);
        Assert.IsType<Page<VesselVisitNotificationStatusDto>>(result);
        Assert.Equal(notifications.Count, result.Items.Count);
    }

    [Fact]
    public async Task FilteredVesselVisitNotification_ReturnsEmptyPage_WhenNoMatches()
    {
        var filter = new VesselVisitNotificationFilter { };

        _repositoryMock.Setup(repo => repo.FilterVesselVisitNotificationsAsync(filter, 908029952))
            .ReturnsAsync(new Page<VesselVisitNotification>
            {
                Items = new List<VesselVisitNotification>(),
                PageNumber = 1,
                PageSize = 0
            });

        var result = await _service.FilterNotifications(filter, "psharply0@yolasite.com");

        Assert.NotNull(result);
        Assert.IsType<Page<VesselVisitNotificationStatusDto>>(result);
        Assert.Empty(result.Items);
    }
}