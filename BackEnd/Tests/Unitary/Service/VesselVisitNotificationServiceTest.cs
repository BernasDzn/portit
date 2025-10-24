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
using Moq;

namespace Tests.Unitary.Service;

public class VesselVisitNotificationServiceTest
{
    private readonly Mock<IVesselVisitNotificationRepository> _notificationRepositoryMock;
    private readonly Mock<IVesselRepository> _vesselRepositoryMock;
    private readonly Mock<IRepresentativeRepository> _representativeRepositoryMock;
    private readonly Mock<IStorageAreaRepository> _storageAreaRepositoryMock;
    private readonly Mock<IContainerRepository> _containerRepositoryMock;
    private readonly VesselVisitNotificationService _service;

    public VesselVisitNotificationServiceTest()
    {
        _notificationRepositoryMock = new Mock<IVesselVisitNotificationRepository>();
    _vesselRepositoryMock = new Mock<IVesselRepository>();
    _representativeRepositoryMock = new Mock<IRepresentativeRepository>();
        _storageAreaRepositoryMock = new Mock<IStorageAreaRepository>();
        _containerRepositoryMock = new Mock<IContainerRepository>();

        // Use a real id generator but provide the mocked notification repository it needs
        var idGenerator = new VesselVisitNotificationIdGenerator(_notificationRepositoryMock.Object);

        _service = new VesselVisitNotificationService(
            _notificationRepositoryMock.Object,
            _vesselRepositoryMock.Object,
            _representativeRepositoryMock.Object,
            _storageAreaRepositoryMock.Object,
            idGenerator,
            _containerRepositoryMock.Object,
            new Mock<ILogger<VesselVisitNotificationService>>().Object
        );
    }

    private (Vessel vessel, Representative representative) BuildVesselWithRepresentative(uint repCitizenId = 123456789)
    {
        // Representative
    	var rep = new Representative(Guid.NewGuid(), repCitizenId, new Designation { Value = "Rep Name" }, new Email { Value = "rep@example.com" }, new PhoneNumber { Value = "4512345678" });

        // Owner organization that contains the representative
        var owner = new ShippingAgentOrganization(
            Guid.NewGuid(),
            new Designation { Value = "Owner Org" },
            new List<Designation> { new Designation { Value = "Alt" } },
            new Address("Street 1", "City", "0000", "Country"),
            new TaxNumber { Value = "PT252252252" },
            new HashSet<Representative> { rep }
        );

        // Vessel type and physical characteristics
        var vesselType = new VesselType(Guid.NewGuid(), new Designation { Value = "TypeA" }, new Designation { Value = "Type A Description" }, 10u, 5u, 3u, new PhysicalCharacteristics { Length = 100, Depth = 20, Draft = 10 });
        var physical = new PhysicalCharacteristics { Length = 50, Depth = 10, Draft = 5 };

   		var vessel = new Vessel(Guid.NewGuid(), new Designation { Value = "Vessel 1" }, new ImoNumber { Value = "IMO1234567" }, vesselType, owner, physical);

        return (vessel, rep);
    }

    [Fact]
    public async Task GetVesselVisitNotifications_ReturnsList()
    {
        var pair = BuildVesselWithRepresentative();
        var notifications = new List<VesselVisitNotification>
        {
            new VesselVisitNotification(
                new VesselVisitNotificationId(new Designation{ Value = "PORTO" }, 1, (uint)DateTime.UtcNow.Year),
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(1),
                false,
                pair.vessel,
                pair.representative
            )
        };

        _notificationRepositoryMock.Setup(r => r.GetVesselVisitNotificationsAsync()).ReturnsAsync(notifications);

        var result = await _service.GetVesselVisitNotifications();

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task Add_ReturnsAddedNotification_WhenValid()
    {
        var (vessel, rep) = BuildVesselWithRepresentative();

    _notificationRepositoryMock.Setup(r => r.GetVesselVisitNotificationByNotificationIdAsync(It.IsAny<string>())).ReturnsAsync((VesselVisitNotification)null!);
        _vesselRepositoryMock.Setup(r => r.GetVesselByIMOAsync(It.IsAny<string>())).ReturnsAsync(vessel);
        _representativeRepositoryMock.Setup(r => r.GetByCitizenIdAsync(It.IsAny<uint>())).ReturnsAsync(rep);

        // Storage area & container used by cargo manifest
        var storageArea = new StorageArea(Guid.NewGuid(), new Code { Value = "YARD01" }, new Designation { Value = "Loc" }, StorageAreaType.Yard, 100, 0, new HashSet<StorageArea.DockRelation>());
        _storageAreaRepositoryMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>())).ReturnsAsync(storageArea);

        // Container repo returns null first to force creation
    _containerRepositoryMock.Setup(r => r.GetContainerByNumberAsync(It.IsAny<string>())).ReturnsAsync((Container)null!);
        _containerRepositoryMock.Setup(r => r.Add(It.IsAny<Container>())).ReturnsAsync((Container c) => c);

        var dto = new CreateVesselVisitNotificationDto
        {
            NotificationId = null!,
            ExpectedArrival = DateTime.UtcNow,
            ExpectedDeparture = DateTime.UtcNow.AddDays(1),
            IsCargoHazardous = false,
            VesselImoNumber = vessel.ImoIdentifier.Value,
            SubmitterId = rep.CitizenshipId,
            LoadCargoManifest = new List<CreateCargoTransportDto>
            {
                new CreateCargoTransportDto
                {
                    Position = new ContainerPosition { Bay = "01", Row = "A", Tier = "01" },
                    StorageAreaCode = storageArea.NameCode.Value,
                    Container = new ContainerDto { ContainerNumber = "CMAU2468103", CargoType = CargoType.OTHER, Description = "desc" }
                }
            }
        };

        _notificationRepositoryMock.Setup(r => r.GetVesselVisitNotificationsAsync()).ReturnsAsync(new List<VesselVisitNotification>());
        _notificationRepositoryMock.Setup(r => r.AddAsync(It.IsAny<VesselVisitNotification>())).ReturnsAsync((VesselVisitNotification v) => v);

        var result = await _service.Add(dto);

        Assert.NotNull(result);
        Assert.NotNull(result.NotificationId);
        _containerRepositoryMock.Verify(r => r.Add(It.IsAny<Container>()), Times.Once);
    }

    [Fact]
    public async Task Add_Throws_WhenAlreadyExists()
    {
        var existingPair = BuildVesselWithRepresentative();
        var existing = new VesselVisitNotification(
            new VesselVisitNotificationId(new Designation{ Value = "PORTO" }, 1, (uint)DateTime.UtcNow.Year),
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(1),
            false,
            existingPair.vessel,
            existingPair.representative
        );

    _notificationRepositoryMock.Setup(r => r.GetVesselVisitNotificationByNotificationIdAsync(It.IsAny<string>())).ReturnsAsync(existing);

        await Assert.ThrowsAsync<EntityAlreadyExistsException>(() => _service.Add(new CreateVesselVisitNotificationDto { NotificationId = existing.NotificationId.ToString(), ExpectedArrival = DateTime.UtcNow, ExpectedDeparture = DateTime.UtcNow.AddDays(1), IsCargoHazardous = false, VesselImoNumber = null!, SubmitterId = 1 }));
    }

    [Fact]
    public async Task Add_Throws_WhenVesselNotFound()
    {
    _notificationRepositoryMock.Setup(r => r.GetVesselVisitNotificationByNotificationIdAsync(It.IsAny<string>())).ReturnsAsync((VesselVisitNotification)null!);
    _vesselRepositoryMock.Setup(r => r.GetVesselByIMOAsync(It.IsAny<string>())).ReturnsAsync((Vessel)null!);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.Add(new CreateVesselVisitNotificationDto { NotificationId = null!, ExpectedArrival = DateTime.UtcNow, ExpectedDeparture = DateTime.UtcNow.AddDays(1), IsCargoHazardous = false, VesselImoNumber = "NONIMO", SubmitterId = 1 }));
    }

    [Fact]
    public async Task Add_Throws_WhenRepresentativeNotFound()
    {
        var (vessel, _) = BuildVesselWithRepresentative();
    _notificationRepositoryMock.Setup(r => r.GetVesselVisitNotificationByNotificationIdAsync(It.IsAny<string>())).ReturnsAsync((VesselVisitNotification)null!);
    _vesselRepositoryMock.Setup(r => r.GetVesselByIMOAsync(It.IsAny<string>())).ReturnsAsync(vessel);
    _representativeRepositoryMock.Setup(r => r.GetByCitizenIdAsync(It.IsAny<uint>())).ReturnsAsync((Representative)null!);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.Add(new CreateVesselVisitNotificationDto { NotificationId = null!, ExpectedArrival = DateTime.UtcNow, ExpectedDeparture = DateTime.UtcNow.AddDays(1), IsCargoHazardous = false, VesselImoNumber = vessel.ImoIdentifier.Value, SubmitterId = 999 }));
    }

    [Fact]
    public async Task Update_ReturnsUpdated_WhenExists()
    {
        var (vessel, rep) = BuildVesselWithRepresentative();
        var existing = new VesselVisitNotification(
            new VesselVisitNotificationId(new Designation{ Value = "PORTO" }, 1, (uint)DateTime.UtcNow.Year),
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(1),
            false,
            vessel,
            rep
        );

        _notificationRepositoryMock.Setup(r => r.GetVesselVisitNotificationByNotificationIdAsync(It.IsAny<string>())).ReturnsAsync(existing);
        _notificationRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<VesselVisitNotification>())).ReturnsAsync((VesselVisitNotification v) => v);

        var dto = new CreateVesselVisitNotificationDto { NotificationId = existing.NotificationId.ToString(), ExpectedArrival = DateTime.UtcNow.AddDays(2), ExpectedDeparture = DateTime.UtcNow.AddDays(3), IsCargoHazardous = false, VesselImoNumber = vessel.ImoIdentifier.Value, SubmitterId = rep.CitizenshipId };

        var result = await _service.Update(existing.NotificationId.ToString(), dto);

        Assert.NotNull(result);
        Assert.Equal(dto.ExpectedArrival, result.ExpectedArrival);
    }

    [Fact]
    public async Task Update_Throws_WhenNotExists()
    {
    _notificationRepositoryMock.Setup(r => r.GetVesselVisitNotificationByNotificationIdAsync(It.IsAny<string>())).ReturnsAsync((VesselVisitNotification)null!);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.Update("nonexistent", new CreateVesselVisitNotificationDto { NotificationId = null!, ExpectedArrival = DateTime.UtcNow, ExpectedDeparture = DateTime.UtcNow.AddDays(1), IsCargoHazardous = false, VesselImoNumber = null!, SubmitterId = 1 }));
    }

    [Fact]
    public async Task FilterNotifications_ReturnsPage()
    {
        var page = new Page<VesselVisitNotification> { Items = new List<VesselVisitNotification>(), PageNumber = 1, PageSize = 10 };
        _notificationRepositoryMock.Setup(r => r.FilterVesselVisitNotificationsAsync(It.IsAny<VesselVisitNotificationFilter>())).ReturnsAsync(page);

    var result = await _service.FilterNotifications(new VesselVisitNotificationFilter { SubmitterCitizeshipId = 1 });

        Assert.NotNull(result);
        Assert.IsType<Page<VesselVisitNotificationStatusDto>>(result);
    }
}
