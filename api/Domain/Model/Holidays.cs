namespace Domain.Model;

using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Factory;

public class Holidays
{
    private HolidayFactory _holidayFactory;
    private List<Holiday> _holidayList = new List<Holiday>();

    public Holidays(HolidayFactory hFactory)
    {
        if (hFactory is not null)
        {
            _holidayFactory = hFactory;
        }
        else
        {
            throw new ArgumentException("Holiday Factory cannot be null");
        }
    }

    public Holiday AddHoliday(IColaborator colaborator)
    {
        Holiday holiday = _holidayFactory.NewHoliday(colaborator);
        _holidayList.Add(holiday);
        return holiday;
    }

    public List<Holiday> GetListHolidayMoreDays(int numberOfDays)
    {
        return _holidayList.Where(h => h.GetHolidaysDaysWithMoreThanXDaysOff(numberOfDays) > 0).ToList();
    }

    public List<Holiday> GetListHolidayFilterByColaborator(IColaborator colaborator, DateOnly startDate, DateOnly endDate)
    {
        IEnumerable<Holiday> holidayList = _holidayList.Where(h => h.HasColaboratorAndHolidayPeriodsDuring(colaborator, startDate, endDate)); 

        if (!holidayList.Any())
        {
            throw new ArgumentException("No holiday found for this colaborator");
        }

        return holidayList.ToList();
    }

    public int GetNumberOfHolidaysDaysForColaboratorDuringPeriod(IColaborator colaborator, DateOnly startDate, DateOnly endDate)
    {
        int totalDaysOff = _holidayList
            .Where(h => h.HasColaboratorAndHolidayPeriodsDuring(colaborator, startDate, endDate))
            .Sum(holiday => holiday.GetNumberOfHolidayPeriodsDays());

        return totalDaysOff;
    }

    public int GetNumberOfDaysByColaborator(IColaborator colaborator)
    {
        return _holidayList.Where(h => h.HasColaborador(colaborator))
                            .Sum(h => h.GetNumberOfHolidayPeriodsDays());
    }
}