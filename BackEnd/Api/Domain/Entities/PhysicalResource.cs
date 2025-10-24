namespace Api.Domain.Entities;

using Api.Domain.ValueObjects;
using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;


public enum ResourceStatus
{
    Available = 0,
    Maintenance = 1,
    OutOfService = 2
}

// https://learn.microsoft.com/en-us/ef/core/modeling/inheritance
public class PhysicalResource : IDTOAble<PhysicalResourceDto>
{
    public Guid Id { get; private set; }
    public Code Code { get; private set; }
    public Designation Description { get; private set; }
    public ResourceStatus Status { get; private set; }
    public TimeSpan SetupTime { get; private set; } // In minutes
    public virtual OperationalWindow OperationalWindow { get; private set; }
    public virtual ICollection<Qualification> Qualifications { get; private set; } // Needed qualifications to operate the resource
    public bool Active { get; private set; } = true; // Soft delete

    protected PhysicalResource() { } // EF Core

    internal PhysicalResource(Guid id, Code code, Designation description, ResourceStatus status, TimeSpan setupTime, HashSet<Qualification> qualifications, OperationalWindow operationalWindow)
    {
        Id = id;
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Status = status;
        SetupTime = setupTime;
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Qualifications = qualifications ?? throw new ArgumentNullException(nameof(qualifications));
        OperationalWindow = operationalWindow ?? throw new ArgumentNullException(nameof(operationalWindow));
    }

    public void Deactivate()
    {
        Active = false;
        Status = ResourceStatus.OutOfService;
    }

    public void UpdateDescription(Designation description) { Description = description; }
    public void UpdateStatus(ResourceStatus status) { Status = status; }
    public void UpdateSetupTime(TimeSpan setupTime) { SetupTime = setupTime; }
    public void UpdateQualifications(HashSet<Qualification> qualifications)
    {
        // To avoid duplicates remove all the unneeded ones instead of clearing and adding
        // Remove unneeded qualifications
        var toRemove = Qualifications
            .Where(q => !qualifications.Any(nq => nq.Id == q.Id))
            .ToList();

        foreach (var q in toRemove)
            Qualifications.Remove(q);

        // Add only new qualifications
        var toAdd = qualifications
            .Where(nq => !Qualifications.Any(q => q.Id == nq.Id))
            .ToList();

        foreach (var q in toAdd)
            Qualifications.Add(q);
    }
    public void UpdateOperationalWindow(OperationalWindow operationalWindow) { OperationalWindow = operationalWindow; }

    public PhysicalResourceDto ToDTO()
    {
        return new PhysicalResourceDto
        {
            Code = this.Code.ToString(),
            Description = this.Description.ToString(),
            Status = this.Status,
            SetupTimeInMinutes = (int)this.SetupTime.TotalMinutes,
            Qualifications = this.Qualifications.Select(q => q.ToDTO()).ToList(),
            OperationalWindow = this.OperationalWindow
        };
    }
}

public class STSCrane : PhysicalResource, IDTOAble<STSCraneDto>
{
    public uint ContainersPerHour { get; private set; }
    public uint LiftingCapacity { get; private set; }
    public virtual Dock ServingDock { get; private set; }

    protected STSCrane() { } // EF Core

    public STSCrane(
        Guid id, Code code, Designation description, ResourceStatus status, TimeSpan setupTime, HashSet<Qualification> qualifications, OperationalWindow operationalWindow,
        uint liftingCapacity, Dock dock, uint averageContainersPerHour
    )
        : base(id, code, description, status, setupTime, qualifications, operationalWindow)
    {
        LiftingCapacity = liftingCapacity;
        ServingDock = dock;
        ContainersPerHour = averageContainersPerHour;
    }

    public void UpdateServingDock(Dock dock) { ServingDock = dock; }
    public void UpdateLiftingCapacity(uint liftingCapacity) { LiftingCapacity = liftingCapacity; }
    public void UpdateContainersPerHour(uint containersPerHour) { ContainersPerHour = containersPerHour; }

    STSCraneDto IDTOAble<STSCraneDto>.ToDTO()
    {
        return new STSCraneDto
        {
            Code = this.Code.ToString(),
            Description = this.Description.ToString(),
            Status = this.Status,
            SetupTimeInMinutes = (int)this.SetupTime.TotalMinutes,
            Qualifications = this.Qualifications.Select(q => q.ToDTO()).ToList(),
            ContainersPerHour = this.ContainersPerHour,
            LiftingCapacity = this.LiftingCapacity,
            ServingDock = this.ServingDock.ToDTO(),
            OperationalWindow = this.OperationalWindow
        };
    }
}

public class YardCrane : PhysicalResource, IDTOAble<YardCraneDto>
{
    public uint ContainersPerHour { get; private set; }
    public uint LiftingCapacity { get; private set; }

    protected YardCrane() { } // EF Core

    public YardCrane(
        Guid id, Code code, Designation description, ResourceStatus status, TimeSpan setupTime, HashSet<Qualification> qualifications, OperationalWindow operationalWindow,
        uint liftingCapacity, uint averageContainersPerHour
    )
        : base(id, code, description, status, setupTime, qualifications, operationalWindow)
    {
        LiftingCapacity = liftingCapacity;
        ContainersPerHour = averageContainersPerHour;
    }

    public void UpdateLiftingCapacity(uint liftingCapacity) { LiftingCapacity = liftingCapacity; }
    public void UpdateContainersPerHour(uint containersPerHour) { ContainersPerHour = containersPerHour; }
    
    YardCraneDto IDTOAble<YardCraneDto>.ToDTO()
    {
        return new YardCraneDto
        {
            Code = this.Code.ToString(),
            Description = this.Description.ToString(),
            Status = this.Status,
            SetupTimeInMinutes = (int)this.SetupTime.TotalMinutes,
            Qualifications = this.Qualifications.Select(q => q.ToDTO()).ToList(),
            ContainersPerHour = this.ContainersPerHour,
            LiftingCapacity = this.LiftingCapacity,
            OperationalWindow = this.OperationalWindow
        };
    }
}


public class Truck : PhysicalResource, IDTOAble<TruckDto>
{
    public uint ContainersPerTrip { get; private set; } // Number of containers the truck can carry per trip
    public uint AverageSpeed { get; private set; } // In km/h
    public uint MaxLoadCapacity { get; private set; } // In kg

    protected Truck() { } // EF Core

    public Truck(
        Guid id, Code code, Designation description, ResourceStatus status, TimeSpan setupTime, HashSet<Qualification> qualifications, OperationalWindow operationalWindow,
        uint maxLoadCapacity, uint containersPerTrip, uint averageSpeed
    )
        : base(id, code, description, status, setupTime, qualifications, operationalWindow)
    {
        MaxLoadCapacity = maxLoadCapacity;
        ContainersPerTrip = containersPerTrip;
        AverageSpeed = averageSpeed;
    }

    public void UpdateContainersPerTrip(uint containersPerTrip) { ContainersPerTrip = containersPerTrip; }
    public void UpdateAverageSpeed(uint averageSpeed) { AverageSpeed = averageSpeed; }
    public void UpdateMaxLoadCapacity(uint maxLoadCapacity) { MaxLoadCapacity = maxLoadCapacity; }

    TruckDto IDTOAble<TruckDto>.ToDTO()
    {
        return new TruckDto
        {
            Code = this.Code.ToString(),
            Description = this.Description.ToString(),
            Status = this.Status,
            SetupTimeInMinutes = (int)this.SetupTime.TotalMinutes,
            Qualifications = this.Qualifications.Select(q => q.ToDTO()).ToList(),
            ContainersPerTrip = this.ContainersPerTrip,
            MaxLoadCapacity = this.MaxLoadCapacity,
            AverageSpeed = this.AverageSpeed,
            OperationalWindow = this.OperationalWindow
        };
    }
}