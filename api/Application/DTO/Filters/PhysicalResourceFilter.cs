public class PhysicalResourceFilter : Pageable
{
    public enum ResourceType
    {
        STSCrane,
        YardCrane,
        Truck
    }

    public string? Code { get; set; }
    public string? Description { get; set; }
    public ResourceStatus? Status { get; set; }
    public ResourceType? Type { get; set; }
}