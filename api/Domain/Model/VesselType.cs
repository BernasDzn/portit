namespace Api.Domain.Model;

public class VesselType
{
    public Guid Id { get; private set; }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public int MaxNumberOfRows { get; private set; }
    public int MaxNumberOfBays { get; private set; }
    public int MaxNumberOfTiers { get; private set; }

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
}