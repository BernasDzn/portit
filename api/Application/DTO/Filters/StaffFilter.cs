public class StaffFilter : Pageable
{
	public string? Code { get; set; }
	public string? Name { get; set; }
	public string? Status { get; set; }
	public string? Email { get; set; }
	public string? PhoneNumber { get; set; }
	public string? Address { get; set; }
	public string? Position { get; set; }
	public IEnumerable<QualificationDto>? Qualifications { get; set; }
}