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

public class VesselServiceTest
{
	private readonly Mock<IVesselRepository> _vesselRepositoryMock;
	private readonly Mock<IVesselTypeRepository> _vesselTypeRepositoryMock;
	private readonly Mock<IShippingAgentOrgRepository> _shippingAgentOrgRepositoryMock;
	private readonly VesselService _service;

	private static readonly VesselType SampleVesselType = new VesselType(
		Guid.NewGuid(),
		new Designation { Value = "Post-Panamax" },
		new Designation { Value = "Large ship" },
		10, 10, 10,
		new PhysicalCharacteristics { Length = 400, Depth = 18, Draft = 14 }
	);

	private static readonly ShippingAgentOrganization SampleOwner = new ShippingAgentOrganization(
		Guid.NewGuid(),
		new Designation { Value = "Global Shipping Co." },
		new List<Designation> { new Designation { Value = "GSC" } },
		new Address("1 Port St", "City", "0000", "Country"),
		new TaxNumber { Value = "PT123456789" },
		new HashSet<Representative>
		{
			new Representative(
				Guid.NewGuid(),
				123456789,
				new Designation { Value = "John Doe" },
				new Email { Value = "em@email.com" },
				new PhoneNumber { Value = "123456789" }
			)
		}
	);

	private static readonly Vessel SampleVessel = new Vessel(
		Guid.NewGuid(),
		new Designation { Value = "Maersk Triple E" },
		new ImoNumber { Value = "IMO 1234567" },
		SampleVesselType,
		SampleOwner,
		new PhysicalCharacteristics { Length = 370, Depth = 16, Draft = 13 }
	);

	public VesselServiceTest()
	{
		_vesselRepositoryMock = new Mock<IVesselRepository>();
		_vesselTypeRepositoryMock = new Mock<IVesselTypeRepository>();
		_shippingAgentOrgRepositoryMock = new Mock<IShippingAgentOrgRepository>();

		_service = new VesselService(
			_vesselRepositoryMock.Object,
			_vesselTypeRepositoryMock.Object,
			_shippingAgentOrgRepositoryMock.Object,
			new Mock<ILogger<VesselService>>().Object
		);
	}

	[Fact]
	public async Task GetVessels_ReturnsListOfVessels()
	{
		_vesselRepositoryMock.Setup(r => r.GetVesselsAsync())
			.ReturnsAsync(new List<Vessel> { SampleVessel });

		var result = await _service.GetVessels();

		Assert.NotNull(result);
		Assert.Single(result);
		Assert.Equal(SampleVessel.Name.Value, result.First().Name);
	}

	[Fact]
	public async Task Add_ReturnsSavedVessel_WhenValid()
	{
		var createDto = new CreateVesselDto
		{
			Name = "New Vessel",
			ImoNumber = "IMO 3815389",
			Type = SampleVesselType.Name.Value,
			Owner = SampleOwner.LegalName.Value,
			Length = 100,
			Depth = 10,
			Draft = 8
		};

		_vesselRepositoryMock.Setup(r => r.GetVesselByIMOAsync(createDto.ImoNumber))
			.ReturnsAsync((Vessel?)null!);

		_vesselTypeRepositoryMock.Setup(r => r.GetVesselTypeByNameAsync(createDto.Type))
			.ReturnsAsync(SampleVesselType);

		_shippingAgentOrgRepositoryMock.Setup(r => r.GetByName(createDto.Owner))
			.Returns(SampleOwner);

		_vesselRepositoryMock.Setup(r => r.Add(It.IsAny<Vessel>()))
			.ReturnsAsync((Vessel v) => v);

		var result = await _service.Add(createDto);

		Assert.NotNull(result);
		Assert.Equal(createDto.ImoNumber, result.ImoNumber);
		Assert.Equal(createDto.Name, result.Name);
	}

	[Fact]
	public async Task Add_ThrowsEntityAlreadyExists_WhenImoExists()
	{
		var createDto = new CreateVesselDto { ImoNumber = SampleVessel.ImoIdentifier.Value, Type = SampleVesselType.Name.Value, Owner = SampleOwner.LegalName.Value, Name = "x", Length = 1, Depth = 1, Draft = 1 };

		_vesselRepositoryMock.Setup(r => r.GetVesselByIMOAsync(createDto.ImoNumber))
			.ReturnsAsync(SampleVessel);

		await Assert.ThrowsAsync<EntityAlreadyExistsException>(() => _service.Add(createDto));
	}

	[Fact]
	public async Task Add_ThrowsEntityNotFound_WhenTypeMissing()
	{
		var createDto = new CreateVesselDto { ImoNumber = "IMO 1234567", Type = "NoSuchType", Owner = SampleOwner.LegalName.Value, Name = "x", Length = 1, Depth = 1, Draft = 1 };

	_vesselRepositoryMock.Setup(r => r.GetVesselByIMOAsync(createDto.ImoNumber)).ReturnsAsync((Vessel?)null!);
	_vesselTypeRepositoryMock.Setup(r => r.GetVesselTypeByNameAsync(createDto.Type)).ReturnsAsync((VesselType?)null!);

		await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.Add(createDto));
	}

	[Fact]
	public async Task Update_ReturnsUpdatedVessel_WhenExists()
	{
		var updateDto = new CreateVesselDto
		{
			Name = "Updated Name",
			ImoNumber = SampleVessel.ImoIdentifier.Value,
			Type = SampleVesselType.Name.Value,
			Owner = SampleOwner.LegalName.Value,
			Length = 200,
			Depth = 12,
			Draft = 9
		};

		_vesselRepositoryMock.Setup(r => r.GetVesselByIMOAsync(SampleVessel.ImoIdentifier.Value)).ReturnsAsync(SampleVessel);
		_vesselTypeRepositoryMock.Setup(r => r.GetVesselTypeByNameAsync(updateDto.Type)).ReturnsAsync(SampleVesselType);
		_shippingAgentOrgRepositoryMock.Setup(r => r.GetByName(updateDto.Owner)).Returns(SampleOwner);
		_vesselRepositoryMock.Setup(r => r.Update(It.IsAny<Vessel>())).ReturnsAsync((Vessel v) => v);

		var result = await _service.Update(SampleVessel.ImoIdentifier.Value, updateDto);

		Assert.NotNull(result);
		Assert.Equal(updateDto.Name, result.Name);
		Assert.Equal(SampleVessel.ImoIdentifier.Value, result.ImoNumber);
	}

	[Fact]
	public async Task Update_ThrowsEntityNotFound_WhenVesselMissing()
	{
		var updateDto = new CreateVesselDto { Name = "n", ImoNumber = "IMO 0000000", Type = SampleVesselType.Name.Value, Owner = SampleOwner.LegalName.Value, Length = 1, Depth = 1, Draft = 1 };

	_vesselRepositoryMock.Setup(r => r.GetVesselByIMOAsync(updateDto.ImoNumber)).ReturnsAsync((Vessel?)null!);

		await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.Update(updateDto.ImoNumber, updateDto));
	}

	[Fact]
	public async Task GetByImo_ReturnsVessel_WhenExists()
	{
		_vesselRepositoryMock.Setup(r => r.GetVesselByIMOAsync(SampleVessel.ImoIdentifier.Value)).ReturnsAsync(SampleVessel);

		var result = await _service.GetByImo(SampleVessel.ImoIdentifier.Value);

		Assert.NotNull(result);
		Assert.Equal(SampleVessel.ImoIdentifier.Value, result.ImoNumber);
	}

	[Fact]
	public async Task GetByImo_ThrowsEntityNotFound_WhenMissing()
	{
	_vesselRepositoryMock.Setup(r => r.GetVesselByIMOAsync(It.IsAny<string>())).ReturnsAsync((Vessel?)null!);

		await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetByImo("IMO 0000000"));
	}

	[Fact]
	public async Task FilterVessels_ReturnsPage()
	{
		var page = new Page<Vessel>
		{
			Items = new List<Vessel> { SampleVessel },
			PageNumber = 1,
			PageSize = 10
		};

		_vesselRepositoryMock.Setup(r => r.FilterVesselsAsync(It.IsAny<VesselFilter>())).ReturnsAsync(page);

		var result = await _service.FilterVessels(new VesselFilter());

		Assert.NotNull(result);
		Assert.IsType<Page<VesselDto>>(result);
		Assert.Single(result.Items);
	}
}

