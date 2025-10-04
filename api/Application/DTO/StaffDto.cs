namespace Api.Representations.DTO;

public class StaffDto
{
	public string MechanograficNumber { get; set; }
	public string Name { get; set; }
	public string Email { get; set; }
	public string PhoneNumber { get; set; }
	public string Status { get; set; }
	public OperationalWindowDto OperationalWindow { get; set; }
	public ICollection<QualificationDto> Qualifications { get; set; }

}