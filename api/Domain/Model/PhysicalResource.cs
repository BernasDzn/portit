using Domain.Model.Generic;

public class PhysicalResource
{
    public Guid Id { get; private set; }
    public Designation Code { get; private set; }
    public Designation Description { get; private set; }
    public uint OperationalCapacity { get; private set; }
    public ResourceStatus Status { get; private set; }
    public TimeSpan SetupTime { get; private set; } // In minutes
    
    public PhysicalResource() { } // EF Core

    public PhysicalResource(Guid id, Designation code, Designation description, uint operationalCapacity, ResourceStatus status, TimeSpan setupTime)
    {
        Id = id;
        Description = description;
        OperationalCapacity = operationalCapacity;
        Status = status;
        SetupTime = setupTime;
        Code = code;
    }
}