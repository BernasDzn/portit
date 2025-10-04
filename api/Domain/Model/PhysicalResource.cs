using Api.Domain.Model;
using Domain.Model.Generic;

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
    public virtual ICollection<Qualification> Qualifications { get; private set; } // Needed qualifications to operate the resource

    protected PhysicalResource() { } // EF Core

    internal PhysicalResource(Guid id, Code code, Designation description, ResourceStatus status, TimeSpan setupTime, ICollection<Qualification> qualifications)
    {
        Id = id;
        Description = description;
        Status = status;
        SetupTime = setupTime;
        Code = code;
        Qualifications = qualifications;
    }

    public PhysicalResourceDto ToDTO()
    {
        return new PhysicalResourceDto
        {
            Code = this.Code.ToString(),
            Description = this.Description.ToString(),
            Status = this.Status,
            SetupTimeInMinutes = (int)this.SetupTime.TotalMinutes,
            Qualifications = this.Qualifications.Select(q => q.ToDTO()).ToList()
        };
    }
}

public class STSCrane : PhysicalResource, IDTOAble<STSCraneDto>
{
    public uint ContainersPerHour { get; private set; }
    public uint LiftingCapacity { get; private set; }
    public virtual Dock ServingDock { get; private set; }

    protected STSCrane() { } // EF Core

    public STSCrane(Guid id, Code code, Designation description, ResourceStatus status, TimeSpan setupTime, ICollection<Qualification> qualifications, uint liftingCapacity, Dock dock, uint averageContainersPerHour)
        : base(id, code, description, status, setupTime, qualifications)
    {
        LiftingCapacity = liftingCapacity;
        ServingDock = dock;
        ContainersPerHour = averageContainersPerHour;
    }

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
            ServingDock = this.ServingDock.ToDTO()
        };
    }
}

public class YardCrane : PhysicalResource, IDTOAble<YardCraneDto>
{
    public uint ContainersPerHour { get; private set; }
    public uint LiftingCapacity { get; private set; }
    public virtual StorageArea YardSection { get; private set; }

    protected YardCrane() { } // EF Core

    public YardCrane(Guid id, Code code, Designation description, ResourceStatus status, TimeSpan setupTime, ICollection<Qualification> qualifications, uint liftingCapacity, StorageArea yardSection, uint averageContainersPerHour)
        : base(id, code, description, status, setupTime, qualifications)
    {
        LiftingCapacity = liftingCapacity;
        YardSection = yardSection;
        ContainersPerHour = averageContainersPerHour;
    }
    
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
            YardSection = this.YardSection.ToDTO()
        };
    }
}


public class Truck : PhysicalResource, IDTOAble<TruckDto>
{
    public uint ContainersPerTrip { get; private set; } // Number of containers the truck can carry per trip
    public uint AverageSpeed { get; private set; } // In km/h
    public uint MaxLoadCapacity { get; private set; } // In kg

    protected Truck() { } // EF Core

    public Truck(Guid id, Code code, Designation description, ResourceStatus status, TimeSpan setupTime, ICollection<Qualification> qualifications, uint maxLoadCapacity, uint containersPerTrip, uint averageSpeed)
        : base(id, code, description, status, setupTime, qualifications)
    {
        MaxLoadCapacity = maxLoadCapacity;
        ContainersPerTrip = containersPerTrip;
        AverageSpeed = averageSpeed;
    }

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
            AverageSpeed = this.AverageSpeed
        };
    }
}