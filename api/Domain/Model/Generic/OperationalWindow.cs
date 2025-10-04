using Microsoft.EntityFrameworkCore;

namespace Domain.Model.Generic;

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
		set
		{
			if (value > _endWeekDay && _endWeekDay != 0)
				throw new ArgumentException("Start week day cannot be after end week day.");
			_startWeekDay = value;
		}
	}

	private DayOfWeek _endWeekDay;
	public DayOfWeek EndWeekDay
	{
		get => _endWeekDay;
		set
		{
			if (value < _startWeekDay && _startWeekDay != 0)
				throw new ArgumentException("End week day cannot be before start week day.");
			_endWeekDay = value;
		}
	}

	private TimeOnly _dayStartTime;
	public TimeOnly DayStartTime
	{
		get => _dayStartTime;
		set
		{
			if (_startWeekDay == _endWeekDay && value >= _dayEndTime)
				throw new ArgumentException("Day start time cannot be after or equal to day end time when start and end week days are the same.");
			_dayStartTime = value;
		}
	}

	private TimeOnly _dayEndTime;
	public TimeOnly DayEndTime
	{
		get => _dayEndTime;
		set
		{
			if (_startWeekDay == _endWeekDay && value <= _dayStartTime)
				throw new ArgumentException("Day end time cannot be before or equal to day start time when start and end week days are the same.");
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