namespace Api.Application.Services;

using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Application.Exceptions;
using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Exceptions;
using Api.Infrastructure.Utilities;
using Serilog.Events;

public class DockService : IDockService
{
    private readonly IDockRepository _dockRepository;
    private readonly IVesselTypeRepository _vesselTypeRepository;
    private readonly ILogger<DockService> _logger;

    public DockService(IDockRepository dockRepository, IVesselTypeRepository vesselTypeRepository, ILogger<DockService> logger)
    {
        _dockRepository = dockRepository;
        _vesselTypeRepository = vesselTypeRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<DockDto>> GetDocks()
    {
        IEnumerable<Dock> docks = await _dockRepository.GetDocksAsync();
        AppLogEvents.LogRetrieve(_logger, "docks", docks.Count());
        return docks.Select(d => d.ToDTO()).ToList();
    }

    public async Task<DockDto> GetByCode(string code)
    {
        Dock? dock = await _dockRepository.GetDockByCodeAsync(code);
        if (dock == null)
            throw new EntityNotFoundException("No dock found with the provided code.");
        AppLogEvents.LogRetrieve(_logger, "dock", 1);
        return dock.ToDTO();
    }

    public async Task<Page<DockDto>> FilterDocks(DockFilter filter)
    {
        Page<Dock> page = await _dockRepository.FilterDocksAsync(filter);
        AppLogEvents.LogFilter(_logger, "docks", page.Items.Count);
        return page.Map(d => d.ToDTO());
    }

    public async Task<DockDto> Add(CreateDockDto dockDto)
    {
        bool exists = await _dockRepository.GetDockByCodeAsync(dockDto.Code) != null;
        if (exists)
            throw new EntityAlreadyExistsException("This dock already exists.");

        HashSet<VesselType> vesselTypes = await GetVesselTypesFromDto(dockDto.SupportedVesselTypes);

        Dock dock = new Dock(Guid.NewGuid(), new Code { Value = dockDto.Code }, new Designation { Value = dockDto.Name }, new Designation { Value = dockDto.Location },
         new PhysicalCharacteristics
         {
             Length = dockDto.PhysicalCharacteristics.Length,
             Depth = dockDto.PhysicalCharacteristics.Depth,
             Draft = dockDto.PhysicalCharacteristics.Draft
         }, vesselTypes);

        Dock savedDock = await _dockRepository.Add(dock);
        DockDto savedDockDto = savedDock.ToDTO();

        //_logger.LogInformation("Dock {DockId} created.", savedDock.Id);
        AppLogEvents.LogCreate(_logger, "Dock", savedDock.Id);
        return savedDockDto;
    }

    public async Task<DockDto> Update(string code, CreateDockDto dockDto)
    {
        if (code != dockDto.Code)
            throw new ArgumentException("The provided code does not match the dock to be updated.");

        Dock? dock = await _dockRepository.GetDockByCodeAsync(dockDto.Code);
        if (dock == null)
            throw new EntityNotFoundException("A dock with the specified code does not exist.");

        HashSet<VesselType> vesselTypes = await GetVesselTypesFromDto(dockDto.SupportedVesselTypes);

        dock.UpdateName(dockDto.Name);
        dock.UpdateLocation(dockDto.Location);

        PhysicalCharacteristics newPhysicalCharacteristics = new PhysicalCharacteristics
        {
            Length = dockDto.PhysicalCharacteristics.Length,
            Depth = dockDto.PhysicalCharacteristics.Depth,
            Draft = dockDto.PhysicalCharacteristics.Draft
        };

        dock.UpdatePhysicalCharacteristics(newPhysicalCharacteristics);
        dock.UpdateVesselTypes(vesselTypes);

        Dock updated = await _dockRepository.Update(dock);

        AppLogEvents.LogUpdate(_logger, "Dock", dock.Id);
        return updated.ToDTO();
    }

    private async Task<HashSet<VesselType>> GetVesselTypesFromDto(List<string> vesselTypesDtos)
    {
        HashSet<VesselType> vesselTypes = new HashSet<VesselType>();

        foreach (string vtName in vesselTypesDtos)
        {
            VesselType? vesselType = await _vesselTypeRepository.GetVesselTypeByNameAsync(vtName);
            if (vesselType == null)
                throw new EntityNotFoundException("The referenced vessel type does not exist.");

            vesselTypes.Add(vesselType);
        }

        AppLogEvents.LogRetrieve(_logger, "vessel types", vesselTypes.Count);
        return vesselTypes;
    }
}