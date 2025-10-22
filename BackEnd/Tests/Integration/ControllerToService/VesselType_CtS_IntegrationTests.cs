using Api.Application.Controllers;
using Api.Application.Services;
using Api.Application.DataTransfer;
using Moq;
using Api.Domain.IRepository;
using Microsoft.Extensions.Logging;
using Api.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Api.Domain.ValueObjects;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;

namespace Tests.Integration.ControllerToService;

public class VesselType_CtS_IntegrationTest
{
    private readonly VesselTypeController _controller;
    private readonly VesselTypeService _service;
    private readonly Mock<IVesselTypeRepository> _repositoryMock;

    public VesselType_CtS_IntegrationTest()
    {
        _repositoryMock = new Mock<IVesselTypeRepository>();
        _service = new VesselTypeService(_repositoryMock.Object,
            new Mock<ILogger<VesselTypeService>>().Object);
        _controller = new VesselTypeController(_service, new Mock<ILogger<VesselTypeController>>().Object);
    }

    [Fact]
    public async Task GetAllVesselTypes_ReturnsOkResult_WithListOfVesselTypes()
    {
        _repositoryMock.Setup(repo => repo.GetVesselTypesAsync())
            .ReturnsAsync(new List<VesselType>());

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<VesselTypeDto>>(okResult.Value);

        Assert.Empty(returnValue);
    }

    [Fact]
    public async Task GetVesselTypeByName_ReturnsOkResult_WithVesselType()
    {
        var testName = "Panamax";

        _repositoryMock.Setup(repo => repo.GetVesselTypeByNameAsync(testName))
            .ReturnsAsync(new VesselType(Guid.NewGuid(),
                new Designation { Value = testName },
                new Designation { Value = "Max size for Panama Canal" },
                20,
                10,
                5,
                new PhysicalCharacteristics
                {
                    Length = 300,
                    Depth = 15,
                    Draft = 12
                }
            ));

        var result = await _controller.GetByName(testName);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<VesselTypeDto>(okResult.Value);
        Assert.Equal(testName, returnValue.Name);
    }


    [Fact]
    public async Task GetVesselTypeByName_ReturnsNotFound_WhenVesselTypeDoesNotExist()
    {
        var testName = "nonexistentname";

        _repositoryMock.Setup(repo => repo.GetVesselTypeByNameAsync(testName))!
            .ReturnsAsync((VesselType?)null);

        var result = await _controller.GetByName(testName);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateVesselType_ReturnsCreatedAtActionResult_WithCreatedVesselType()
    {
        var newVesselTypeDto = new VesselTypeDto
        {
            Name = "Large Vessel",
            Description = "Description for new vessel type",
            MaxNumberOfRows = 25,
            MaxNumberOfBays = 12,
            MaxNumberOfTiers = 6,
            PhysicalCharacteristics = new PhysicalCharacteristics
            {
                Length = 350,
                Depth = 20,
                Draft = 15
            }
        };

        _repositoryMock.Setup(repo => repo.GetVesselTypeByNameAsync(newVesselTypeDto.Name))!
            .ReturnsAsync((VesselType?)null);

        _repositoryMock.Setup(repo => repo.Add(It.IsAny<VesselType>()))
            .ReturnsAsync((VesselType v) => v);

        var result = await _controller.Create(newVesselTypeDto);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<VesselTypeDto>(createdAtActionResult.Value);
        Assert.Equal(newVesselTypeDto.Name, returnValue.Name);
    }


    [Fact]
    public async Task CreateVesselType_ReturnsBadRequest_OnException()
    {
        var newVesselTypeDto = new VesselTypeDto
        {
            Name = "Large Vessel",
            Description = "Description for new vessel type",
            MaxNumberOfRows = 25,
            MaxNumberOfBays = 12,
            MaxNumberOfTiers = 6,
            PhysicalCharacteristics = new PhysicalCharacteristics
            {
                Length = 350,
                Depth = 20,
                Draft = 15
            }
        };

        _repositoryMock.Setup(repo => repo.Add(It.IsAny<VesselType>()))
            .ThrowsAsync(new System.Exception("Test exception"));
        var result = await _controller.Create(newVesselTypeDto);
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateVesselType_ReturnsUpdatedVesselType()
    {
        var nameToUpdate = "Panamax";
        var updateVesselTypeDto = new VesselTypeDto
        {
            Name = nameToUpdate,
            Description = "Updated Description",
            MaxNumberOfRows = 30,
            MaxNumberOfBays = 15,
            MaxNumberOfTiers = 7,
            PhysicalCharacteristics = new PhysicalCharacteristics
            {
                Length = 500,
                Depth = 35,
                Draft = 20
            }
        };

        _repositoryMock.Setup(repo => repo.GetVesselTypeByNameAsync(nameToUpdate))
            .ReturnsAsync(new VesselType(Guid.NewGuid(),
                new Designation { Value = nameToUpdate },
                new Designation { Value = "Existing Vessel" },
                30,
                15,
                7,
                new PhysicalCharacteristics
                {
                    Length = 300,
                    Depth = 25,
                    Draft = 10
                }
            )
            );

        _repositoryMock.Setup(repo => repo.Update(It.IsAny<VesselType>()))
            .ReturnsAsync((VesselType v) => v);

        var result = await _controller.Update(nameToUpdate, updateVesselTypeDto);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<VesselTypeDto>(okResult.Value);
        Assert.Equal(nameToUpdate, returnValue.Name);
        Assert.Equal("Updated Description", returnValue.Description);
    }

    [Fact]
    public async Task FilterVesselType_ReturnsOkResult_WithFilteredVesselTypes()
    {
        var filter = new VesselTypeFilter { };
        _repositoryMock.Setup(repo => repo.FilterVesselTypesAsync(filter))
            .ReturnsAsync(new Page<VesselType>
            {
                Items = new List<VesselType>(),
                PageNumber = 1,
                PageSize = 10,
            });

        var result = await _controller.Filter(filter);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<Page<VesselTypeDto>>(okResult.Value);
        Assert.Empty(returnValue.Items);
    }

    [Fact]
    public async Task FilterVesselType_ReturnsInternalServerError_OnException()
    {
        var filter = new VesselTypeFilter { };
        _repositoryMock.Setup(repo => repo.FilterVesselTypesAsync(filter))
            .ThrowsAsync(new System.Exception("Database error"));

        var result = await _controller.Filter(filter);

        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }
}