using Api.Domain.Model;
using Domain.Model.Generic;

public class STSCrane : PhysicalResource
{
    public uint LiftingCapacity { get; private set; }
    public virtual Dock Dock { get; private set; } 

    public STSCrane(Guid id, Designation code, Designation description, uint operationalCapacity, ResourceStatus status, TimeSpan setupTime, uint liftingCapacity, Dock dock)
        : base(id, code, description, operationalCapacity, status, setupTime)
    {
        LiftingCapacity = liftingCapacity;
        Dock = dock;
    }
}