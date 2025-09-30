using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Domain.Model.Generic;

[Owned]
public class Designation
{
	[MaxLength(100)]
	public required string Value { get; set; }
}