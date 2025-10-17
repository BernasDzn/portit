namespace Api.Application.Services;

using Api.Application.DataTransfer;
using Api.Application.Exceptions;
using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Exceptions;

public class StorageAreaService : IStorageAreaService
{
    private readonly IStorageAreaRepository _storageAreaRepository;
    private readonly IDockRepository _dockRepository;
    private readonly ILogger<StorageAreaService> _logger;

    public StorageAreaService(IStorageAreaRepository storageAreaRepository, IDockRepository dockRepository, ILogger<StorageAreaService> logger)
    {
        _storageAreaRepository = storageAreaRepository;
        _dockRepository = dockRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<StorageAreaDto>> GetStorageAreas()
    {
        var qualifications = await _storageAreaRepository.GetStorageAreasAsync();
        AppLogEvents.LogRetrieve(_logger, "storage areas", qualifications.Count());
        return qualifications.Select(q => q.ToDTO()).ToList();
    }

    public async Task<StorageAreaDto> GetStorageAreaByCode(string name)
    {
        StorageArea? qualification = await _storageAreaRepository.GetStorageAreaByCodeAsync(name);
        if (qualification == null)
            throw new EntityNotFoundException("Storage area not found.");

        AppLogEvents.LogRetrieve(_logger, "storage area", 1);
        return qualification.ToDTO();
    }

    public async Task<StorageAreaDto> CreateStorageArea(StorageAreaDto createStorageAreaDto)
    {
        var existing = await _storageAreaRepository.GetStorageAreaByCodeAsync(createStorageAreaDto.NameCode);
        if (existing != null)
            throw new EntityAlreadyExistsException("A storage area with the same code already exists.");

        HashSet<StorageArea.DockRelation> dockRelations = ConvertToDockRelations(createStorageAreaDto.DockServices);

        var storageArea = new StorageArea(
            Guid.NewGuid(),
            new Code { Value = createStorageAreaDto.NameCode },
            new Designation { Value = createStorageAreaDto.Location },
            createStorageAreaDto.Type,
            createStorageAreaDto.Capacity,
            createStorageAreaDto.CurrentOccupancy,
            dockRelations
        );

        await _storageAreaRepository.Add(storageArea);
        AppLogEvents.LogCreate(_logger, "Storage area", createStorageAreaDto.NameCode);
        return storageArea.ToDTO();
    }

    public async Task<StorageAreaDto> UpdateStorageArea(string id, StorageAreaDto updateStorageAreaDto)
    {
        var storageArea = await _storageAreaRepository.GetStorageAreaByCodeAsync(id);
        if (storageArea == null)
            throw new EntityNotFoundException("Storage area not found.");

        storageArea.UpdateLocation(updateStorageAreaDto.Location);
        storageArea.UpdateCapacity(updateStorageAreaDto.Capacity);
        storageArea.UpdateOccupancy(updateStorageAreaDto.CurrentOccupancy);
        storageArea.UpdateAreaType(updateStorageAreaDto.Type);

        HashSet<StorageArea.DockRelation> dockRelations = ConvertToDockRelations(updateStorageAreaDto.DockServices);
        storageArea.UpdateDockServices(dockRelations.Count > 0 ? dockRelations : null);

        await _storageAreaRepository.Update(storageArea);
        AppLogEvents.LogUpdate(_logger, "Storage area", id);
        return storageArea.ToDTO();
    }

    private HashSet<StorageArea.DockRelation> ConvertToDockRelations(IEnumerable<DockRelationDto>? dockRelationDtos)
    {
        HashSet<StorageArea.DockRelation> dockRelations = new HashSet<StorageArea.DockRelation>();
        if (dockRelationDtos != null)
        {
            foreach (var ds in dockRelationDtos)
            {
                var dock = _dockRepository.GetDockByCodeAsync(ds.Dock.Code).Result;
                if (dock == null)
                    throw new EntityNotFoundException($"Dock with code {ds.Dock.Code} not found.");

                dockRelations.Add(new StorageArea.DockRelation(dock, ds.Distance, ds.IsServingDock));
            }
        }

        return dockRelations;
    }
}