using Api.Application.Controllers;
using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Application.Services;
using Api.Infrastructure.Utilities;
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
        Assert.Empty(returnValue);
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
        Assert.Equal(testId, returnValue.IdCode);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenQualificationDoesNotExist()
    {
        _qualificationServiceMock.Setup(service => service.GetQualificationById(It.IsAny<string>()))
            .ReturnsAsync((QualificationDto?)null);

        var result = await _controller.GetById("non-existent-id");

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("No qualification found with id: non-existent-id", notFoundResult.Value);
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
        Assert.Equal("new-id", returnValue.IdCode);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenCreationFails()
    {
        _qualificationServiceMock.Setup(service => service.Add(It.IsAny<QualificationDto>()))
            .ReturnsAsync((QualificationDto?)null);

        var newQualification = new QualificationDto { IdCode = "new-id", QualificationName = "New Qualification" };

        var result = await _controller.Create(newQualification);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Could not create qualification", badRequestResult.Value);
    }

    [Fact]
    public async Task Update_ReturnsOkResult_WithUpdatedQualification()
    {
        _qualificationServiceMock.Setup(service => service.Update(It.IsAny<string>(), It.IsAny<QualificationDto>()))
            .ReturnsAsync((string id, QualificationDto dto) => dto);

        var updatedQualification = new QualificationDto { IdCode = "existing-id", QualificationName = "Updated Qualification" };

        var result = await _controller.Update("existing-id", updatedQualification);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<QualificationDto>(okResult.Value);
        Assert.Equal("existing-id", returnValue.IdCode);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenUpdateFails()
    {
        _qualificationServiceMock.Setup(service => service.Update(It.IsAny<string>(), It.IsAny<QualificationDto>()))
            .ReturnsAsync((QualificationDto?)null);

        var updatedQualification = new QualificationDto { IdCode = "existing-id", QualificationName = "Updated Qualification" };

        var result = await _controller.Update("existing-id", updatedQualification);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Could not update qualification", badRequestResult.Value);
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
        Assert.Empty(returnValue.Items);
    }
}