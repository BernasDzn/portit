using Api.Application.DataTransfer;
using Api.Application.Exceptions;
using Api.Application.Services;
using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Unitary.Service;

public class StorageAreaServiceTest
{
    private readonly StorageAreaService _service;
    private readonly Mock<IStorageAreaRepository> _repoMock;
    private readonly Mock<IDockRepository> _dockRepoMock;
    private readonly Mock<ILogger<StorageAreaService>> _loggerMock;
    private readonly CreateStorageAreaDto _createDto;
    private StorageArea _existingStorageArea = null!;

    public StorageAreaServiceTest()
    {
        _repoMock = new Mock<IStorageAreaRepository>();
        _dockRepoMock = new Mock<IDockRepository>();
        _loggerMock = new Mock<ILogger<StorageAreaService>>();
        _service = new StorageAreaService(_repoMock.Object, _dockRepoMock.Object, _loggerMock.Object);
        _createDto = CreateValidStorageAreaDto();
        _existingStorageArea = CreateValidStorageArea();
    }

    private StorageArea CreateValidStorageArea()
    {
        return new StorageArea(Guid.NewGuid(), new Code { Value = "SA001" }, new Designation { Value = "Location" }, StorageAreaType.Yard, 10, 0, new HashSet<StorageArea.DockRelation>());
    }

    private CreateStorageAreaDto CreateValidStorageAreaDto()
    {
        return new CreateStorageAreaDto
        {
            NameCode = "SA001",
            Location = "Location",
            Type = StorageAreaType.Yard,
            Capacity = 10,
            CurrentOccupancy = 0,
            DockServices = new HashSet<CreateDockRelationDto>()
        };
    }

    [Fact]
    public async Task GetStorageAreas_WhenRequested_ReturnsList()
    {
        _repoMock.Setup(r => r.GetStorageAreasAsync())
            .ReturnsAsync(new List<StorageArea>());

        var result = await _service.GetStorageAreas();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetStorageAreaByCode_WhenFound_ReturnsDto()
    {
        _repoMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>()))
            .ReturnsAsync(_existingStorageArea);

        var result = await _service.GetStorageAreaByCode(_existingStorageArea.NameCode.Value);
        Assert.NotNull(result);
        Assert.Equal(_existingStorageArea.NameCode.Value, result.NameCode);
    }

    [Fact]
    public async Task GetStorageAreaByCode_WhenNotFound_Throws()
    {
        _repoMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>()))
            .ReturnsAsync((StorageArea?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetStorageAreaByCode("NOPE"));
    }

    [Fact]
    public async Task CreateStorageArea_WhenAlreadyExists_Throws()
    {
        _repoMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>()))
            .ReturnsAsync(_existingStorageArea);

        await Assert.ThrowsAsync<EntityAlreadyExistsException>(() => _service.CreateStorageArea(_createDto));
    }

    [Fact]
    public async Task CreateStorageArea_WhenDockNotFound_Throws()
    {
        var dto = CreateValidStorageAreaDto();
        dto.DockServices = new HashSet<CreateDockRelationDto> { new CreateDockRelationDto { DockCode = "D1", Distance = 1, IsServingDock = true } };
        _repoMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>())).ReturnsAsync((StorageArea?)null);
        _dockRepoMock.Setup(d => d.GetDockByCodeAsync(It.IsAny<string>())).ReturnsAsync((Dock?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.CreateStorageArea(dto));
    }

    [Fact]
    public async Task CreateStorageArea_WhenRepositoryThrows_Propagates()
    {
        _repoMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>())).ReturnsAsync((StorageArea?)null);
        _repoMock.Setup(r => r.Add(It.IsAny<StorageArea>())).ThrowsAsync(new Exception("Test"));

        await Assert.ThrowsAsync<Exception>(() => _service.CreateStorageArea(_createDto));
    }

    [Fact]
    public async Task CreateStorageArea_ReturnsDto_OnSuccess()
    {
        _repoMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>())).ReturnsAsync((StorageArea?)null);
        _repoMock.Setup(r => r.Add(It.IsAny<StorageArea>())).ReturnsAsync((StorageArea sa) => sa);

        var result = await _service.CreateStorageArea(_createDto);
        Assert.NotNull(result);
        Assert.Equal(_createDto.NameCode, result.NameCode);
    }

    [Fact]
    public async Task UpdateStorageArea_ReturnsDto_OnSuccess()
    {
        var existing = CreateValidStorageArea();
        _repoMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>())).ReturnsAsync(existing);
        _repoMock.Setup(r => r.Update(It.IsAny<StorageArea>())).ReturnsAsync((StorageArea sa) => sa);

        var dto = CreateValidStorageAreaDto();
        dto.Location = "NewLoc";

        var result = await _service.UpdateStorageArea(existing.NameCode.Value, dto);
        Assert.NotNull(result);
        Assert.Equal("NewLoc", result.Location);
    }

    [Fact]
    public async Task UpdateStorageArea_WhenNotFound_Throws()
    {
        _repoMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>())).ReturnsAsync((StorageArea?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.UpdateStorageArea("X", CreateValidStorageAreaDto()));
    }

    [Fact]
    public async Task UpdateStorageArea_WhenDockNotFound_Throws()
    {
        var existing = CreateValidStorageArea();
        _repoMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>())).ReturnsAsync(existing);
        _dockRepoMock.Setup(d => d.GetDockByCodeAsync(It.IsAny<string>())).ReturnsAsync((Dock?)null);

        var dto = CreateValidStorageAreaDto();
        dto.DockServices = new HashSet<CreateDockRelationDto> { new CreateDockRelationDto { DockCode = "D1", Distance = 1, IsServingDock = true } };

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.UpdateStorageArea(existing.NameCode.Value, dto));
    }

    [Fact]
    public async Task UpdateStorageArea_WhenRepositoryThrows_Propagates()
    {
        var existing = CreateValidStorageArea();
        _repoMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>())).ReturnsAsync(existing);
        _repoMock.Setup(r => r.Update(It.IsAny<StorageArea>())).ThrowsAsync(new Exception("Err"));

        await Assert.ThrowsAsync<Exception>(() => _service.UpdateStorageArea(existing.NameCode.Value, CreateValidStorageAreaDto()));
    }

}
