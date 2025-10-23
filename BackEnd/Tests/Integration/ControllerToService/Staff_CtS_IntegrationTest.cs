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

namespace Tests.Integration.ControllerToService;

public class Staff_CtS_IntegrationTest
{
	// Service and Controller to test
	private readonly StaffService _staffService;
	private readonly StaffController _controller;

	// Repository mocks
	private readonly Mock<IStaffRepository> _staffRepoMock;
	private readonly Mock<IQualificationRepository> _qualRepoMock;

	// Staff mock objects
	private Staff _staff_mock;
	private CreateStaffDto _createStaffDto_mock;
	private StaffDto _staffDto_mock;

	// Qualification mock objects
	private Qualification _qualification_mock;
	private QualificationDto _qualificationDto_mock;

	public Staff_CtS_IntegrationTest()
	{
		_staffRepoMock = new Mock<IStaffRepository>();
		_qualRepoMock = new Mock<IQualificationRepository>();

		var loggerService = new Mock<ILogger<StaffService>>();
		var loggerController = new Mock<ILogger<StaffController>>();

		_staffService = new StaffService(_staffRepoMock.Object, _qualRepoMock.Object, loggerService.Object);
		_controller = new StaffController(_staffService, loggerController.Object);

		_qualification_mock = new Qualification(
			Guid.NewGuid(),
			new Code { Value = "QUAL001" },
			new Designation { Value = "Qualification Test" }
		);
		_qualificationDto_mock = _qualification_mock.ToDTO();

		_staff_mock = new Staff(
			new StaffMechanograficNumber { Value = "INTMEC001" },
			new Designation { Value = "Integration Test" },
			new Email { Value = "int.test@example.com" },
			new PhoneNumber { Value = "900000001" },
			OperationalWindow.FullWeek(),
			new List<Qualification>() { _qualification_mock }
		);
		_staffDto_mock = _staff_mock.ToDTO();
		_createStaffDto_mock = new CreateStaffDto
		{
			MechanograficNumber = _staff_mock.MechanograficNumber.Value,
			Name = _staff_mock.Name.Value,
			Email = _staff_mock.Email.Value,
			PhoneNumber = _staff_mock.PhoneNumber.Value,
			Status = (int)_staff_mock.Status,
			OperationalWindow = _staff_mock.OperationalWindow,
			QualificationsCodes = _staff_mock.Qualifications.Select(q => q.NameCode.Value).ToList()
		};
	}

	[Fact]
	public async Task GetAll_ReturnsOkWithListOfStaffs()
	{
		_staffRepoMock.Setup(r => r.GetStaffsAsync())
			.ReturnsAsync(new List<Staff> { _staff_mock });

		var result = await _controller.GetAll();
		var response = Assert.IsType<OkObjectResult>(result.Result);
		var value = Assert.IsType<List<StaffDto>>(response.Value);
	}

	[Fact]
	public async Task GetAll_ShouldReturnInternalServerError_OnException()
	{
		// Arrange repository to throw so service will bubble up as general exception
		_staffRepoMock.Setup(r => r.GetStaffsAsync())
			.ThrowsAsync(new Exception("Test Exception"));

		// Act
		var result = await _controller.GetAll();

		// Controller maps unexpected exceptions to 500
		var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
		Assert.Equal(500, statusCodeResult.StatusCode);
	}

	[Fact]
	public async Task Create_ReturnsCreatedStaff()
	{
		_qualRepoMock.Setup(r => r.GetQualificationByIdAsync(It.IsAny<string>()))
			.ReturnsAsync(_qualification_mock);

		_staffRepoMock.Setup(r => r.GetStaffByMecNumberAsync(It.IsAny<string>()))
			.ReturnsAsync((Staff?)null);

		_staffRepoMock.Setup(r => r.Add(It.IsAny<Staff>()))
			.ReturnsAsync(_staff_mock);

		var result = await _controller.Create(_createStaffDto_mock);
		var response = Assert.IsType<CreatedAtActionResult>(result.Result);
		var value = Assert.IsType<StaffDto>(response.Value);
		Assert.Equal(_createStaffDto_mock.MechanograficNumber, value.MechanograficNumber);
	}

	[Fact]
	public async Task Create_WhenStaffExists_ReturnsBadRequest()
	{
		_qualRepoMock.Setup(repo => repo.GetQualificationByIdAsync(It.IsAny<string>()))
			.ReturnsAsync(_qualification_mock);

		// Existing staff found -> controller should return BadRequest
		_staffRepoMock.Setup(repo => repo.GetStaffByMecNumberAsync(It.IsAny<string>()))
			.ReturnsAsync(_staff_mock);

		var result = await _controller.Create(_createStaffDto_mock);
		Assert.IsType<ConflictObjectResult>(result.Result);
	}

	[Fact]
	public async Task Create_ShouldReturnInternalServerError_OnRepoException()
	{
		_qualRepoMock.Setup(r => r.GetQualificationByIdAsync(It.IsAny<string>()))
			.ReturnsAsync(_qualification_mock);

		_staffRepoMock.Setup(r => r.GetStaffByMecNumberAsync(It.IsAny<string>()))
			.ReturnsAsync((Staff?)null);

		// Simulate repository throwing unexpected exception
		_staffRepoMock.Setup(r => r.Add(It.IsAny<Staff>()))
			.ThrowsAsync(new Exception("Test Exception"));

		var result = await _controller.Create(_createStaffDto_mock);

		// Controller maps unexpected exceptions to 500
		var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
		Assert.Equal(500, statusCodeResult.StatusCode);
	}

	[Fact]
	public async Task Create_ShouldReturnNotFound_WhenQualificationNotFound()
	{
		// No existing staff
		_staffRepoMock.Setup(r => r.GetStaffByMecNumberAsync(It.IsAny<string>()))
			.ReturnsAsync((Staff?)null);

		// Qualification lookup returns null -> controller should return BadRequest
		_qualRepoMock.Setup(r => r.GetQualificationByIdAsync(It.IsAny<string>()))
			.ReturnsAsync((Qualification?)null);

		var result = await _controller.Create(_createStaffDto_mock);
		Assert.IsType<NotFoundObjectResult>(result.Result);
	}

	[Fact]
	public async Task Deactivate_ReturnsOk_WithValidData()
	{
		_staffRepoMock.Setup(r => r.GetStaffByMecNumberAsync(It.IsAny<string>()))
			.ReturnsAsync(_staff_mock);

		_staffRepoMock.Setup(r => r.Update(It.IsAny<Staff>()))
			.ReturnsAsync(_staff_mock);

		var result = await _controller.Deactivate(_staff_mock.MechanograficNumber.Value);
		var response = Assert.IsType<OkObjectResult>(result);
		var value = Assert.IsType<StaffDto>(response.Value);
	}

	[Fact]
	public async Task Deactivate_ShouldReturnInternalServerError_OnException()
	{
		_staffRepoMock.Setup(r => r.GetStaffByMecNumberAsync(It.IsAny<string>()))
			.ThrowsAsync(new Exception("Test Exception"));

		var result = await _controller.Deactivate(_staff_mock.MechanograficNumber.Value);

		var statusCodeResult = Assert.IsType<ObjectResult>(result);
		Assert.Equal(500, statusCodeResult.StatusCode);
	}

	[Fact]
	public async Task Deactivate_ShouldReturnNotFound_WhenStaffNotFound()
	{
		_staffRepoMock.Setup(r => r.GetStaffByMecNumberAsync(It.IsAny<string>()))
			.ReturnsAsync((Staff?)null);

		var result = await _controller.Deactivate(_staff_mock.MechanograficNumber.Value);
		Assert.IsType<NotFoundObjectResult>(result);
	}

	[Fact]
	public async Task Update_ReturnsOk_WithValidData()
	{
		_qualRepoMock.Setup(r => r.GetQualificationByIdAsync(It.IsAny<string>()))
			.ReturnsAsync(_qualification_mock);

		_staffRepoMock.Setup(r => r.GetStaffByMecNumberAsync(It.IsAny<string>()))
			.ReturnsAsync(_staff_mock);

		_staffRepoMock.Setup(r => r.Update(It.IsAny<Staff>()))
			.ReturnsAsync(_staff_mock);

		var result = await _controller.Update(_staff_mock.MechanograficNumber.Value, _createStaffDto_mock);
		var response = Assert.IsType<OkObjectResult>(result.Result);
		var value = Assert.IsType<StaffDto>(response.Value);
	}

	[Fact]
	public async Task Update_ShouldReturnInternalServerError_OnException()
	{
		_staffRepoMock.Setup(r => r.GetStaffByMecNumberAsync(It.IsAny<string>()))
			.ThrowsAsync(new Exception("Test Exception"));

		var result = await _controller.Update(_staff_mock.MechanograficNumber.Value, _createStaffDto_mock);

		var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
		Assert.Equal(500, statusCodeResult.StatusCode);
	}

	[Fact]
	public async Task Update_ShouldReturnNotFound_WhenStaffNotFound()
	{
		_staffRepoMock.Setup(r => r.GetStaffByMecNumberAsync(It.IsAny<string>()))
			.ReturnsAsync((Staff?)null);

		var result = await _controller.Update(_staff_mock.MechanograficNumber.Value, _createStaffDto_mock);
		Assert.IsType<NotFoundObjectResult>(result.Result);
	}

	[Fact]
	public async Task Filter_ShouldReturnOk_WhenFound()
	{
		var page = Page<Staff>.Of(new List<Staff> { _staff_mock }, new Pageable { PageNumber = 1, PageSize = 10 });
		_staffRepoMock.Setup(r => r.FilterStaffsAsync(It.IsAny<StaffFilter>()))
			.ReturnsAsync(page);

		var result = await _controller.Filter(new StaffFilter());
		var response = Assert.IsType<OkObjectResult>(result.Result);
		var value = Assert.IsType<Page<StaffDto>>(response.Value);
	}

	[Fact]
	public async Task Filter_ShouldReturnInternalServerError_WhenNotFound()
	{
		// Service/repository throws application-level EntityNotFoundException -> controller returns NotFound
		_staffRepoMock.Setup(r => r.FilterStaffsAsync(It.IsAny<StaffFilter>()))
			.ThrowsAsync(new Api.Application.Exceptions.EntityNotFoundException());

		var result = await _controller.Filter(new StaffFilter());

		var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
		Assert.Equal(500, statusCodeResult.StatusCode);
	}

	[Fact]
	public async Task Filter_ShouldReturnInternalServerError_WhenOtherException()
	{
		_staffRepoMock.Setup(r => r.FilterStaffsAsync(It.IsAny<StaffFilter>()))
			.ThrowsAsync(new Exception("Test Exception"));

		var result = await _controller.Filter(new StaffFilter());

		var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
		Assert.Equal(500, statusCodeResult.StatusCode);
	}
	
}
