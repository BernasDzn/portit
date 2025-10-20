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
	public async Task Create_ReturnsCreatedStaff()
	{
		_qualRepoMock.Setup(r => r.GetQualificationByIdAsync(It.IsAny<string>()))
			.ReturnsAsync(_qualification_mock);
		
		_staffRepoMock.Setup(r => r.Add(It.IsAny<Staff>()))
			.ReturnsAsync(_staff_mock);

		var result = await _controller.Create(_createStaffDto_mock);
		var response = Assert.IsType<CreatedAtActionResult>(result.Result);
		var value = Assert.IsType<StaffDto>(response.Value);
		Assert.Equal(_createStaffDto_mock.MechanograficNumber, value.MechanograficNumber);
	}

	[Fact]
	public async Task Create_WhenStaffExists_ThrowsException()
	{
		_qualRepoMock.Setup(repo => repo.GetQualificationByIdAsync(It.IsAny<string>()))
			.ReturnsAsync(_qualification_mock);

		_staffRepoMock.Setup(repo => repo.GetStaffByMecNumberAsync(It.IsAny<string>()))
			.ReturnsAsync(_staff_mock);

		var result = await _controller.Create(_createStaffDto_mock);
		Assert.IsType<BadRequestObjectResult>(result.Result);
	}

}
