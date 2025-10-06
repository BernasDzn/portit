using Api.Domain.ValueObjects;
namespace Api.Application.DataTransfer;

public class OperationalWindowDto
{
    public HashSet<OperationalWindow.Shift> Shifts { get; set; } = new HashSet<OperationalWindow.Shift>();
}