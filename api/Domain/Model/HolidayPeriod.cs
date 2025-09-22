namespace Domain.Model;

public class HolidayPeriod
{
	DateOnly _startDate;
	DateOnly _endDate;

	int _status;

	public DateOnly StartDate
	{
		get { return _startDate; }
	}

	public DateOnly EndDate
	{
		get { return _endDate; }
	}

	public HolidayPeriod(DateOnly startDate, DateOnly endDate)
	{
		if (!IsStartDateIsValid(startDate, endDate))
		{
			throw new ArgumentException("invalid arguments: start date >= end date.");
		}
		
		this._startDate = startDate;
		this._endDate = endDate;
	}

	public bool IsStartDateIsValid(DateOnly startDate, DateOnly endDate)
	{
		if( startDate >= endDate ) 
		{
			return false;
		}
		return true;
	}

	public int GetNumberOfDays()
	{
		int startDateDays = _startDate.DayNumber;
		int endDateDays = _endDate.DayNumber;
		return endDateDays - startDateDays;
	}
}

