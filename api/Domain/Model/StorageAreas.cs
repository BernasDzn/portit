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
    public class DockService : IDTOAble<DockServiceDto>
    {
        public virtual Dock ServingDock { get; set; }
        public uint Distance { get; set; }

        protected DockService() { } // EF Core
        public DockService(Dock servingDock, uint distance)
        {
            ServingDock = servingDock;
            Distance = distance;
        }

        public DockServiceDto ToDTO()
        {
            return new DockServiceDto
            {
                Dock = ServingDock.ToDTO(),
                Distance = Distance
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
    public virtual HashSet<DockService> DockServices { get; private set; } = new(); // Empty means it serves all docks

    protected StorageArea() { } // EF Core

    public StorageArea(Guid id, Code nameCode, Designation location, StorageAreaType areaType, uint capacity, uint currentOccupancy, HashSet<DockService> dockServices = null!)
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
        return DockServices.Count == 0 || DockServices.Any(ds => ds.ServingDock.Id == dock.Id);
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