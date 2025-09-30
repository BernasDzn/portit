using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Domain.Model.Generic;

[Owned]
public class Designation
{
	private string _value;

	[MaxLength(100), MinLength(2)]
	public string Value
	{
		get => _value;
		set
		{
			if (string.IsNullOrWhiteSpace(value))
				throw new ArgumentException("Designation cannot be null or empty", nameof(value));
			if (value.Length > 100 || value.Length < 2)
				throw new ArgumentException("Designation must be between 2 and 100 characters", nameof(value));

			_value = value;
		}
	}

	public override string ToString() => Value;
}