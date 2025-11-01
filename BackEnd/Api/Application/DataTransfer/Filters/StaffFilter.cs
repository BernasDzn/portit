namespace Api.Application.DataTransfer.Filters;

using Api.Domain.Entities;
using Api.Infrastructure.Utilities;

public class StaffFilter : Pageable
{
	public string? MechanographicNumber { get; set; }
	public string? Name { get; set; }
	public StaffStatus? Status { get; set; }
	public string? Email { get; set; }
	public string? PhoneNumber { get; set; }
	public IEnumerable<string>? QualificationCodes { get; set; }
}