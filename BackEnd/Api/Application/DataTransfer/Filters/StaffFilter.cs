namespace Api.Application.DataTransfer.Filters;

using Api.Infrastructure.Utilities;

public class StaffFilter : Pageable
{
	public string? MechanograficNumber { get; set; }
	public string? Name { get; set; }
	public string? Status { get; set; }
	public string? Email { get; set; }
	public string? PhoneNumber { get; set; }
	public IEnumerable<string>? QualificationCodes { get; set; }

	public bool IsEmpty()
	{
		return MechanograficNumber == null &&
			   Name == null &&
			   Status == null &&
			   Email == null &&
			   PhoneNumber == null &&
			   (QualificationCodes == null || !QualificationCodes.Any());
	}
}