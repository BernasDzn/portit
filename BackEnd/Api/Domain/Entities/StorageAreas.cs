namespace Api.Domain.Entities;

using Api.Domain.ValueObjects;
using Api.Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;


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
    public uint Capacity { get; private set; }

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

    public virtual ICollection<DockRelation>? DockServices { get; private set; }

    protected StorageArea() { } // EF Core

    public StorageArea(Guid id, Code nameCode, Designation location, StorageAreaType areaType, uint capacity, uint currentOccupancy, HashSet<DockRelation> dockServices)
    {
        Id = id;
        NameCode = nameCode ?? throw new ArgumentNullException(nameof(nameCode));
        Location = location ?? throw new ArgumentNullException(nameof(location));
        AreaType = areaType;
        Capacity = capacity;
        CurrentOccupancy = currentOccupancy;

        if (AreaType == StorageAreaType.Warehouse && dockServices != null && dockServices.Any(ds => !ds.IsServingDock))
            throw new ArgumentException("A warehouse must serve all docks it is related to.");

        DockServices = dockServices;
    }

    public bool CanServeDock(Dock dock)
    {
        return
            AreaType == StorageAreaType.Warehouse ||
            (DockServices != null && DockServices.Any(ds => ds.ServingDock.Id == dock.Id && ds.IsServingDock));
    }

    public void UpdateNameCode(string newCode)
    {
        NameCode = new Code { Value = newCode };
    }
    public void UpdateLocation(string newLocation)
    {
        Location = new Designation { Value = newLocation };
    }
    public void UpdateCapacity(uint newCapacity)
    {
        if (newCapacity < CurrentOccupancy)
            throw new StorageFullException($"New capacity cannot be less than current occupancy. Current Occupancy: {CurrentOccupancy}, New Capacity: {newCapacity}");

        Capacity = newCapacity;
    }
    public void UpdateOccupancy(uint newOccupancy)
    {
        CurrentOccupancy = newOccupancy;
    }

    public void UpdateAreaType(StorageAreaType newType)
    {
        AreaType = newType;
    }

    public void UpdateDockServices(HashSet<DockRelation>? newDockServices)
    {
        if (AreaType == StorageAreaType.Warehouse && newDockServices != null && newDockServices.Any(ds => !ds.IsServingDock))
            throw new ArgumentException("A warehouse must serve all docks it is related to.");

        DockServices = newDockServices;
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
            DockServices = (DockServices ?? Enumerable.Empty<DockRelation>()).Select(ds => ds.ToDTO()).ToHashSet()
        };
    }
}