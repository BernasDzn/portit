using Microsoft.EntityFrameworkCore;
using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;
using System.Text.Json.Serialization;

namespace Api.Domain.ValueObjects;

public class OperationalWindow
{
    public class Shift
    {
        public required DayOfWeek Day { get; set; }
        public required TimeOnly StartTime { get; set; }
        public required TimeOnly EndTime { get; set; }
        public override string ToString() => $"{Day}: {StartTime} - {EndTime}";
    }
    public static OperationalWindow Merge(ICollection<OperationalWindow> windows)
    {
        var merged = new OperationalWindow();
        var allShifts = windows.SelectMany(w => w.Shifts).ToList();

        // Group by day
        foreach (var group in allShifts.GroupBy(s => s.Day))
        {
            var shifts = group
                .OrderBy(s => s.StartTime)
                .ToList();

            var mergedShifts = new List<Shift>();

            // Start with first shift
            var current = new Shift
            {
                Day = group.Key,
                StartTime = shifts[0].StartTime,
                EndTime = shifts[0].EndTime
            };

            for (int i = 1; i < shifts.Count; i++)
            {
                var next = shifts[i];

                // Overlapping or touching?
                if (next.StartTime <= current.EndTime)
                {
                    // Extend
                    if (next.EndTime > current.EndTime)
                        current.EndTime = next.EndTime;
                }
                else
                {
                    // No overlap, push previous block
                    mergedShifts.Add(current);
                    current = new Shift
                    {
                        Day = group.Key,
                        StartTime = next.StartTime,
                        EndTime = next.EndTime
                    };
                }
            }

            // Add last shift block
            mergedShifts.Add(current);

            foreach (var s in mergedShifts)
                merged.Shifts.Add(s);
        }

        return merged;
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
    private ICollection<Shift> _shifts { get; set; } = new List<Shift>();
    public virtual ICollection<Shift> Shifts
    {
        get => _shifts;
        set
        {
            if (value == null)
                throw new ArgumentNullException("Shifts collection cannot be null.");

            foreach (var shift in value)
            {
                if (shift == null)
                    throw new ArgumentNullException("Shift cannot be null.");

                if (!IsValidShift(shift))
                    throw new ArgumentException($"Invalid shift detected: Ensure no overlapping shifts and that start time is before end time. {shift}");
            }

            _shifts = value;
        }
    }

    [JsonConstructor]
    public OperationalWindow() { }

    private bool IsValidShift(Shift shift)
    {
        foreach (var existingShift in Shifts.Where(s => s.Day == shift.Day))
        {
            if (shift.StartTime < existingShift.EndTime && existingShift.StartTime < shift.EndTime)
                return false;
        }
        return shift.EndTime > shift.StartTime;
    }

    public OperationalWindow Intercept(OperationalWindow other)
    {
        var intersected = new OperationalWindow();

        foreach (var shiftA in Shifts)
        {
            foreach (var shiftB in other.Shifts)
            {
                if (shiftA.Day == shiftB.Day)
                {
                    var latestStart = shiftA.StartTime > shiftB.StartTime ? shiftA.StartTime : shiftB.StartTime;
                    var earliestEnd = shiftA.EndTime < shiftB.EndTime ? shiftA.EndTime : shiftB.EndTime;

                    if (latestStart < earliestEnd)
                    {
                        intersected.Shifts.Add(new Shift
                        {
                            Day = shiftA.Day,
                            StartTime = latestStart,
                            EndTime = earliestEnd
                        });
                    }
                }
            }
        }

        return intersected;
    }

    public bool IsEmpty()
    {
        return !Shifts.Any();
    }
    
    public override string ToString()
    {
        return string.Join("; ", Shifts.Select(s => s.ToString()));
    }
}