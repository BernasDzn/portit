namespace Api.Application.Services;

using System.Collections;
using Api.Application.Controllers;
using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Application.Exceptions;
using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Exceptions;
using Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Authorization;

public class VesselVisitNotificationService : IVesselVisitNotificationService
{
    private readonly IVesselVisitNotificationRepository _notificationRepository;
    private readonly IVesselRepository _vesselRepository;
    private readonly IRepresentativeRepository _representativeRepository;
    private readonly IStorageAreaRepository _storageAreaRepository;
    private readonly IContainerRepository _containerRepository;
    private readonly VesselVisitNotificationIdGenerator _idGenerator;
    private readonly IDockRepository _dockRepository;
    private readonly IPhysicalResourceRepository _physicalResourceRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly ILogger<VesselVisitNotificationService> _logger;

    public VesselVisitNotificationService(
        IVesselVisitNotificationRepository notificationRepository,
        IVesselRepository vesselRepository,
        IRepresentativeRepository representativeRepository,
        IStorageAreaRepository storageAreaRepository,
        VesselVisitNotificationIdGenerator idGenerator,
        IContainerRepository containerRepository,
        IDockRepository dockRepository,
        IPhysicalResourceRepository physicalResourceRepository,
        IStaffRepository staffRepository,
        ILogger<VesselVisitNotificationService> logger
    )
    {
        _notificationRepository = notificationRepository;
        _vesselRepository = vesselRepository;
        _representativeRepository = representativeRepository;
        _storageAreaRepository = storageAreaRepository;
        _containerRepository = containerRepository;
        _idGenerator = idGenerator;
        _dockRepository = dockRepository;
        _physicalResourceRepository = physicalResourceRepository;
        _staffRepository = staffRepository;
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

    public async Task<VesselVisitNotificationDto> Add(CreateVesselVisitNotificationDto vesselVisitNotificationDto, string userEmail)
    {
        VesselVisitNotification? existingNotification =
            await _notificationRepository.GetVesselVisitNotificationByNotificationIdAsync(vesselVisitNotificationDto.NotificationId);

        if (existingNotification != null) throw new EntityAlreadyExistsException($"Vessel Visit Notification with id {vesselVisitNotificationDto.NotificationId} already exists.");

        Vessel? vessel = await _vesselRepository.GetVesselByIMOAsync(vesselVisitNotificationDto.VesselImoNumber);
        if (vessel == null) throw new EntityNotFoundException($"Vessel with IMO {vesselVisitNotificationDto.VesselImoNumber} was not found.");

        Representative representative = await _representativeRepository.GetByEmailAsync(
            userEmail
        );

        if (representative == null) throw new EntityNotFoundException($"Representative with email {userEmail} was not found.");

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

    public async Task<VesselVisitNotificationDto> Update(string vvnID, CreateVesselVisitNotificationDto vvnDTO, string userEmail)
    {
        var existingNotification =
            await _notificationRepository.GetVesselVisitNotificationByNotificationIdAsync(vvnID) ??
            throw new EntityNotFoundException($"Vessel Visit Notification with id {vvnID} was not found.");

        var saor = await _representativeRepository.GetByEmailAsync(userEmail);
        if (saor == null)
            throw new EntityNotFoundException($"System user with email {userEmail} was not found.");

        if (existingNotification.Submitter.Id != saor.Id)
            throw new UnauthorizedAccessException("You may not update this notification, you are not its original author");

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

    public async Task<Page<VesselVisitNotificationStatusDto>> FilterNotifications(VesselVisitNotificationFilter filter, string userEmail)
    {
        var saor = await _representativeRepository.GetByEmailAsync(userEmail);
        if (saor == null)
            throw new EntityNotFoundException($"System user with email {userEmail} was not found.");

        Page<VesselVisitNotification> page = await _notificationRepository.FilterVesselVisitNotificationsAsync(filter, saor.CitizenshipId);
        AppLogEvents.LogFilter(_logger, "vessel visit notifications", page.Items.Count);
        return page.Map(vvn => vvn.ToStatusDTO());
    }

    public async Task SubmitNotification(string vvnID, string userEmail)
    {
        var existingNotification =
            await _notificationRepository.GetVesselVisitNotificationByNotificationIdAsync(vvnID) ??
            throw new EntityNotFoundException($"Vessel Visit Notification with id {vvnID} was not found.");

        var saor = await _representativeRepository.GetByEmailAsync(userEmail);
        if (saor == null)
            throw new EntityNotFoundException($"System user with email {userEmail} was not found.");

        if (existingNotification.Submitter.Id != saor.Id)
            throw new UnauthorizedAccessException("You may not submit this notification, you are not its original author");

        existingNotification.Submit();
        await _notificationRepository.UpdateAsync(existingNotification);
    }

    public async Task DeleteNotificationDraft(string vvnID, string userEmail)
    {
        var existingNotification =
            await _notificationRepository.GetVesselVisitNotificationByNotificationIdAsync(vvnID) ??
            throw new EntityNotFoundException($"Vessel Visit Notification with id {vvnID} was not found.");

        if (existingNotification.Status != NotificationStatus.InProgress)
            throw new InvalidOperationException("Cannot delete this notification, it was already submitted");

        var saor = await _representativeRepository.GetByEmailAsync(userEmail);
        if (saor == null)
            throw new EntityNotFoundException($"System user with email {userEmail} was not found.");

        if (existingNotification.Submitter.Id != saor.Id)
            throw new UnauthorizedAccessException("You may not delete this notification, you are not its original author");

        await _notificationRepository.DeleteAsync(existingNotification);
    }

    public async Task<IEnumerable<VesselVisitNotificationDto>> GetVesselVisitNotificationsOnDay(DateTime day, uint daysAhead)
    {
        List<VesselVisitNotification> items = await _notificationRepository.GetVesselVisitNotificationsOnDayAsync(day, daysAhead);
        AppLogEvents.LogFilter(_logger, "vessel visit notifications", items.Count);
        return items.Select(n => n.ToDTO());
    }

    public async Task<SchedulingResultDto> CollectSchedulingData(DateTime date, uint daysAhead, Code dockCode)
    {
        var dock = await _dockRepository.GetDockByCodeAsync(dockCode.Value);
        if (dock == null)
            throw new EntityNotFoundException($"Dock with code {dockCode.Value} was not found.");

        List<VesselVisitNotification> notifications = await _notificationRepository.GetVesselVisitNotificationsOnDayAsync(date, daysAhead);
        // Filter notifications to only those assigned to the specified dock
        // In the future, we might need to handle multiple docks
        var filteredNotifications = notifications.Where(n => n.GetLatestDecision()!.AssignedDock!.Code.Value == dockCode.Value);

        IEnumerable<STSCrane> cranesServingDock = await _physicalResourceRepository.GetSTSCranesByDockCodeAsync(dock.Code.Value);
        // Select only available cranes
        cranesServingDock = cranesServingDock.Where(c => c.Status == ResourceStatus.Available);

        if (!cranesServingDock.Any())
            throw new EntityNotFoundException($"No STS cranes found serving dock with code {dock.Code.Value}.");

        // Crane selection algorithm
        // For now we will choose the fastest crane available
        // We will change this to support multiple cranes in the future (é uma US troll face)
        STSCrane selectedCrane = cranesServingDock.OrderBy(c => c.ContainersPerHour).First();

        SchedulingResultDto result = new SchedulingResultDto
        {
            CraneWorkloads = new List<CraneWorkloadDto>
            {
                new CraneWorkloadDto
                {
                    Crane = selectedCrane.Code.Value,
                    operatingWindow = await CalculateEffectiveCraneOperatingWindow(selectedCrane, date, daysAhead),
                    VesselTaskFacts = new List<VesselTaskFactDto>()
                }
            },
            Comment = string.Empty
        };

        // TODO: Extend this logic when supporting multiple cranes
        if (result.CraneWorkloads[0].operatingWindow.IsEmpty())
        {
            result.Comment = "No qualified staff available to operate the selected resource.";
            return result;
        }

        try
        {
            foreach (var notification in filteredNotifications)
            {
                var decision = notification.GetLatestDecision()!;
                var vessel = notification.Vessel;

                VesselTaskFactDto vesselTaskFact = new VesselTaskFactDto
                {
                    Vessel = vessel.ToDTO(),
                    ETA = CalculateBaseHour(date, notification.ExpectedArrival),
                    ETD = CalculateBaseHour(date, notification.ExpectedDeparture),
                    LoadingTime = CalculateLoadUnloadingTime(notification.LoadCargoManifest ?? new List<CargoTransport>(), selectedCrane),
                    UnloadingTime = CalculateLoadUnloadingTime(notification.UnloadCargoManifest ?? new List<CargoTransport>(), selectedCrane),
                };

                if (vesselTaskFact.LoadingTime > 0 && vesselTaskFact.UnloadingTime > 0)
                    result.CraneWorkloads[0].VesselTaskFacts.Add(vesselTaskFact);
            }
        }
        catch (System.Exception e)
        {
            result.CraneWorkloads[0].VesselTaskFacts.Clear();
            result.Comment = "Error calculating scheduling data. " + e.Message;
        }

        AppLogEvents.LogRetrieve(_logger, "scheduling data", result.CraneWorkloads.Count);
        return result;
    }

    private uint CalculateBaseHour(DateTime pivot, DateTime target)
    {
        return (uint)(target - pivot).TotalHours;
    }

    private double CalculateLoadUnloadingTime(ICollection<CargoTransport> cargoManifest, STSCrane crane)
    {
        uint totalContainers = (uint)cargoManifest.Count;
        return (double)totalContainers / (double)crane.ContainersPerHour;
    }

    private async Task<OperationalWindow> CalculateEffectiveCraneOperatingWindow(STSCrane crane, DateTime date, uint daysAhead)
    {
        // To calculate the effective operating window of the crane, we need to consider its own operational window
        // and the operational window of the staff that may operate it. That is get the qualifications required to operate the crane,
        // then get the staff that have those qualifications, and get their operational windows. And merge everything together badabing badaboom
        IEnumerable<string> qualificationCodes = crane.Qualifications.Select(q => q.NameCode.Value).AsEnumerable();
        Page<Staff> staff = await _staffRepository.FilterStaffsAsync(
            new StaffFilter
            {
                QualificationCodes = qualificationCodes,
                PageSize = await _staffRepository.CountAsync()
            }
        );

        List<OperationalWindow> staffOperationalWindows = staff.Items
            .Select(s => s.OperationalWindow)
            .ToList();

        OperationalWindow qualifiedStaffAvailablity = OperationalWindow.Merge(staffOperationalWindows);
        return crane.OperationalWindow.Intercept(qualifiedStaffAvailablity);
    }
}