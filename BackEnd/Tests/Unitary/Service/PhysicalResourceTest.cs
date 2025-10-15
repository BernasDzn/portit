using Api.Application.DataTransfer;
using Api.Application.Services;
using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Domain.ValueObjects;
using Moq;

namespace Tests.Unitary.Service;

public class PhysicalResourceServiceTest
{
    private readonly Mock<IPhysicalResourceRepository> _physicalResourceRepositoryMock;
    private readonly Mock<IDockRepository> _dockRepositoryMock;
    private readonly Mock<IQualificationRepository> _qualificationRepositoryMock;
    private readonly Mock<IStorageAreaRepository> _storageAreaRepositoryMock;

    private readonly PhysicalResourceService _service;

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
            null!,
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
            null!,
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

    public PhysicalResourceServiceTest()
    {
        _physicalResourceRepositoryMock = new Mock<IPhysicalResourceRepository>();
        _dockRepositoryMock = new Mock<IDockRepository>();
        _qualificationRepositoryMock = new Mock<IQualificationRepository>();
        _storageAreaRepositoryMock = new Mock<IStorageAreaRepository>();

        _service = new PhysicalResourceService(
            _physicalResourceRepositoryMock.Object,
            _dockRepositoryMock.Object,
            _qualificationRepositoryMock.Object,
            _storageAreaRepositoryMock.Object,
            new Mock<Microsoft.Extensions.Logging.ILogger<PhysicalResourceService>>().Object
        );
    }

    [Fact]
    public async Task GetPhysicalResources_ReturnsListOfResources()
    {
        _physicalResourceRepositoryMock.Setup(repo => repo.GetPhysicalResourcesAsync())
            .ReturnsAsync(resources);

        var result = await _service.GetPhysicalResources();

        var r = (PhysicalResource)result.ElementAt(0);

        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.Equal(resources.ElementAt(0).ToDTO().Code, r.Code.Value);
    }

    [Fact]
    public async Task GetResourceByCode_ReturnsResource_WhenExists()
    {
        _physicalResourceRepositoryMock.Setup(repo => repo.GetResourceByCodeAsync(It.IsAny<string>()))
            .ReturnsAsync((string code) => resources.FirstOrDefault(r => r.Code.Value == code));

        var result = await _service.GetResourceByCode(resources.ElementAt(0).Code.Value);

        var r = (PhysicalResource)result!;

        Assert.NotNull(result);
        Assert.Equal(resources.ElementAt(0).ToDTO().Code, r.Code.Value);
    }

    [Fact]
    public async Task GetResourceByCode_ReturnsNull_WhenNotExists()
    {
        _physicalResourceRepositoryMock.Setup(repo => repo.GetResourceByCodeAsync(It.IsAny<string>()))
            .ReturnsAsync((PhysicalResource?)null);

        var result = await _service.GetResourceByCode("NonExistentCode");

        Assert.Null(result);
    }

    [Fact]
    public async Task AddSTSCraneAsync_ReturnsAddedCrane()
    {
        var newCraneDto = new STSCraneDto
        {
            Code = "CRANE2",
            Description = "STS Crane 2",
            Status = ResourceStatus.Available,
            SetupTimeInMinutes = 20,
            Qualifications = new List<QualificationDto>(),
            OperationalWindow = OperationalWindow.FullWeek(),
            LiftingCapacity = 60,
            ServingDock = null!,
            ContainersPerHour = 12,
        };

        _physicalResourceRepositoryMock.Setup(repo => repo.GetResourceByCodeAsync(newCraneDto.Code))
            .ReturnsAsync(resources.FirstOrDefault(r => r.Code.Value == newCraneDto.Code));

        _dockRepositoryMock.Setup(repo => repo.GetDockByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(new Dock(
                Guid.NewGuid(),
                new Code { Value = "DOCK1" },
                new Designation { Value = "Dock 1" },
                new Designation { Value = "Location 1" },
                new PhysicalCharacteristics { Depth = 10, Length = 300, Draft = 15 },
                new HashSet<VesselType>()
            ));

        _qualificationRepositoryMock.Setup(repo => repo.GetQualificationByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new Qualification(Guid.NewGuid(), new Code { Value = "Q1" }, new Designation { Value = "Qualification 1" }));

        _physicalResourceRepositoryMock.Setup(repo => repo.AddSTSCrane(It.IsAny<STSCrane>()))
            .ReturnsAsync((STSCrane crane) => crane);

        var result = await _service.AddSTSCraneAsync(newCraneDto);

        Assert.NotNull(result);
        Assert.Equal(newCraneDto.Code, result.Code);
    }
}