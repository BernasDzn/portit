using Api.Domain.IRepository;
using Api.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Services;
public class VesselVisitNotificationIdGenerator
{
    private const string PORT_CODE = "PORTO"; 

    IVesselVisitNotificationRepository _notificationRepository;
    public VesselVisitNotificationIdGenerator(IVesselVisitNotificationRepository repository)
    {
        _notificationRepository = repository;
    }
    
    protected VesselVisitNotificationIdGenerator() {}

    public VesselVisitNotificationId Generate(uint year)
    {
        // Query the latest NotificationId with the same prefix
        var latestNotification = _notificationRepository.GetAll().Count();
        int nextSequenceNumber = latestNotification + 1;

        VesselVisitNotificationId newId = new VesselVisitNotificationId(new Designation { Value = PORT_CODE }, (uint)nextSequenceNumber, year);
        return newId;
    }
}
