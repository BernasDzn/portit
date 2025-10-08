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
        set
        {
            if (string.IsNullOrWhiteSpace(value) || !System.Text.RegularExpressions.Regex.IsMatch(value, IdPattern))
                throw new ArgumentException("Invalid VesselVisitNotificationId format. Expected format: YYYY-PORTCODE-XXXXXX", value);

            _value = value;
        }
    }

    public VesselVisitNotificationId(string portCode = "PORT", string number = "000001")
    {
        if (string.IsNullOrWhiteSpace(portCode) || portCode.Length < 2 || portCode.Length > 10 || !System.Text.RegularExpressions.Regex.IsMatch(portCode, @"^[A-Z0-9]+$"))
            throw new ArgumentException("Port code must be 3 to 5 uppercase alphanumeric characters", portCode);

        Value = $"{DateTime.UtcNow.Year}-{portCode}-{number}";
    }
    public VesselVisitNotificationId() { } // For EF Core

    public override string ToString() => Value;
}