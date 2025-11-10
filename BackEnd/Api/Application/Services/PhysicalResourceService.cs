namespace Api.Application.Services;

using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Application.Exceptions;
using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Exceptions;
using Api.Infrastructure.Utilities;

public class PhysicalResourceService : IPhysicalResourceService
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

    private IEnumerable<Qualification> GetQualificationsAsync(IEnumerable<string> qualificationsCodes)
    {
        HashSet<Qualification> qualifications = new HashSet<Qualification>();
        foreach (var IdCode in qualificationsCodes)
        {
            Qualification? qualification = _qualificationRepository.GetQualificationByIdAsync(IdCode).Result;
            if (qualification == null)
                throw new EntityNotFoundException($"The qualification with ID {IdCode} does not exist.");

            qualifications.Add(qualification);
        }
        return qualifications;
    }

    private static object ConvertToDto(PhysicalResource resource)
    {
        if (resource is STSCrane stsCrane) return ((IDTOAble<STSCraneDto>)stsCrane).ToDTO();
        else if (resource is YardCrane yardCrane) return ((IDTOAble<YardCraneDto>)yardCrane).ToDTO();
        else if (resource is Truck truck) return ((IDTOAble<TruckDto>)truck).ToDTO();

        throw new UnknownPhysicalResourceTypeException("Unknown physical resource type.");
    }

    public async Task<IEnumerable<object>> GetPhysicalResources()
    {
        IEnumerable<object> resources = await _physicalResourceRepository.GetPhysicalResourcesAsync();

        List<object> resourceDtos = resources
            .Select(resource => ConvertToDto((PhysicalResource)resource))
            .ToList();

        AppLogEvents.LogRetrieve(_logger, "physical resources", resourceDtos.Count);
        return resourceDtos;
    }

    public async Task<object> GetResourceByCode(string code)
    {
        object? resource = await _physicalResourceRepository.GetResourceByCodeAsync(code);
        if (resource == null) throw new EntityNotFoundException("Physical resource not found");

        object resourceDto = ConvertToDto((PhysicalResource)resource);
        AppLogEvents.LogRetrieve(_logger, "physical resource", 1);
        return resourceDto;
    }

    public async Task<int> CountPhysicalResourcesAsync()
    {
        int count = await _physicalResourceRepository.CountAsync();
        AppLogEvents.LogRetrieve(_logger, "physical resources", count);
        return count;
    }

    public async Task<STSCraneDto> AddSTSCraneAsync(CreateSTSCraneDto resourceDto)
    {
        bool exists = await _physicalResourceRepository.GetResourceByCodeAsync(resourceDto.Code) != null;
        if (exists)
            throw new EntityAlreadyExistsException("A physical resource with this code already exists.");

        Dock? dock = await _dockRepository.GetDockByCodeAsync(resourceDto.ServingDockCode);
        if (dock == null)
            throw new EntityNotFoundException("The specified dock does not exist.");

        IEnumerable<Qualification> qualifications = GetQualificationsAsync(resourceDto.QualificationsCodes);

        STSCrane crane = new STSCrane(
            Guid.NewGuid(),
            new Code { Value = resourceDto.Code },
            new Designation { Value = resourceDto.Description },
            resourceDto.Status,
            TimeSpan.FromMinutes(resourceDto.SetupTimeInMinutes),
            qualifications.ToHashSet(),
            resourceDto.OperationalWindow,
            resourceDto.LiftingCapacity,
            dock,
            resourceDto.ContainersPerHour
        );

        //_logger.LogInformation("Adding new STS Crane with code {CraneCode}", crane.Code.Value);
        AppLogEvents.LogCreate(_logger, "STS Crane", crane.Id);
        return ((IDTOAble<STSCraneDto>)await _physicalResourceRepository.AddSTSCrane(crane)).ToDTO();
    }

    public async Task<YardCraneDto> AddYardCraneAsync(CreateYardCraneDto resourceDto)
    {
        bool exists = await _physicalResourceRepository.GetResourceByCodeAsync(resourceDto.Code) != null;
        if (exists)
            throw new EntityAlreadyExistsException("A physical resource with this code already exists.");

        IEnumerable<Qualification> qualifications = GetQualificationsAsync(resourceDto.QualificationsCodes);

        YardCrane crane = new YardCrane(
            Guid.NewGuid(),
            new Code { Value = resourceDto.Code },
            new Designation { Value = resourceDto.Description },
            resourceDto.Status,
            TimeSpan.FromMinutes(resourceDto.SetupTimeInMinutes),
            qualifications.ToHashSet(),
            resourceDto.OperationalWindow,
            resourceDto.LiftingCapacity,
            resourceDto.ContainersPerHour
        );

        //_logger.LogInformation("Adding new Yard Crane with code {CraneCode}", crane.Code.Value);
        AppLogEvents.LogCreate(_logger, "Yard Crane", crane.Id);
        return ((IDTOAble<YardCraneDto>)await _physicalResourceRepository.AddYardCrane(crane)).ToDTO();
    }

    public async Task<TruckDto> AddTruckAsync(CreateTruckDto resourceDto)
    {
        bool exists = await _physicalResourceRepository.GetResourceByCodeAsync(resourceDto.Code) != null;
        if (exists)
            throw new EntityAlreadyExistsException("A physical resource with this code already exists.");

        IEnumerable<Qualification> qualifications = GetQualificationsAsync(resourceDto.QualificationsCodes);

        Truck truck = new Truck(
            Guid.NewGuid(),
            new Code { Value = resourceDto.Code },
            new Designation { Value = resourceDto.Description },
            resourceDto.Status,
            TimeSpan.FromMinutes(resourceDto.SetupTimeInMinutes),
            qualifications.ToHashSet(),
            resourceDto.OperationalWindow,
            resourceDto.MaxLoadCapacity,
            resourceDto.ContainersPerTrip,
            resourceDto.AverageSpeed
        );

        //_logger.LogInformation("Adding new Truck with code {TruckCode}", truck.Code.Value);
        AppLogEvents.LogCreate(_logger, "Truck", truck.Id);
        return ((IDTOAble<TruckDto>)await _physicalResourceRepository.AddTruck(truck)).ToDTO();
    }

    public async Task<STSCraneDto> UpdateSTSCraneAsync(string code, CreateSTSCraneDto crane)
    {
        PhysicalResource? existingCrane = await _physicalResourceRepository.GetResourceByCodeAsync(code);
        if (existingCrane == null)
            throw new EntityNotFoundException("STS Crane to update not found.");

        if (existingCrane is not STSCrane)
            throw new InvalidOperationException("The physical resource with the specified code is not an STS Crane.");

        List<Qualification> qualifications = GetQualificationsAsync(crane.QualificationsCodes).ToList();

        Dock? dock = await _dockRepository.GetDockByCodeAsync(crane.ServingDockCode);
        if (dock == null)
            throw new EntityNotFoundException("The specified dock does not exist.");

        STSCrane craneObject = (existingCrane as STSCrane)!;

        craneObject.UpdateDescription(new Designation { Value = crane.Description });
        craneObject.UpdateStatus(crane.Status);
        craneObject.UpdateSetupTime(TimeSpan.FromMinutes(crane.SetupTimeInMinutes));
        craneObject.UpdateQualifications(qualifications.ToHashSet());
        craneObject.UpdateOperationalWindow(crane.OperationalWindow);

        craneObject.UpdateLiftingCapacity(crane.LiftingCapacity);
        craneObject.UpdateContainersPerHour(crane.ContainersPerHour);
        craneObject.UpdateServingDock(dock);

        //_logger.LogInformation("Updating STS Crane with code {CraneCode}", craneObject.Code.Value);
        AppLogEvents.LogUpdate(_logger, "STS Crane", craneObject.Id);
        return ((IDTOAble<STSCraneDto>)await _physicalResourceRepository.UpdateSTSCrane(craneObject)).ToDTO();
    }

    public async Task<YardCraneDto> UpdateYardCraneAsync(string code, CreateYardCraneDto crane)
    {
        PhysicalResource? existingCrane = await _physicalResourceRepository.GetResourceByCodeAsync(code);
        if (existingCrane == null)
            throw new EntityNotFoundException("Yard Crane to update not found.");

        if (existingCrane is not YardCrane)
            throw new InvalidOperationException("The physical resource with the specified code is not a Yard Crane.");

        List<Qualification> qualifications = GetQualificationsAsync(crane.QualificationsCodes).ToList();

        YardCrane craneObject = (existingCrane as YardCrane)!;

        craneObject.UpdateDescription(new Designation { Value = crane.Description });
        craneObject.UpdateStatus(crane.Status);
        craneObject.UpdateSetupTime(TimeSpan.FromMinutes(crane.SetupTimeInMinutes));
        craneObject.UpdateQualifications(qualifications.ToHashSet());
        craneObject.UpdateOperationalWindow(crane.OperationalWindow);

        craneObject.UpdateLiftingCapacity(crane.LiftingCapacity);
        craneObject.UpdateContainersPerHour(crane.ContainersPerHour);

        //_logger.LogInformation("Updating Yard Crane with code {CraneCode}", craneObject.Code.Value);
        AppLogEvents.LogUpdate(_logger, "Yard Crane", craneObject.Id);
        return ((IDTOAble<YardCraneDto>)await _physicalResourceRepository.UpdateYardCrane(craneObject)).ToDTO();
    }

    public async Task<TruckDto> UpdateTruckAsync(string code, CreateTruckDto truck)
    {
        PhysicalResource? existingTruck = await _physicalResourceRepository.GetResourceByCodeAsync(code);
        if (existingTruck == null)
            throw new EntityNotFoundException("Truck to update not found.");

        if (existingTruck is not Truck)
            throw new InvalidOperationException("The physical resource with the specified code is not a Truck.");

        List<Qualification> qualifications = GetQualificationsAsync(truck.QualificationsCodes).ToList();

        Truck truckObject = (existingTruck as Truck)!;

        truckObject.UpdateDescription(new Designation { Value = truck.Description });
        truckObject.UpdateStatus(truck.Status);
        truckObject.UpdateSetupTime(TimeSpan.FromMinutes(truck.SetupTimeInMinutes));
        truckObject.UpdateQualifications(qualifications.ToHashSet());
        truckObject.UpdateOperationalWindow(truck.OperationalWindow);

        truckObject.UpdateMaxLoadCapacity(truck.MaxLoadCapacity);
        truckObject.UpdateContainersPerTrip(truck.ContainersPerTrip);
        truckObject.UpdateAverageSpeed(truck.AverageSpeed);

        //_logger.LogInformation("Updating Truck with code {TruckCode}", truckObject.Code.Value);
        AppLogEvents.LogUpdate(_logger, "Truck", truckObject.Id);
        return ((IDTOAble<TruckDto>)await _physicalResourceRepository.UpdateTruck(truckObject)).ToDTO();
    }

    public async Task<Page<object>> FilterPhysicalResources(PhysicalResourceFilter filter)
    {
        Page<PhysicalResource> page = await _physicalResourceRepository.FilterPhysicalResourcesAsync(filter);
        AppLogEvents.LogFilter(_logger, "physical resources", page.Items.Count);
        return page.Map<object>(resource => ConvertToDto(resource));
    }

    public async Task<bool> DeactivateResource(string code)
    {
        PhysicalResource? resource = await _physicalResourceRepository.GetResourceByCodeAsync(code);
        if (resource == null) throw new EntityNotFoundException("Physical resource to deactivate not found.");

        resource.Deactivate();
        await _physicalResourceRepository.Update(resource);

        AppLogEvents.LogDeactivate(_logger, "physical resource", resource.Id);
        return true;
    }
}
