namespace Domain.Factory;

using Domain.Model;

public class HolidayFactory
{
    public Holiday NewHoliday(IColaborator colaborator)
    {
        return new Holiday(colaborator);
    }
}