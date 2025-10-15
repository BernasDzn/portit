using Api.Application.Controllers;
using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Application.Services;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Utilities;
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
                Type = null,
                Owner = null,
                PhysicalCharacteristics = null
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
}