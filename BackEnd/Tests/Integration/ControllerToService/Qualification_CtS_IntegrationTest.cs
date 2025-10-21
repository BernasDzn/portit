using Xunit;
using Api.Application.Controllers;
using Api.Application.Services;
using Api.Application.DataTransfer;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;
using Api.Infrastructure.Persistence.Repositories;
using Api.Infrastructure.Persistence;
using Moq;
using Api.Domain.IRepository;
using Microsoft.Extensions.Logging;
using Api.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Api.Domain.ValueObjects;

namespace Tests.Integration.ControllerToService;

public class Qualification_CtS_IntegrationTest
{
    private readonly QualificationController _controller;
    private readonly QualificationService _service;
    private readonly Mock<IQualificationRepository> _repositoryMock;

    public Qualification_CtS_IntegrationTest()
    {
        _repositoryMock = new Mock<IQualificationRepository>();
        _service = new QualificationService(_repositoryMock.Object, new Mock<ILogger<QualificationService>>().Object);
        _controller = new QualificationController(_service, new Mock<ILogger<QualificationController>>().Object);
    }

    [Fact]
    public async Task GetAllQualifications_ReturnsOkResult_WithListOfQualifications()
    {
        _repositoryMock.Setup(repo => repo.GetQualificationsAsync())
            .ReturnsAsync(new List<Qualification>());

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<QualificationDto>>(okResult.Value);
        Assert.Empty(returnValue);
    }

    [Fact]
    public async Task GetQualificationById_ReturnsOkResult_WithQualification()
    {
        var testId = "testid";
        _repositoryMock.Setup(repo => repo.GetQualificationByIdAsync(testId))
            .ReturnsAsync(new Qualification(Guid.NewGuid(), new Code { Value = testId }, new Designation { Value = "Sample Qualification" }));

        var result = await _controller.GetById(testId);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<QualificationDto>(okResult.Value);
        Assert.Equal(testId, returnValue.IdCode);
    }

    [Fact]
    public async Task GetQualificationById_ReturnsNotFound_WhenQualificationDoesNotExist()
    {
        var testId = "nonexistentid";
        _repositoryMock.Setup(repo => repo.GetQualificationByIdAsync(testId))
            .ReturnsAsync((Qualification?)null);

        var result = await _controller.GetById(testId);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task FilterQualifications_ReturnsOkResult_WithFilteredQualifications()
    {
        var filter = new QualificationFilter { };
        _repositoryMock.Setup(repo => repo.FilterQualificationsAsync(filter))
            .ReturnsAsync(new Page<Qualification>
            {
                Items = new List<Qualification>(),
                PageNumber = 1,
                PageSize = 10,
            });

        var result = await _controller.Filter(filter);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<Page<QualificationDto>>(okResult.Value);
        Assert.Empty(returnValue.Items);
    }

    [Fact]
    public async Task AddQualification_ReturnsCreatedQualification()
    {
        var qualificationDto = new QualificationDto
        {
            IdCode = "newid",
            QualificationName = "New Qualification"
        };

        _repositoryMock.Setup(repo => repo.GetQualificationByIdAsync(qualificationDto.IdCode))
            .ReturnsAsync((Qualification?)null);
        _repositoryMock.Setup(repo => repo.Add(It.IsAny<Qualification>()))
            .ReturnsAsync((Qualification q) => q);

        var result = await _controller.Create(qualificationDto);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<QualificationDto>(createdAtActionResult.Value);
        Assert.Equal(qualificationDto.IdCode, returnValue.IdCode);
    }

    [Fact]
    public async Task UpdateQualification_ReturnsUpdatedQualification()
    {
        var qualificationDto = new QualificationDto
        {
            IdCode = "existingid",
            QualificationName = "Updated Qualification"
        };

        _repositoryMock.Setup(repo => repo.GetQualificationByIdAsync(qualificationDto.IdCode))
            .ReturnsAsync(new Qualification(Guid.NewGuid(), new Code { Value = qualificationDto.IdCode }, new Designation { Value = "Old Qualification" }));
        _repositoryMock.Setup(repo => repo.Update(It.IsAny<Qualification>()))
            .ReturnsAsync((Qualification q) => q);

        var result = await _controller.Update(qualificationDto.IdCode, qualificationDto);
        var okResult = Assert.IsType<NoContentResult>(result);
    }
}
