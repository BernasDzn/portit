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

public class DockServiceTest
{
    private readonly Mock<IDockRepository> _dockRepositoryMock;
    private readonly Mock<IVesselTypeRepository> _vesselTypeRepositoryMock;
    private readonly DockService _service;

    private static ICollection<Dock> docks = new List<Dock>
    {
        new Dock(
            Guid.NewGuid(),
            new Code { Value = "DOCK1" },
            new Designation { Value = "Dock 1" },
            new Designation { Value = "Location 1" },
            new PhysicalCharacteristics {
                Depth = 10,
                Length = 300,
                Draft = 15
            },
            new HashSet<VesselType>() {
                new VesselType(
                    Guid.NewGuid(),
                    new Designation { Value = "Panamax" },
                    new Designation { Value = "A large container ship" },
                    10,
                    10,
                    10,
                    new PhysicalCharacteristics { Depth = 8, Length = 200, Draft = 10 }
                )
            }
        ),
        new Dock(
            Guid.NewGuid(),
            new Code { Value = "DOCK2" },
            new Designation { Value = "Dock 2" },
            new Designation { Value = "Location 2" },
            new PhysicalCharacteristics {
                Depth = 10,
                Length = 300,
                Draft = 15
            },
            new HashSet<VesselType>() {
                new VesselType(
                    Guid.NewGuid(),
                    new Designation { Value = "Post-Panamax" },
                    new Designation { Value = "A large container ship" },
                    10,
                    10,
                    10,
                    new PhysicalCharacteristics { Depth = 8, Length = 200, Draft = 10 }
                )
            }
        )
    };

    public DockServiceTest()
    {
        _dockRepositoryMock = new Mock<IDockRepository>();
        _vesselTypeRepositoryMock = new Mock<IVesselTypeRepository>();

        _service = new DockService(_dockRepositoryMock.Object, _vesselTypeRepositoryMock.Object, new Mock<ILogger<DockService>>().Object);
    }

    [Fact]
    public async Task GetDocks_ReturnsListOfDocks()
    {
        _dockRepositoryMock.Setup(repo => repo.GetDocksAsync())
            .ReturnsAsync(docks);

        var result = await _service.GetDocks();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal(docks.ElementAt(0).ToDTO().Code, result.ElementAt(0).Code);
    }

    [Fact]
    public async Task GetDockByCode_ReturnsDock_WhenExists()
    {
        _dockRepositoryMock.Setup(repo => repo.GetDockByCodeAsync(It.IsAny<string>()))
            .ReturnsAsync((string id) => docks.FirstOrDefault(d => d.Code.Value == id));

        var result = await _service.GetByCode(docks.ElementAt(0).Code.Value);

        Assert.NotNull(result);
        Assert.Equal(docks.ElementAt(0).ToDTO().Code, result.Code);
    }

    [Fact]
    public async Task GetDockByCode_ThrowsException_WhenNotExists()
    {
        _dockRepositoryMock.Setup(repo => repo.GetDockByCodeAsync(It.IsAny<string>()))
            .ReturnsAsync((Dock?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _service.GetByCode("NonExistentCode"));
    }

    [Fact]
    public async Task Add_ReturnsAddedDock()
    {
        var newDockDto = new CreateDockDto
        {
            Code = "DOCK3",
            Name = "Dock 3",
            Location = "Location 3",
            PhysicalCharacteristics = new PhysicalCharacteristics
            {
                Depth = 12,
                Length = 350,
                Draft = 18
            },
            SupportedVesselTypes = new List<string> { "Panamax" }
        };

        _dockRepositoryMock.Setup(repo => repo.GetDockByCodeAsync(It.Is<string>(s => s == newDockDto.Code)))
            .ReturnsAsync(docks.FirstOrDefault(d => d.Code.Value == newDockDto.Code));

        _vesselTypeRepositoryMock.Setup(repo => repo.GetVesselTypeByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((string name) => docks.SelectMany(d => d.SupportedVesselTypes)
                    .FirstOrDefault(vt => vt.Name.Value == name));

        _dockRepositoryMock.Setup(repo => repo.Add(It.IsAny<Dock>()))
            .ReturnsAsync((Dock d) => d);

        var result = await _service.Add(newDockDto);

        Assert.NotNull(result);
        Assert.Equal(newDockDto.Code, result.Code);
    }

    [Fact]
    public async Task Add_ThrowsException_WhenCodeAlreadyExists()
    {
        var existingDockDto = new CreateDockDto
        {
            Code = docks.ElementAt(0).Code.Value,
            Name = docks.ElementAt(0).Name.Value,
            Location = docks.ElementAt(0).Location.Value,
            PhysicalCharacteristics = new PhysicalCharacteristics
            {
                Depth = docks.ElementAt(0).PhysicalCharacteristics.Depth,
                Length = docks.ElementAt(0).PhysicalCharacteristics.Length,
                Draft = docks.ElementAt(0).PhysicalCharacteristics.Draft
            },
            SupportedVesselTypes = new List<string> { "Panamax" }
        };

        _dockRepositoryMock.Setup(repo => repo.GetDockByCodeAsync(existingDockDto.Code))
            .ReturnsAsync(docks.FirstOrDefault(d => d.Code.Value == existingDockDto.Code));

        await Assert.ThrowsAsync<EntityAlreadyExistsException>(() => _service.Add(existingDockDto));
    }

    [Fact]
    public async Task Add_ThrowsException_WhenVesselTypeNotFound()
    {
        var newDockDto = new CreateDockDto
        {
            Code = "DOCK4",
            Name = "Dock 4",
            Location = "Location 4",
            PhysicalCharacteristics = new PhysicalCharacteristics
            {
                Depth = 15,
                Length = 400,
                Draft = 20
            },
            SupportedVesselTypes = new List<string> { "NonExistentVesselType" }
        };

        _dockRepositoryMock.Setup(repo => repo.GetDockByCodeAsync(It.Is<string>(s => s == newDockDto.Code)))
            .ReturnsAsync(docks.FirstOrDefault(d => d.Code.Value == newDockDto.Code));

        _vesselTypeRepositoryMock.Setup(repo => repo.GetVesselTypeByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((VesselType?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.Add(newDockDto));
    }

    [Fact]
    public async Task UpdateDock_ReturnsUpdatedDock_WhenExists()
    {
        var existingDock = docks.ElementAt(0);
        var updatedDockDto = new CreateDockDto
        {
            Code = existingDock.Code.Value,
            Name = "Updated Dock Name",
            Location = existingDock.Location.Value,
            PhysicalCharacteristics = new PhysicalCharacteristics
            {
                Depth = existingDock.PhysicalCharacteristics.Depth,
                Length = existingDock.PhysicalCharacteristics.Length,
                Draft = existingDock.PhysicalCharacteristics.Draft
            },
            SupportedVesselTypes = new List<string> { "Panamax" }
        };

        _dockRepositoryMock.Setup(repo => repo.GetDockByCodeAsync(existingDock.Code.Value))
            .ReturnsAsync(existingDock);

        _vesselTypeRepositoryMock.Setup(repo => repo.GetVesselTypeByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((string name) => docks.SelectMany(d => d.SupportedVesselTypes)
                    .FirstOrDefault(vt => vt.Name.Value == name));

        _dockRepositoryMock.Setup(repo => repo.Update(It.IsAny<Dock>()))
            .ReturnsAsync((Dock d) => d);

        var result = await _service.Update(existingDock.Code.Value, updatedDockDto);

        Assert.NotNull(result);
        Assert.Equal(updatedDockDto.Name, result.Name);
    }

    [Fact]
    public async Task UpdateDock_ThrowsException_WhenCodeNotExists()
    {
        var nonExistentDockDto = new CreateDockDto
        {
            Code = "NonExistentId",
            Name = "Some Name",
            Location = "Some Location",
            PhysicalCharacteristics = new PhysicalCharacteristics
            {
                Depth = 10,
                Length = 100,
                Draft = 5
            },
            SupportedVesselTypes = new List<string> { "Panamax" }
        };

        _dockRepositoryMock.Setup(repo => repo.GetDockByCodeAsync(nonExistentDockDto.Code))
            .ReturnsAsync((Dock?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.Update(nonExistentDockDto.Code, nonExistentDockDto));
    }

    [Fact]
    public async Task UpdateDock_ThrowsException_WhenVesselTypeNotFound()
    {
        var existingDock = docks.ElementAt(0);
        var updatedDockDto = new CreateDockDto
        {
            Code = existingDock.Code.Value,
            Name = "Updated Dock Name",
            Location = existingDock.Location.Value,
            PhysicalCharacteristics = new PhysicalCharacteristics
            {
                Depth = existingDock.PhysicalCharacteristics.Depth,
                Length = existingDock.PhysicalCharacteristics.Length,
                Draft = existingDock.PhysicalCharacteristics.Draft
            },
            SupportedVesselTypes = new List<string> { "NonExistentVesselType" }
        };

        _dockRepositoryMock.Setup(repo => repo.GetDockByCodeAsync(existingDock.Code.Value))
            .ReturnsAsync(existingDock);

        _vesselTypeRepositoryMock.Setup(repo => repo.GetVesselTypeByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((VesselType?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.Update(existingDock.Code.Value, updatedDockDto));
    }

    [Fact]
    public async Task FilteredDock_ReturnsPage()
    {
        var filter = new DockFilter { };

        _dockRepositoryMock.Setup(repo => repo.FilterDocksAsync(filter))
            .ReturnsAsync(new Page<Dock>
            {
                Items = docks.ToList(),
                PageNumber = 1,
                PageSize = docks.Count
            });

        var result = await _service.FilterDocks(filter);

        Assert.NotNull(result);
        Assert.IsType<Page<DockDto>>(result);
        Assert.Equal(docks.Count, result.Items.Count);
    }
}