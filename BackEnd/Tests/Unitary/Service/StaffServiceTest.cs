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

public class StaffServiceTest
{

	private readonly StaffService _staffService;
	private readonly Mock<IStaffRepository> _staffRepositoryMock;
	private readonly Mock<IQualificationRepository> _qualificationRepositoryMock;
	private readonly Mock<ILogger<StaffService>> _loggerMock;
	private readonly CreateStaffDto _staff_dto_mock;
	private readonly Staff _staff_mock;
	private Qualification _qualification_mock = null!;
	private QualificationDto _qualification_dto_mock = null!;

	public StaffServiceTest()
	{
		_staffRepositoryMock = new Mock<IStaffRepository>();
		_qualificationRepositoryMock = new Mock<IQualificationRepository>();
		_loggerMock = new Mock<ILogger<StaffService>>();
		_staffService = new StaffService(_staffRepositoryMock.Object, _qualificationRepositoryMock.Object, _loggerMock.Object);
		_staff_dto_mock = CreateValidStaffDto();
		_staff_mock = CreateValidStaff();
	}

	private Staff CreateValidStaff()
	{
		_qualification_mock = new Qualification(Guid.NewGuid(), new Code { Value = "Q1" }, new Designation { Value = "Qualification 1" });
		return new Staff(
			new StaffMechanographicNumber { Value = "ST250001" },
			new Designation { Value = "John Test" },
			new Email { Value = "john.test@example.com" },
			new PhoneNumber { Value = "900000000" },
			OperationalWindow.FullWeek(),
			new List<Qualification>() { _qualification_mock }
		);
	}

	private CreateStaffDto CreateValidStaffDto()
	{
		_qualification_dto_mock = new QualificationDto { IdCode = "Q3", QualificationName = "Qualification 3" };
		return new CreateStaffDto
		{
			MechanographicNumber = "STF250001",
			Name = "John Test",
			Email = "john.test@example.com",
			PhoneNumber = "900000000",
			Status = 0,
			OperationalWindow = OperationalWindow.FullWeek(),
			QualificationsCodes = new List<string>() { _qualification_dto_mock.IdCode }
		};
	}

	[Fact]
	public async Task GetStaffs_WhenRequested_ReturnsStaffs()
	{
		_staffRepositoryMock.Setup(repo => repo.GetStaffsAsync())
			.ReturnsAsync(new List<Staff>());

		var Result = await _staffService.GetStaffs();
		Assert.NotNull(Result);
	}

	[Fact]
	public async Task Add_WhenStaffAlreadyExists_ThrowException()
	{
		_qualificationRepositoryMock.Setup(repo => repo.GetQualificationByIdAsync(It.IsAny<string>()))
			.ReturnsAsync(_qualification_mock);

		_staffRepositoryMock.Setup(repo => repo.Add(It.IsAny<Staff>()))
			.ThrowsAsync(new EntityAlreadyExistsException());

		await Assert.ThrowsAsync<EntityAlreadyExistsException>(() => _staffService.Create(_staff_dto_mock));
	}

	[Fact]
	public async Task Add_WhenQualificationDoesNotExist_ThrowException()
	{
		_qualificationRepositoryMock.Setup(repo => repo.GetQualificationByIdAsync(It.IsAny<string>()))
			.ReturnsAsync((Qualification?)null);

		_staffRepositoryMock.Setup(repo => repo.Add(It.IsAny<Staff>()))
			.ThrowsAsync(new EntityNotFoundException());

		await Assert.ThrowsAsync<EntityNotFoundException>(() => _staffService.Create(_staff_dto_mock));
	}

	[Fact]
	public async Task Add_WhenGenericException_ThrowException()
	{
		_qualificationRepositoryMock.Setup(repo => repo.GetQualificationByIdAsync(It.IsAny<string>()))
			.ReturnsAsync(_qualification_mock);
		
		_staffRepositoryMock.Setup(repo => repo.Add(It.IsAny<Staff>()))
			.ThrowsAsync(new Exception("Test Exception"));

		await Assert.ThrowsAsync<Exception>(() => _staffService.Create(_staff_dto_mock));
	}

	[Fact]
	public async Task Add_ReturnsStaff()
	{
		_qualificationRepositoryMock.Setup(repo => repo.GetQualificationByIdAsync(It.IsAny<string>()))
			.ReturnsAsync(_qualification_mock);

		_staffRepositoryMock.Setup(repo => repo.Add(It.IsAny<Staff>()))
			.ReturnsAsync((Staff s) => s);
		
		_staffRepositoryMock.Setup(repo => repo.FilterStaffsAsync(It.IsAny<StaffFilter>()))
			.ReturnsAsync(new Page<Staff> { Items = new List<Staff> { _staff_mock } });

		var Result = await _staffService.Create(_staff_dto_mock);

		Assert.NotNull(Result);
		Assert.Equal("STF250002", Result.MechanographicNumber);
	}

	[Fact]
	public async Task Update_ReturnsUpdatedStaff()
	{
		Staff _updated_staff_mock = _staff_mock;
		_updated_staff_mock.UpdateName("Updated Name");

		CreateStaffDto _updated_staff_dto_mock = _staff_dto_mock;
		_updated_staff_dto_mock.Name = "Updated Name";

		_qualificationRepositoryMock.Setup(repo => repo.GetQualificationByIdAsync(It.IsAny<string>()))
			.ReturnsAsync(_qualification_mock);

		_staffRepositoryMock.Setup(repo => repo.GetStaffByMecNumberAsync(It.IsAny<string>()))
			.ReturnsAsync(_staff_mock);

		_staffRepositoryMock.Setup(repo => repo.Update(It.IsAny<Staff>()))
			.ReturnsAsync(_updated_staff_mock);

		var Result = await _staffService.Update(_staff_dto_mock.MechanographicNumber, _updated_staff_dto_mock);

		Assert.NotNull(Result);
		Assert.Equal(_updated_staff_dto_mock.Name, Result.Name);
	}

	[Fact]
	public async Task Update_WhenStaffDoesNotExist_ThrowException()
	{
		_staffRepositoryMock.Setup(repo => repo.GetStaffByMecNumberAsync(It.IsAny<string>()))
			.ReturnsAsync((Staff?)null);

		await Assert.ThrowsAsync<EntityNotFoundException>(() => _staffService.Update(_staff_dto_mock.MechanographicNumber, _staff_dto_mock));
	}

	[Fact]
	public async Task Update_WhenQualificationDoesNotExist_ThrowException()
	{
		_staffRepositoryMock.Setup(repo => repo.GetStaffByMecNumberAsync(It.IsAny<string>()))
			.ReturnsAsync(_staff_mock);

		_qualificationRepositoryMock.Setup(repo => repo.GetQualificationByIdAsync(It.IsAny<string>()))
			.ReturnsAsync((Qualification?)null);

		await Assert.ThrowsAsync<EntityNotFoundException>(() => _staffService.Update(_staff_dto_mock.MechanographicNumber, _staff_dto_mock));
	}

	[Fact]
	public async Task Filter_ReturnFilteredStaffs()
	{
		var page = new Page<Staff>
		{
			Items = new List<Staff> { _staff_mock },
			PageNumber = 1,
			PageSize = 10
		};

		_staffRepositoryMock.Setup(repo => repo.FilterStaffsAsync(It.IsAny<StaffFilter>()))
			.ReturnsAsync(page);

		var Result = await _staffService.FilterStaffs(new StaffFilter());
		Assert.NotNull(Result);
		Assert.NotEmpty(Result.Items);
	}

	[Fact]
	public async Task Filter_ReturnNullPageWhenNoStaffsFound()
	{
		var page = new Page<Staff>
		{
			Items = new List<Staff>(),
			PageNumber = 1,
			PageSize = 10
		};

		_staffRepositoryMock.Setup(repo => repo.FilterStaffsAsync(It.IsAny<StaffFilter>()))
			.ReturnsAsync(page);

		var Result = await _staffService.FilterStaffs(new StaffFilter());
		Assert.NotNull(Result);
		Assert.Empty(Result.Items);
	}

	[Fact]
	public async Task Deactivate_ReturnsDeactivatedStaff()
	{
		_staffRepositoryMock.Setup(repo => repo.GetStaffByMecNumberAsync(It.IsAny<string>()))
			.ReturnsAsync(_staff_mock);

		_staffRepositoryMock.Setup(repo => repo.Update(It.IsAny<Staff>()))
			.ReturnsAsync((Staff s) => s);

		var Result = await _staffService.Deactivate(_staff_dto_mock.MechanographicNumber);
		Assert.NotNull(Result);
		Assert.Equal(_staff_dto_mock.MechanographicNumber, Result.MechanographicNumber);
	}

	[Fact]
	public async Task Deactivate_ThrowsException_WhenStaffNotFound()
	{
		_staffRepositoryMock.Setup(repo => repo.GetStaffByMecNumberAsync(It.IsAny<string>()))
			.ReturnsAsync((Staff?)null);

		await Assert.ThrowsAsync<EntityNotFoundException>(() => _staffService.Deactivate(_staff_dto_mock.MechanographicNumber));
	}


}