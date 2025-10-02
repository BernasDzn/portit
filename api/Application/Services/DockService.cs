using Api.Application.Exceptions;
using Api.Domain.Model;
using Domain.IRepository;
using Domain.Model.Generic;

public class DockService
{
    private readonly IDockRepository _dockRepository;
    private readonly IVesselTypeRepository _vesselTypeRepository;

    public bool IsServingDock { get; internal set; }

    public DockService(IDockRepository dockRepository, IVesselTypeRepository vesselTypeRepository)
    {
        _dockRepository = dockRepository;
        _vesselTypeRepository = vesselTypeRepository;
    }

    public async Task<IEnumerable<DockDto>> GetDocks()
    {
        IEnumerable<Dock> docks = await _dockRepository.GetDocksAsync();

        return docks.Select(d => d.ToDTO()).ToList();
    }

    public async Task<DockDto?> GetDockByName(string name)
    {
        Dock dock = await _dockRepository.GetDockByNameAsync(name);
        if (dock == null)
            throw new EntityNotFoundException("Dock with the specified name not found.");

        return dock.ToDTO();
    }

    public async Task<IEnumerable<DockDto>?> GetDockByVesselType(string vesselType)
    {
        IEnumerable<Dock> docks = await _dockRepository.GetDockByVesselTypeAsync(vesselType);
        if (docks == null || docks.Count() == 0)
            throw new EntityNotFoundException("Dock(s) with the specified vessel type not found.");

        return docks.Select(d => d.ToDTO()).ToList();
    }

    public async Task<DockDto?> GetDockByLocation(string location)
    {
        Dock dock = await _dockRepository.GetDockByLocationAsync(location);
        if (dock == null)
            throw new EntityNotFoundException("Dock with the specified location not found.");

        return dock.ToDTO();
    }

    public async Task<DockDto?> Add(DockDto dockDto)
    {
        bool exists = await _dockRepository.GetDockByNameAsync(dockDto.Name) != null;
        if (exists)
            throw new EntityAlreadyExistsException("This dock already exists.");

        List<VesselType> vesselTypes = await GetVesselTypesFromDto(dockDto.SupportedVesselTypes);

        Dock dock = new Dock(Guid.NewGuid(), new Designation { Value = dockDto.Name }, new Designation { Value = dockDto.Location },
         dockDto.Length, dockDto.Depth, dockDto.MaxDraft, vesselTypes);

        Dock savedDock = await _dockRepository.Add(dock);
        DockDto savedDockDto = savedDock.ToDTO();

        return savedDockDto;
    }

    public async Task<DockDto?> Update(string name, DockDto dockDto)
    {
        if (name != dockDto.Name)
            throw new ArgumentException("The provided name does not match the dock to be updated.");

        Dock dock = await _dockRepository.GetDockByNameAsync(dockDto.Name);
        if (dock == null)
            throw new EntityNotFoundException("A dock with the specified name does not exist.");

        List<VesselType> vesselTypes = await GetVesselTypesFromDto(dockDto.SupportedVesselTypes);

        dock.UpdateLocation(dockDto.Location);
        dock.UpdateDepth(dockDto.Depth);
        dock.UpdateLength(dockDto.Length);
        dock.UpdateMaxDraft(dockDto.MaxDraft);
        dock.UpdateVesselTypes(vesselTypes);

        bool updated = await _dockRepository.Update(dock);

        if (!updated)
            throw new PersistencyFailedException("Dock update failed.");

        Dock updatedDock = await _dockRepository.GetDockByNameAsync(name);

        return updatedDock.ToDTO();
    }

    private async Task<List<VesselType>> GetVesselTypesFromDto(List<VesselTypeDto> vesselTypesDtos)
    {
        List<VesselType> vesselTypes = new List<VesselType>();

        foreach (VesselTypeDto vtDto in vesselTypesDtos)
        {
            VesselType? vesselType = await _vesselTypeRepository.GetVesselTypeByNameAsync(vtDto.Name);
            if (vesselType == null)
                throw new EntityNotFoundException("The referenced vessel type does not exist.");

            vesselTypes.Add(vesselType);
        }

        return vesselTypes;
    }
}