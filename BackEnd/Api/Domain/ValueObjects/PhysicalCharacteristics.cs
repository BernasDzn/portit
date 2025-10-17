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
			_length = value;
		}
	}

	private double _depth;
	public double Depth
	{
		get => _depth;
		set
		{
			_depth = value;
		}
	}

	private double _draft;
	public double Draft
	{
		get => _draft;
		set
		{
			_draft = value;
		}
	}

	public override string ToString() => "Physical Characteristics: " + Length + " x " + Depth + " x " + Draft + ".";

}