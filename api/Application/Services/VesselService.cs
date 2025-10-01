namespace Application.Services;

using Api.Domain.Model;
using Api.Models;
using Domain.IRepository;
using Domain.Model.Generic;
using Microsoft.EntityFrameworkCore;

public class VesselService
{
    private readonly IVesselRepository _vesselRepository;

    public VesselService(IVesselRepository vesselRepository)
    {
        _vesselRepository = vesselRepository;
    }

    public async Task<IEnumerable<VesselDto>> GetVessels()
    {
        var vessels = await _vesselRepository.GetVesselsAsync();
        return vessels.Select(v => v.ToDTO()).ToList();
    }

    public async Task<VesselDto?> GetVesselByName(string name, List<string> errorMessage)
    {
        Vessel vessel = await _vesselRepository.GetVesselByNameAsync(name);
        if (vessel == null)
        {
            errorMessage.Add("Vessel not found.");
            return null;
        }
        return vessel.ToDTO();
    }

    public async Task<VesselDto?> Add(VesselDto vesselDto, List<string> errorMessage)
    {
        bool exists = await _vesselRepository.GetVesselByNameAsync(vesselDto.Name) != null;

        if (exists)
        {
            errorMessage.Add("Vessel with the same name already exists.");
            return null;
        }

        Vessel vessel = new Vessel(
            Guid.NewGuid(),
            new Designation { Value = vesselDto.Name },
            new ImoNumber { Value = vesselDto.ImoNumber },
            vesselDto.Type,
            new ShippingAgentOrganization(

                Guid.NewGuid(),
                new Designation { Value = vesselDto.Owner.Name },
                vesselDto.Owner.AltNames?.Select(n => new Designation { Value = n }).ToList() ?? new List<Designation>(),
                new Address(
                    vesselDto.Owner.Address.Street,
                    vesselDto.Owner.Address.City,
                    vesselDto.Owner.Address.PostalCode,
                    vesselDto.Owner.Address.Country
                ),
                new TaxNumber { Value = vesselDto.Owner.TaxNumber },
                vesselDto.Owner.Representatives?.Select(r => new Representative(
                    Guid.NewGuid(),
                    r.CitizenshipId,
                    new Designation { Value = r.Name },
                    new Email { Value = r.EmailAddress },
                    new PhoneNumber { Value = r.Phone }
                )).ToList() ?? new List<Representative>()
            )
        );
        Vessel savedVessel = await _vesselRepository.Add(vessel);
        VesselDto savedVesselDto = savedVessel.ToDTO();

        return savedVesselDto;
    }

    public async Task<VesselDto?> Update(string name, VesselDto vesselDto, List<string> errorMessage)
    {
        bool updateResult = await _vesselRepository.Update(name, vesselDto, errorMessage);
        if (!updateResult)
        {
            return null;
        }

        Vessel updatedVessel = await _vesselRepository.GetVesselByNameAsync(vesselDto.Name);
        return updatedVessel.ToDTO();
    }
}