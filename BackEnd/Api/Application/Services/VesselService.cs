namespace Api.Application.Services;

using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Application.Exceptions;
using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Exceptions;
using Api.Infrastructure.Utilities;

public class VesselService
{
    private readonly IVesselRepository _vesselRepository;
    private readonly IVesselTypeRepository _vesselTypeRepository;
    private readonly IShippingAgentOrgRepository _shippingAgentOrgRepository;

    public VesselService(IVesselRepository vesselRepository, IVesselTypeRepository vesselTypeRepository, IShippingAgentOrgRepository shippingAgentOrgRepository)
    {
        _vesselRepository = vesselRepository;
        _vesselTypeRepository = vesselTypeRepository;
        _shippingAgentOrgRepository = shippingAgentOrgRepository;
    }

    public async Task<IEnumerable<VesselDto>> GetVessels()
    {
        var vessels = await _vesselRepository.GetVesselsAsync();
        return vessels.Select(v => v.ToDTO()).ToList();
    }

    public async Task<VesselDto?> Add(VesselDto vesselDto)
    {
        bool exists = await _vesselRepository.GetVesselByIMOAsync(vesselDto.ImoNumber) != null;
        if (exists)
            throw new EntityAlreadyExistsException("This vessel already exists");

        VesselType? vesselType = await _vesselTypeRepository.GetVesselTypeByNameAsync(vesselDto.Type.Name);
        if (vesselType == null)
            throw new EntityNotFoundException("The referenced vessel type does not exist");
        ShippingAgentOrganization? org = null;
        if (vesselDto.Owner != null)
            org = _shippingAgentOrgRepository.GetByName(vesselDto.Owner.Name);

        Vessel vessel = new Vessel(
            Guid.NewGuid(),
            new Designation { Value = vesselDto.Name },
            new ImoNumber { Value = vesselDto.ImoNumber },
            vesselType,
            org,
            new PhysicalCharacteristics
            {
                Length = vesselDto.PhysicalCharacteristics.Length,
                Depth = vesselDto.PhysicalCharacteristics.Depth,
                Draft = vesselDto.PhysicalCharacteristics.Draft
            }
        );

        Vessel savedVessel = await _vesselRepository.Add(vessel);
        VesselDto savedVesselDto = savedVessel.ToDTO();

        return savedVesselDto;
    }

    public async Task<VesselDto?> Update(string imo, VesselDto vesselDto)
    {
        Vessel vessel = await _vesselRepository.GetVesselByIMOAsync(imo);
        if (vessel == null)
            throw new EntityNotFoundException("Vessel not found.");

        VesselType? vesselType = await _vesselTypeRepository.GetVesselTypeByNameAsync(vesselDto.Type.Name);
        if (vesselType == null)
            throw new EntityNotFoundException("The referenced vessel type does not exist");

        ShippingAgentOrganization? org = null;
        if (vesselDto.Owner != null)
            org = _shippingAgentOrgRepository.GetByName(vesselDto.Owner.Name);

        vessel.UpdateName(vesselDto.Name);
        vessel.UpdateImoNumber(vesselDto.ImoNumber);
        vessel.UpdateVesselType(vesselType);
        vessel.UpdateOwner(org);

        Vessel? updateResult = await _vesselRepository.Update(vessel);
        if (updateResult == null)
            throw new PersistencyFailedException("Unable to perform an update");

        Vessel updatedVessel = await _vesselRepository.GetVesselByIMOAsync(vesselDto.ImoNumber);
        return updatedVessel.ToDTO();
    }

    internal async Task<Page<VesselDto>> FilterVessels(VesselFilter filter)
    {
        Page<Vessel> page = await _vesselRepository.FilterVesselsAsync(filter);
        return page.Map(v => v.ToDTO());
    }
}