using Microsoft.EntityFrameworkCore;

namespace Api.Domain.ValueObjects;

using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;


[Owned]
public class PhysicalCharacteristics : IDTOAble<PhysicalCharacteristicsDto>
{

	private double _length;
	public double Length
	{
		get => _length;
		set
		{
			if (value <= 0)
				throw new ArgumentException("Length must be positive.");
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
				throw new ArgumentException("Depth must be positive.");
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
				throw new ArgumentException("Draft must be positive.");
			_draft = value;
		}
	}

	public override string ToString() => "Physical Characteristics: " + Length + " x " + Depth + " x " + Draft + ".";

	public PhysicalCharacteristicsDto ToDTO()
	{
		return new PhysicalCharacteristicsDto
        {
            Length = _length,
            Depth = _depth,
            Draft = _draft
		};
	}

}