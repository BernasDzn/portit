using Api.Application.Controllers;
using Api.Application.DataTransfer;
using Api.Application.Services;
using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Integration.ControllerToService;

public class StorageArea_CtS_IntegrationTest
{
	// Service and Controller to test
	private readonly StorageAreaService _service;
	private readonly StorageAreaController _controller;

	// Repository mocks
	private readonly Mock<IStorageAreaRepository> _repoMock;
	private readonly Mock<IDockRepository> _dockRepoMock;

	// StorageArea mock objects
	private StorageArea _storageArea_mock;
	private CreateStorageAreaDto _createDto_mock;
	private StorageAreaDto _storageAreaDto_mock;

	public StorageArea_CtS_IntegrationTest()
	{
		_repoMock = new Mock<IStorageAreaRepository>();
		_dockRepoMock = new Mock<IDockRepository>();
		var loggerService = new Mock<ILogger<StorageAreaService>>();
		var loggerController = new Mock<ILogger<StorageAreaController>>();

		_service = new StorageAreaService(_repoMock.Object, _dockRepoMock.Object, loggerService.Object);
		_controller = new StorageAreaController(_service, loggerController.Object);

		_storageArea_mock = new StorageArea(
			Guid.NewGuid(),
			new Code { Value = "SA001" },
			new Designation { Value = "Loc" },
			StorageAreaType.Yard,
			10,
			0,
			new HashSet<StorageArea.DockRelation>()
		);
		_storageAreaDto_mock = _storageArea_mock.ToDTO();
		_createDto_mock = new CreateStorageAreaDto
		{
			NameCode = _storageArea_mock.NameCode.Value,
			Location = _storageArea_mock.Location.Value,
			Type = _storageArea_mock.AreaType,
			Capacity = _storageArea_mock.Capacity,
			CurrentOccupancy = _storageArea_mock.CurrentOccupancy,
			DockServices = new HashSet<CreateDockRelationDto>()
		};
	}

	[Fact]
	public async Task GetAll_ReturnsOkWithListOfStorageAreas()
	{
		_repoMock.Setup(r => r.GetStorageAreasAsync())
			.ReturnsAsync(new List<StorageArea> { _storageArea_mock });

		var result = await _controller.GetAll();
		var response = Assert.IsType<OkObjectResult>(result.Result);
		var value = Assert.IsType<List<StorageAreaDto>>(response.Value);
	}

	[Fact]
	public async Task GetAll_ShouldReturnInternalServerError_OnException()
	{
		_repoMock.Setup(r => r.GetStorageAreasAsync())
			.ThrowsAsync(new Exception("Test Exception"));

		var result = await _controller.GetAll();
		var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
		Assert.Equal(500, statusCodeResult.StatusCode);
	}

	[Fact]
	public async Task Get_ReturnsOk_WhenFound()
	{
		_repoMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>()))
			.ReturnsAsync(_storageArea_mock);

		var result = await _controller.Get(_storageArea_mock.NameCode.Value);
		var response = Assert.IsType<OkObjectResult>(result.Result);
		var value = Assert.IsType<StorageAreaDto>(response.Value);
	}

	[Fact]
	public async Task Get_ShouldReturnNotFound_WhenNotFound()
	{
		_repoMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>()))
			.ThrowsAsync(new Api.Application.Exceptions.EntityNotFoundException());

		var result = await _controller.Get(_storageArea_mock.NameCode.Value);
		Assert.IsType<NotFoundObjectResult>(result.Result);
	}

	[Fact]
	public async Task Get_ShouldReturnInternalServerError_OnException()
	{
		_repoMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>()))
			.ThrowsAsync(new Exception("Test Exception"));

		var result = await _controller.Get(_storageArea_mock.NameCode.Value);
		var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
		Assert.Equal(500, statusCodeResult.StatusCode);
	}

	[Fact]
	public async Task Create_ReturnsCreatedStorageArea()
	{
		_repoMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>()))
			.ReturnsAsync((StorageArea?)null);
		_repoMock.Setup(r => r.Add(It.IsAny<StorageArea>()))
			.ReturnsAsync(_storageArea_mock);

		var result = await _controller.Create(_createDto_mock);
		var response = Assert.IsType<CreatedAtActionResult>(result.Result);
		var value = Assert.IsType<StorageAreaDto>(response.Value);
		Assert.Equal(_createDto_mock.NameCode, value.NameCode);
	}

	[Fact]
	public async Task Create_ShouldReturnConflict_WhenAlreadyExists()
	{
		_repoMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>()))
			.ReturnsAsync(_storageArea_mock);

		var result = await _controller.Create(_createDto_mock);
		Assert.IsType<ConflictObjectResult>(result.Result);
	}

	[Fact]
	public async Task Create_ShouldReturnNotFound_WhenDockMissing()
	{
		_repoMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>()))
			.ReturnsAsync((StorageArea?)null);
		_dockRepoMock.Setup(d => d.GetDockByCodeAsync(It.IsAny<string>()))
			.ReturnsAsync((Dock?)null);

		var dtoWithDock = new CreateStorageAreaDto
		{
			NameCode = _createDto_mock.NameCode,
			Location = _createDto_mock.Location,
			Type = _createDto_mock.Type,
			Capacity = _createDto_mock.Capacity,
			CurrentOccupancy = _createDto_mock.CurrentOccupancy,
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
		_repoMock.Setup(r => r.Add(It.IsAny<StorageArea>()))
			.ThrowsAsync(new ArgumentException("Invalid"));

		var result = await _controller.Create(_createDto_mock);
		Assert.IsType<BadRequestObjectResult>(result.Result);
	}

	[Fact]
	public async Task Create_ShouldReturnInternalServerError_OnException()
	{
		_repoMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>()))
			.ReturnsAsync((StorageArea?)null);
		_repoMock.Setup(r => r.Add(It.IsAny<StorageArea>()))
			.ThrowsAsync(new Exception("Test Exception"));

		var result = await _controller.Create(_createDto_mock);
		var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
		Assert.Equal(500, statusCodeResult.StatusCode);
	}

	[Fact]
	public async Task Update_ReturnsOk_WithValidData()
	{
		_repoMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>()))
			.ReturnsAsync(_storageArea_mock);
		_repoMock.Setup(r => r.Update(It.IsAny<StorageArea>()))
			.ReturnsAsync(_storageArea_mock);

		var result = await _controller.Update(_storageArea_mock.NameCode.Value, _createDto_mock);
		var response = Assert.IsType<OkObjectResult>(result.Result);
		var value = Assert.IsType<StorageAreaDto>(response.Value);
	}

	[Fact]
	public async Task Update_ShouldReturnNotFound_WhenNotFound()
	{
		_repoMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>()))
			.ThrowsAsync(new Api.Application.Exceptions.EntityNotFoundException());

		var result = await _controller.Update(_storageArea_mock.NameCode.Value, _createDto_mock);
		Assert.IsType<NotFoundObjectResult>(result.Result);
	}

	[Fact]
	public async Task Update_ShouldReturnBadRequest_OnInvalidArgument()
	{
		_repoMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>()))
			.ReturnsAsync(_storageArea_mock);
		_repoMock.Setup(r => r.Update(It.IsAny<StorageArea>()))
			.ThrowsAsync(new ArgumentException("Invalid"));

		var result = await _controller.Update(_storageArea_mock.NameCode.Value, _createDto_mock);
		Assert.IsType<BadRequestObjectResult>(result.Result);
	}

	[Fact]
	public async Task Update_ShouldReturnInternalServerError_OnException()
	{
		_repoMock.Setup(r => r.GetStorageAreaByCodeAsync(It.IsAny<string>()))
			.ReturnsAsync(_storageArea_mock);
		_repoMock.Setup(r => r.Update(It.IsAny<StorageArea>()))
			.ThrowsAsync(new Exception("Test Exception"));

		var result = await _controller.Update(_storageArea_mock.NameCode.Value, _createDto_mock);
		var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
		Assert.Equal(500, statusCodeResult.StatusCode);
	}
}
