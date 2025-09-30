using Domain.Model.Generic;

namespace Api.Domain.Model;

public class Staff
{
	public Guid Id { get; private set; }

	public Designation Name { get; private set; }

	public Email Email { get; private set; }

	public PhoneNumber PhoneNumber { get; private set; }

	public StaffStatus Status { get; private set; }

	// TODO: add public StaffOperationWindow OperationWindow { get; private set; }

	public ICollection<Qualification> Qualification { get; private set; }

	protected Staff() { }


}

