using Api.Application.Exceptions;
using Api.Domain.Model;
using Domain.IRepository;
using Domain.Model.Generic;

public class PhysicalResourceService
{
    private readonly IPhysicalResourceRepository _physicalResourceRepository;
    private readonly IDockRepository _dockRepository;
    private readonly IQualificationRepository _qualificationRepository;
    private readonly IStorageAreaRepository _storageAreaRepository;

    public PhysicalResourceService(
        IPhysicalResourceRepository physicalResourceRepository, IDockRepository dockRepository,
        IQualificationRepository qualificationRepository, IStorageAreaRepository storageAreaRepository
    )
    {
        _physicalResourceRepository = physicalResourceRepository;
        _dockRepository = dockRepository;
        _qualificationRepository = qualificationRepository;
        _storageAreaRepository = storageAreaRepository;
    }

    public async Task<IEnumerable<object>> GetPhysicalResources()
    {
        IEnumerable<object> resources = await _physicalResourceRepository.GetPhysicalResourcesAsync();

        List<object> resourceDtos = new List<object>();
        foreach (var resource in resources)
        {
            if (resource is STSCrane stsCrane) resourceDtos.Add(((IDTOAble<STSCraneDto>)stsCrane).ToDTO());
            else if (resource is YardCrane yardCrane) resourceDtos.Add(((IDTOAble<YardCraneDto>)yardCrane).ToDTO());
            else if (resource is Truck truck) resourceDtos.Add(((IDTOAble<TruckDto>)truck).ToDTO());
        }
        return resourceDtos;
    }

    public async Task<object?> GetResourceByCode(string code)
    {
        object? resource = await _physicalResourceRepository.GetResourceByCodeAsync(code);
        if (resource == null) return null;

        object? resourceDto = null;
        if (resource is STSCrane stsCrane) resourceDto = ((IDTOAble<STSCraneDto>)stsCrane).ToDTO();
        else if (resource is YardCrane yardCrane) resourceDto = ((IDTOAble<YardCraneDto>)yardCrane).ToDTO();
        else if (resource is Truck truck) resourceDto = ((IDTOAble<TruckDto>)truck).ToDTO();

        return resourceDto;
    }

    public async Task<STSCraneDto?> AddSTSCraneAsync(STSCraneDto resourceDto)
    {
        bool exists = await _physicalResourceRepository.GetResourceByCodeAsync(resourceDto.Code) != null;
        if (exists)
            throw new EntityAlreadyExistsException("A physical resource with this code already exists.");

        Dock? dock = await _dockRepository.GetDockByNameAsync(resourceDto.ServingDock.Name);
        if (dock == null)
            throw new EntityNotFoundException("The specified dock does not exist.");

        List<Qualification> qualifications = new List<Qualification>();
        foreach (var qualificationDto in resourceDto.Qualifications)
        {
            Qualification? qualification = await _qualificationRepository.GetQualificationByIdAsync(qualificationDto.IdCode);
            if (qualification == null)
                throw new EntityNotFoundException($"The qualification with ID {qualificationDto.IdCode} does not exist.");

            qualifications.Add(qualification);
        }

        STSCrane crane = new STSCrane(
            Guid.NewGuid(),
            new Code { Value = resourceDto.Code },
            new Designation { Value = resourceDto.Description },
            resourceDto.Status,
            TimeSpan.FromMinutes(resourceDto.SetupTimeInMinutes),
            qualifications,
            resourceDto.LiftingCapacity,
            dock,
            resourceDto.ContainersPerHour
        );

        return ((IDTOAble<STSCraneDto>) await _physicalResourceRepository.AddSTSCrane(crane)).ToDTO();
    }

    public async Task<YardCraneDto?> AddYardCraneAsync(YardCraneDto resourceDto)
    {
        bool exists = await _physicalResourceRepository.GetResourceByCodeAsync(resourceDto.Code) != null;
        if (exists)
            throw new EntityAlreadyExistsException("A physical resource with this code already exists.");

        List<Qualification> qualifications = new List<Qualification>();
        foreach (var qualificationDto in resourceDto.Qualifications)
        {
            Qualification? qualification = await _qualificationRepository.GetQualificationByIdAsync(qualificationDto.IdCode);
            if (qualification == null)
                throw new EntityNotFoundException($"The qualification with ID {qualificationDto.IdCode} does not exist.");

            qualifications.Add(qualification);
        }

        StorageArea? storageArea = await _storageAreaRepository.GetStorageAreaByCodeAsync(resourceDto.YardSection.NameCode);
        if (storageArea == null) throw new EntityNotFoundException("The specified storage area does not exist.");
        else if (storageArea.AreaType != StorageAreaType.Yard) throw new InvalidOperationException("The specified storage area is not a yard section.");

        YardCrane crane = new YardCrane(
            Guid.NewGuid(),
            new Code { Value = resourceDto.Code },
            new Designation { Value = resourceDto.Description },
            resourceDto.Status,
            TimeSpan.FromMinutes(resourceDto.SetupTimeInMinutes),
            qualifications,
            resourceDto.LiftingCapacity,
            storageArea,
            resourceDto.ContainersPerHour
        );

        return ((IDTOAble<YardCraneDto>)await _physicalResourceRepository.AddYardCrane(crane)).ToDTO();
    }

    public async Task<TruckDto?> AddTruckAsync(TruckDto resourceDto)
    {
        bool exists = await _physicalResourceRepository.GetResourceByCodeAsync(resourceDto.Code) != null;
        if (exists)
            throw new EntityAlreadyExistsException("A physical resource with this code already exists.");

        List<Qualification> qualifications = new List<Qualification>();
        foreach (var qualificationDto in resourceDto.Qualifications)
        {
            Qualification? qualification = await _qualificationRepository.GetQualificationByIdAsync(qualificationDto.IdCode);
            if (qualification == null)
                throw new EntityNotFoundException($"The qualification with ID {qualificationDto.IdCode} does not exist.");

            qualifications.Add(qualification);
        }

        Truck truck = new Truck(
            Guid.NewGuid(),
            new Code { Value = resourceDto.Code },
            new Designation { Value = resourceDto.Description },
            resourceDto.Status,
            TimeSpan.FromMinutes(resourceDto.SetupTimeInMinutes),
            qualifications,
            resourceDto.MaxLoadCapacity,
            resourceDto.ContainersPerTrip,
            resourceDto.AverageSpeed
        );

        return ((IDTOAble<TruckDto>)await _physicalResourceRepository.AddTruck(truck)).ToDTO();
    }
}
