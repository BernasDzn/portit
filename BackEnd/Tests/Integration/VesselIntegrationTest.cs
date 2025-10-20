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

namespace Tests.Integration;

public class VesselIntegrationTest
{
    private readonly VesselController _controller;
    private readonly VesselService _service;
    private readonly Mock<IVesselRepository> _repositoryMock;
    private readonly Mock<IVesselTypeRepository> _vesselTypeRepositoryMock;
    private readonly Mock<IShippingAgentOrgRepository> _organizationRepositoryMock;

    public VesselIntegrationTest()
    {
        _repositoryMock = new Mock<IVesselRepository>();
        _vesselTypeRepositoryMock = new Mock<IVesselTypeRepository>();
        _organizationRepositoryMock = new Mock<IShippingAgentOrgRepository>();
        _service = new VesselService(_repositoryMock.Object,
            _vesselTypeRepositoryMock.Object,
            _organizationRepositoryMock.Object,
            new Mock<ILogger<VesselService>>().Object);
        _controller = new VesselController(_service, new Mock<ILogger<VesselController>>().Object);
    }

    [Fact]
    public async Task GetAllVessels_ReturnsOkResult_WithListOfVessels()
    {
        _repositoryMock.Setup(repo => repo.GetVesselsAsync())
            .ReturnsAsync(new List<Vessel>());

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<VesselDto>>(okResult.Value);
        Assert.Empty(returnValue);
    }

    [Fact]
    public async Task GetVesselByImo_ReturnsOkResult_WithVessel()
    {
        var testImo = "IMO1234567";
        _repositoryMock.Setup(repo => repo.GetVesselByIMOAsync(testImo))
            .ReturnsAsync(new Vessel(Guid.NewGuid(),
                new Designation { Value = "Sample Vessel" },
                new ImoNumber { Value = testImo },
                new VesselType(
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
                ),
                new ShippingAgentOrganization(
                    Guid.NewGuid(),
                    new Designation { Value = "Maersk" },
                    new List<Designation> { new Designation { Value = "A major shipping company" } },
                    new Address("123 Ocean Drive", "Copenhagen", "Denmark", "2100"),
                    new TaxNumber { Value = "PT252252252" },
                    new HashSet<Representative>()
                    {
                        new Representative(
                            Guid.NewGuid(),
                            123456789,
                            new Designation { Value = "rep" },
                            new Email { Value = "email@email.com" },
                            new PhoneNumber { Value = "4512345678" }
                        )
                    }
                ),
                new PhysicalCharacteristics
                {
                    Length = 300,
                    Depth = 15,
                    Draft = 10
                }));

        var result = await _controller.GetByImo(testImo);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<VesselDto>(okResult.Value);
        Assert.Equal(testImo, returnValue.ImoNumber);
    }

    [Fact]
    public async Task GetVesselByImo_ReturnsNotFound_WhenVesselDoesNotExist()
    {
        var testImo = "nonexistentimo";
        _repositoryMock.Setup(repo => repo.GetVesselByIMOAsync(testImo))!
            .ReturnsAsync((Vessel?)null);

        var result = await _controller.GetByImo(testImo);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateVessel_ReturnsCreatedAtActionResult_WithCreatedVessel()
    {
        var newVesselDto = new CreateVesselDto
        {
            ImoNumber = "IMO 8476310",
            Name = "New Vessel",
            Type = "Post-Panamax",
            Owner = "Maersk",
            Length = 200,
            Depth = 18,
            Draft = 10
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

        _organizationRepositoryMock.Setup(repo => repo.GetByName("Maersk"))
            .Returns(new ShippingAgentOrganization(
                    Guid.NewGuid(),
                    new Designation { Value = "Maersk" },
                    new List<Designation> { new Designation { Value = "A major shipping company" } },
                    new Address("123 Ocean Drive", "Copenhagen", "Denmark", "2100"),
                    new TaxNumber { Value = "PT252252252" },
                    new HashSet<Representative>()
                    {
                        new Representative(
                            Guid.NewGuid(),
                            123456789,
                            new Designation { Value = "rep" },
                            new Email { Value = "email@email.com" },
                            new PhoneNumber { Value = "4512345678" }
                        )
                    }
                ));

        // Ensure the repository check for existing IMO matches the DTO used above
        _repositoryMock.Setup(repo => repo.GetVesselByIMOAsync("IMO 8476310"))!
            .ReturnsAsync((Vessel?)null);

        _repositoryMock.Setup(repo => repo.Add(It.IsAny<Vessel>()))
            .ReturnsAsync((Vessel v) => v);

        var result = await _controller.Create(newVesselDto);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<VesselDto>(createdAtActionResult.Value);
        Assert.Equal(newVesselDto.ImoNumber, returnValue.ImoNumber);
    }

    [Fact]
    public async Task CreateVessel_ReturnsBadRequest_OnException()
    {
        var newVesselDto = new CreateVesselDto
        {
            ImoNumber = "IMO 8476310",
            Name = "New Vessel",
            Type = "Post-Panamax",
            Owner = "Maersk",
            Length = 200,
            Depth = 18,
            Draft = 10
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

        _organizationRepositoryMock.Setup(repo => repo.GetByName("Maersk"))
            .Returns(new ShippingAgentOrganization(
                    Guid.NewGuid(),
                    new Designation { Value = "Maersk" },
                    new List<Designation> { new Designation { Value = "A major shipping company" } },
                    new Address("123 Ocean Drive", "Copenhagen", "Denmark", "2100"),
                    new TaxNumber { Value = "PT252252252" },
                    new HashSet<Representative>()
                    {
                        new Representative(
                            Guid.NewGuid(),
                            123456789,
                            new Designation { Value = "rep" },
                            new Email { Value = "email@email.com" },
                            new PhoneNumber { Value = "4512345678" }
                        )
                    }
                ));
        _repositoryMock.Setup(repo => repo.Add(It.IsAny<Vessel>()))
            .ThrowsAsync(new System.Exception("Test exception"));
        var result = await _controller.Create(newVesselDto);
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateVessel_ReturnsUpdatedVessel()
    {
        var imoToUpdate = "IMO1234567";
        var updateVesselDto = new CreateVesselDto
        {
            ImoNumber = imoToUpdate,
            Name = "Updated Vessel",
            Type = "Post-Panamax",
            Owner = "Maersk",
            Length = 250,
            Depth = 16,
            Draft = 11
        };

        _repositoryMock.Setup(repo => repo.GetVesselByIMOAsync(imoToUpdate))
            .ReturnsAsync(new Vessel(Guid.NewGuid(),
                new Designation { Value = "Existing Vessel" },
                new ImoNumber { Value = imoToUpdate },
                new VesselType(
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
                ),
                new ShippingAgentOrganization(
                    Guid.NewGuid(),
                    new Designation { Value = "Maersk" },
                    new List<Designation> { new Designation { Value = "A major shipping company" } },
                    new Address("123 Ocean Drive", "Copenhagen", "Denmark", "2100"),
                    new TaxNumber { Value = "PT252252252" },
                    new HashSet<Representative>()
                    {
                        new Representative(
                            Guid.NewGuid(),
                            123456789,
                            new Designation { Value = "rep" },
                            new Email { Value = "email@email.com" },
                            new PhoneNumber { Value = "4512345678" }
                        )
                    }
                ),
                new PhysicalCharacteristics
                {
                    Length = 200,
                    Depth = 18,
                    Draft = 10
                }));

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

        _organizationRepositoryMock.Setup(repo => repo.GetByName("Maersk"))
            .Returns(new ShippingAgentOrganization(
                    Guid.NewGuid(),
                    new Designation { Value = "Maersk" },
                    new List<Designation> { new Designation { Value = "A major shipping company" } },
                    new Address("123 Ocean Drive", "Copenhagen", "Denmark", "2100"),
                    new TaxNumber { Value = "PT252252252" },
                    new HashSet<Representative>()
                    {
                        new Representative(
                            Guid.NewGuid(),
                            123456789,
                            new Designation { Value = "rep" },
                            new Email { Value = "email@email.com" },
                            new PhoneNumber { Value = "4512345678" }
                        )
                    }
                ));

        _repositoryMock.Setup(repo => repo.Update(It.IsAny<Vessel>()))
            .ReturnsAsync((Vessel v) => v);

        var result = await _controller.Update(imoToUpdate, updateVesselDto);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<VesselDto>(okResult.Value);
        Assert.Equal(imoToUpdate, returnValue.ImoNumber);
        Assert.Equal("Updated Vessel", returnValue.Name);
    }

    [Fact]
    public async Task UpdateVessel_ReturnsNotFound_WhenVesselDoesNotExist()
    {
        var imoToUpdate = "nonexistentimo";
        var updateVesselDto = new CreateVesselDto
        {
            ImoNumber = imoToUpdate,
            Name = "Updated Vessel",
            Type = "Post-Panamax",
            Owner = "Maersk",
            Length = 250,
            Depth = 16,
            Draft = 11
        };

        _repositoryMock.Setup(repo => repo.GetVesselByIMOAsync(imoToUpdate))!
            .ReturnsAsync((Vessel?)null);

        var result = await _controller.Update(imoToUpdate, updateVesselDto);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }
}