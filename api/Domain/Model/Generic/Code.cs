using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace Domain.Model.Generic;

[Owned]
public class Code
{
	private static readonly string Pattern = @"^[a-zA-Z0-9]+$"; // Alphanumeric pattern
	private string _value;

	public Code() { }

	[MaxLength(50), MinLength(1)]
	public string Value
	{
		get => _value;
		set
		{
			if (string.IsNullOrWhiteSpace(value))
				throw new ArgumentException("Designation cannot be null or empty", nameof(value));
			if (value.Length > 50 || value.Length < 1)
				throw new ArgumentException("Designation must be between 2 and 100 characters", nameof(value));

			if (!Regex.IsMatch(value, Pattern))
				throw new ArgumentException("Designation must be alphanumeric", nameof(value));

			_value = value;
		}
	}
	
	public override string ToString() => Value;
}