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

        return notifications.Select(n => n.ToDTO()).ToList();
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
            var loadCargoManifestItems = new List<CargoTransport>();
            foreach (var item in vesselVisitNotificationDto.LoadCargoManifest.Items)
            {
                Either<StorageArea, ContainerPosition> source;
                if (item.Source.IsLeft)
                {
                    var storageArea = await _storageAreaRepository.GetStorageAreaByCodeAsync(item.Source.Left.NameCode);
                    if (storageArea == null)
                    {
                        continue;
                    }
                    source = new Either<StorageArea, ContainerPosition>(storageArea);
                }
                else
                {
                    var containerPositionDto = item.Source.Right;
                    var containerPosition = new ContainerPosition(
                        containerPositionDto.Row,
                        containerPositionDto.Bay,
                        containerPositionDto.Tier
                    );
                    source = new Either<StorageArea, ContainerPosition>(containerPosition);
                }

                Either<StorageArea, ContainerPosition> destination;
                if (item.Destination.IsLeft)
                {
                    var storageArea = await _storageAreaRepository.GetStorageAreaByCodeAsync(item.Destination.Left.NameCode);
                    if (storageArea == null)
                    {
                        continue;
                    }
                    destination = new Either<StorageArea, ContainerPosition>(storageArea);
                }
                else
                {
                    var containerPositionDto = item.Destination.Right;
                    var containerPosition = new ContainerPosition(
                        containerPositionDto.Row,
                        containerPositionDto.Bay,
                        containerPositionDto.Tier
                    );
                    destination = new Either<StorageArea, ContainerPosition>(containerPosition);
                }

                var containerDto = item.Container;
                var container = new Container(
                    new ContainerNumber(containerDto.ContainerNumber),
                    new ContainerPosition(containerDto.ContainerRow, containerDto.ContainerBay, containerDto.ContainerTier),
                    new CargoType(CargoType.FromString(containerDto.CargoType)),
                    containerDto.Description
                );

                var cargoTransport = new CargoTransport(
                    source,
                    destination,
                    container
                );
                loadCargoManifestItems.Add(cargoTransport);
            }
            loadCargoManifest = new CargoManifest(loadCargoManifestItems);
        }

        CargoManifest? unloadCargoManifest = null;
        if (vesselVisitNotificationDto.UnloadCargoManifest != null)
        {
            var unloadCargoManifestItems = new List<CargoTransport>();
            foreach (var item in vesselVisitNotificationDto.UnloadCargoManifest.Items)
            {
                Either<StorageArea, ContainerPosition> source;
                if (item.Source.IsLeft)
                {
                    var storageArea = await _storageAreaRepository.GetStorageAreaByCodeAsync(item.Source.Left.NameCode);
                    if (storageArea == null)
                    {
                        continue;
                    }
                    source = new Either<StorageArea, ContainerPosition>(storageArea);
                }
                else
                {
                    var containerPositionDto = item.Source.Right;
                    var containerPosition = new ContainerPosition(
                        containerPositionDto.Row,
                        containerPositionDto.Bay,
                        containerPositionDto.Tier
                    );
                    source = new Either<StorageArea, ContainerPosition>(containerPosition);
                }

                Either<StorageArea, ContainerPosition> destination;
                if (item.Destination.IsLeft)
                {
                    var storageArea = await _storageAreaRepository.GetStorageAreaByCodeAsync(item.Destination.Left.NameCode);
                    if (storageArea == null)
                    {
                        continue;
                    }
                    destination = new Either<StorageArea, ContainerPosition>(storageArea);
                }
                else
                {
                    var containerPositionDto = item.Destination.Right;
                    var containerPosition = new ContainerPosition(
                        containerPositionDto.Row,
                        containerPositionDto.Bay,
                        containerPositionDto.Tier
                    );
                    destination = new Either<StorageArea, ContainerPosition>(containerPosition);
                }

                var containerDto = item.Container;
                var container = new Container(
                    new ContainerNumber(containerDto.ContainerNumber),
                    new ContainerPosition(containerDto.ContainerRow, containerDto.ContainerBay, containerDto.ContainerTier),
                    new CargoType(CargoType.FromString(containerDto.CargoType)),
                    containerDto.Description
                );

                var cargoTransport = new CargoTransport(
                    source,
                    destination,
                    container
                );
                unloadCargoManifestItems.Add(cargoTransport);
            }
            unloadCargoManifest = new CargoManifest(unloadCargoManifestItems);
        }

        VesselVisitNotification notification = new VesselVisitNotification(
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