namespace Api.Application.Services;

using Api.Application.Controllers;
using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Application.Exceptions;
using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Exceptions;
using Api.Infrastructure.Utilities;

public class VesselVisitNotificationService : IVesselVisitNotificationService
{
    private readonly IVesselVisitNotificationRepository _notificationRepository;
    private readonly IVesselRepository _vesselRepository;
    private readonly IRepresentativeRepository _representativeRepository;
    private readonly IStorageAreaRepository _storageAreaRepository;
    private readonly IContainerRepository _containerRepository;
    private readonly VesselVisitNotificationIdGenerator _idGenerator;
    private readonly ILogger<VesselVisitNotificationService> _logger;

    public VesselVisitNotificationService(
        IVesselVisitNotificationRepository notificationRepository,
        IVesselRepository vesselRepository,
        IRepresentativeRepository representativeRepository,
        IStorageAreaRepository storageAreaRepository,
        VesselVisitNotificationIdGenerator idGenerator,
        IContainerRepository containerRepository,
        ILogger<VesselVisitNotificationService> logger
    )
    {
        _notificationRepository = notificationRepository;
        _vesselRepository = vesselRepository;
        _representativeRepository = representativeRepository;
        _storageAreaRepository = storageAreaRepository;
        _containerRepository = containerRepository;
        _idGenerator = idGenerator;
        _logger = logger;
    }

    public async Task<IEnumerable<VesselVisitNotificationDto>> GetVesselVisitNotifications()
    {
        IEnumerable<VesselVisitNotification> notifications = await _notificationRepository.GetVesselVisitNotificationsAsync();
        AppLogEvents.LogRetrieve(_logger, "vessel visit notifications", notifications.Count());
        return notifications.Select(n => n.ToDTO());
    }

    
    public async Task<VesselVisitNotificationDto> GetById(string vvnID)
    {
        var notification = await _notificationRepository.GetVesselVisitNotificationByNotificationIdAsync(vvnID);
        if (notification == null) throw new EntityNotFoundException($"Vessel Visit Notification with id {vvnID} was not found.");
        return notification.ToDTO();
    }

    private List<CargoTransport>? GetNewCargoManifestFromDTO(ICollection<CreateCargoTransportDto>? cargoManifestDto)
    {
        if (cargoManifestDto == null) return null;

        var newCargoManifestItems = new List<CargoTransport>();
        foreach (var item in cargoManifestDto)
        {
            StorageArea? itemStorageArea = _storageAreaRepository.GetStorageAreaByCodeAsync(item.StorageAreaCode).Result;
            if (itemStorageArea == null) throw new EntityNotFoundException($"Storage Area with code {item.StorageAreaCode} was not found.");

            Container? container = _containerRepository.GetContainerByNumberAsync(item.Container.ContainerNumber).Result;
            if (container == null)
            {
                container = new Container(
                    Guid.NewGuid(),
                    new ContainerNumber { Value = item.Container.ContainerNumber },
                    item.Container.CargoType,
                    new Designation { Value = item.Container.Description }
                );

                _containerRepository.Add(container).Wait();
            }

            CargoTransport cargoTransport = new CargoTransport(
                item.Position,
                itemStorageArea,
                container
            );

            newCargoManifestItems.Add(cargoTransport);
        }

        return newCargoManifestItems;
    }

    public async Task<VesselVisitNotificationDto> Add(CreateVesselVisitNotificationDto vesselVisitNotificationDto)
    {
        VesselVisitNotification? existingNotification =
            await _notificationRepository.GetVesselVisitNotificationByNotificationIdAsync(vesselVisitNotificationDto.NotificationId);

        if (existingNotification != null) throw new EntityAlreadyExistsException($"Vessel Visit Notification with id {vesselVisitNotificationDto.NotificationId} already exists.");

        Vessel vessel = await _vesselRepository.GetVesselByIMOAsync(vesselVisitNotificationDto.VesselImoNumber);
        if (vessel == null) throw new EntityNotFoundException($"Vessel with IMO {vesselVisitNotificationDto.VesselImoNumber} was not found.");

        Representative representative = await _representativeRepository.GetByCitizenIdAsync(
            vesselVisitNotificationDto.SubmitterId
        );

        if (representative == null) throw new EntityNotFoundException($"Representative with Citizenship ID {vesselVisitNotificationDto.SubmitterId} was not found.");

        Crew? crew = null;
        if (vesselVisitNotificationDto.CrewDetails != null)
        {
            crew = new Crew(
                vesselVisitNotificationDto.CrewDetails.Captain,
                vesselVisitNotificationDto.CrewDetails.TotalCrewMembers,
                vesselVisitNotificationDto.CrewDetails.SafetyOfficers
            );
        }

        List<CargoTransport>? loadCargoManifest = GetNewCargoManifestFromDTO(vesselVisitNotificationDto.LoadCargoManifest);
        List<CargoTransport>? unloadCargoManifest = GetNewCargoManifestFromDTO(vesselVisitNotificationDto.UnloadCargoManifest);

        IEnumerable<VesselVisitNotification> notifications = await _notificationRepository.GetVesselVisitNotificationsAsync();
        int sequenceNumber = notifications.Count(n => n.ExpectedArrival.Year == DateTime.UtcNow.Year) + 1;
        string sequenceNumberStr = sequenceNumber.ToString("D6"); // Pad with leading zeros

        VesselVisitNotification notification = new VesselVisitNotification(
            _idGenerator.Generate((uint)vesselVisitNotificationDto.ExpectedArrival.Date.Year),
            vesselVisitNotificationDto.ExpectedArrival,
            vesselVisitNotificationDto.ExpectedDeparture,
            vesselVisitNotificationDto.IsCargoHazardous,
            vessel,
            representative,
            vesselVisitNotificationDto.SpecialRequirements,
            crew,
            loadCargoManifest,
            unloadCargoManifest
        );

        //_logger.LogInformation("Vessel Visit Notification with id {VvnId} created.", notification.NotificationId);
        AppLogEvents.LogCreate(_logger, "vessel visit notification", notification.NotificationId);
        await _notificationRepository.AddAsync(notification);
        return notification.ToDTO();
    }

    public async Task<VesselVisitNotificationDto> Update(string vvnID, CreateVesselVisitNotificationDto vvnDTO)
    {
        var existingNotification =
            await _notificationRepository.GetVesselVisitNotificationByNotificationIdAsync(vvnID) ??
            throw new EntityNotFoundException($"Vessel Visit Notification with id {vvnID} was not found.");

        Crew? newCrewDetails =
            vvnDTO.CrewDetails != null ?
            new Crew(vvnDTO.CrewDetails.Captain, vvnDTO.CrewDetails.TotalCrewMembers, vvnDTO.CrewDetails.SafetyOfficers) :
            null;

        List<CargoTransport>? newLoadCargoManifest = GetNewCargoManifestFromDTO(vvnDTO.LoadCargoManifest);
        List<CargoTransport>? newUnloadCargoManifest = GetNewCargoManifestFromDTO(vvnDTO.UnloadCargoManifest);

        existingNotification.Update(vvnDTO.ExpectedArrival, vvnDTO.ExpectedDeparture, vvnDTO.IsCargoHazardous,
            vvnDTO.SpecialRequirements, newCrewDetails, newLoadCargoManifest, newUnloadCargoManifest
        );

        //_logger.LogInformation("Vessel Visit Notification with id {VvnId} updated.", vvnID);
        AppLogEvents.LogUpdate(_logger, "vessel visit notification", vvnID);
        return (await _notificationRepository.UpdateAsync(existingNotification)).ToDTO();
    }

    public async Task<Page<VesselVisitNotificationStatusDto>> FilterNotifications(VesselVisitNotificationFilter filter)
    {
        Page<VesselVisitNotification> page = await _notificationRepository.FilterVesselVisitNotificationsAsync(filter);
        AppLogEvents.LogFilter(_logger, "vessel visit notifications", page.Items.Count);
        return page.Map(vvn => vvn.ToStatusDTO());
    }
}