using Api.Domain.ValueObjects;

namespace Api.Application.DataTransfer;

public class StaffDto
{
	public string MechanographicNumber { get; set; }
	public string Name { get; set; }
	public string Email { get; set; }
	public string PhoneNumber { get; set; }
	public int Status { get; set; }
	public OperationalWindow OperationalWindow { get; set; }
	public ICollection<QualificationDto> Qualifications { get; set; }

}

public class CreateStaffDto
{
	public string MechanographicNumber { get; set; }
	public string Name { get; set; }
	public string Email { get; set; }
	public string PhoneNumber { get; set; }
	public int Status { get; set; }
	public OperationalWindow OperationalWindow { get; set; }
	public ICollection<string> QualificationsCodes { get; set; }

}