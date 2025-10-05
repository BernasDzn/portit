using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Api.Domain.ValueObjects;

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
			var trimmed = value?.Trim();
			if (string.IsNullOrWhiteSpace(trimmed))
				throw new ArgumentException("Designation cannot be null or empty", nameof(value));
			if (trimmed.Length > 100 || trimmed.Length < 2)
				throw new ArgumentException("Designation must be between 2 and 100 characters", nameof(value));
			if (!Regex.IsMatch(trimmed, @"^[\p{L}0-9 .\-()]+$", RegexOptions.None))
				throw new ArgumentException("Designation can only contain alphanumeric characters, spaces, and hyphens", nameof(value));

			_value = trimmed;
		}
	}

	public override string ToString() => Value;
}