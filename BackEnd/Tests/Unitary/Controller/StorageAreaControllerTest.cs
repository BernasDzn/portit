using Api.Application.Controllers;
using Api.Application.DataTransfer;
using Api.Application.Exceptions;
using Api.Application.Services;
using Api.Domain.Entities;
using Api.Domain.ValueObjects;
using Api.Domain.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Unitary.Controller;

public class StorageAreaControllerTest
{
    private readonly Mock<IStorageAreaRepository> _repoMock;
    private readonly Mock<IDockRepository> _dockRepoMock;
    private readonly StorageAreaService _service;
    private readonly StorageAreaController _controller;
    private readonly CreateStorageAreaDto _createDto;

    public StorageAreaControllerTest()
    {
        _repoMock = new Mock<IStorageAreaRepository>();
        _dockRepoMock = new Mock<IDockRepository>();
        var loggerService = new Mock<ILogger<StorageAreaService>>();
        _service = new StorageAreaService(_repoMock.Object, _dockRepoMock.Object, loggerService.Object);
        _controller = new StorageAreaController(_service, new Mock<ILogger<StorageAreaController>>().Object);

        _createDto = new CreateStorageAreaDto
        {
            NameCode = "SA001",
            Location = "Loc",
            Type = StorageAreaType.Yard,
            Capacity = 10,
            CurrentOccupancy = 0,
            DockServices = new HashSet<CreateDockRelationDto>()
        };
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk_WithListOfStorageAreas()
    {
        _repoMock.Setup(r => r.GetStorageAreasAsync())
            .ReturnsAsync(new List<StorageArea>());

        var result = await _controller.GetAll();

        Assert.IsType<OkObjectResult>(result.Result);
        Assert.IsType<List<StorageAreaDto>>(((OkObjectResult)result.Result).Value);
    }

    [Fact]
    public async Task GetAll_ShouldReturnServerError_OnException()
    {
        _repoMock.Setup(r => r.GetStorageAreasAsync())
            .ThrowsAsync(new Exception("Test Exception"));

        var result = await _controller.GetAll();

        var obj = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, obj.StatusCode);
    }

    [Fact]
    public async Task Get_ShouldReturnOk_WhenFound()
    {
        var sa = new StorageArea(Guid.NewGuid(), new Code { Value = "SA001" }, new Designation { Value = "Loc" }, StorageAreaType.Yard, 1, 0, new HashSet<StorageArea.DockRelation>());
        _repoMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>()))
            .ReturnsAsync(sa);

        var result = await _controller.Get("SA001");
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.IsType<StorageAreaDto>(ok.Value);
    }

    [Fact]
    public async Task Get_ShouldReturnNotFound_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>()))
            .ThrowsAsync(new EntityNotFoundException());

        var result = await _controller.Get("SA001");
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Get_ShouldReturnServerError_OnException()
    {
        _repoMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Test"));

        var result = await _controller.Get("SA001");
        var obj = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, obj.StatusCode);
    }

    [Fact]
    public async Task Create_ShouldReturnCreated_WhenValid()
    {
        _repoMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>()))
            .ReturnsAsync((StorageArea?)null);
        _repoMock.Setup(r => r.Add(It.IsAny<StorageArea>())).ReturnsAsync((StorageArea sa) => sa);

        var result = await _controller.Create(_createDto);
        Assert.IsType<CreatedAtActionResult>(result.Result);
    }

    [Fact]
    public async Task Create_ShouldReturnConflict_WhenAlreadyExists()
    {
        _repoMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>()))
            .ReturnsAsync(new StorageArea(Guid.NewGuid(), new Code { Value = "SA001" }, new Designation { Value = "Loc" }, StorageAreaType.Yard, 1, 0, new HashSet<StorageArea.DockRelation>()));

        var result = await _controller.Create(_createDto);
        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_ShouldReturnNotFound_WhenDependencyMissing()
    {
        _repoMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>()))
            .ReturnsAsync((StorageArea?)null);
        _dockRepoMock.Setup(d => d.GetDockByCodeAsync(It.IsAny<string>())).ReturnsAsync((Dock?)null);

        var dtoWithDock = new CreateStorageAreaDto
        {
            NameCode = _createDto.NameCode,
            Location = _createDto.Location,
            Type = _createDto.Type,
            Capacity = _createDto.Capacity,
            CurrentOccupancy = _createDto.CurrentOccupancy,
            DockServices = new HashSet<CreateDockRelationDto>
            {
                new CreateDockRelationDto { DockCode = "D1", Distance = 0, IsServingDock = true }
            }
        };

        var result = await _controller.Create(dtoWithDock);
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_OnInvalidArgument()
    {
        _repoMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>()))
            .ReturnsAsync((StorageArea?)null);
        _repoMock.Setup(r => r.Add(It.IsAny<StorageArea>())).ThrowsAsync(new ArgumentException("Invalid"));

        var result = await _controller.Create(_createDto);
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Update_ShouldReturnOk_WhenValid()
    {
    var existing = new StorageArea(Guid.NewGuid(), new Code { Value = "SA001" }, new Designation { Value = "Loc" }, StorageAreaType.Yard, 5, 0, new HashSet<StorageArea.DockRelation>());
    _repoMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>())).ReturnsAsync(existing);
    _repoMock.Setup(r => r.Update(It.IsAny<StorageArea>())).ReturnsAsync((StorageArea sa) => sa);

        var result = await _controller.Update("SA001", _createDto);
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.IsType<StorageAreaDto>(ok.Value);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>())).ThrowsAsync(new EntityNotFoundException());

        var result = await _controller.Update("SA001", _createDto);
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_OnInvalidArgument()
    {
    var existing = new StorageArea(Guid.NewGuid(), new Code { Value = "SA001" }, new Designation { Value = "Loc" }, StorageAreaType.Yard, 5, 0, new HashSet<StorageArea.DockRelation>());
    _repoMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>())).ReturnsAsync(existing);
    _repoMock.Setup(r => r.Update(It.IsAny<StorageArea>())).ThrowsAsync(new ArgumentException("Invalid"));

        var result = await _controller.Update("SA001", _createDto);
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Update_ShouldReturnInternalServerError_OnOtherException()
    {
    var existing = new StorageArea(Guid.NewGuid(), new Code { Value = "SA001" }, new Designation { Value = "Loc" }, StorageAreaType.Yard, 5, 0, new HashSet<StorageArea.DockRelation>());
    _repoMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>())).ReturnsAsync(existing);
    _repoMock.Setup(r => r.Update(It.IsAny<StorageArea>())).ThrowsAsync(new Exception("Err"));

        var result = await _controller.Update("SA001", _createDto);
        var resultType = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, resultType.StatusCode);
    }

}
