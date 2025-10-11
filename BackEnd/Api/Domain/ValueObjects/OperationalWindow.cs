using Microsoft.EntityFrameworkCore;
using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;
using System.Text.Json.Serialization;

namespace Api.Domain.ValueObjects;

public class OperationalWindow : IDTOAble<OperationalWindowDto>
{
    public class Shift
    {
        public required DayOfWeek Day { get; set; }
        public required TimeOnly StartTime { get; set; }
        public required TimeOnly EndTime { get; set; }

        public override string ToString() => $"{Day}: {StartTime} - {EndTime}";
    }

    public static OperationalWindow FullWeek() => new OperationalWindow
    {
        Shifts = new List<Shift>
        {
            new Shift { Day = DayOfWeek.Monday, StartTime = TimeOnly.MinValue, EndTime = TimeOnly.MaxValue },
            new Shift { Day = DayOfWeek.Tuesday, StartTime = TimeOnly.MinValue, EndTime = TimeOnly.MaxValue },
            new Shift { Day = DayOfWeek.Wednesday, StartTime = TimeOnly.MinValue, EndTime = TimeOnly.MaxValue },
            new Shift { Day = DayOfWeek.Thursday, StartTime = TimeOnly.MinValue, EndTime = TimeOnly.MaxValue },
            new Shift { Day = DayOfWeek.Friday, StartTime = TimeOnly.MinValue, EndTime = TimeOnly.MaxValue },
            new Shift { Day = DayOfWeek.Saturday, StartTime = TimeOnly.MinValue, EndTime = TimeOnly.MaxValue },
            new Shift { Day = DayOfWeek.Sunday, StartTime = TimeOnly.MinValue, EndTime = TimeOnly.MaxValue }
        }
    };

    public static OperationalWindow Weekdays(TimeOnly startTime, TimeOnly endTime) => new OperationalWindow
    {
        Shifts = new List<Shift>
        {
            new Shift { Day = DayOfWeek.Monday, StartTime = startTime, EndTime = endTime },
            new Shift { Day = DayOfWeek.Tuesday, StartTime = startTime, EndTime = endTime },
            new Shift { Day = DayOfWeek.Wednesday, StartTime = startTime, EndTime = endTime },
            new Shift { Day = DayOfWeek.Thursday, StartTime = startTime, EndTime = endTime },
            new Shift { Day = DayOfWeek.Friday, StartTime = startTime, EndTime = endTime }
        }
    };

    // Make sure this has a public setter for EF Core
    public virtual ICollection<Shift> Shifts { get; set; } = new List<Shift>();

    [JsonConstructor]
    public OperationalWindow() { }
    
    public OperationalWindow(HashSet<Shift> shifts)
    {
        foreach (var shift in shifts)
        {
            if (shift == null)
                throw new ArgumentNullException("Shift cannot be null.");

            if (!IsValidShift(shift))
                throw new ArgumentException($"Invalid shift detected: Ensure no overlapping shifts and that start time is before end time. {shift}");
        }

        Shifts = shifts;
    }

    private bool IsValidShift(Shift shift)
    {
        foreach (var existingShift in Shifts.Where(s => s.Day == shift.Day))
        {
            if (shift.StartTime < existingShift.EndTime && existingShift.StartTime < shift.EndTime)
                return false;
        }
        return shift.EndTime > shift.StartTime;
    }

    public OperationalWindowDto ToDTO()
    {
        return new OperationalWindowDto
        {
            Shifts = this.Shifts.ToHashSet()
        };
    }
}