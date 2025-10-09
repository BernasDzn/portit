namespace Api.Application.Services;

using Api.Application.DataTransfer;
using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Utilities;

public class VesselVisitNotificationService
{
    private readonly IVesselVisitNotificationRepository _notificationRepository;
    private readonly IVesselRepository _vesselRepository;
    private readonly IRepresentativeRepository _representativeRepository;
    private readonly IStorageAreaRepository _storageAreaRepository;

    public VesselVisitNotificationService(IVesselVisitNotificationRepository notificationRepository,
                                          IVesselRepository vesselRepository,
                                          IRepresentativeRepository representativeRepository,
                                          IStorageAreaRepository storageAreaRepository)
    {
        _notificationRepository = notificationRepository;
        _vesselRepository = vesselRepository;
        _representativeRepository = representativeRepository;
        _storageAreaRepository = storageAreaRepository;
    }

    public async Task<IEnumerable<VesselVisitNotificationDto>> GetVesselVisitNotifications()
    {
        IEnumerable<VesselVisitNotification> notifications = await _notificationRepository.GetVesselVisitNotificationsAsync();
        return notifications.Select(n => n.ToDTO());
    }

    public async Task<VesselVisitNotificationDto?> Add(VesselVisitNotificationDto vesselVisitNotificationDto)
    {
        Vessel vessel = await _vesselRepository.GetVesselByIMOAsync(vesselVisitNotificationDto.Vessel.ImoNumber);
        if (vessel == null) return null;

        Representative representative = await _representativeRepository.GetByCitizenIdAsync(
            vesselVisitNotificationDto.Representative.CitizenshipId.ToString()
            );
        if (representative == null) return null;

        Crew? crew = null;
        if (vesselVisitNotificationDto.CrewDetails != null)
        {
            crew = new Crew(
                vesselVisitNotificationDto.CrewDetails.Captain,
                vesselVisitNotificationDto.CrewDetails.TotalCrewMembers,
                vesselVisitNotificationDto.CrewDetails.SafetyOfficers
            );
        }

        CargoManifest? loadCargoManifest = null;
        if (vesselVisitNotificationDto.LoadCargoManifest != null)
        {
            var loadItems = new List<CargoTransport>();
            foreach (var item in vesselVisitNotificationDto.LoadCargoManifest.Items)
            {
                Container container = new Container(
                    new ContainerNumber(item.Container.ContainerNumber),
                    item.Container.ContainerRow != null && item.Container.ContainerBay != null && item.Container.ContainerTier != null
                        ? new ContainerPosition(item.Container.ContainerRow, item.Container.ContainerBay, item.Container.ContainerTier)
                        : null,
                    new CargoType(CargoType.FromString(item.Container.CargoType)),
                    item.Container.Description
                );

                StorageArea? area = await _storageAreaRepository.GetStorageAreaByCodeAsync(item.Area.NameCode);
                if (area == null) throw new ArgumentException("Invalid Storage Area in Load Cargo Manifest: " + item.Area.NameCode);

                CargoTransport cargoTransport = new CargoTransport(
                    new ContainerPosition(item.Position.Row, item.Position.Bay, item.Position.Tier),
                    area,
                    container
                );
                loadItems.Add(cargoTransport);
            }
            loadCargoManifest = new CargoManifest(loadItems);
        }

        CargoManifest? unloadCargoManifest = null;
        if (vesselVisitNotificationDto.UnloadCargoManifest != null)
        {
            var unloadItems = new List<CargoTransport>();
            foreach (var item in vesselVisitNotificationDto.UnloadCargoManifest.Items)
            {
                Container container = new Container(
                    new ContainerNumber(item.Container.ContainerNumber),
                    item.Container.ContainerRow != null && item.Container.ContainerBay != null && item.Container.ContainerTier != null
                        ? new ContainerPosition(item.Container.ContainerRow, item.Container.ContainerBay, item.Container.ContainerTier)
                        : null,
                    new CargoType(CargoType.FromString(item.Container.CargoType)),
                    item.Container.Description
                );

                StorageArea? area = await _storageAreaRepository.GetStorageAreaByCodeAsync(item.Area.NameCode);
                if (area == null) throw new ArgumentException("Invalid Storage Area in Unload Cargo Manifest: " + item.Area.NameCode);

                CargoTransport cargoTransport = new CargoTransport(
                    new ContainerPosition(item.Position.Row, item.Position.Bay, item.Position.Tier),
                    area,
                    container
                );
                unloadItems.Add(cargoTransport);
            }
            unloadCargoManifest = new CargoManifest(unloadItems);
        } 

        IEnumerable<VesselVisitNotification> notifications = await _notificationRepository.GetVesselVisitNotificationsAsync();
        int sequenceNumber = notifications.Count(n => n.ExpectedArrival.Year == DateTime.UtcNow.Year) + 1;
        string sequenceNumberStr = sequenceNumber.ToString("D6"); // Pad with leading zeros
        
        VesselVisitNotification notification = new VesselVisitNotification(
            "PORTO",
            sequenceNumberStr,
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
}