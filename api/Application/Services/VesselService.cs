namespace Application.Services;

using Api.Application.Exceptions;
using Api.Domain.Model;
using Api.Models;
using Domain.IRepository;
using Domain.Model.Generic;
using Microsoft.EntityFrameworkCore;

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

    public async Task<VesselDto?> GetVesselByName(string name)
    {
        Vessel vessel = await _vesselRepository.GetVesselByNameAsync(name);
        if (vessel == null)
            throw new EntityNotFoundException("Vessel not found.");
        return vessel.ToDTO();
    }

    public async Task<VesselDto?> Add(VesselDto vesselDto)
    {
        bool exists = await _vesselRepository.GetVesselByNameAsync(vesselDto.Name) != null;
        if (exists)
            throw new EntityAlreadyExistsException("This vessel already exists");

        VesselType? vesselType = await _vesselTypeRepository.GetVesselTypeByNameAsync(vesselDto.Type.Name);
        if (vesselType == null)
            throw new EntityNotFoundException("The referenced vessel type does not exist");

        ShippingAgentOrganization? org = _shippingAgentOrgRepository.GetByName(vesselDto.Owner.Name);
        if (org == null)
            throw new EntityNotFoundException("The referenced shipping agent organization does not exist");

        Vessel vessel = new Vessel(
            Guid.NewGuid(),
            new Designation { Value = vesselDto.Name },
            new ImoNumber { Value = vesselDto.ImoNumber },
            vesselType,
            org
        );

        Vessel savedVessel = await _vesselRepository.Add(vessel);
        VesselDto savedVesselDto = savedVessel.ToDTO();

        return savedVesselDto;
    }

    public async Task<VesselDto?> Update(string name, VesselDto vesselDto)
    {
        Vessel vessel = await _vesselRepository.GetVesselByNameAsync(name);
        if (vessel == null)
            throw new EntityNotFoundException("Vessel not found.");

        VesselType? vesselType = await _vesselTypeRepository.GetVesselTypeByNameAsync(vesselDto.Type.Name);
        if (vesselType == null)
            throw new EntityNotFoundException("The referenced vessel type does not exist");

        ShippingAgentOrganization? org = _shippingAgentOrgRepository.GetByName(vesselDto.Owner.Name);
        if (org == null)
            throw new EntityNotFoundException("The referenced shipping agent organization does not exist");

        vessel.UpdateName(vesselDto.Name);
        vessel.UpdateImoNumber(vesselDto.ImoNumber);
        vessel.UpdateVesselType(vesselType);
        vessel.UpdateOwner(org);

        Vessel? updateResult = await _vesselRepository.Update(vessel);
        if (updateResult == null)
            throw new PersistencyFailedException("Unable to perform an update");

        Vessel updatedVessel = await _vesselRepository.GetVesselByNameAsync(vesselDto.Name);
        return updatedVessel.ToDTO();
    }
}