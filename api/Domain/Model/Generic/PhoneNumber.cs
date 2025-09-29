namespace Domain.Model.Generic;

public class PhoneNumber
{
    private string _value;
    public string Value { get => _value; set => _value = value; }

    //EF Core
    private PhoneNumber() { }

    public PhoneNumber(string value)
    {
        _value = value;
    }
}