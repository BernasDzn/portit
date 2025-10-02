using Domain.Model.Generic;

public class YardCrane : PhysicalResource
{
    public uint LiftingCapacity { get; private set; }
    public Designation YardSection { get; private set; } 

    public YardCrane(Guid id, Designation code, Designation description, uint operationalCapacity, ResourceStatus status, TimeSpan setupTime, uint liftingCapacity, Designation yardSection)
        : base(id, code, description, operationalCapacity, status, setupTime)
    {
        LiftingCapacity = liftingCapacity;
        YardSection = yardSection;
    }
}