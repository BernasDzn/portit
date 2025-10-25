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
using Api.Infrastructure.Exceptions;

namespace Tests.Unitary.Controller;

public class VesselControllerTest
{
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
    public async Task GetByImo_ReturnsOkResult_WithVessel()
    {
        var testImo = "IMO1234567";
        var vesselDto = new VesselDto
        {
            Name = "TestVessel",
            ImoNumber = testImo,
            Type = null!,
            Owner = null!,
            PhysicalCharacteristics = null!
        };
        _vesselServiceMock.Setup(service => service.GetByImo(It.IsAny<string>()))
            .ReturnsAsync(vesselDto);

        var result = await _controller.GetByImo(testImo);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<VesselDto>(okResult.Value);
        Assert.Equal(testImo, returnValue.ImoNumber);
    }

    [Fact]
    public async Task GetByImo_ReturnsNotFound_WhenVesselDoesNotExist()
    {
        var testImo = "NONEXISTENT";
        _vesselServiceMock.Setup(service => service.GetByImo(It.IsAny<string>()))
            .ThrowsAsync(new EntityNotFoundException("Vessel not found"));

        var result = await _controller.GetByImo(testImo);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetByImo_ReturnsBadRequest_OnException()
    {
        var testImo = "IMO1234567";
        _vesselServiceMock.Setup(service => service.GetByImo(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Test exception"));

        var result = await _controller.GetByImo(testImo);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtActionResult_WhenVesselIsCreated()
    {
        var newVessel = new CreateVesselDto
        {
            Name = "NewVessel",
            ImoNumber = "IMO7654321",
            Type = "TypeA",
            Owner = "OwnerA",
            Length = 100,
            Depth = 20,
            Draft = 10
        };
        var vesselDto = new VesselDto
        {
            Name = newVessel.Name,
            ImoNumber = newVessel.ImoNumber,
            Type = null!,
            Owner = null!,
            PhysicalCharacteristics = null!
        };
        _vesselServiceMock.Setup(service => service.Add(It.IsAny<CreateVesselDto>()))
            .ReturnsAsync(vesselDto);

        var result = await _controller.Create(newVessel);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<VesselDto>(createdAtActionResult.Value);
        Assert.Equal(newVessel.Name, returnValue.Name);
    }

    [Fact]
    public async Task Create_ReturnsConflict_WhenEntityAlreadyExists()
    {
        var newVessel = new CreateVesselDto
        {
            Name = "ExistingVessel",
            ImoNumber = "IMO0000001",
            Type = "TypeA",
            Owner = "OwnerA",
            Length = 100,
            Depth = 20,
            Draft = 10
        };
        _vesselServiceMock.Setup(service => service.Add(It.IsAny<CreateVesselDto>()))
            .ThrowsAsync(new EntityAlreadyExistsException("This vessel already exists."));

        var result = await _controller.Create(newVessel);

        var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsNotFound_WhenReferencedEntityDoesNotExist()
    {
        var newVessel = new CreateVesselDto
        {
            Name = "VesselWithMissingType",
            ImoNumber = "IMO0000002",
            Type = "MissingType",
            Owner = "OwnerA",
            Length = 100,
            Depth = 20,
            Draft = 10
        };
        _vesselServiceMock.Setup(service => service.Add(It.IsAny<CreateVesselDto>()))
            .ThrowsAsync(new EntityNotFoundException("The referenced vessel type does not exist"));

        var result = await _controller.Create(newVessel);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsInternalServerError_OnPersistencyFailed()
    {
        var newVessel = new CreateVesselDto
        {
            Name = "VesselWithPersistencyError",
            ImoNumber = "IMO0000003",
            Type = "TypeA",
            Owner = "OwnerA",
            Length = 100,
            Depth = 20,
            Draft = 10
        };
        _vesselServiceMock.Setup(service => service.Add(It.IsAny<CreateVesselDto>()))
            .ThrowsAsync(new PersistencyFailedException("Persistency failed"));

        var result = await _controller.Create(newVessel);

        var objectResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_OnException()
    {
        var newVessel = new CreateVesselDto
        {
            Name = "BadVessel",
            ImoNumber = "IMO0000004",
            Type = "TypeA",
            Owner = "OwnerA",
            Length = 100,
            Depth = 20,
            Draft = 10
        };
        _vesselServiceMock.Setup(service => service.Add(It.IsAny<CreateVesselDto>()))
            .ThrowsAsync(new Exception("Test exception"));

        var result = await _controller.Create(newVessel);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Filter_ReturnsOkResult_WithPagedVessels()
    {
        var filter = new VesselFilter
        {
            PageNumber = 1,
            PageSize = 10
        };
        var pagedResult = new Page<VesselDto>
        {
            Items = new List<VesselDto> { new VesselDto { Name = "Test", ImoNumber = "IMO1", Type = null!, Owner = null!, PhysicalCharacteristics = null! } },
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
        _vesselServiceMock.Setup(service => service.FilterVessels(It.IsAny<VesselFilter>()))
            .ReturnsAsync(pagedResult);

        var result = await _controller.Filter(filter);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<Page<VesselDto>>(okResult.Value);
        Assert.Single(returnValue.Items);
        Assert.Equal(filter.PageNumber, returnValue.PageNumber);
        Assert.Equal(filter.PageSize, returnValue.PageSize);
    }

    [Fact]
    public async Task Filter_ReturnsNotFound_WhenNoVesselsFound()
    {
        var filter = new VesselFilter
        {
            PageNumber = 1,
            PageSize = 10
        };
        var pagedResult = new Page<VesselDto>
        {
            Items = new List<VesselDto>(),
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
        _vesselServiceMock.Setup(service => service.FilterVessels(It.IsAny<VesselFilter>()))
            .ReturnsAsync(pagedResult);

        var result = await _controller.Filter(filter);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Filter_ReturnsNotFound_OnException()
    {
        var filter = new VesselFilter
        {
            PageNumber = 1,
            PageSize = 10
        };
        _vesselServiceMock.Setup(service => service.FilterVessels(It.IsAny<VesselFilter>()))
            .ThrowsAsync(new Exception("Test exception"));

        var result = await _controller.Filter(filter);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenVesselIsUpdated()
    {
        var updatedVessel = new VesselDto
        {
            Name = "UpdatedVessel",
            ImoNumber = "IMO9999999",
            Type = null!,
            Owner = null!,
            PhysicalCharacteristics = null!
        };
        _vesselServiceMock.Setup(service => service.Update(It.IsAny<string>(), It.IsAny<CreateVesselDto>()))
            .ReturnsAsync(updatedVessel);

        var updateDto = new CreateVesselDto
        {
            Name = "UpdatedVessel",
            ImoNumber = "IMO9999999",
            Type = "TypeA",
            Owner = "OwnerA",
            Length = 100,
            Depth = 20,
            Draft = 10
        };
        var result = await _controller.Update("IMO9999999", updateDto);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<VesselDto>(okResult.Value);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenUpdateFails()
    {
        _vesselServiceMock.Setup(service => service.Update(It.IsAny<string>(), It.IsAny<CreateVesselDto>()))
            .ThrowsAsync(new PersistencyFailedException("Unable to perform an update"));

        var updateDto = new CreateVesselDto
        {
            Name = "BadVessel",
            ImoNumber = "IMO0000005",
            Type = "TypeA",
            Owner = "OwnerA",
            Length = 100,
            Depth = 20,
            Draft = 10
        };
        var result = await _controller.Update("IMO0000005", updateDto);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_OnException()
    {
        _vesselServiceMock.Setup(service => service.Update(It.IsAny<string>(), It.IsAny<CreateVesselDto>()))
            .ThrowsAsync(new Exception("Test exception"));

        var updateDto = new CreateVesselDto
        {
            Name = "BadVessel2",
            ImoNumber = "IMO0000006",
            Type = "TypeA",
            Owner = "OwnerA",
            Length = 100,
            Depth = 20,
            Draft = 10
        };
        var result = await _controller.Update("IMO0000006", updateDto);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}