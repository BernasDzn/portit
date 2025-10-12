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

	public virtual OperationalWindow OperationalWindow { get; private set; }

	public virtual ICollection<Qualification> Qualifications { get; private set; }

	public bool isActive { get; private set; } = true;

	protected Staff() { }

	public Staff(
		StaffMechanograficNumber mechanograficNumber,
		Designation name,
		Email email,
		PhoneNumber phoneNumber,
		OperationalWindow operationalWindow,
		ICollection<Qualification> qualifications)
	{
		Id = Guid.NewGuid();
		MechanograficNumber = mechanograficNumber;
		Name = name;
		Email = email;
		PhoneNumber = phoneNumber;
		Status = StaffStatus.Available;
		OperationalWindow = operationalWindow;
		Qualifications = qualifications;
	}

	public void Update(
		string newName,
		string newEmail,
		string newPhoneNumber,
		int newStatus,
		OperationalWindow newOperationalWindow,
		HashSet<Qualification> newQualifications
	)
	{
		UpdateName(newName);
		UpdateEmail(newEmail);
		UpdatePhoneNumber(newPhoneNumber);
		UpdateStatus(newStatus);
		UpdateOperationalWindow(newOperationalWindow);
		UpdateQualifications(newQualifications);
	}

	public void UpdateName(string name)
	{
		Name = new Designation { Value = name };
	}

	public void UpdateEmail(string email)
	{
		Email = new Email { Value = email };
	}

	public void UpdatePhoneNumber(string phoneNumber)
	{
		PhoneNumber = new PhoneNumber { Value = phoneNumber };
	}

	public void UpdateOperationalWindow(OperationalWindow operationalWindow)
	{
		OperationalWindow = operationalWindow;
	}

	public void UpdateQualifications(ICollection<Qualification> newQualifications)
	{
		Qualifications.Clear();
		Qualifications = newQualifications;
	}

	public void UpdateStatus(int status)
	{
		foreach (StaffStatus s in Enum.GetValues<StaffStatus>())
		{
			if ((int)s == status)
			{
				Status = s;
				return;
			}
		}
		throw new ArgumentException("Trying to update to invalid Status.");
	}

	public void Deactivate()
	{
		Status = StaffStatus.Unavailable;
		isActive = false;
	}

	public StaffDto ToDTO()
	{
		return new StaffDto
		{
			MechanograficNumber = MechanograficNumber.Value,
			Name = Name.Value,
			Email = Email.Value,
			PhoneNumber = PhoneNumber.Value,
			Status = (int)Status,
			OperationalWindow = OperationalWindow,
			Qualifications = Qualifications.Select(q => q.ToDTO()).ToList()
		};
	}	
	public override string ToString() =>
		$"Staff [MechanograficNumber={MechanograficNumber.Value}, Name={Name.Value}, Email={Email.Value}, PhoneNumber={PhoneNumber.Value}, Status={Status}, OperationalWindow=({OperationalWindow}), Qualifications=[{string.Join(", ", Qualifications)}]]";
}

public enum StaffStatus
{
	Available = 0,
	Unavailable = 1,
	TemporarilyReassigned = 2
}