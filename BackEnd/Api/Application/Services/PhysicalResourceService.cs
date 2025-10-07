namespace Api.Application.Services;

using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Application.Exceptions;
using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Exceptions;
using Api.Infrastructure.Utilities;

public class PhysicalResourceService
{
    private readonly IPhysicalResourceRepository _physicalResourceRepository;
    private readonly IDockRepository _dockRepository;
    private readonly IQualificationRepository _qualificationRepository;
    private readonly IStorageAreaRepository _storageAreaRepository;
    private readonly ILogger<PhysicalResourceService> _logger;

    public PhysicalResourceService(
        IPhysicalResourceRepository physicalResourceRepository, IDockRepository dockRepository,
        IQualificationRepository qualificationRepository, IStorageAreaRepository storageAreaRepository,
        ILogger<PhysicalResourceService> logger
    )
    {
        _physicalResourceRepository = physicalResourceRepository;
        _dockRepository = dockRepository;
        _qualificationRepository = qualificationRepository;
        _storageAreaRepository = storageAreaRepository;
        _logger = logger;
    }

    private HashSet<Qualification> GetQualificationsAsync(PhysicalResourceDto resourceDto)
    {
        HashSet<Qualification> qualifications = new HashSet<Qualification>();
        foreach (var qualificationDto in resourceDto.Qualifications)
        {
            Qualification? qualification = _qualificationRepository.GetQualificationByIdAsync(qualificationDto.IdCode).Result;
            if (qualification == null)
                throw new EntityNotFoundException($"The qualification with ID {qualificationDto.IdCode} does not exist.");

            qualifications.Add(qualification);
        }
        return qualifications;
    }

    private static object ConvertToDto(PhysicalResource resource)
    {
        if (resource is STSCrane stsCrane) return ((IDTOAble<STSCraneDto>)stsCrane).ToDTO();
        else if (resource is YardCrane yardCrane) return ((IDTOAble<YardCraneDto>)yardCrane).ToDTO();
        else if (resource is Truck truck) return ((IDTOAble<TruckDto>)truck).ToDTO();

        throw new UnknownPhysicalResourceType("Unknown physical resource type.");
    }

    public async Task<IEnumerable<object>> GetPhysicalResources()
    {
        IEnumerable<object> resources = await _physicalResourceRepository.GetPhysicalResourcesAsync();

        List<object> resourceDtos = resources
            .Select(resource => ConvertToDto((PhysicalResource)resource))
            .ToList();

        return resourceDtos;
    }

    public async Task<object?> GetResourceByCode(string code)
    {
        object? resource = await _physicalResourceRepository.GetResourceByCodeAsync(code);
        if (resource == null) return null;

        object? resourceDto = ConvertToDto((PhysicalResource)resource);
        return resourceDto;
    }

    public async Task<STSCraneDto> AddSTSCraneAsync(STSCraneDto resourceDto)
    {
        bool exists = await _physicalResourceRepository.GetResourceByCodeAsync(resourceDto.Code) != null;
        if (exists)
            throw new EntityAlreadyExistsException("A physical resource with this code already exists.");

        Dock? dock = await _dockRepository.GetDockByNameAsync(resourceDto.ServingDock.Name);
        if (dock == null)
            throw new EntityNotFoundException("The specified dock does not exist.");

        HashSet<Qualification> qualifications = GetQualificationsAsync(resourceDto);

        STSCrane crane = new STSCrane(
            Guid.NewGuid(),
            new Code { Value = resourceDto.Code },
            new Designation { Value = resourceDto.Description },
            resourceDto.Status,
            TimeSpan.FromMinutes(resourceDto.SetupTimeInMinutes),
            qualifications,
            new OperationalWindow(
                new Guid(),
                resourceDto.OperationalWindow.Shifts
            ),
            resourceDto.LiftingCapacity,
            dock,
            resourceDto.ContainersPerHour
        );

        _logger.LogInformation("Adding new STS Crane with code {CraneCode}", crane.Code.Value);
        return ((IDTOAble<STSCraneDto>)await _physicalResourceRepository.AddSTSCrane(crane)).ToDTO();
    }

    public async Task<YardCraneDto> AddYardCraneAsync(YardCraneDto resourceDto)
    {
        bool exists = await _physicalResourceRepository.GetResourceByCodeAsync(resourceDto.Code) != null;
        if (exists)
            throw new EntityAlreadyExistsException("A physical resource with this code already exists.");

        HashSet<Qualification> qualifications = GetQualificationsAsync(resourceDto);

        StorageArea? storageArea = await _storageAreaRepository.GetStorageAreaByCodeAsync(resourceDto.YardSection.NameCode);
        if (storageArea == null) throw new EntityNotFoundException("The specified storage area does not exist.");

        YardCrane crane = new YardCrane(
            Guid.NewGuid(),
            new Code { Value = resourceDto.Code },
            new Designation { Value = resourceDto.Description },
            resourceDto.Status,
            TimeSpan.FromMinutes(resourceDto.SetupTimeInMinutes),
            qualifications,
            new OperationalWindow(
                new Guid(),
                resourceDto.OperationalWindow.Shifts
            ),
            resourceDto.LiftingCapacity,
            storageArea,
            resourceDto.ContainersPerHour
        );

        _logger.LogInformation("Adding new Yard Crane with code {CraneCode}", crane.Code.Value);
        return ((IDTOAble<YardCraneDto>)await _physicalResourceRepository.AddYardCrane(crane)).ToDTO();
    }

    public async Task<TruckDto> AddTruckAsync(TruckDto resourceDto)
    {
        bool exists = await _physicalResourceRepository.GetResourceByCodeAsync(resourceDto.Code) != null;
        if (exists)
            throw new EntityAlreadyExistsException("A physical resource with this code already exists.");

        HashSet<Qualification> qualifications = GetQualificationsAsync(resourceDto);

        Truck truck = new Truck(
            Guid.NewGuid(),
            new Code { Value = resourceDto.Code },
            new Designation { Value = resourceDto.Description },
            resourceDto.Status,
            TimeSpan.FromMinutes(resourceDto.SetupTimeInMinutes),
            qualifications,
            new OperationalWindow(
                new Guid(),
                resourceDto.OperationalWindow.Shifts
            ),
            resourceDto.MaxLoadCapacity,
            resourceDto.ContainersPerTrip,
            resourceDto.AverageSpeed
        );

        _logger.LogInformation("Adding new Truck with code {TruckCode}", truck.Code.Value);
        return ((IDTOAble<TruckDto>)await _physicalResourceRepository.AddTruck(truck)).ToDTO();
    }

    public async Task<STSCraneDto> UpdateSTSCraneAsync(string code, STSCraneDto crane)
    {
        PhysicalResource? existingCrane = await _physicalResourceRepository.GetResourceByCodeAsync(code);
        if (existingCrane == null)
            throw new EntityNotFoundException("STS Crane to update not found.");

        if (existingCrane is not STSCrane)
            throw new InvalidOperationException("The physical resource with the specified code is not an STS Crane.");

        List<Qualification> qualifications = GetQualificationsAsync(crane).ToList();

        Dock? dock = await _dockRepository.GetDockByNameAsync(crane.ServingDock.Name);
        if (dock == null)
            throw new EntityNotFoundException("The specified dock does not exist.");

        STSCrane craneObject = (existingCrane as STSCrane)!;

        craneObject.UpdateDescription(new Designation { Value = crane.Description });
        craneObject.UpdateStatus(crane.Status);
        craneObject.UpdateSetupTime(TimeSpan.FromMinutes(crane.SetupTimeInMinutes));
        craneObject.UpdateQualifications(qualifications.ToHashSet());

        craneObject.UpdateLiftingCapacity(crane.LiftingCapacity);
        craneObject.UpdateContainersPerHour(crane.ContainersPerHour);
        craneObject.UpdateServingDock(dock);

        _logger.LogInformation("Updating STS Crane with code {CraneCode}", craneObject.Code.Value);
        return ((IDTOAble<STSCraneDto>)await _physicalResourceRepository.UpdateSTSCrane(craneObject)).ToDTO();
    }

    public async Task<YardCraneDto> UpdateYardCraneAsync(string code, YardCraneDto crane)
    {
        PhysicalResource? existingCrane = await _physicalResourceRepository.GetResourceByCodeAsync(code);
        if (existingCrane == null)
            throw new EntityNotFoundException("Yard Crane to update not found.");

        if (existingCrane is not YardCrane)
            throw new InvalidOperationException("The physical resource with the specified code is not a Yard Crane.");

        List<Qualification> qualifications = GetQualificationsAsync(crane).ToList();

        StorageArea? storageArea = await _storageAreaRepository.GetStorageAreaByCodeAsync(crane.YardSection.NameCode);
        if (storageArea == null) throw new EntityNotFoundException("The specified storage area does not exist.");

        YardCrane craneObject = (existingCrane as YardCrane)!;

        craneObject.UpdateDescription(new Designation { Value = crane.Description });
        craneObject.UpdateStatus(crane.Status);
        craneObject.UpdateSetupTime(TimeSpan.FromMinutes(crane.SetupTimeInMinutes));
        craneObject.UpdateQualifications(qualifications.ToHashSet());

        craneObject.UpdateLiftingCapacity(crane.LiftingCapacity);
        craneObject.UpdateContainersPerHour(crane.ContainersPerHour);
        craneObject.UpdateYardSection(storageArea);

        _logger.LogInformation("Updating Yard Crane with code {CraneCode}", craneObject.Code.Value);
        return ((IDTOAble<YardCraneDto>)await _physicalResourceRepository.UpdateYardCrane(craneObject)).ToDTO();
    }

    public async Task<TruckDto> UpdateTruckAsync(string code, TruckDto truck)
    {
        PhysicalResource? existingTruck = await _physicalResourceRepository.GetResourceByCodeAsync(code);
        if (existingTruck == null)
            throw new EntityNotFoundException("Truck to update not found.");

        if (existingTruck is not Truck)
            throw new InvalidOperationException("The physical resource with the specified code is not a Truck.");

        List<Qualification> qualifications = GetQualificationsAsync(truck).ToList();

        Truck truckObject = (existingTruck as Truck)!;

        truckObject.UpdateDescription(new Designation { Value = truck.Description });
        truckObject.UpdateStatus(truck.Status);
        truckObject.UpdateSetupTime(TimeSpan.FromMinutes(truck.SetupTimeInMinutes));
        truckObject.UpdateQualifications(qualifications.ToHashSet());

        truckObject.UpdateMaxLoadCapacity(truck.MaxLoadCapacity);
        truckObject.UpdateContainersPerTrip(truck.ContainersPerTrip);
        truckObject.UpdateAverageSpeed(truck.AverageSpeed);

        _logger.LogInformation("Updating Truck with code {TruckCode}", truckObject.Code.Value);
        return ((IDTOAble<TruckDto>)await _physicalResourceRepository.UpdateTruck(truckObject)).ToDTO();
    }

    public async Task<Page<object>> FilterPhysicalResources(PhysicalResourceFilter filter)
    {
        Page<PhysicalResource> page = await _physicalResourceRepository.FilterPhysicalResourcesAsync(filter);
        return page.Map<object>(resource => ConvertToDto(resource));
    }

    internal async Task<bool> DeactivateResource(string code)
    {
        PhysicalResource? resource = await _physicalResourceRepository.GetResourceByCodeAsync(code);
        if (resource == null) return false;

        resource.Deactivate();
        await _physicalResourceRepository.Update(resource);

        _logger.LogInformation("Deactivated physical resource with code {ResourceCode}", resource.Code.Value);
        return true;
    }
}
