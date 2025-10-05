namespace Api.Application.DataTransfer;

public class OperationalWindowDto
{
	public DayOfWeek StartWeekDay { get; set; }
	public DayOfWeek EndWeekDay { get; set; }
	public TimeOnly DayStartTime { get; set; }
	public TimeOnly DayEndTime { get; set; }
}