using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Model.Generic;

[ComplexType]
public class Designation
{
	[MaxLength(100)]
	public required string Value { get; set; }
}