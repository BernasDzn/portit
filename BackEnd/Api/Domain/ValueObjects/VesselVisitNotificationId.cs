using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace Api.Domain.ValueObjects;

[Owned]
public class VesselVisitNotificationId
{
    //{YEAR}-{PORT_CODE}-{SEQUENTIAL_NUMBER}
    /*
    -- YEAR: the calendar year when the visit is first registered.
    -- PORT_CODE: a short alphanumeric code uniquely identifying the port (e.g., “PTLEI” for "Porto de Leixões").
    -- SEQUENTIAL_NUMBER: a zero-padded integer (e.g., 000001, 000002, …) assigned incrementally per port and year.
    The combination (YEAR, PORT_CODE, SEQUENTIAL_NUMBER) must be unique across the system.
    */
    private static readonly string IdPattern = @"^\d{4}-[A-Z0-9]{2,10}-\d{6}$";

    public string Year => Value.Split('-')[0];
    public string PortCode => Value.Split('-')[1];
    public string SequentialNumber => Value.Split('-')[2];

    private string _value;
    public string Value
    {
        get => _value;
        private set
        {
            if (!Regex.IsMatch(value, IdPattern))
                throw new ArgumentException("Invalid Vessel Visit Notification ID format.", nameof(value));

            _value = value;
        }
    }

    public VesselVisitNotificationId(Designation portCode, uint number, uint? date = null)
    {
        if (portCode == null || portCode.Value.Length < 2 || portCode.Value.Length > 10 || !Regex.IsMatch(portCode.Value, @"^[A-Z0-9]+$"))
            throw new ArgumentException("Port code must be 2 to 10 uppercase alphanumeric characters.", nameof(portCode));

        string year = date.ToString() ?? (DateTime.UtcNow.Year.ToString());
        string sequentialNumber = number.ToString("D6");
        Value = $"{year}-{portCode.Value}-{sequentialNumber}";
    }
    public VesselVisitNotificationId() { } // For EF Core

    public override string ToString() => Value;
}