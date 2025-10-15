namespace Api.Application.Services;

using Api.Application.Controllers;
using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Application.Exceptions;
using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Utilities;

public class VesselVisitNotificationService : IVesselVisitNotificationService
{
    private readonly IVesselVisitNotificationRepository _notificationRepository;
    private readonly IVesselRepository _vesselRepository;
    private readonly IRepresentativeRepository _representativeRepository;
    private readonly IStorageAreaRepository _storageAreaRepository;
    private readonly IContainerRepository _containerRepository;
    private readonly VesselVisitNotificationIdGenerator _idGenerator;

    public VesselVisitNotificationService(
        IVesselVisitNotificationRepository notificationRepository,
        IVesselRepository vesselRepository,
        IRepresentativeRepository representativeRepository,
        IStorageAreaRepository storageAreaRepository,
        VesselVisitNotificationIdGenerator idGenerator,
        IContainerRepository containerRepository
    )
    {
        _notificationRepository = notificationRepository;
        _vesselRepository = vesselRepository;
        _representativeRepository = representativeRepository;
        _storageAreaRepository = storageAreaRepository;
        _containerRepository = containerRepository;
        _idGenerator = idGenerator;
    }

    public async Task<IEnumerable<VesselVisitNotificationDto>> GetVesselVisitNotifications()
    {
        IEnumerable<VesselVisitNotification> notifications = await _notificationRepository.GetVesselVisitNotificationsAsync();
        return notifications.Select(n => n.ToDTO());
    }

    private List<CargoTransport>? GetNewCargoManifestFromDTO(ICollection<CargoTransportDto>? cargoManifestDto)
    {
        if (cargoManifestDto == null) return null;

        var newCargoManifestItems = new List<CargoTransport>();
        foreach (var item in cargoManifestDto)
        {
            StorageArea? itemStorageArea = _storageAreaRepository.GetStorageAreaByCodeAsync(item.Area.NameCode).Result;
            if (itemStorageArea == null) throw new EntityNotFoundException($"Storage Area with code {item.Area.NameCode} was not found.");

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

    public async Task<VesselVisitNotificationDto?> Add(VesselVisitNotificationDto vesselVisitNotificationDto)
    {
        Vessel vessel = await _vesselRepository.GetVesselByIMOAsync(vesselVisitNotificationDto.Vessel.ImoNumber);
        if (vessel == null) throw new EntityNotFoundException($"Vessel with IMO {vesselVisitNotificationDto.Vessel.ImoNumber} was not found.");

        Representative representative = await _representativeRepository.GetByCitizenIdAsync(
            vesselVisitNotificationDto.Submitter.CitizenshipId.ToString()
        );

        if (representative == null) throw new EntityNotFoundException($"Representative with Citizenship ID {vesselVisitNotificationDto.Submitter.CitizenshipId} was not found.");

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

        await _notificationRepository.AddAsync(notification);
        return notification.ToDTO();
    }

    public async Task<VesselVisitNotificationDto?> Update(string vvnID, VesselVisitNotificationDto vvnDTO)
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

        Console.WriteLine("Updated Notification: " + existingNotification.ToString());

        return (await _notificationRepository.UpdateAsync(existingNotification)).ToDTO();
    }

    internal async Task<Page<VesselVisitNotificationStatusDto>> FilterNotifications(VesselVisitNotificationFilter filter)
    {
        Page<VesselVisitNotification> page = await _notificationRepository.FilterVesselVisitNotificationsAsync(filter);
        return page.Map(vvn => vvn.ToStatusDTO());
    }
}