using Api.Application.Exceptions;
using Api.Domain.Model;
using Domain.Model.Generic;
using Microsoft.EntityFrameworkCore;

public enum StorageAreaType
{
    Yard,
    Warehouse
}

public class StorageArea : IDTOAble<StorageAreaDto>
{
    // A dock that this storage area serves, and the distance to it
    [Owned]
    public class DockRelation : IDTOAble<DockRelationDto>
    {
        public virtual Dock ServingDock { get; set; }
        public bool IsServingDock { get; set; }
        // Distance may be null as it is complementary information
        public uint? Distance { get; set; } // Distance in meters

        protected DockRelation() { } // EF Core
        public DockRelation(Dock servingDock, uint? distance, bool isServingDock)
        {
            ServingDock = servingDock ?? throw new ArgumentNullException(nameof(servingDock));
            Distance = distance;
            IsServingDock = isServingDock;
        }

        public DockRelationDto ToDTO()
        {
            return new DockRelationDto
            {
                Dock = ServingDock.ToDTO(),
                Distance = Distance,
                IsServingDock = IsServingDock
            };
        }
    }

    public Guid Id { get; private set; }
    public Code NameCode { get; private set; }
    public Designation Location { get; private set; }
    public StorageAreaType AreaType { get; private set; }
    public uint Capacity { get; set; }

    private uint _currentOccupancy;
    public uint CurrentOccupancy
    {
        get => _currentOccupancy;
        private set
        {
            if (value > Capacity)
                throw new StorageFullException($"Current occupancy cannot exceed capacity. Capacity: {Capacity}, Attempted Occupancy: {value}");

            _currentOccupancy = value;
        }
    }

    // List of docks this storage area serves
    // This list will only store the known data about the relation of each dock and this storage area
    // If a dock is not in this list, it means this storage area does not serve it and does not know anything about it
    // Unless this storage area is a warehouse, in which case it serves all docks
    public virtual HashSet<DockRelation> DockServices { get; private set; } = new();

    protected StorageArea() { } // EF Core

    public StorageArea(Guid id, Code nameCode, Designation location, StorageAreaType areaType, uint capacity, uint currentOccupancy, HashSet<DockRelation> dockServices = null!)
    {
        Id = id;
        NameCode = nameCode;
        Location = location;
        AreaType = areaType;
        Capacity = capacity;
        CurrentOccupancy = currentOccupancy;
        
        if (dockServices != null)
            DockServices = dockServices;
    }

    public bool CanServeDock(Dock dock)
    {
        return
            AreaType == StorageAreaType.Warehouse ||
            DockServices.Any(ds => ds.ServingDock.Id == dock.Id && ds.IsServingDock);
    }

    public StorageAreaDto ToDTO()
    {
        return new StorageAreaDto
        {
            NameCode = NameCode.Value,
            Location = Location.Value,
            Type = AreaType,
            Capacity = Capacity,
            CurrentOccupancy = CurrentOccupancy,
            DockServices = DockServices.Select(ds => ds.ToDTO()).ToHashSet()
        };
    }
}