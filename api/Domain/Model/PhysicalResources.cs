using Api.Domain.Model;
using Domain.Model.Generic;

public class PhysicalResource
{
    public Guid Id { get; private set; }
    public Designation Code { get; private set; }
    public Designation Description { get; private set; }
    public ResourceStatus Status { get; private set; }
    public TimeSpan SetupTime { get; private set; } // In minutes
    public virtual ICollection<Qualification> Qualifications { get; private set; } // Needed qualifications to operate the resource

    protected PhysicalResource() { } // EF Core

    public PhysicalResource(Guid id, Designation code, Designation description, ResourceStatus status, TimeSpan setupTime, ICollection<Qualification> qualifications)
    {
        Id = id;
        Description = description;
        Status = status;
        SetupTime = setupTime;
        Code = code;
        Qualifications = qualifications;
    }
}

public class STSCrane : PhysicalResource
{
    public uint ContainersPerHour  { get; private set; } 
    public uint LiftingCapacity { get; private set; }
    public virtual Dock ServingDock { get; private set; } 

    protected STSCrane() { } // EF Core

    public STSCrane(Guid id, Designation code, Designation description, ResourceStatus status, TimeSpan setupTime, ICollection<Qualification> qualifications, uint liftingCapacity, Dock dock, uint averageContainersPerHour)
        : base(id, code, description, status, setupTime, qualifications)
    {
        LiftingCapacity = liftingCapacity;
        ServingDock = dock;
        ContainersPerHour = averageContainersPerHour;
    }
}

public class YardCrane : PhysicalResource
{
    public uint ContainersPerHour  { get; private set; } 
    public uint LiftingCapacity { get; private set; }
    public virtual StorageArea YardSection { get; private set; }

    protected YardCrane() { } // EF Core

    public YardCrane(Guid id, Designation code, Designation description, ResourceStatus status, TimeSpan setupTime, ICollection<Qualification> qualifications, uint liftingCapacity, StorageArea yardSection, uint averageContainersPerHour)
        : base(id, code, description, status, setupTime, qualifications)
    {
        LiftingCapacity = liftingCapacity;
        YardSection = yardSection;
        ContainersPerHour  = averageContainersPerHour;
    }
}


public class Truck : PhysicalResource
{
    public uint ContainersPerTrip { get; private set; } // Number of containers the truck can carry per trip
    public uint AverageSpeed { get; private set; } // In km/h
    public uint MaxLoadCapacity { get; private set; } // In kg

    protected Truck() { } // EF Core

    public Truck(Guid id, Designation code, Designation description, ResourceStatus status, TimeSpan setupTime, ICollection<Qualification> qualifications, uint maxLoadCapacity, uint containersPerTrip)
        : base(id, code, description, status, setupTime, qualifications)
    {
        MaxLoadCapacity = maxLoadCapacity;
        ContainersPerTrip = containersPerTrip;
    }
}