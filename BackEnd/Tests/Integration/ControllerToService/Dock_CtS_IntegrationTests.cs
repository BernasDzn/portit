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

public class Dock_CtS_IntegrationTest
{
    private readonly DockController _controller;
    private readonly DockService _service;
    private readonly Mock<IDockRepository> _repositoryMock;
    private readonly Mock<IVesselTypeRepository> _vesselTypeRepositoryMock;

    public Dock_CtS_IntegrationTest()
    {
        _repositoryMock = new Mock<IDockRepository>();
        _vesselTypeRepositoryMock = new Mock<IVesselTypeRepository>();
        _service = new DockService(_repositoryMock.Object,
            _vesselTypeRepositoryMock.Object,
            new Mock<ILogger<DockService>>().Object);
        _controller = new DockController(_service, new Mock<ILogger<DockController>>().Object);
    }

    [Fact]
    public async Task GetAllDocks_ReturnsOkResult_WithListOfDocks()
    {
        _repositoryMock.Setup(repo => repo.GetDocksAsync())
            .ReturnsAsync(new List<Dock>());

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<DockDto>>(okResult.Value);

        Assert.Empty(returnValue);
    }

    [Fact]
    public async Task GetDockByCode_ReturnsOkResult_WithDock()
    {
        var testCode = "DCK001";

        _repositoryMock.Setup(repo => repo.GetDockByCodeAsync(testCode))
            .ReturnsAsync(new Dock(Guid.NewGuid(),
                new Code { Value = testCode },
                new Designation { Value = "Sample Dock" },
                new Designation { Value = "A sample location" },
                new PhysicalCharacteristics
                {
                    Length = 500,
                    Depth = 35,
                    Draft = 20
                },
                new HashSet<VesselType>
                {
                    new VesselType(
                        Guid.NewGuid(),
                        new Designation { Value = "Panamax" },
                        new Designation { Value = "Max size for Panama Canal" },
                        20,
                        10,
                        5,
                        new PhysicalCharacteristics {
                            Length = 300,
                            Depth = 15,
                            Draft = 12 
                        }
                    ),
                    new VesselType(
                        Guid.NewGuid(),
                        new Designation { Value = "Handymax" },
                        new Designation { Value = "Medium-sized bulk carrier" },
                        15,
                        8,
                        4,
                        new PhysicalCharacteristics {
                            Length = 250,
                            Depth = 12,
                            Draft = 10
                        }
                    )
                }
                )
                );


        var result = await _controller.GetByCode(testCode);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<DockDto>(okResult.Value);
        Assert.Equal(testCode, returnValue.Code);
    }

    [Fact]
    public async Task GetDockByCode_ReturnsNotFound_WhenDockDoesNotExist()
    {
        var testCode = "nonexistentcode";

        _repositoryMock.Setup(repo => repo.GetDockByCodeAsync(testCode))!
            .ReturnsAsync((Dock?)null);

        var result = await _controller.GetByCode(testCode);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateDock_ReturnsCreatedAtActionResult_WithCreatedDock()
    {
        var newDockDto = new CreateDockDto
        {
            Code = "DCK004",
            Name = "New Dock",
            Location = "New Location",
            PhysicalCharacteristics = new PhysicalCharacteristics
            {
                Length = 400,
                Depth = 18,
                Draft = 14
            },
            SupportedVesselTypes = new List<string> { "Post-Panamax" }
        };

        _vesselTypeRepositoryMock.Setup(repo => repo.GetVesselTypeByNameAsync("Post-Panamax"))
            .ReturnsAsync(new VesselType(
                    Guid.NewGuid(),
                    new Designation { Value = "Post-Panamax" },
                    new Designation { Value = "Larger than Panamax" },
                    30,
                    15,
                    7,
                    new PhysicalCharacteristics
                    {
                        Length = 400,
                        Depth = 18,
                        Draft = 14
                    }
                ));

        _repositoryMock.Setup(repo => repo.GetDockByCodeAsync("DCK004"))!
            .ReturnsAsync((Dock?)null);

        _repositoryMock.Setup(repo => repo.Add(It.IsAny<Dock>()))
            .ReturnsAsync((Dock v) => v);

        var result = await _controller.Create(newDockDto);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<DockDto>(createdAtActionResult.Value);
        Assert.Equal(newDockDto.Code, returnValue.Code);
    }

    [Fact]
    public async Task CreateDock_ReturnsBadRequest_OnException()
    {
        var newDockDto = new CreateDockDto
        {
            Code = "DCK004",
            Name = "New Dock",
            Location = "New Location",
            PhysicalCharacteristics = new PhysicalCharacteristics
            {
                Length = 500,
                Depth = 35,
                Draft = 20
            },
            SupportedVesselTypes = new List<string> { "Post-Panamax" }
        };

        _vesselTypeRepositoryMock.Setup(repo => repo.GetVesselTypeByNameAsync("Post-Panamax"))
            .ReturnsAsync(new VesselType(
                    Guid.NewGuid(),
                    new Designation { Value = "Post-Panamax" },
                    new Designation { Value = "Larger than Panamax" },
                    30,
                    15,
                    7,
                    new PhysicalCharacteristics
                    {
                        Length = 400,
                        Depth = 18,
                        Draft = 14
                    }
                ));

        _repositoryMock.Setup(repo => repo.Add(It.IsAny<Dock>()))
            .ThrowsAsync(new System.Exception("Test exception"));
        var result = await _controller.Create(newDockDto);
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateDock_ReturnsUpdatedDock()
    {
        var codeToUpdate = "DCK005";
        var updateDockDto = new CreateDockDto
        {
            Code = codeToUpdate,
            Name = "Updated Dock",
            Location = "Updated Location",
            PhysicalCharacteristics = new PhysicalCharacteristics
            {
                Length = 500,
                Depth = 35,
                Draft = 20
            },
            SupportedVesselTypes = new List<string> { "Panamax", "Handymax" }
        };

        _repositoryMock.Setup(repo => repo.GetDockByCodeAsync(codeToUpdate))
            .ReturnsAsync(new Dock(Guid.NewGuid(),
                new Code { Value = codeToUpdate },
                new Designation { Value = "Existing Dock" },
                new Designation { Value = "Existing Location" },
                new PhysicalCharacteristics
                {
                    Length = 500,
                    Depth = 35,
                    Draft = 20
                },
                new HashSet<VesselType>
                {
                    new VesselType(
                        Guid.NewGuid(),
                        new Designation { Value = "Panamax" },
                        new Designation { Value = "Max size for Panama Canal" },
                        20,
                        10,
                        5,
                        new PhysicalCharacteristics {
                            Length = 300,
                            Depth = 15,
                            Draft = 12
                        }
                    ),
                    new VesselType(
                        Guid.NewGuid(),
                        new Designation { Value = "Handymax" },
                        new Designation { Value = "Medium-sized bulk carrier" },
                        15,
                        8,
                        4,
                        new PhysicalCharacteristics {
                            Length = 250,
                            Depth = 12,
                            Draft = 10
                        }
                    )
                }
                )
                );

        _vesselTypeRepositoryMock.Setup(repo => repo.GetVesselTypeByNameAsync("Panamax"))
            .ReturnsAsync(new VesselType(
                        Guid.NewGuid(),
                        new Designation { Value = "Panamax" },
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
                    )
                    );

        _vesselTypeRepositoryMock.Setup(repo => repo.GetVesselTypeByNameAsync("Handymax"))
        .ReturnsAsync(new VesselType(
                        Guid.NewGuid(),
                        new Designation { Value = "Handymax" },
                        new Designation { Value = "Medium-sized bulk carrier" },
                        15,
                        8,
                        4,
                        new PhysicalCharacteristics
                        {
                            Length = 250,
                            Depth = 12,
                            Draft = 10
                        }
                    )
                );

        _repositoryMock.Setup(repo => repo.Update(It.IsAny<Dock>()))
            .ReturnsAsync((Dock v) => v);

        var result = await _controller.Update(codeToUpdate, updateDockDto);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<DockDto>(okResult.Value);
        Assert.Equal(codeToUpdate, returnValue.Code);
        Assert.Equal("Updated Dock", returnValue.Name);
    }

    [Fact]
    public async Task FilterDock_ReturnsOkResult_WithFilteredDocks()
    {
        var filter = new DockFilter { };
        _repositoryMock.Setup(repo => repo.FilterDocksAsync(filter))
            .ReturnsAsync(new Page<Dock>
            {
                Items = new List<Dock>(),
                PageNumber = 1,
                PageSize = 10,
            });

        var result = await _controller.Filter(filter);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<Page<DockDto>>(okResult.Value);
        Assert.Empty(returnValue.Items);
    }

    [Fact]
    public async Task FilterDock_ReturnsInternalServerError_OnException()
    {
        var filter = new DockFilter { };
        _repositoryMock.Setup(repo => repo.FilterDocksAsync(filter))
            .ThrowsAsync(new System.Exception("Database error"));

        var result = await _controller.Filter(filter);

        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }
}