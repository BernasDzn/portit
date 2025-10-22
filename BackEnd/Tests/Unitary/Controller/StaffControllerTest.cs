using Api.Application.Controllers;
using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Application.Exceptions;
using Api.Application.Services;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Unitary.Controller;

public class StaffControllerTest
{

	private readonly Mock<IStaffService> _service_mock;
	private readonly StaffController _controller;
	private CreateStaffDto _staffdto = new CreateStaffDto();

	public StaffControllerTest()
	{
		_service_mock = new Mock<IStaffService>();
		_controller = new StaffController(_service_mock.Object, new Mock<ILogger<StaffController>>().Object);

	}

	[Fact]
	public async Task GetAll_ShouldReturnOk_WithListOfStaffs()
	{
		_service_mock.Setup(service => service.GetStaffs())
			.ReturnsAsync(new List<StaffDto>());

		var Result = await _controller.GetAll();

		Assert.IsType<OkObjectResult>(Result.Result);
		Assert.IsType<List<StaffDto>>(((OkObjectResult)Result.Result).Value);
	}

	[Fact]
	public async Task GetAll_ShouldReturnBadRequest_OnException()
	{
		_service_mock.Setup(service => service.GetStaffs())
			.ThrowsAsync(new Exception("Test Exception"));

		var Result = await _controller.GetAll();

		Assert.IsType<BadRequestObjectResult>(Result.Result);
	}

	[Fact]
	public async Task Create_ShouldReturnOk_WithValidData()
	{
		var expectedDto = new StaffDto();
		_service_mock.Setup(service => service.Add(It.IsAny<CreateStaffDto>()))
			.ReturnsAsync(expectedDto);

		var Result = await _controller.Create(_staffdto);

		Assert.IsType<CreatedAtActionResult>(Result.Result);
		Assert.IsType<StaffDto>(((CreatedAtActionResult)Result.Result).Value);
	}

	[Fact]
	public async Task Create_ShouldReturnBadRequest_WithInvalidData()
	{
		_service_mock.Setup(service => service.Add(It.IsAny<CreateStaffDto>()))
			.ThrowsAsync(new Exception("Test Exception"));

		var Result = await _controller.Create(_staffdto);

		Assert.IsType<BadRequestObjectResult>(Result.Result);
	}

	[Fact]
	public async Task Create_ShouldReturnBadRequest_WhenServiceReturnsNull()
	{
		_service_mock.Setup(service => service.Add(It.IsAny<CreateStaffDto>()))
			.ReturnsAsync((StaffDto?)null);

		var Result = await _controller.Create(_staffdto);

		Assert.IsType<BadRequestObjectResult>(Result.Result);
	}

	[Fact]
	public async Task Create_ShouldReturnBadRequest_OnEntityAlreadyExists()
	{
		_service_mock.Setup(service => service.Add(It.IsAny<CreateStaffDto>()))
			.ThrowsAsync(new Api.Infrastructure.Exceptions.EntityAlreadyExistsException("Exists"));

		var Result = await _controller.Create(_staffdto);

		Assert.IsType<BadRequestObjectResult>(Result.Result);
	}

	[Fact]
	public async Task Deactivate_ShouldReturnOk_WithValidData()
	{
		_service_mock.Setup(service => service.Deactivate(It.IsAny<string>()))
			.ReturnsAsync(new StaffDto());

		var Result = await _controller.Deactivate("123");

		Assert.IsType<OkObjectResult>(Result);
		Assert.IsType<StaffDto>(((OkObjectResult)Result).Value);
	}

	[Fact]
	public async Task Deactivate_ShouldReturnBadRequest_OnException()
	{
		_service_mock.Setup(service => service.Deactivate(It.IsAny<string>()))
			.ThrowsAsync(new Exception("Test Exception"));

		var Result = await _controller.Deactivate("123");

		Assert.IsType<BadRequestObjectResult>(Result);
	}

	[Fact]
	public async Task Deactivate_ShouldReturnBadRequest_WithInvalidData()
	{
		_service_mock.Setup(service => service.Deactivate(It.IsAny<string>()))
			.ReturnsAsync((StaffDto?)null);

		var Result = await _controller.Deactivate("123");

		Assert.IsType<BadRequestObjectResult>(Result);
	}

	[Fact]
	public async Task Update_ShouldReturnOk_WithValidData()
	{

		var expectedDto = new StaffDto();

	_service_mock.Setup(service => service.Update(It.IsAny<string>(), It.IsAny<CreateStaffDto>()))
			.ReturnsAsync(expectedDto);

	var Result = await _controller.Update("123", _staffdto);

	Assert.IsType<OkObjectResult>(Result.Result);
	Assert.IsType<StaffDto>(((OkObjectResult)Result.Result).Value);
}

	[Fact]
	public async Task Update_ShouldReturnBadRequest_OnException()
	{
		_service_mock.Setup(service => service.Update(It.IsAny<string>(), It.IsAny<CreateStaffDto>()))
			.ThrowsAsync(new Exception("Test Exception"));

		var Result = await _controller.Update("123", _staffdto);
		Assert.IsType<BadRequestObjectResult>(Result.Result);
	}

	[Fact]
	public async Task Update_ShouldReturnBadRequest_WithInvalidData()
	{
		_service_mock.Setup(service => service.Update(It.IsAny<string>(), It.IsAny<CreateStaffDto>()))
			.ReturnsAsync((StaffDto?)null);

		var Result = await _controller.Update("123", _staffdto);
		Assert.IsType<BadRequestObjectResult>(Result.Result);
	}

	[Fact]
	public async Task Filter_ShouldReturnOk_WhenFound()
	{
		_service_mock.Setup(service => service.FilterStaffs(It.IsAny<StaffFilter>()))
			.ReturnsAsync(new Page<StaffDto>());

		var Result = await _controller.Filter(new StaffFilter());
		Assert.IsType<OkObjectResult>(Result.Result);
		Assert.IsType<Page<StaffDto>>(((OkObjectResult)Result.Result).Value);
	}

	[Fact]
	public async Task Filter_ShouldReturnNotFound_WhenNotFound()
	{
		_service_mock.Setup(service => service.FilterStaffs(It.IsAny<StaffFilter>()))
			.ThrowsAsync(new EntityNotFoundException());

		var Result = await _controller.Filter(new StaffFilter());
		Assert.IsType<NotFoundResult>(Result.Result);
	}

	[Fact]
	public async Task Filter_ShouldReturnBadRequest_WhenOtherException()
	{
		_service_mock.Setup(service => service.FilterStaffs(It.IsAny<StaffFilter>()))
			.ThrowsAsync(new Exception());

		var Result = await _controller.Filter(new StaffFilter());
		Assert.IsType<BadRequestResult>(Result.Result);
	}

}