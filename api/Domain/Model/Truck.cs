using Domain.Model.Generic;

public class Truck : PhysicalResource
{
    public uint MaxLoadCapacity { get; private set; } // In kg

    public Truck(Guid id, Designation code, Designation description, uint operationalCapacity, ResourceStatus status, TimeSpan setupTime, uint maxLoadCapacity)
        : base(id, code, description, operationalCapacity, status, setupTime)
    {
        MaxLoadCapacity = maxLoadCapacity;
    }
}