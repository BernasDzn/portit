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

public class VesselTypeServiceTest
{
    private readonly Mock<IVesselTypeRepository> _vesselTypeRepositoryMock;
    private readonly VesselTypeService _service;

    private static ICollection<VesselType> vesselTypes = new List<VesselType>
    {
        new VesselType(
            Guid.NewGuid(),
            new Designation { Value = "Panamax" },
            new Designation { Value = "A large container ship" },
            10,
            10,
            10,
            new PhysicalCharacteristics { Depth = 8, Length = 200, Draft = 10 }

        ),
        new VesselType(
            Guid.NewGuid(),
            new Designation { Value = "Post-Panamax" },
            new Designation { Value = "A large container ship" },
            10,
            10,
            10,
            new PhysicalCharacteristics { Depth = 8, Length = 200, Draft = 10 }
        )
    };

    public VesselTypeServiceTest()
    {
        _vesselTypeRepositoryMock = new Mock<IVesselTypeRepository>();

        _service = new VesselTypeService(_vesselTypeRepositoryMock.Object, new Mock<ILogger<VesselTypeService>>().Object);
    }

    [Fact]
    public async Task GetVesselTypes_ReturnsListOfVesselTypes()
    {
        _vesselTypeRepositoryMock.Setup(repo => repo.GetVesselTypesAsync())
            .ReturnsAsync(vesselTypes);

        var result = await _service.GetVesselTypes();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal(vesselTypes.ElementAt(0).ToDTO().Name, result.ElementAt(0).Name);
    }

    [Fact]
    public async Task GetVesselTypeByName_ReturnsVesselType_WhenExists()
    {
        _vesselTypeRepositoryMock.Setup(repo => repo.GetVesselTypeByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((string name) => vesselTypes.FirstOrDefault(vt => vt.Name.Value == name));

        var result = await _service.GetByName(vesselTypes.ElementAt(0).Name.Value);

        Assert.NotNull(result);
        Assert.Equal(vesselTypes.ElementAt(0).ToDTO().Name, result.Name);
    }

    [Fact]
    public async Task GetVesselTypeByName_ThrowsException_WhenNotExists()
    {
        _vesselTypeRepositoryMock.Setup(repo => repo.GetVesselTypeByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((VesselType?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _service.GetByName("Non existing vessel type"));
    }

    [Fact]
    public async Task Add_ReturnsAddedDock()
    {
        var newVesselTypeDto = new VesselTypeDto
        {
            Name = "Vessel Type 3",
            Description = "Description 3",
            MaxNumberOfBays = 10,
            MaxNumberOfRows = 10,
            MaxNumberOfTiers = 10,
            PhysicalCharacteristics = new PhysicalCharacteristics
            {
                Depth = 8,
                Length = 200,
                Draft = 10
            }
        };

        _vesselTypeRepositoryMock.Setup(repo => repo.GetVesselTypeByNameAsync(It.Is<string>(s => s == newVesselTypeDto.Name)))
            .ReturnsAsync(vesselTypes.FirstOrDefault(vt => vt.Name.Value == newVesselTypeDto.Name));

        _vesselTypeRepositoryMock.Setup(repo => repo.Add(It.IsAny<VesselType>()))
            .ReturnsAsync((VesselType vt) => vt);

        var result = await _service.Add(newVesselTypeDto);

        Assert.NotNull(result);
        Assert.Equal(newVesselTypeDto.Name, result.Name);
    }

    [Fact]
    public async Task Add_ThrowsException_WhenNameAlreadyExists()
    {
        var existingVesselTypeDto = new VesselTypeDto
        {
            Name = vesselTypes.ElementAt(0).Name.Value,
            Description = vesselTypes.ElementAt(0).Description.Value,
            MaxNumberOfBays = vesselTypes.ElementAt(0).MaxNumberOfBays,
            MaxNumberOfRows = vesselTypes.ElementAt(0).MaxNumberOfRows,
            MaxNumberOfTiers = vesselTypes.ElementAt(0).MaxNumberOfTiers,
            PhysicalCharacteristics = vesselTypes.ElementAt(0).PhysicalCharacteristics
        };

        _vesselTypeRepositoryMock.Setup(repo => repo.GetVesselTypeByNameAsync(existingVesselTypeDto.Name))
            .ReturnsAsync(vesselTypes.FirstOrDefault(vt => vt.Name.Value == existingVesselTypeDto.Name));

        await Assert.ThrowsAsync<EntityAlreadyExistsException>(() => _service.Add(existingVesselTypeDto));
    }

    [Fact]
    public async Task Update_ReturnsUpdatedVesselType_WhenExists()
    {
        var existingVesselType = vesselTypes.ElementAt(0);
        var updatedVesselTypeDto = new VesselTypeDto
        {
            Name = existingVesselType.Name.Value,
            Description = "Updated Description",
            MaxNumberOfBays = existingVesselType.MaxNumberOfBays,
            MaxNumberOfRows = existingVesselType.MaxNumberOfRows,
            MaxNumberOfTiers = existingVesselType.MaxNumberOfTiers,
            PhysicalCharacteristics = existingVesselType.PhysicalCharacteristics
        };

        _vesselTypeRepositoryMock.Setup(repo => repo.GetVesselTypeByNameAsync(existingVesselType.Name.Value))
            .ReturnsAsync(existingVesselType);

        _vesselTypeRepositoryMock.Setup(repo => repo.Update(It.IsAny<VesselType>()))
            .ReturnsAsync((VesselType vt) => vt);

        var result = await _service.Update(existingVesselType.Name.Value, updatedVesselTypeDto);

        Assert.NotNull(result);
        Assert.Equal(updatedVesselTypeDto.Name, result.Name);
    }

    [Fact]
    public async Task Update_ThrowsException_WhenNameNotExists()
    {
        var nonExistentVesselTypeDto = new VesselTypeDto
        {
            Name = "NonExistentName",
            Description = "Some Description",
            MaxNumberOfBays = 5,
            MaxNumberOfRows = 5,
            MaxNumberOfTiers = 5,
            PhysicalCharacteristics = new PhysicalCharacteristics
            {
                Depth = 10,
                Length = 100,
                Draft = 5
            }
        };

        _vesselTypeRepositoryMock.Setup(repo => repo.GetVesselTypeByNameAsync(nonExistentVesselTypeDto.Name))
            .ReturnsAsync((VesselType?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.Update(nonExistentVesselTypeDto.Name, nonExistentVesselTypeDto));
    }

    [Fact]
    public async Task FilteredVesselTypes_ReturnsPage()
    {
        var filter = new VesselTypeFilter { };

        _vesselTypeRepositoryMock.Setup(repo => repo.FilterVesselTypesAsync(filter))
            .ReturnsAsync(new Page<VesselType>
            {
                Items = vesselTypes.ToList(),
                PageNumber = 1,
                PageSize = vesselTypes.Count
            });

        var result = await _service.FilterVesselTypes(filter);

        Assert.NotNull(result);
    Assert.IsType<Page<VesselTypeDto>>(result);
        Assert.Equal(vesselTypes.Count, result.Items.Count);
    }
}