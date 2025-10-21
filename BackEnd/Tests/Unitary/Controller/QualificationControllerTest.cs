using Api.Application.Controllers;
using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Application.Exceptions;
using Api.Application.Services;
using Api.Infrastructure.Exceptions;
using Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Unitary.Controller;

public class QualificationControllerTest
{
    // Mocked qualification service to ensure isolation of controller tests
    private readonly Mock<IQualificationService> _qualificationServiceMock;
    private readonly QualificationController _controller;

    public QualificationControllerTest()
    {
        _qualificationServiceMock = new Mock<IQualificationService>();
        _controller = new QualificationController(_qualificationServiceMock.Object, new Mock<ILogger<QualificationController>>().Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithListOfQualifications()
    {
        _qualificationServiceMock.Setup(service => service.GetQualifications())
            .ReturnsAsync(new List<QualificationDto>());

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<QualificationDto>>(okResult.Value);
    }

    [Fact]
    public async Task GetAll_ReturnsInternalServerError_OnException()
    {
        _qualificationServiceMock.Setup(service => service.GetQualifications())
            .ThrowsAsync(new Exception("Test exception"));

        var result = await _controller.GetAll();
        var objectResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsOkResult_WithQualification()
    {
        _qualificationServiceMock.Setup(service => service.GetQualificationById(It.IsAny<string>()))
            .ReturnsAsync((string id) => new QualificationDto { IdCode = id, QualificationName = "Sample Qualification" });

        var testId = "test-id";

        var result = await _controller.GetById(testId);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<QualificationDto>(okResult.Value);
    }

    [Fact]
    public async Task GetById_ReturnsInternalServerError_OnException()
    {
        _qualificationServiceMock.Setup(service => service.GetQualificationById(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Test exception"));

        var result = await _controller.GetById("test-id");
        var objectResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenQualificationDoesNotExist()
    {
        _qualificationServiceMock.Setup(service => service.GetQualificationById(It.IsAny<string>()))
            .ThrowsAsync(new EntityNotFoundException("Qualification not found"));

        var result = await _controller.GetById("non-existent-id");
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsOkResult_WithCreatedQualification()
    {
        _qualificationServiceMock.Setup(service => service.Add(It.IsAny<QualificationDto>()))
            .ReturnsAsync((QualificationDto dto) => dto);

        var newQualification = new QualificationDto { IdCode = "new-id", QualificationName = "New Qualification" };

        var result = await _controller.Create(newQualification);

        var okResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<QualificationDto>(okResult.Value);
    }

    [Fact]
    public async Task Create_ReturnsInternalServerError_OnException()
    {
        _qualificationServiceMock.Setup(service => service.Add(It.IsAny<QualificationDto>()))
            .ThrowsAsync(new Exception("Test exception"));

        var newQualification = new QualificationDto { IdCode = "new-id", QualificationName = "New Qualification" };

        var result = await _controller.Create(newQualification);
        var objectResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsConflict_WhenQualificationAlreadyExists()
    {
        _qualificationServiceMock.Setup(service => service.Add(It.IsAny<QualificationDto>()))
            .ThrowsAsync(new EntityAlreadyExistsException("Qualification already exists"));

        var newQualification = new QualificationDto { IdCode = "new-id", QualificationName = "New Qualification" };
        var result = await _controller.Create(newQualification);
        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenCreationFails()
    {
        _qualificationServiceMock.Setup(service => service.Add(It.IsAny<QualificationDto>()))
            .ThrowsAsync(new ArgumentException("Invalid qualification data"));

        var newQualification = new QualificationDto { IdCode = "new-id", QualificationName = "New Qualification" };

        var result = await _controller.Create(newQualification);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Update_ReturnsNoContent_WithUpdatedQualification()
    {
        _qualificationServiceMock.Setup(service => service.Update(It.IsAny<string>(), It.IsAny<QualificationDto>()))
            .ReturnsAsync((string id, QualificationDto dto) => dto);

        var updatedQualification = new QualificationDto { IdCode = "existing-id", QualificationName = "Updated Qualification" };

        var result = await _controller.Update("existing-id", updatedQualification);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenQualificationDoesNotExist()
    {
        _qualificationServiceMock.Setup(service => service.Update(It.IsAny<string>(), It.IsAny<QualificationDto>()))
            .ThrowsAsync(new EntityNotFoundException("Qualification not found"));

        var updatedQualification = new QualificationDto { IdCode = "non-existent-id", QualificationName = "Updated Qualification" };

        var result = await _controller.Update("non-existent-id", updatedQualification);
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsInternalServerError_OnException()
    {
        _qualificationServiceMock.Setup(service => service.Update(It.IsAny<string>(), It.IsAny<QualificationDto>()))
            .ThrowsAsync(new Exception("Test exception"));

        var updatedQualification = new QualificationDto { IdCode = "existing-id", QualificationName = "Updated Qualification" };

        var result = await _controller.Update("existing-id", updatedQualification);
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_OnException()
    {
        _qualificationServiceMock.Setup(service => service.Update(It.IsAny<string>(), It.IsAny<QualificationDto>()))
            .ThrowsAsync(new ArgumentException("Invalid qualification data"));

        var updatedQualification = new QualificationDto { IdCode = "existing-id", QualificationName = "Updated Qualification" };

        var result = await _controller.Update("existing-id", updatedQualification);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Filter_ReturnsOkResult_WithFilteredQualifications()
    {
        _qualificationServiceMock.Setup(service => service.FilterQualifications(It.IsAny<QualificationFilter>()))
            .ReturnsAsync(new Page<QualificationDto> { Items = new List<QualificationDto>(), PageSize = 10, PageNumber = 1 });

        var filter = new QualificationFilter { PageNumber = 1, PageSize = 10 };

        var result = await _controller.Filter(filter);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<Page<QualificationDto>>(okResult.Value);
    }

    [Fact]
    public async Task Filter_ReturnsInternalServerError_OnException()
    {
        _qualificationServiceMock.Setup(service => service.FilterQualifications(It.IsAny<QualificationFilter>()))
            .ThrowsAsync(new Exception("Test exception"));

        var filter = new QualificationFilter { PageNumber = 1, PageSize = 10 };

        var result = await _controller.Filter(filter);
        var objectResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, objectResult.StatusCode);
    }
}