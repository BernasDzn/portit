namespace Api.Domain.Entities;

using Api.Domain.ValueObjects;
using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;


public class Staff : IDTOAble<StaffDto>
{
	public Guid Id { get; private set; }

	public StaffMechanograficNumber MechanograficNumber { get; private set; }

	public Designation Name { get; private set; }

	public Email Email { get; private set; }

	public PhoneNumber PhoneNumber { get; private set; }

	public StaffStatus Status { get; private set; }

	public OperationalWindow OperationalWindow { get; private set; }

	public virtual ICollection<Qualification> Qualifications { get; private set; }

	protected Staff() { }

	public Staff(
		StaffMechanograficNumber mechanograficNumber,
		Designation name,
		Email email,
		PhoneNumber phoneNumber,
		OperationalWindow operationalWindow,
		ICollection<Qualification> qualifications)
	{
		Id = new Guid();
		MechanograficNumber = mechanograficNumber;
		Name = name;
		Email = email;
		PhoneNumber = phoneNumber;
		Status = StaffStatus.Active;
		OperationalWindow = operationalWindow;
		Qualifications = qualifications;
	}

	public StaffDto ToDTO()
	{
		return new StaffDto
		{
			MechanograficNumber = MechanograficNumber.Value,
			Name = Name.Value,
			Email = Email.Value,
			PhoneNumber = PhoneNumber.Value,
			Status = Status.ToString(),
			OperationalWindow = OperationalWindow.ToDTO(),
			Qualifications = Qualifications.Select(q => q.ToDTO()).ToList()
		};
	}

	public override string ToString()
	{
		return $"Staff [MechanograficNumber={MechanograficNumber.Value}, Name={Name.Value}, Email={Email.Value}, PhoneNumber={PhoneNumber.Value}, Status={Status}, OperationalWindow=({OperationalWindow}), Qualifications=[{string.Join(", ", Qualifications)}]]";
	}

}

public enum StaffStatus
{
	Active,
	Inactive,
	Suspended
}