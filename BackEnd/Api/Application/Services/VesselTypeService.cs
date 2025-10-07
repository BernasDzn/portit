namespace Api.Application.Services;

using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Exceptions;
using Api.Infrastructure.Utilities;

public class VesselTypeService
{
    private readonly IVesselTypeRepository _vesselTypeRepository;
    private readonly ILogger<VesselTypeService> _logger;

    public VesselTypeService(IVesselTypeRepository vesselTypeRepository, ILogger<VesselTypeService> logger)
    {
        _vesselTypeRepository = vesselTypeRepository;
        _logger = logger;
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
            throw new EntityAlreadyExistsException("Vessel Type with the specified name already exists");
    
        VesselType vType = new VesselType(Guid.NewGuid(), new Designation { Value = vesselTypeDto.Name }, new Designation { Value = vesselTypeDto.Description },
         vesselTypeDto.MaxNumberOfRows, vesselTypeDto.MaxNumberOfBays, vesselTypeDto.MaxNumberOfTiers,
         new PhysicalCharacteristics { Length = vesselTypeDto.PhysicalCharacteristics.Length, Depth = vesselTypeDto.PhysicalCharacteristics.Depth, Draft = vesselTypeDto.PhysicalCharacteristics.Draft });

        VesselType savedVesselType = await _vesselTypeRepository.Add(vType);

        _logger.LogInformation("Vessel Type {VesselTypeId} created.", savedVesselType.Id);
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
            throw new EntityAlreadyExistsException("Failed to update Vessel Type.");

        VesselType updatedVesselType = await _vesselTypeRepository.GetVesselTypeByNameAsync(vesselTypeDto.Name);

        _logger.LogInformation("Vessel Type {VesselTypeId} updated.", updatedVesselType.Id);
        return updatedVesselType.ToDTO();
    }
}