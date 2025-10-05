using Microsoft.EntityFrameworkCore;

namespace Api.Domain.ValueObjects;

using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;


[Owned]
public class OperationalWindow : IDTOAble<OperationalWindowDto>
{
	public static OperationalWindow FullWeek() => new OperationalWindow
	{
		StartWeekDay = DayOfWeek.Sunday,
		EndWeekDay = DayOfWeek.Saturday,
		DayStartTime = TimeOnly.Parse("00:00"),
		DayEndTime = TimeOnly.Parse("23:59")
	};

	private DayOfWeek _startWeekDay;
	public DayOfWeek StartWeekDay
	{
		get => _startWeekDay;
		set => _startWeekDay = value;
	}

	private DayOfWeek _endWeekDay;
	public DayOfWeek EndWeekDay
	{
		get => _endWeekDay;
		set => _endWeekDay = value;
	}

	private TimeOnly _dayStartTime = TimeOnly.Parse("00:00:00");
	public TimeOnly DayStartTime
	{
		get => _dayStartTime;
		set
		{
			if (value >= _dayEndTime)
				throw new ArgumentException("Day start time cannot be after or equal to day end time.");
			_dayStartTime = value;
		}
	}

	private TimeOnly _dayEndTime = TimeOnly.Parse("23:59:59");
	public TimeOnly DayEndTime
	{
		get => _dayEndTime;
		set
		{
			if (value <= _dayStartTime)
				throw new ArgumentException("Day end time cannot be before or equal to day start time.");
			_dayEndTime = value;
		}
	}

	public override string ToString() => "Operation Window: " + StartWeekDay + " to " + EndWeekDay + ", between " + DayStartTime + " and " + DayEndTime + ".";

	public OperationalWindowDto ToDTO()
	{
		return new OperationalWindowDto
		{
			StartWeekDay = StartWeekDay,
			EndWeekDay = EndWeekDay,
			DayStartTime = DayStartTime,
			DayEndTime = DayEndTime
		};
	}

}