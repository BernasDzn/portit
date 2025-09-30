using Api.Domain.Model;
using Domain.IRepository;
using Domain.Model.Generic;

public class DockService
{
    private readonly IDockRepository _dockRepository;

    public DockService(IDockRepository dockRepository)
    {
        _dockRepository = dockRepository;
    }

    public async Task<IEnumerable<DockDto>> GetDocks()
    {
        IEnumerable<Dock> docks = await _dockRepository.GetDocksAsync();

        return docks.Select(d => d.ToDTO()).ToList();
    }

    public async Task<DockDto?> GetDockByName(string name, List<string> errorMessage)
    {
        Dock dock = await _dockRepository.GetDockByNameAsync(name);
        if (dock == null)
        {
            errorMessage.Add("Dock with the specified name not found.");
            return null;
        }

        return dock.ToDTO();
    }

    public async Task<IEnumerable<DockDto>?> GetDockByVesselType(string vesselType, List<string> errorMessage)
    {
        IEnumerable<Dock> docks = await _dockRepository.GetDockByVesselTypeAsync(vesselType);
        if (docks == null || docks.Count() == 0)
        {
            errorMessage.Add("Dock(s) with the specified vessel type not found.");
            return null;
        }

        return docks.Select(d => d.ToDTO()).ToList();
    }

    public async Task<DockDto?> GetDockByLocation(string location, List<string> errorMessage)
    {
        Dock dock = await _dockRepository.GetDockByLocationAsync(location);
        if (dock == null)
        {
            errorMessage.Add("Dock with the specified location not found.");
            return null;
        }

        return dock.ToDTO();
    }

    public async Task<DockDto?> Add(DockDto dockDto, List<string> errorMessage)
    {
        bool exists = await _dockRepository.GetDockByNameAsync(dockDto.Name) != null;
        if (exists)
        {
            errorMessage.Add("A dock with that name already exists!");
            return null;
        }

        Dock dock = new Dock(Guid.NewGuid(), new Designation { Value = dockDto.Name }, new Designation { Value = dockDto.Location },
         dockDto.Length, dockDto.Depth, dockDto.MaxDraft, dockDto.SupportedVesselTypes);

        Dock savedDock = await _dockRepository.Add(dock);
        DockDto savedDockDto = savedDock.ToDTO();

        return savedDockDto;
    }

    public async Task<DockDto?> Update(string name, DockDto dockDto, List<string> errorMessage)
	{
		bool updateResult = await _dockRepository.Update(name, dockDto, errorMessage);
		if (!updateResult)
		{
            errorMessage.Add("Failed to update the dock.");
			return null;
		}

		return await GetDockByName(dockDto.Name, errorMessage);
	}

}