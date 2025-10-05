namespace Api.Application.Services;

using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Utilities;

public class VesselTypeService
{
    private readonly IVesselTypeRepository _vesselTypeRepository;

    public VesselTypeService(IVesselTypeRepository vesselTypeRepository)
    {
        _vesselTypeRepository = vesselTypeRepository;
    }

    public async Task<IEnumerable<VesselTypeDto>> GetVesselTypes()
    {
        IEnumerable<VesselType> vtypes = await _vesselTypeRepository.GetVesselTypesAsync();
        return vtypes.Select(vt => vt.ToDTO()).ToList();
    }

    public async Task<Page<VesselTypeDto>> FilterVesselTypes(VesselTypeFilter filter)
    {
        Page<VesselType> page = await _vesselTypeRepository.FilterVesselTypesAsync(filter);
        return page.Map(vt => vt.ToDTO());
    }

    public async Task<VesselTypeDto?> Add(VesselTypeDto vesselTypeDto)
    {
        bool exists = await _vesselTypeRepository.GetVesselTypeByNameAsync(vesselTypeDto.Name) != null;
        if (exists)
        {
            throw new Exception("Vessel Type with the specified name already exists");
        }
        VesselType vType = new VesselType(Guid.NewGuid(), new Designation { Value = vesselTypeDto.Name }, new Designation { Value = vesselTypeDto.Description },
         vesselTypeDto.MaxNumberOfRows, vesselTypeDto.MaxNumberOfBays, vesselTypeDto.MaxNumberOfTiers,
         new PhysicalCharacteristics { Length = vesselTypeDto.PhysicalCharacteristics.Length, Depth = vesselTypeDto.PhysicalCharacteristics.Depth, Draft = vesselTypeDto.PhysicalCharacteristics.Draft });

        VesselType savedVesselType = await _vesselTypeRepository.Add(vType);

        return savedVesselType.ToDTO();
    }

    public async Task<VesselTypeDto?> Update(string name, VesselTypeDto vesselTypeDto)
    {
        VesselType vesselType = await _vesselTypeRepository.GetVesselTypeByNameAsync(name);
        if (vesselType == null)
            throw new Exception("Vessel Type not found.");

        vesselType.UpdateName( vesselTypeDto.Name);
        vesselType.UpdateDescription(vesselTypeDto.Description);
        vesselType.UpdateMaxNumberOfRows(vesselTypeDto.MaxNumberOfRows);
        vesselType.UpdateMaxNumberOfBays(vesselTypeDto.MaxNumberOfBays);
        vesselType.UpdateMaxNumberOfTiers(vesselTypeDto.MaxNumberOfTiers);
        vesselType.UpdatePhysicalCharacteristics(new PhysicalCharacteristics
        {
            Length = vesselTypeDto.PhysicalCharacteristics.Length,
            Depth = vesselTypeDto.PhysicalCharacteristics.Depth,
            Draft = vesselTypeDto.PhysicalCharacteristics.Draft
        });

        bool updated = await _vesselTypeRepository.Update(vesselType);
        if (!updated)
        {
            throw new Exception("Failed to update Vessel Type.");
        }

        VesselType updatedVesselType = await _vesselTypeRepository.GetVesselTypeByNameAsync(vesselTypeDto.Name);
        return updatedVesselType.ToDTO();
    }

}