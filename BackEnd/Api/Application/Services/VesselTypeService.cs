namespace Api.Application.Services;

using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Application.Exceptions;
using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Exceptions;
using Api.Infrastructure.Utilities;

public class VesselTypeService : IVesselTypeService
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
        AppLogEvents.LogRetrieve(_logger, "vessel types", vtypes.Count());
        return vtypes.Select(vt => vt.ToDTO()).ToList();
    }

    public async Task<VesselTypeDto> GetByName(string name)
    {
        VesselType? vType = await _vesselTypeRepository.GetVesselTypeByNameAsync(name);
        if (vType == null)
            throw new EntityNotFoundException("Vessel Type not found.");

        AppLogEvents.LogRetrieve(_logger, "vessel type", 1);
        return vType.ToDTO();
    }

    public async Task<Page<VesselTypeDto>> FilterVesselTypes(VesselTypeFilter filter)
    {
        Page<VesselType> page = await _vesselTypeRepository.FilterVesselTypesAsync(filter);
        AppLogEvents.LogFilter(_logger, "vessel types", page.Items.Count);
        return page.Map(vt => vt.ToDTO());
    }

    public async Task<VesselTypeDto> Add(VesselTypeDto vesselTypeDto)
    {
        bool exists = await _vesselTypeRepository.GetVesselTypeByNameAsync(vesselTypeDto.Name) != null;
        if (exists)
            throw new EntityAlreadyExistsException("Vessel Type with the specified name already exists");
    
        VesselType vType = new VesselType(Guid.NewGuid(), new Designation { Value = vesselTypeDto.Name }, new Designation { Value = vesselTypeDto.Description },
         vesselTypeDto.MaxNumberOfRows, vesselTypeDto.MaxNumberOfBays, vesselTypeDto.MaxNumberOfTiers,
         new PhysicalCharacteristics {
             Length = vesselTypeDto.PhysicalCharacteristics.Length,
             Depth = vesselTypeDto.PhysicalCharacteristics.Depth,
             Draft = vesselTypeDto.PhysicalCharacteristics.Draft
         });

        VesselType savedVesselType = await _vesselTypeRepository.Add(vType);

        AppLogEvents.LogCreate(_logger, "Vessel Type", vesselTypeDto.Name);
        return savedVesselType.ToDTO();
    }

    public async Task<VesselTypeDto> Update(string name, VesselTypeDto vesselTypeDto)
    {
        VesselType? vesselType = await _vesselTypeRepository.GetVesselTypeByNameAsync(name);
        if (vesselType == null)
            throw new Exception("Vessel Type not found.");

        vesselType.UpdateName(vesselTypeDto.Name);
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

        VesselType? updated = await _vesselTypeRepository.Update(vesselType);
        if (updated == null)
            throw new PersistencyFailedException("Failed to update Vessel Type.");

        AppLogEvents.LogUpdate(_logger, "Vessel Type", vesselType.Id);
        return updated.ToDTO();
    }
}