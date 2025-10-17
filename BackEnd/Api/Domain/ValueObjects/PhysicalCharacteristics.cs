using Microsoft.EntityFrameworkCore;

namespace Api.Domain.ValueObjects;

using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;


[Owned]
public class PhysicalCharacteristics
{

	private double _length;
	public double Length
	{
		get => _length;
		set
		{
			if (value <= 0)
				throw new ArgumentException("Length must be greater than zero.");
			_length = value;
		}
	}

	private double _depth;
	public double Depth
	{
		get => _depth;
		set
		{
			if (value <= 0)
				throw new ArgumentException("Depth must be greater than zero.");
			_depth = value;
		}
	}

	private double _draft;
	public double Draft
	{
		get => _draft;
		set
		{
			if (value <= 0)
				throw new ArgumentException("Draft must be greater than zero.");
			_draft = value;
		}
	}

	public override string ToString() => "Physical Characteristics: " + Length + " x " + Depth + " x " + Draft + ".";

}