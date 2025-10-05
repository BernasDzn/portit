using Microsoft.EntityFrameworkCore;

namespace Api.Domain.ValueObjects;

[Owned]
public class StaffMechanograficNumber
{

	private string _value;
	public string Value
	{
		get => _value;
		set
		{
			if (string.IsNullOrWhiteSpace(value))
				throw new ArgumentException("Mechanografic number cannot be null or empty.");
			_value = value;
		}
	}

	// TODO: add domain logic to validate mechanografic number, ask client

}