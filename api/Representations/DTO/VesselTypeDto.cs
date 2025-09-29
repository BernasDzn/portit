public class VesselTypeDto
{
    public Guid Id { get; set; }
    public string Name{ get; set; }
    public string Description{ get; set; }
    public int MaxNumberOfRows{ get; set; }
    public int MaxNumberOfBays{ get; set; }
    public int MaxNumberOfTiers{ get; set; }
}