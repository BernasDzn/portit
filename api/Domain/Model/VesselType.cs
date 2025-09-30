namespace Api.Domain.Model;

public class VesselType : IDTOAble<VesselTypeDto>
{
    public Guid Id { get; private set; }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public int MaxNumberOfRows { get; private set; }
    public int MaxNumberOfBays { get; private set; }
    public int MaxNumberOfTiers { get; private set; }

    //EF Core
    protected VesselType() { }
    public VesselType(Guid id, string name, string description, int maxNumberOfRows, int maxNumberOfBays, int maxNumberOfTiers)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description) || maxNumberOfRows < 0 || maxNumberOfBays < 0 || maxNumberOfTiers < 0)
            throw new ArgumentException("Invalid arguments!");

        Id = id;
        Name = name;
        Description = description;
        MaxNumberOfRows = maxNumberOfRows;
        MaxNumberOfBays = maxNumberOfBays;
        MaxNumberOfTiers = maxNumberOfTiers;
    }

    public VesselTypeDto ToDTO()
    {
        return new VesselTypeDto
        {
            Name = this.Name,
            Description = this.Description,
            MaxNumberOfRows = this.MaxNumberOfRows,
            MaxNumberOfBays = this.MaxNumberOfBays,
            MaxNumberOfTiers = this.MaxNumberOfTiers
        };
    }

    public void Update(VesselTypeDto vtype)
    {
        if (string.IsNullOrEmpty(vtype.Name)) throw new ArgumentException("Name cannot be null or empty", nameof(vtype.Name));
        if (string.IsNullOrEmpty(vtype.Description)) throw new ArgumentException("Description cannot be null or empty", nameof(vtype.Description));
        if (vtype.MaxNumberOfRows < 0) throw new ArgumentException("MaxNumberOfRows cannot be negative", nameof(vtype.MaxNumberOfRows));
        if (vtype.MaxNumberOfBays < 0) throw new ArgumentException("MaxNumberOfBays cannot be negative", nameof(vtype.MaxNumberOfBays));
        if (vtype.MaxNumberOfTiers < 0) throw new ArgumentException("MaxNumberOfTiers cannot be negative", nameof(vtype.MaxNumberOfTiers));

        Name = vtype.Name;
        Description = vtype.Description;
        MaxNumberOfRows = vtype.MaxNumberOfRows;
        MaxNumberOfBays = vtype.MaxNumberOfBays;
        MaxNumberOfTiers = vtype.MaxNumberOfTiers;
    }
}