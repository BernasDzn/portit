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
using Api.Infrastructure.Exceptions;
using Api.Infrastructure.Utilities;

namespace Tests.Integration.ControllerToService;

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
    public async Task GetVesselByImo_ReturnsBadRequest_OnException()
    {
        var testImo = "IMOEXCEPTION";
        _repositoryMock.Setup(repo => repo.GetVesselByIMOAsync(testImo))
            .ThrowsAsync(new System.Exception("Test exception"));

        var result = await _controller.GetByImo(testImo);
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateVessel_ReturnsConflict_WhenEntityAlreadyExists()
    {
        var newVesselDto = new CreateVesselDto
        {
            ImoNumber = "IMO1234567",
            Name = "Existing Vessel",
            Type = "Post-Panamax",
            Owner = "Maersk",
            Length = 200,
            Depth = 18,
            Draft = 10
        };
        var dummyRep = new Representative(
            Guid.NewGuid(),
            123456789,
            new Designation { Value = "rep" },
            new Email { Value = "email@email.com" },
            new PhoneNumber { Value = "4512345678" }
        );
        _repositoryMock.Setup(repo => repo.GetVesselByIMOAsync("IMO1234567"))
            .ReturnsAsync(new Vessel(Guid.NewGuid(), new Designation { Value = "Existing Vessel" }, new ImoNumber { Value = "IMO1234567" },
                new VesselType(Guid.NewGuid(), new Designation { Value = "Post-Panamax" }, new Designation { Value = "desc" }, 30, 15, 7, new PhysicalCharacteristics { Length = 400, Depth = 18, Draft = 14 }),
                new ShippingAgentOrganization(Guid.NewGuid(), new Designation { Value = "Maersk" }, new List<Designation>(), new Address("", "", "", ""), new TaxNumber { Value = "PT123456789" }, new HashSet<Representative> { dummyRep }),
                new PhysicalCharacteristics { Length = 200, Depth = 18, Draft = 10 }
            ));

        var result = await _controller.Create(newVesselDto);
        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateVessel_ReturnsNotFound_WhenReferencedEntityDoesNotExist()
    {
        var newVesselDto = new CreateVesselDto
        {
            ImoNumber = "IMONOTYPE",
            Name = "VesselWithMissingType",
            Type = "MissingType",
            Owner = "Maersk",
            Length = 200,
            Depth = 18,
            Draft = 10
        };
        _repositoryMock.Setup(repo => repo.GetVesselByIMOAsync("IMONOTYPE"))
            .ReturnsAsync((Vessel?)null!);
        _vesselTypeRepositoryMock.Setup(repo => repo.GetVesselTypeByNameAsync("MissingType"))
            .ReturnsAsync((VesselType?)null);

        var result = await _controller.Create(newVesselDto);
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Filter_ReturnsOkResult_WithPagedVessels()
    {
        var filter = new VesselFilter
        {
            PageNumber = 1,
            PageSize = 10
        };
        var dummyRep = new Representative(
            Guid.NewGuid(),
            123456789,
            new Designation { Value = "rep" },
            new Email { Value = "email@email.com" },
            new PhoneNumber { Value = "4512345678" }
        );
        var pagedResult = new Page<Vessel>
        {
            Items = new List<Vessel> { new Vessel(Guid.NewGuid(), new Designation { Value = "Test" }, new ImoNumber { Value = "IMO1234567" },
                new VesselType(Guid.NewGuid(), new Designation { Value = "Post-Panamax" }, new Designation { Value = "desc" }, 30, 15, 7, new PhysicalCharacteristics { Length = 400, Depth = 18, Draft = 14 }),
                new ShippingAgentOrganization(Guid.NewGuid(), new Designation { Value = "Maersk" }, new List<Designation>(), new Address("", "", "", ""), new TaxNumber { Value = "PT123456789" }, new HashSet<Representative> { dummyRep }),
                new PhysicalCharacteristics { Length = 200, Depth = 18, Draft = 10 }
            ) },
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
        _repositoryMock.Setup(repo => repo.FilterVesselsAsync(It.IsAny<VesselFilter>()))
            .ReturnsAsync(pagedResult);

        var result = await _controller.Filter(filter);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
    var returnValue = Assert.IsType<Page<VesselDto>>(okResult.Value);
    Assert.Single(returnValue.Items);
    Assert.Equal(filter.PageNumber, returnValue.PageNumber);
    Assert.Equal(filter.PageSize, returnValue.PageSize);
    }

    [Fact]
    public async Task Filter_ReturnsEmpty_WhenNoVesselsFound()
    {
        var filter = new VesselFilter
        {
            PageNumber = 1,
            PageSize = 10
        };
        var pagedResult = new Page<Vessel>
        {
            Items = new List<Vessel>(),
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
        _repositoryMock.Setup(repo => repo.FilterVesselsAsync(It.IsAny<VesselFilter>()))
            .ReturnsAsync(pagedResult);

        var result = await _controller.Filter(filter);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<Page<VesselDto>>(okResult.Value);
        Assert.Empty(returnValue.Items);
    }

    [Fact]
    public async Task Filter_ReturnsNotFound_OnException()
    {
        var filter = new VesselFilter
        {
            PageNumber = 1,
            PageSize = 10
        };
        _repositoryMock.Setup(repo => repo.FilterVesselsAsync(It.IsAny<VesselFilter>()))
            .ThrowsAsync(new System.Exception("Test exception"));

        var result = await _controller.Filter(filter);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task UpdateVessel_ReturnsBadRequest_WhenUpdateFails()
    {
        var imoToUpdate = "IMO1234567";
        var updateVesselDto = new CreateVesselDto
        {
            ImoNumber = imoToUpdate,
            Name = "BadVessel",
            Type = "Post-Panamax",
            Owner = "Maersk",
            Length = 250,
            Depth = 16,
            Draft = 11
        };
        var dummyRep = new Representative(
            Guid.NewGuid(),
            123456789,
            new Designation { Value = "rep" },
            new Email { Value = "email@email.com" },
            new PhoneNumber { Value = "4512345678" }
        );

        var newDummyRep = new Representative(
            Guid.NewGuid(),
            987654321,
            new Designation { Value = "rep2" },
            new Email { Value = "email@email2.com" },
            new PhoneNumber { Value = "4598765432" }
        );


        _repositoryMock.Setup(repo => repo.GetVesselByIMOAsync(imoToUpdate))
            .ReturnsAsync(new Vessel(Guid.NewGuid(), new Designation { Value = "Existing Vessel" }, new ImoNumber { Value = imoToUpdate },
                new VesselType(Guid.NewGuid(), new Designation { Value = "Post-Panamax" }, new Designation { Value = "desc" }, 30, 15, 7, new PhysicalCharacteristics { Length = 400, Depth = 18, Draft = 14 }),
                new ShippingAgentOrganization(Guid.NewGuid(), new Designation { Value = "Maersk" }, new List<Designation>(), new Address("", "", "", ""), new TaxNumber { Value = "PT123456789" }, new HashSet<Representative> { dummyRep }),
                new PhysicalCharacteristics { Length = 200, Depth = 18, Draft = 10 }
            ));
        _vesselTypeRepositoryMock.Setup(repo => repo.GetVesselTypeByNameAsync("Post-Panamax"))
            .ReturnsAsync(new VesselType(Guid.NewGuid(), new Designation { Value = "Post-Panamax" }, new Designation { Value = "desc" }, 30, 15, 7, new PhysicalCharacteristics { Length = 400, Depth = 18, Draft = 14 }));
        _organizationRepositoryMock.Setup(repo => repo.GetByName("Maersk"))
            .Returns(new ShippingAgentOrganization(Guid.NewGuid(), new Designation { Value = "Maersk" }, new List<Designation>(), new Address("", "", "", ""), new TaxNumber { Value = "PT123456789" }, new HashSet<Representative> { newDummyRep }));
        _repositoryMock.Setup(repo => repo.Update(It.IsAny<Vessel>()))
            .ReturnsAsync((Vessel?)null!);

        var result = await _controller.Update(imoToUpdate, updateVesselDto);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateVessel_ReturnsBadRequest_OnException()
    {
        var imoToUpdate = "IMOEXCEPTION";
        var updateVesselDto = new CreateVesselDto
        {
            ImoNumber = imoToUpdate,
            Name = "BadVessel2",
            Type = "Post-Panamax",
            Owner = "Maersk",
            Length = 250,
            Depth = 16,
            Draft = 11
        };
        _repositoryMock.Setup(repo => repo.GetVesselByIMOAsync(imoToUpdate))
            .ThrowsAsync(new System.Exception("Test exception"));

        var result = await _controller.Update(imoToUpdate, updateVesselDto);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
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
}