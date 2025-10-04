using Microsoft.EntityFrameworkCore;

namespace Api.Domain.Model;

[Owned]
public class StaffMechanograficNumber
{

	private string _value;
	public string Value { get => _value; set => _value = value; }

	// TODO: add domain logic to validate mechanografic number, ask client

}