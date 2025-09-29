namespace Domain.Model.Generic;

public class Email 
{
    private string _value;
    public string Value { get => _value; set => _value = value; }

    //EF Core
    private Email() { }

    public Email(string value)
    {
        _value = value;
    }
}