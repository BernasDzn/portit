using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace Api.Domain.ValueObjects;

[Owned]
public class StaffMechanograficNumber
{

	Regex alphanumericRegex = new Regex("^[a-zA-Z0-9]*$");

	private string _value;
	public string Value
	{
		get => _value;
		set
		{
			if (string.IsNullOrWhiteSpace(value))
				throw new ArgumentException("Mechanografic number cannot be null or empty.");
			if (!alphanumericRegex.IsMatch(value))
				throw new ArgumentException("Mechanografic number must be alphanumeric.");
			_value = value;
		}
	}

	// TODO: add domain logic to validate mechanografic number, ask client

}