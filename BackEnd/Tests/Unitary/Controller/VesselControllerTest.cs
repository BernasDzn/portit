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

public class VesselControllerTest
{
    // Mocked vessel service to ensure isolation of controller tests
    private readonly Mock<IVesselService> _vesselServiceMock;
    private readonly VesselController _controller;

    public VesselControllerTest()
    {
        _vesselServiceMock = new Mock<IVesselService>();
        _controller = new VesselController(_vesselServiceMock.Object, new Mock<ILogger<VesselController>>().Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithListOfVessels()
    {
        _vesselServiceMock.Setup(service => service.GetVessels())
            .ReturnsAsync(new List<VesselDto>());

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<VesselDto>>(okResult.Value);
        Assert.Empty(returnValue);
    }

    [Fact]
    public async Task GetById_ReturnsOkResult_WithVessel()
    {
        var testImo = "IMO1234567";

        _vesselServiceMock.Setup(service => service.GetByImo(It.IsAny<string>()))
            .ReturnsAsync((string id) => new VesselDto
            {
                ImoNumber = id,
                Name = "Sample Vessel",
                Type = null!,
                Owner = null!,
                Length = 100,
                Depth = 50,
                Draft = 30,
            });

        var result = await _controller.GetByImo(testImo);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<VesselDto>(okResult.Value);
        Assert.Equal(testImo, returnValue.ImoNumber);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenVesselDoesNotExist()
    {
        _vesselServiceMock.Setup(service => service.GetByImo(It.IsAny<string>()))
            .ReturnsAsync((VesselDto?)null);

        var result = await _controller.GetByImo("non-existent-id");

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtActionResult_WithCreatedVessel()
    {
        var vesselDto = new VesselDto
        {
            ImoNumber = "IMO1234567",
            Name = "New Vessel",
            Type = null!,
            Owner = null!,
            Length = 100,
            Depth = 50,
            Draft = 30,
        };

        _vesselServiceMock.Setup(service => service.Add(It.IsAny<VesselDto>()))
            .ReturnsAsync(vesselDto);

        var result = await _controller.Create(vesselDto);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<VesselDto>(createdAtActionResult.Value);
        Assert.Equal(vesselDto.ImoNumber, returnValue.ImoNumber);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_OnException()
    {
        var vesselDto = new VesselDto
        {
            ImoNumber = "IMO1234567",
            Name = "New Vessel",
            Type = null!,
            Owner = null!,
            Length = 100,
            Depth = 50,
            Draft = 30,
        };

        _vesselServiceMock.Setup(service => service.Add(It.IsAny<VesselDto>()))
            .ThrowsAsync(new Exception("Test exception"));

        var result = await _controller.Create(vesselDto);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task FilterVessels_ReturnsOkResult_WithPagedVessels()
    {
        var filter = new VesselFilter { PageNumber = 1, PageSize = 10 };

        _vesselServiceMock.Setup(service => service.FilterVessels(It.IsAny<VesselFilter>()))
            .ReturnsAsync(new Page<VesselDto>
            {
                Items = new List<VesselDto>
                {
                    new VesselDto { ImoNumber = "IMO1234567", Name = "Vessel 1", Type = null!, Owner = null!, Length = 100,
                    Depth = 50,
                    Draft = 30, },
                    new VesselDto { ImoNumber = "IMO2345678", Name = "Vessel 2", Type = null!, Owner = null!, Length = 100,
                    Depth = 50,
                    Draft = 30, }
                },
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            });

        var result = await _controller.Filter(filter);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<Page<VesselDto>>(okResult.Value);
        Assert.Equal(2, returnValue.Items.Count());
        Assert.Equal(filter.PageNumber, returnValue.PageNumber);
        Assert.Equal(filter.PageSize, returnValue.PageSize);
    }

    [Fact]
    public async Task FilterVessels_ReturnsNotFound_WhenNoVesselsMatchFilter()
    {
        var filter = new VesselFilter { PageNumber = 1, PageSize = 10 };

        _vesselServiceMock.Setup(service => service.FilterVessels(It.IsAny<VesselFilter>()))
            .ReturnsAsync(Page<VesselDto>.Empty());

        var result = await _controller.Filter(filter);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
    }
}