namespace Domain.Model;

using Domain.Factory;

public class Holiday : IHoliday
{
	private IColaborator _colaborator;

	private List<HolidayPeriod> _holidayPeriods = new List<HolidayPeriod>();

	private IColaborator Colaborador;
	// {
	// 	get { return _colaborator; }
	// }

	public Holiday(IColaborator colab)
	{
		if (colab != null)
		{
			_colaborator = colab;
		}
		else
			throw new ArgumentException("Invalid argument: colaborator must be non null");
	}

	public HolidayPeriod AddHolidayPeriod(IHolidayPeriodFactory hpFactory, DateOnly startDate, DateOnly endDate)
	{
		HolidayPeriod holidayPeriod = hpFactory.NewHolidayPeriod(startDate, endDate);
		_holidayPeriods.Add(holidayPeriod);
		return holidayPeriod;
	}

	public string GetName()
	{
		return _colaborator.GetName();
	}

	public List<HolidayPeriod> GetHolidayPeriodsDuring(DateOnly startDate, DateOnly endDate)
	{
		return _holidayPeriods.Where(hp => hp.EndDate > startDate && hp.StartDate < endDate)
								// .Select(hp => new HolidayPeriod(hp.StartDate < startDate ? startDate : hp.StartDate,
								// 			hp.EndDate > endDate ? endDate : hp.EndDate))
								.ToList();
	}

	public bool HasColaboratorAndHolidayPeriodsDuring(IColaborator colaborator, DateOnly startDate, DateOnly endDate)
	{
			return _colaborator == colaborator && GetHolidayPeriodsDuring(startDate, endDate).Any();
	}

	

	public int GetHolidaysDaysWithMoreThanXDaysOff(int intDaysOff)
	{
		int numberOfDays = 0;

		foreach (HolidayPeriod hp in _holidayPeriods)
		{
			numberOfDays += hp.GetNumberOfDays();
		}
		if (numberOfDays > intDaysOff)
		{
			return numberOfDays;
		}
		else
		{
			return 0;
		}
	}

	public int GetNumberOfHolidayPeriodsDays()
	{
		return _holidayPeriods.Sum(hp => hp.GetNumberOfDays());
	}

	public bool HasColaborador(IColaborator colab)
	{
		return _colaborator == colab;
	}
}