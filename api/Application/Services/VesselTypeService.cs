using Api.Domain.Model;
using Domain.Model.Generic;

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

    public async Task<VesselTypeDto?> GetVesselTypeByName(string name)
    {
        VesselType vtype = await _vesselTypeRepository.GetVesselTypeByNameAsync(name);
        if (vtype == null)
            throw new Exception("Vessel Type with the specified name not found.");
        

        return vtype.ToDTO();
    }

    public async Task<VesselTypeDto?> GetVesselTypeByDescription(string description)
    {
        VesselType vtype = await _vesselTypeRepository.GetVesselTypeByDescriptionAsync(description);
        if (vtype == null)
            throw new Exception("Vessel Type with the specified description not found.");

        return vtype.ToDTO();
    }

    public async Task<VesselTypeDto?> Add(VesselTypeDto vesselTypeDto)
    {
        bool exists = await GetVesselTypeByName(vesselTypeDto.Name) != null;
        if (exists)
        {
            throw new Exception("Vessel Type with the specified name already exists");
        }
        VesselType vType = new VesselType(Guid.NewGuid(), new Designation { Value = vesselTypeDto.Name }, new Designation { Value = vesselTypeDto.Description },
         vesselTypeDto.MaxNumberOfRows, vesselTypeDto.MaxNumberOfBays, vesselTypeDto.MaxNumberOfTiers);

        VesselType savedVesselType = await _vesselTypeRepository.Add(vType);

        return savedVesselType.ToDTO();
    }

    public async Task<VesselTypeDto?> Update(string name, VesselTypeDto vesselTypeDto)
    {
        bool updated = await _vesselTypeRepository.Update(name, vesselTypeDto);
        if (!updated)
        {
            throw new Exception("Failed to update Vessel Type.");
        }

        return await GetVesselTypeByName(vesselTypeDto.Name);
    }

}