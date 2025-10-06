namespace Api.Domain.ValueObjects;

public class SafetyOfficer
{
    private int _citizenID;
    public int CitizenID
    {
        get => _citizenID;
        set
        {
            // TODO: add Id validation logic

            _citizenID = value;
        }
    }
    private Designation _name;
    public string Name
    {
        get => _name.Value;
        set => _name = new Designation { Value = value };
    }

    private string _nationality;
    public string Nationality
    {
        get => _nationality;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Nationality cannot be empty");

            _nationality = value;
        }
    }

    public override string ToString() => $"ID: {CitizenID}, Name:{Name}, Nationality: {Nationality}";
}